using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayerController : MonoBehaviour
{
    #region Delegates and Events

    // Delegate for grapple hook usage
    public delegate void GrappleUsedHandler();
    // Event for grapple hook usage
    public static event GrappleUsedHandler OnGrappleUsed;

    // Delegate for grapple hook usage
    public delegate void DashUsedHandler();
    // Event for grapple hook usage
    public static event DashUsedHandler OnDashUsed;

    #endregion

    #region Serialized Fields

    [Header("Player Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 4.0f;
    [SerializeField] private float crouchSpeed = 1.0f;
    [SerializeField] private float jumpHeight = 1.0f;

    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Grapple Hook")]
    [SerializeField] private float grappleDistance = 20.0f;
    [SerializeField] private float grappleSpeed = 10.0f;
    [SerializeField] private float grappleCooldown = 15.0f;
    [SerializeField] private GameObject grappleLineObject;
    [SerializeField] private Slider grappleCooldownSlider;

    [Header("Dash Settings")]
    [SerializeField] private float dashCooldown = 5.0f;
    [SerializeField] private float dashSpeed = 10.0f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private Slider dashCooldownSlider;


    [Header("Camera Settings")]
    [SerializeField] private GameObject CameraSource;  // Reference to the camera or object to move

    #endregion

    #region Private Fields
    private CharacterController m_CharacterController;
    private Transform m_CameraTransform;
    private InputManager m_InputManager;
    private LineRenderer m_LineRenderer;
    private Vector3 m_PlayerVelocity;
    private Vector3 originalCenter;
    private Vector3 cameraOriginalPosition;
    private bool _isGrounded;
    private bool _isCrouching;
    private bool _isGrappling;
    private bool _canGrapple = true;
    private float _grappleCooldownTime;
    private Vector3 _grapplePoint;
    private float _currentSpeed;
    private bool isPaused = false;
    private int _jumpCount = 0;
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _dashTime;
    private float _dashCooldownTime;
    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        if (isPaused) return;

        HandleGroundCheck();
        HandleMovement();
        HandleCrouching();
        HandleJumping();
        HandleGrappleHook();
        ApplyGravity();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_InputManager = InputManager.Instance;
        originalCenter = m_CharacterController.center;

        if (Camera.main != null) m_CameraTransform = Camera.main.transform;

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store the original position of the CameraSource
        if (CameraSource != null)
        {
            cameraOriginalPosition = CameraSource.transform.localPosition;
        }
        else
        {
            Debug.LogError("CameraSource is not assigned!");
        }

        // Initialize the LineRenderer for the grapple hook
        if (grappleLineObject != null)
        {
            m_LineRenderer = grappleLineObject.GetComponent<LineRenderer>();
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("GrappleLineObject is not assigned!");
        }

        // Initialize the grapple cooldown slider
        if (grappleCooldownSlider != null)
        {
            grappleCooldownSlider.maxValue = grappleCooldown;
            grappleCooldownSlider.value = grappleCooldown;
        }
        else
        {
            Debug.LogError("GrappleCooldownSlider is not assigned!");
        }



        _currentSpeed = playerSpeed; // Set the default player speed
    }

    #endregion

    #region Player Rotation

    /// <summary>
    /// Rotates the player to face the same direction as the Cinemachine camera.
    /// </summary>


    #endregion

    #region Movement and Crouching

    /// <summary>
    /// Handles player movement.
    /// </summary>
    private void HandleMovement()
    {
        Vector2 movement = m_InputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        // Synchronize player rotation with the camera's forward direction (Y-axis only)
        Vector3 cameraForward = m_CameraTransform.forward;
        cameraForward.y = 0f; // Zero out Y to keep the player upright
        transform.rotation = Quaternion.LookRotation(cameraForward); // Rotate player to face camera direction

        // Movement based on camera's forward and right vectors
        move = m_CameraTransform.forward * move.z + m_CameraTransform.right * move.x;

        m_CharacterController.Move(move * (Time.deltaTime * _currentSpeed));
        HandleDashing(move);
        // Update the camera position to follow the player
        if (CameraSource != null)
        {
            CameraSource.transform.position = transform.position + cameraOriginalPosition;
        }
    }

    /// <summary>
    /// Handles crouching behavior, including moving the CameraSource.
    /// </summary>
    private void HandleCrouching()
    {
        if (_isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (!_isCrouching)
                {
                    _isCrouching = true;
                    m_CharacterController.height = 1.0f;
                    m_CharacterController.center = new Vector3(originalCenter.x, -0.5f, originalCenter.z);

                    // Move the CameraSource down by 0.75 units when crouching
                    CameraSource.transform.localPosition = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y - 0.75f, cameraOriginalPosition.z);
                    _currentSpeed = crouchSpeed;
                }
            }
            else if (_isCrouching)
            {
                // Reset the camera and player position when standing up
                _isCrouching = false;
                m_CharacterController.height = 2.0f;
                m_CharacterController.center = originalCenter;

                // Reset the CameraSource back to its original position
                CameraSource.transform.localPosition = cameraOriginalPosition;
                _currentSpeed = playerSpeed;
            }
        }
    }
    /// <summary>
    /// Handles Dashing behaviour
    /// </summary>
    private void HandleDashing(Vector3 direction)
    {
        // If the player is not dashing, check if the dash cooldown has expired
        if (!_canDash)
        {
            _dashTime -= Time.deltaTime;
            if (dashCooldownSlider != null)
            {
                dashCooldownSlider.value = _dashCooldownTime;
            }

            if (_dashCooldownTime <= 0)
            {
                _canDash = true;
                if (dashCooldownSlider != null)
                {
                    dashCooldownSlider.value = dashCooldown;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && _canDash)
        {
            _isDashing = true;
            _dashTime = dashDuration;
            _canDash = false;
            _dashCooldownTime = dashCooldown; 
        }
        
        if (_isDashing)
        {
            if (_dashTime > 0)
            {
                m_CharacterController.Move(direction * (Time.deltaTime * dashSpeed));
                _dashTime -= Time.deltaTime;
            }
            else
            {
                _isDashing = false;
                OnDashUsed?.Invoke(); // Trigger the event

                if (grappleCooldownSlider != null)
                {
                    grappleCooldownSlider.value = 0;
                }

            }
        }
        else
        {
            m_CharacterController.Move(direction * (Time.deltaTime * _currentSpeed)); // Modified to use _currentSpeed
        }
    }

    #endregion

    #region Jumping

    /// <summary>
    /// Handles jumping logic.
    /// </summary>
    private void HandleJumping()
    {
        if (m_InputManager.PlayerJumpedThisFrame() && _jumpCount < maxJumpCount)
        {
            m_PlayerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            _jumpCount++;
        }
    }

    #endregion

    #region Grapple Hook

    /// <summary>
    /// Handles grapple hook mechanics.
    /// </summary>
    private void HandleGrappleHook()
    {
        if (Input.GetMouseButtonDown(1) && _canGrapple)
        {
            // Debug.Log("Grapple Hook used!");
            RaycastHit hit;
            if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out hit, grappleDistance, groundMask))
            {
                _isGrappling = true;
                _grapplePoint = hit.point;
                _canGrapple = false;
                _grappleCooldownTime = grappleCooldown;
                m_LineRenderer.enabled = true;
                // Debug.Log("Grapple Point: " + _grapplePoint);
                OnGrappleUsed?.Invoke(); // Trigger the event

                if (grappleCooldownSlider != null)
                {
                    grappleCooldownSlider.value = 0;
                }
            }
        }

        if (_isGrappling)
        {
            // Check for jump input to break the grapple
            if (m_InputManager.PlayerJumpedThisFrame())
            {
                _isGrappling = false;
                m_LineRenderer.enabled = false; // Disable the line renderer
            }
            else
            {
                Debug.Log("Grapple Hooking!");
                Vector3 direction = (_grapplePoint - transform.position).normalized;
                m_CharacterController.Move(direction * (Time.deltaTime * grappleSpeed));

                m_LineRenderer.SetPosition(0, transform.position);
                m_LineRenderer.SetPosition(1, _grapplePoint);

                if (Vector3.Distance(transform.position, _grapplePoint) < 1.0f)
                {
                    _isGrappling = false;
                    m_PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
                    m_LineRenderer.enabled = false;
                }
            }
        }

        if (!_canGrapple)
        {
            _grappleCooldownTime -= Time.deltaTime;
            if (grappleCooldownSlider != null)
            {
                grappleCooldownSlider.value = _grappleCooldownTime;
            }

            if (_grappleCooldownTime <= 0)
            {
                _canGrapple = true;
                if (grappleCooldownSlider != null)
                {
                    grappleCooldownSlider.value = grappleCooldown;
                }
            }
        }
    }

    #endregion

    #region Gravity and Ground Check

    /// <summary>
    /// Applies gravity to the player.
    /// </summary>
    private void ApplyGravity()
    {
        m_PlayerVelocity.y += gravityValue * Time.deltaTime;
        m_CharacterController.Move(m_PlayerVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Checks if the player is grounded.
    /// </summary>
    private void HandleGroundCheck()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && m_PlayerVelocity.y < 0)
        {
            m_PlayerVelocity.y = -0.1f;  // Small buffer to keep the player grounded
            _jumpCount = 0;
        }
    }

    #endregion

    #region Miscellaneous

    /// <summary>
    /// Pauses or unpauses the player controller.
    /// </summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    #endregion
}