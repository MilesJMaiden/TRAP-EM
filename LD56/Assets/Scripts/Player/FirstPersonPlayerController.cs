using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

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
    [SerializeField] private float playerSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.5f;

    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Grapple Hook")]
    [SerializeField] private float grappleDistance = 100.0f;
    [SerializeField] private float grappleSpeed = 50.0f;
    [SerializeField] private float grappleCooldown = 15.0f;
    [SerializeField] private GameObject grappleLineObject;
    [SerializeField] private Slider grappleCooldownSlider;

    [Header("Dash Settings")]
    [SerializeField] private float dashCooldown = 5.0f;
    [SerializeField] private float dashSpeed = 50.0f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private Slider dashCooldownSlider;


    [Header("Camera Settings")]
    [SerializeField] private GameObject CameraSource;  // Reference to the camera or object to move

    [Header("Detection Settings")]
    [SerializeField] private SphereCollider detectionCollider; // Reference to the SphereCollider
    [SerializeField] private float playerDetectionRadius = 3.0f; // Radius to detect objects
    [SerializeField] private float sprintingDetectionRadius = 7.0f; // Radius to detect objects while sprinting
    [SerializeField] private float crouchingDetectionRadius = 1.0f; // Radius to detect objects while crouching

    [Header("Butterfly Net Settings")]
    [SerializeField] private ButterflyNet butterflyNet; // Reference to the ButterflyNet script


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

        HandleSliderCheck();
        HandleGroundCheck();
        HandleMovement();
        HandleCrouching();
        HandleJumping();
        HandleGrappleHook();
        HandleButterflyNet();
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

        // Initialize the dash cooldown slider
        if (dashCooldownSlider != null)
        {
            dashCooldownSlider.maxValue = dashCooldown;
            dashCooldownSlider.value = dashCooldown;
        }
        else
        {
            Debug.LogError("dashCooldownSlider is not assigned!");
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
        // Check if the detectionCollider is assigned
        if (detectionCollider == null)
        {
            Debug.LogError("detectionCollider is not assigned!");
        }



        _currentSpeed = playerSpeed; // Set the default player speed
    }

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
        Vector3 cameraRight = m_CameraTransform.right;
        cameraRight.y = 0f; // Zero out Y to keep the player upright
        cameraRight.Normalize(); // Normalize to maintain direction magnitude

        move = cameraForward * move.z + cameraRight * move.x;

        _currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : playerSpeed;
        m_CharacterController.Move(move * (Time.deltaTime * _currentSpeed));

        UpdateDetectionRadius(); // Update the detection radius

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

                    UpdateDetectionRadius(); // Update the detection radius
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

                UpdateDetectionRadius(); // Update the detection radius
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
            _dashCooldownTime -= Time.deltaTime;
            if (dashCooldownSlider != null)
            {
                dashCooldownSlider.value = dashCooldown - _dashCooldownTime;
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

        if (Input.GetKeyDown(KeyCode.F) && _canDash)
        {
            _isDashing = true;
            _dashTime = dashDuration;
            _canDash = false;
            _dashCooldownTime = dashCooldown;
            OnDashUsed?.Invoke(); // Trigger the event

            if (dashCooldownSlider != null)
            {
                dashCooldownSlider.value = 0;
            }
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
            RaycastHit hit;
            if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out hit, grappleDistance, groundMask))
            {
                _isGrappling = true;
                _grapplePoint = hit.point;
                _canGrapple = false;
                _grappleCooldownTime = grappleCooldown;
                m_LineRenderer.enabled = true;
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
                else
                {
                    // Disable gravity while grappling
                    m_PlayerVelocity.y = 0;
                }
            }
        }

        if (!_canGrapple)
        {
            _grappleCooldownTime -= Time.deltaTime;
            if (grappleCooldownSlider != null)
            {
                grappleCooldownSlider.value = grappleCooldown - _grappleCooldownTime;
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

        // Stop the force when the player lands
        if (_isGrounded && m_PlayerVelocity.y < 0)
        {
           m_PlayerVelocity = Vector3.zero;
        }
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

    #region Coroutines

    public void HandleSliderCheck()
    {
        if (dashCooldownSlider == null)
        {
            StartCoroutine(FindDashCooldownSlider());
        }
        if (grappleCooldownSlider == null)
        {
            StartCoroutine(FindGrappleCooldownSlider());
        }
    }

    /// <summary>
    /// Coroutine to find the grapple cooldown slider if it is null.
    /// </summary>
    private IEnumerator FindGrappleCooldownSlider()
    {
        while (grappleCooldownSlider == null)
        {
            grappleCooldownSlider = GameObject.Find("Grapple Progress Slider")?.GetComponent<Slider>();



            yield return null;
        }
    }

    /// <summary>
    /// Coroutine to find the dash cooldown slider if it is null.
    /// </summary>
    private IEnumerator FindDashCooldownSlider()
    {
        while (dashCooldownSlider == null)
        {
            dashCooldownSlider = GameObject.Find("Dash Progress Slider")?.GetComponent<Slider>();
            yield return null;
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
    /// <summary>
    /// Handles interaction with Landmines.
    /// </summary>
    /// <param name="force"></param>
    public void ApplyExplosionForce(Vector3 force, float maxDistance)
    {
        m_PlayerVelocity += force;
        StartCoroutine(StopForceAfterDistance(maxDistance));
    }

    /// <summary>
    /// Coroutine to stop the force after a given distance.
    /// </summary>
    private IEnumerator StopForceAfterDistance(float maxDistance)
    {
        float distanceTraveled = 0f;
        Vector3 previousPosition = transform.position;

        while (distanceTraveled < maxDistance)
        {
            yield return null;
            distanceTraveled += Vector3.Distance(previousPosition, transform.position);
            previousPosition = transform.position;
        }

        m_PlayerVelocity = Vector3.zero;
    }

    /// <summary>
    /// Updates the detection radius based on the player's current state.
    /// </summary>
    private void UpdateDetectionRadius()
    {
        if (_isCrouching)
        {
            detectionCollider.radius = crouchingDetectionRadius;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            detectionCollider.radius = sprintingDetectionRadius;
        }
        else
        {
            detectionCollider.radius = playerDetectionRadius;
        }
    }

    /// <summary>
    /// Handles the usage of the butterfly net.
    /// </summary>
    private void HandleButterflyNet()
    {
        if (m_InputManager.PlayerShotThisFrame()) // Change the key as needed
        {
            if (butterflyNet != null)
            {
                butterflyNet.Swing();
            }
            else
            {
                Debug.LogError("ButterflyNet is not assigned!");
            }
        }
    }

    



    #endregion
}