
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager m_Instance;
    private FirstPersonControls m_FirstPersonControls;

    private void Awake()
    {
        if(m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            m_Instance = null;
        } 
        else
        {
            m_Instance= this;
        }

        m_FirstPersonControls = new FirstPersonControls();
    }

    private void OnEnable()
    {
        m_FirstPersonControls.Enable();
    }

    private void OnDisable()
    {
        m_FirstPersonControls.Disable();
    }

    //Helper function
    public Vector2 GetPlayerMovement() 
    {
        return m_FirstPersonControls.FirstPersonPlayer.Movement.ReadValue<Vector2>();
    }

    //Helper function
    public Vector2 GetMouseDelta()
    {
        return m_FirstPersonControls.FirstPersonPlayer.Look.ReadValue<Vector2>();
    }

    //Helper function
    public bool PlayerJumpedThisFrame()
    {
        return m_FirstPersonControls.FirstPersonPlayer.Jump.triggered;
    }

    public static InputManager Instance
    {
        get { return m_Instance; }
    }
}
