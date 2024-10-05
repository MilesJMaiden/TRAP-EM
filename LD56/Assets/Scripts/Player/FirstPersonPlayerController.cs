using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayerController : MonoBehaviour
{
    public delegate void GrappleUsedHandler(); // Delegate for grapple hook usage
    public static event GrappleUsedHandler OnGrappleUsed; // Event for grapple hook usage

    private CharacterController m_CharacterController;
    private Vector3 m_PlayerVelocity;
    private InputManager m_InputManager;
    private Transform m_CameraTransform;
    private LineRenderer m_LineRenderer;

    [SerializeField]
    private float playerSpeed = 2.0f; // Speed of the player
    [SerializeField]
    private float sprintSpeed = 4.0f; // Speed when sprinting
    [SerializeField]
    private float crouchSpeed = 1.0f; // Speed when crouching
    [SerializeField]
    private float _currentSpeed; // Current speed of the player
    [SerializeField]
    private float jumpHeight = 1.0f; // Height of the jump
    [SerializeField]
    private float dashSpeed = 10.0f; // Speed during dash
    [SerializeField]
    private float dashDuration = 0.2f; // Duration of the dash
    [SerializeField]
    private float gravityValue = -9.81f; // Gravity value

    // New fields for ground detection
    [SerializeField]
    private Transform groundCheck; // Transform to check if the player is grounded
    [SerializeField]
    private float groundDistance = 0.1f; // Distance to the ground
    [SerializeField]
    private LayerMask groundMask; // Mask to determine what is ground

    private bool _isGrounded; // Boolean to store whether the player is grounded or not
    // Fields for Double Jump
    private int _jumpCount = 0; // Number of jumps performed
    private const int MaxJumpCount = 2; // Maximum number of jumps allowed
    // Fields for Crouching
    private bool _isCrouching = false; // Boolean to store whether the player is crouching or not
    // Fields for Dashing
    private bool _canDash = true; // Boolean to store whether the player can dash or not
    private bool _isDashing = false; // Boolean to store whether the player is dashing or not
    private float _dashTime; // Time remaining for the dash

    // Fields for Grapple Hook
    [SerializeField]
    private float grappleDistance = 20.0f; // Maximum distance for the grapple hook
    [SerializeField]
    private float grappleSpeed = 10.0f; // Speed of the grapple hook
    [SerializeField]
    private float grappleCooldown = 15.0f; // Cooldown time for the grapple hook
    private bool _canGrapple = true; // Boolean to store whether the player can use the grapple hook
    private bool _isGrappling = false; // Boolean to store whether the player is currently grappling
    private Vector3 _grapplePoint; // Point to grapple to
    private float _grappleCooldownTime; // Time remaining for the grapple cooldown
    // Reference to the GameObject containing the LineRenderer
    [SerializeField]
    private GameObject grappleLineObject;
    // Reference to the Grapple Cooldown Slider
    [SerializeField]
    private Slider grappleCooldownSlider;

    private bool isPaused = false; // Boolean to store whether the game is paused or not

    private Vector3 originalCenter; // Original center of the CharacterController

    [SerializeField]
    private GameObject CameraSource;  // Reference to the camera or object to move

    private Vector3 cameraOriginalPosition;  // To store the original position of the CameraSource

    // New field for camera tracking position



    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_InputManager = InputManager.Instance;

        // Store the original center of the CharacterController
        originalCenter = m_CharacterController.center;


        // Get the LineRenderer from the referenced GameObject
        if (grappleLineObject != null)
        {
            m_LineRenderer = grappleLineObject.GetComponent<LineRenderer>();
        }
        else
        {
            Debug.LogError("GrappleLineObject is not assigned in the Inspector.");
        }

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize LineRenderer
        if (m_LineRenderer != null)
        {
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.enabled = false;
        }

        _currentSpeed = playerSpeed; // Set the current speed to the player speed

        // Initialize Grapple Cooldown Slider
        if (grappleCooldownSlider != null)
        {
            grappleCooldownSlider.maxValue = grappleCooldown;
            grappleCooldownSlider.value = grappleCooldown;
        }
        else
        {
            Debug.LogError("GrappleCooldownSlider is not assigned in the Inspector.");
        }

        if (CameraSource != null)
        {
            cameraOriginalPosition = CameraSource.transform.localPosition;  // Store the original local position
        }
        else
        {
            Debug.LogError("CameraSource is not assigned in the Inspector.");
        }
    }

    void Update()
    {
        if (isPaused) return; // Do not update if the game is paused

        // Use CheckSphere for ground detection
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && m_PlayerVelocity.y < 0)
        {
            m_PlayerVelocity.y = -0.1f;
            _jumpCount = 0;
            _canDash = true; // Reset dash ability when grounded
        }

        Vector2 movement = m_InputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);

        if (_isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _isCrouching = false;
                m_CharacterController.height = 2.0f;
                m_CharacterController.center = originalCenter;
                CameraSource.transform.localPosition = cameraOriginalPosition;  // Reset camera to original position

                _currentSpeed = sprintSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                _isCrouching = true;
                m_CharacterController.height = 1.0f;
                m_CharacterController.center = new Vector3(originalCenter.x, -0.5f, originalCenter.z);
                CameraSource.transform.localPosition = new Vector3(cameraOriginalPosition.x, cameraOriginalPosition.y - 0.75f, cameraOriginalPosition.z);  // Move camera down

                _currentSpeed = crouchSpeed;
            }
            else
            {
                if (_isCrouching && !Input.GetKey(KeyCode.LeftControl))
                {
                    _isCrouching = false;
                    m_CharacterController.height = 2.0f;
                    m_CharacterController.center = originalCenter;
                    CameraSource.transform.localPosition = cameraOriginalPosition;  // Reset camera to original position
                }
                _currentSpeed = playerSpeed;
            }
        }


        // Handle dashing
        if (Input.GetKeyDown(KeyCode.E) && !_isGrounded && _canDash)
        {
            _isDashing = true;
            _dashTime = dashDuration;
            _canDash = false;
        }

        if (_isDashing)
        {
            if (_dashTime > 0)
            {
                m_CharacterController.Move(move * (Time.deltaTime * dashSpeed));
                _dashTime -= Time.deltaTime;
            }
            else
            {
                _isDashing = false;
            }
        }
        else
        {
            m_CharacterController.Move(move * (Time.deltaTime * _currentSpeed)); // Modified to use _currentSpeed
        }

        // Handle jumping
        if (m_InputManager.PlayerJumpedThisFrame() && _jumpCount < MaxJumpCount)
        {
            m_PlayerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            _jumpCount++;
            Debug.Log("isCrouched" + _isCrouching);
            Debug.Log("m_CharacterController.height: " + m_CharacterController.height);
            Debug.Log("isGrounded: " + _isGrounded);
        }

        m_PlayerVelocity.y += gravityValue * Time.deltaTime;
        m_CharacterController.Move(m_PlayerVelocity * Time.deltaTime);

        // Handle grapple hook
        if (Input.GetMouseButtonDown(1) && _canGrapple)
        {
            RaycastHit hit;
            if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out hit, grappleDistance, groundMask))
            {
                _isGrappling = true;
                _grapplePoint = hit.point;
                _canGrapple = false;
                _grappleCooldownTime = grappleCooldown;
                m_LineRenderer.enabled = true; // Enable the line renderer

                OnGrappleUsed?.Invoke(); // Trigger the event

                // Start the grapple cooldown
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

                // Update the line renderer positions
                m_LineRenderer.SetPosition(0, transform.position);
                m_LineRenderer.SetPosition(1, _grapplePoint);

                // Check if the player has reached the grapple point
                if (Vector3.Distance(transform.position, _grapplePoint) < 1.0f)
                {
                    _isGrappling = false;
                    m_PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue); // Small jump to prevent getting stuck
                    m_LineRenderer.enabled = false; // Disable the line renderer
                }
                else
                {
                    // Disable gravity while grappling
                    m_PlayerVelocity.y = 0;
                }
            }
        }

        // Handle grapple cooldown
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

        m_PlayerVelocity.y += gravityValue * Time.deltaTime;
        m_CharacterController.Move(m_PlayerVelocity * Time.deltaTime);
    }
    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    // To visualize the ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}