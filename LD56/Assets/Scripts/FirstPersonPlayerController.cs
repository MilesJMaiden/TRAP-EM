using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayerController : MonoBehaviour
{
    private CharacterController m_CharacterController;
    private Vector3 m_PlayerVelocity;
    private InputManager m_InputManager;
    private Transform m_CameraTransform;

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float sprintSpeed = 4.0f;
    [SerializeField]
    private float crouchSpeed = 1.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    // New fields for ground detection
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundDistance = 0.1f;
    [SerializeField]
    private LayerMask groundMask;

    private bool _isGrounded;
    // Fields for Double Jump
    private int _jumpCount = 0;
    private const int MaxJumpCount = 2;
    // Fields for Crouching
    private bool _isCrouching = false;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_InputManager = InputManager.Instance;
        if (Camera.main != null) m_CameraTransform = Camera.main.transform;

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        // Use CheckSphere for ground detection
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && m_PlayerVelocity.y < 0)
        {
            m_PlayerVelocity.y = -0.1f;
            _jumpCount = 0;
        }

        Vector2 movement = m_InputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0f, movement.y);
        move = m_CameraTransform.forward * move.z + m_CameraTransform.right * move.x;

        // Handle sprinting and crouching
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _isCrouching = false;
            m_CharacterController.height = 2.0f; // Reset height if previously crouched
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            _isCrouching = true;
            m_CharacterController.height = 1.0f; // Reduce height for crouching
        }
        else
        {
            _isCrouching = false;
            m_CharacterController.height = 2.0f; // Reset height if previously crouched
        }

        float currentSpeed = _isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : playerSpeed);
        m_CharacterController.Move(move * (Time.deltaTime * currentSpeed));

        // Handle jumping
        if (m_InputManager.PlayerJumpedThisFrame() && _jumpCount < MaxJumpCount)
        {
            m_PlayerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            _jumpCount++;
        }

        m_PlayerVelocity.y += gravityValue * Time.deltaTime;
        m_CharacterController.Move(m_PlayerVelocity * Time.deltaTime);
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