using UnityEngine;

namespace Echo.Visualization.VisCam
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
        public GameObject m_targetIndicator;      // This indicator stays at the center of the object that is currently being focussed on
        public SpriteRenderer m_targetRenderer;
        public Color m_targetFollowColour;
        public GameObject m_pointerIndicator;     // This indicator represents the end of the laser beam when picking an object
        public float m_focusTargetScaleDistance;



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
            {
                // Follow the target
                m_pivotPoint.position = m_focusTarget.position;

                // Set the targeting icon to be a different color to indicate that follow mode is on
                m_targetRenderer.color = m_targetFollowColour;
            }
            else
            {
                // Use the normal target icon colour
                m_targetRenderer.color = Color.white;
            }

            // Get the mouse x, y, and scroll wheel
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            // Show the pointing indicator and the target indicator if we should do so
            if (m_shouldShowTargetIndicator)
            {
                // If holding control, we should show the pointing icon, otherwise we should hide it
                if (Input.GetKey(KeyCode.LeftControl))
                    ShowPointer();
                else
                    HidePointer();

                // If a target was already picked, we should show the indicator inside of it as well, otherwise we should hide it
                if (m_focusTarget != null)
                    ShowTargetIndicator();
                else
                    HideTargetIndicator();
            }
            else
            {
                // Otherwise, hide them
                HidePickingTargetIcons();
            }

            // Handle all of the controls for the camera system
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
            else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl)) // Right mouse + left alt + left control
            {
                // Clear the focus target
                m_focusTarget = null;

                // Stop following the focus target
                m_followFocusTarget = false;
            }
            else if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl)) // Right mouse + left control
            {
                // If shift and right click were pressed, move towards a new focus target
                CheckForFocusTarget();
            }
            else if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftControl)) // F + left control
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
            else if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl)) // R + Left control
            {
                // Reset the view to the focus target OR just go all the way back to 0,0,0
                if (m_focusTarget != null)
                {
                    m_pivotPoint.transform.position = m_focusTarget.position;
                    MoveToMaxZoomPoint();
                }
                else
                {
                    m_pivotPoint.transform.position = Vector3.zero;
                }
            }
            else if (Input.GetKeyDown(KeyCode.H) && Input.GetKey(KeyCode.LeftControl)) // H + Left Control
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
                HidePickingTargetIcons();
            }
        }

        public void ShowPointer()
        {
            // Create a ray that is fired from the mouse relative to the controllable camera
            Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);

            // Fire the ray and check for a hit
            if (Physics.Raycast(ray, out var raycastHit, m_maxFocusPickDist, m_pickLayers))
            {
                // Enable the pointer picking target
                m_pointerIndicator.SetActive(true);

                // If we hit an object put the indicator at the location of the hit
                m_pointerIndicator.transform.position = raycastHit.point;
                m_pointerIndicator.transform.forward = raycastHit.normal;
                m_pointerIndicator.transform.position += m_pointerIndicator.transform.forward * 0.1f;
            }
            else
            {
                // Hide the pointer if nothing is hit
                m_pointerIndicator.SetActive(false);
            }
        }

        public void ShowTargetIndicator()
        {
            // Enable the locked target
            m_targetIndicator.SetActive(true);

            // Move the target indicator to the middle of the focus target
            m_targetIndicator.transform.position = m_focusTarget.position;

            // Billboard to the camera
            Vector3 billboardVec = -m_cam.transform.forward;
            m_targetIndicator.transform.forward = billboardVec;

            // Calculate a scale based on the distance to the target. If under the distance range, scale normally. Otherwise, use the range as a multiplier
            float distToTarget = Vector3.Distance(m_cam.transform.position, m_targetIndicator.transform.position);
            float distanceMultiplier = (distToTarget < m_focusTargetScaleDistance) ? 1.0f : distToTarget / m_focusTargetScaleDistance;

            // Scale the object
            m_targetIndicator.transform.localScale = Vector3.one * distanceMultiplier;
        }

        public void HidePointer()
        {
            // Hide the pointer
            m_pointerIndicator.SetActive(false);
        }

        public void HideTargetIndicator()
        {
            // Hide the focus indicator
            m_targetIndicator.SetActive(false);
        }

        public void HidePickingTargetIcons()
        {
            // Hide both icons
            HidePointer();
            HideTargetIndicator();
        }

        public void StopFollowing()
        {
            // Disable the follow
            m_followFocusTarget = false;
        }
    }
}
