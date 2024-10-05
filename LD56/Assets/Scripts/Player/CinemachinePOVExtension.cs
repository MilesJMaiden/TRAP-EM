using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachinePOVExtension : CinemachineExtension
{
    private InputManager m_InputManager;
    private Vector3 m_StartingRotation;
    private bool m_RotationInitialized = false;

    [SerializeField]
    private float horizontalCameraSpeed = 10f;
    [SerializeField]
    private float verticalCameraSpeed = 10f;
    [SerializeField]
    private float cameraClampAngle = 90f;

    protected override void Awake()
    {
        m_InputManager = InputManager.Instance;
        base.Awake();
    }

    /// <summary>
    /// Callback for Cinemachine to adjust camera post-pipeline.
    /// Applies custom rotation based on player input for first-person view control.
    /// </summary>
    /// <param name="vcam">The virtual camera to adjust.</param>
    /// <param name="stage">The stage in the Cinemachine pipeline.</param>
    /// <param name="state">The current camera state.</param>
    /// <param name="deltaTime">Time elapsed since the last frame.</param>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        // Early exit if the virtual camera or input manager is not properly set up
        if (vcam == null || m_InputManager == null) return;

        // Proceed only if the camera is in the Aim stage and has a follow target
        if (vcam.Follow && stage == CinemachineCore.Stage.Aim)
        {
            // Initialize starting rotation only once to avoid setting it every frame
            if (!m_RotationInitialized)
            {
                m_StartingRotation = transform.localRotation.eulerAngles;
                m_RotationInitialized = true;
            }

            // Retrieve mouse movement input from the InputManager
            Vector2 deltaInput = m_InputManager.GetMouseDelta();

            // Adjust the starting rotation based on input and camera speed settings
            m_StartingRotation.x += deltaInput.x * verticalCameraSpeed * deltaTime;
            m_StartingRotation.y += deltaInput.y * horizontalCameraSpeed * deltaTime;

            // Clamp the vertical rotation to prevent over-rotation beyond the set limits
            m_StartingRotation.y = Mathf.Clamp(m_StartingRotation.y, -cameraClampAngle, cameraClampAngle);

            // Apply the calculated rotation to the camera's orientation
            state.RawOrientation = Quaternion.Euler(-m_StartingRotation.y, m_StartingRotation.x, 0f);
        }
    }
}
