using UnityEngine;

namespace Thesis.Visualization.VisCam
{
    public class VisCam_OrbitCam : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Generic Controls")]
        public Camera m_cam;
        public float m_sprintMultiplier;

        [Header("Pan Controls")]
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

        [Header("Focus Controls")]
        public LayerMask m_pickLayers;
        public float m_maxFocusPickDist;
        public GameObject m_targetIndicator;



        //--- Private Variables ---//
        private Transform m_focusTarget;
        private bool m_followFocusTarget;
        private bool m_shouldShowTargetIndicator;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_focusTarget = null;
            m_followFocusTarget = false;
            m_shouldShowTargetIndicator = true;
        }



        //--- Methods ---//
        public void UpdateCamera(float _speedMultiplier)
        {
            // If there is a focus target, the pivot point should always move with them
            // We don't want to parent the pivot point because then rotations would mess it up
            if (m_focusTarget != null && m_followFocusTarget)
                m_pivotPoint.position = m_focusTarget.position;

            // Get the mouse x, y, and scroll wheel
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            // When holding shift, move the target indicator so that it shows where the pointer is aiming
            // When not holding shift, leave the indicator inside of the already picked target OR hide it entirely if there is no picked target
            if (Input.GetKey(KeyCode.LeftShift) && m_shouldShowTargetIndicator)
                ShowPickingTarget();
            else if (m_focusTarget != null && m_shouldShowTargetIndicator)
                DisplayPickedTargetIndicator();
            else
                HidePickingTarget();

            // Click the scroll wheel for pan, alt/left mouse for rotate, wheel for zoom in/out
            // Also, shift/right click to focus on a different object
            // Also, alt/right click clears the focus
            // Also shift/F will follow the focus target
            if (Input.GetMouseButton(2)) // Middle click
            {
                // We cannot pan if we are following the focus target since we are moving with them the whole time
                if (!m_followFocusTarget)
                {
                    // The scroll wheel has been pressed in so we should pan
                    Pan(mouseX, mouseY, _speedMultiplier);
                }
            }
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)) // Left mouse + left alt
            {
                // The left mouse and alt keys have been pressed so we should rotate
                RotateOrbit(mouseX, mouseY);
            }
            else if (mouseWheel != 0.0f)
            {
                // If the mouse wheel was moved, we should zoom in or out
                Zoom(mouseWheel, _speedMultiplier);
            }
            else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift)) // Right mouse + left shift
            {
                // If shift and right click were pressed, move towards a new focus target
                CheckForFocusTarget();
            }
            else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftAlt)) // Right mouse + left alt
            {
                // Clear the focus target
                m_focusTarget = null;

                // Stop following the focus target
                m_followFocusTarget = false;
            }
            else if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift)) // F + left shift
            {
                // If there is a focus target, we should toggle following it around
                // Also use the same key to turn off the folow
                if (m_focusTarget != null)
                {
                    // Toggle focus on the target
                    m_followFocusTarget = !m_followFocusTarget;

                    // Zoom all the way if we are now following
                    if (m_followFocusTarget)
                        MoveToMaxZoomPoint();
                }
            }
            else if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift)) // R + Left Shift
            {
                // Reset the view to the focus target OR just go all the way back to 0,0,0
                if (m_focusTarget != null)
                {
                    m_pivotPoint.transform.position = m_focusTarget.position;
                }
                else
                {
                    m_pivotPoint.transform.position = Vector3.zero;
                }
            }
            else if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftShift)) // H + Left Shift
            {
                // Toggle whether or not we should draw the targeting indicator at all
                m_shouldShowTargetIndicator = !m_shouldShowTargetIndicator;
            }
        }

        public void Pan(float _mouseX, float _mouseY, float _speedMultiplier)
        {
            // If "sprinting", need to include that as well
            float finalSpeed = m_panningSpeed * _speedMultiplier;
            finalSpeed = (Input.GetKey(KeyCode.LeftShift)) ? finalSpeed * m_sprintMultiplier : finalSpeed;

            // Create a movement vector for the up/down, left/right movement
            float moveX = _mouseX * Time.deltaTime * finalSpeed;
            float moveY = _mouseY * Time.deltaTime * finalSpeed;
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
            // If "sprinting", need to include that as well
            float finalSpeed = m_rotationSpeed;
            finalSpeed = (Input.GetKey(KeyCode.LeftShift)) ? finalSpeed * m_sprintMultiplier : finalSpeed;

            // Determine the amounts of pitch and yaw
            float pitch = _mouseY * finalSpeed * Time.deltaTime;
            float yaw = _mouseX * finalSpeed * Time.deltaTime;

            // Invert if need be
            pitch = (m_invertPitch) ? pitch * -1.0f : pitch;
            yaw = (m_invertYaw) ? yaw * -1.0f : yaw;

            // Rotate the pivot point around according to the pitch and yaw
            // Always use the global up vector but use the local right vector
            m_pivotPoint.Rotate(Vector3.right, pitch, Space.Self);
            m_pivotPoint.Rotate(Vector3.up, yaw, Space.World);
        }

        public void Zoom(float _mouseWheel, float _speedMultiplier)
        {
            // If "sprinting", need to include that as well
            float finalSpeed = m_zoomSpeed * _speedMultiplier;
            finalSpeed = (Input.GetKey(KeyCode.LeftShift)) ? finalSpeed * m_sprintMultiplier : finalSpeed;

            // Get the camera's forward vec since that is where it will move along
            Vector3 camForward = m_cam.transform.forward;

            // Multiply the forward vector by the zoom amount to determine where to move to
            Vector3 zoomVec = camForward * _mouseWheel * Time.deltaTime * finalSpeed;

            // Apply the movement to the camera
            m_cam.transform.position += zoomVec;

            // If the camera is too close to the pivot point, we need to push it back
            if (Vector3.Distance(m_cam.transform.position, m_pivotPoint.position) < m_closestZoomDistance)
            {
                // Move to the very edge of the zoom position
                MoveToMaxZoomPoint();
            }
        }

        public void MoveToMaxZoomPoint()
        {
            // Get the opposite movement vector to the camera forward
            Vector3 camReverse = m_cam.transform.forward * -1.0f;

            // Scale the vector to match the closest zoom distance
            Vector3 closestZoomVec = camReverse * m_closestZoomDistance;

            // Place the camera at the end of the vector relative to the pivot point
            m_cam.transform.position = m_pivotPoint.position + closestZoomVec;
        }

        public void CheckForFocusTarget()
        {
            // Create a ray that is fired from the mouse relative to the controllable camera
            Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);

            // Fire the ray and check for a hit
            if (Physics.Raycast(ray, out var raycastHit, m_maxFocusPickDist, m_pickLayers))
            {
                // If we hit an object, move the pivot point to it and set it as the focus target
                m_focusTarget = raycastHit.transform;
                m_pivotPoint.position = m_focusTarget.position;

                // Stop following the old target
                m_followFocusTarget = false;
            }
            else
            {
                // If we didn't find a target, we should hide the indicator
                HidePickingTarget();
            }
        }

        public void ShowPickingTarget()
        {
            // Create a ray that is fired from the mouse relative to the controllable camera
            Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);

            // Fire the ray and check for a hit
            if (Physics.Raycast(ray, out var raycastHit, m_maxFocusPickDist, m_pickLayers))
            {
                // Enable the picking target
                m_targetIndicator.SetActive(true);

                // If we hit an object put the indicator at the location of the hit
                m_targetIndicator.transform.position = raycastHit.point;
                m_targetIndicator.transform.forward = raycastHit.normal;
                m_targetIndicator.transform.position += m_targetIndicator.transform.forward * 0.1f;
            }
        }

        public void DisplayPickedTargetIndicator()
        {
            // Enable the picking target
            m_targetIndicator.SetActive(true);

            // Move the target indicator to the middle of the focus target
            m_targetIndicator.transform.position = m_focusTarget.position;

            // Billboard to the camera
            Vector3 billboardVec = -m_cam.transform.forward;
            m_targetIndicator.transform.forward = billboardVec;
        }

        public void HidePickingTarget()
        {
            // Don't show the picking target at all
            m_targetIndicator.SetActive(false);
        }
    }
}
