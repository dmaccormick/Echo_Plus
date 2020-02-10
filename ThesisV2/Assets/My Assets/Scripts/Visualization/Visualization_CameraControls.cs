using UnityEngine;

public class Visualization_CameraControls : MonoBehaviour
{
    //--- Public Variables ---//
    [Header("Pan Controls")]
    public Camera m_cam;
    public float m_panningSpeed;
    public bool m_invertPan;

    [Header("Rotation Controls")]
    public Transform m_pivotPoint;
    public float m_rotationSpeed;

    [Header("Zoom Controls")]
    public float m_zoomSpeed;
    public float m_closestZoomDistance;
    public bool m_invertYaw;
    public bool m_invertPitch;



    //--- Private Variables ---//
    private bool m_controlsActive;



    //--- Unity Methods ---//
    private void Awake()
    {
        // Init the private variables
        m_controlsActive = true;
    }

    private void Update()
    {
        // Only control the camera if actually able to do so
        if (m_controlsActive)
        {
            // Get the mouse x, y, and scroll wheel
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            // Click the scroll wheel for pan, alt/left mouse for rotate, wheel for zoom in/out
            if (Input.GetMouseButton(2))
            {
                // The scroll wheel has been pressed in so we should pan
                Pan(mouseX, mouseY);
            }
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                // The left mouse and alt keys have been pressed so we should rotate
                RotateOrbit(mouseX, mouseY);
            }
            else if (mouseWheel != 0.0f)
            {
                // If the mouse wheel was moved, we should zoom in or out
                Zoom(mouseWheel);
            }
        }
    }



    //--- Methods ---//
    public void Pan(float _mouseX, float _mouseY)
    {
        // Create a movement vector for the up/down, left/right movement
        float moveX = _mouseX * Time.deltaTime * m_panningSpeed;
        float moveY = _mouseY * Time.deltaTime * m_panningSpeed;
        Vector3 movementDir = new Vector3(moveX, moveY, 0.0f);

        // Invert if need be
        movementDir = (m_invertPan) ? movementDir * -1.0f : movementDir;

        // Make the vector relative to the camera itself
        Vector3 transformedDir = m_cam.transform.TransformDirection(movementDir);

        // Now, move the actual pivot point instead so the camera goes with it
        m_pivotPoint.position += transformedDir;
    }

    public void RotateOrbit(float _mouseX, float _mouseY)
    {
        // Determine the amounts of pitch and yaw
        float pitch = _mouseY * m_rotationSpeed * Time.deltaTime;
        float yaw = _mouseX * m_rotationSpeed * Time.deltaTime;

        // Invert if need be
        pitch = (m_invertPitch) ? pitch * -1.0f : pitch;
        yaw = (m_invertYaw) ? yaw * -1.0f : yaw;

        // Rotate the pivot point around according to the pitch and yaw
        // Always use the global up vector but use the local right vector
        m_pivotPoint.Rotate(Vector3.right, pitch, Space.Self);
        m_pivotPoint.Rotate(Vector3.up, yaw, Space.World);
    }

    public void Zoom(float _mouseWheel)
    {
        // Get the camera's forward vec since that is where it will move along
        Vector3 camForward = m_cam.transform.forward;

        // Multiply the forward vector by the zoom amount to determine where to move to
        Vector3 zoomVec = camForward * m_zoomSpeed * _mouseWheel * Time.deltaTime;

        // Apply the movement to the camera
        m_cam.transform.position += zoomVec;

        // If the camera is too close to the pivot point, we need to push it back
        if (Vector3.Distance(m_cam.transform.position, m_pivotPoint.position) < m_closestZoomDistance)
        {
            // Get the opposite movement vector to the camera forward
            Vector3 camReverse = m_cam.transform.forward * -1.0f;

            // Scale the vector to match the closest zoom distance
            Vector3 closestZoomVec = camReverse * m_closestZoomDistance;

            // Place the camera at the end of the vector relative to the pivot point
            m_cam.transform.position = m_pivotPoint.position + closestZoomVec;
        }
    }

    public void ToggleControls()
    {
        // Toggle whether or not the controls are actually active
        m_controlsActive = !m_controlsActive;
    }
}
