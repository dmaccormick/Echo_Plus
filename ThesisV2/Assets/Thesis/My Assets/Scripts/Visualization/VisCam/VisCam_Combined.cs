using UnityEngine;

namespace Thesis.Visualization.VisCam
{
    public class VisCam_Combined : MonoBehaviour
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
        public bool m_invertYaw;
        public bool m_invertPitch;

        [Header("Zoom Controls")]
        public float m_zoomSpeed;
        public float m_closestZoomDistance;

        [Header("Focus Controls")]
        public LayerMask m_pickLayers;
        public float m_maxFocusPickDist;

        [Header("Mouse Cursors")]
        public Texture2D m_cursorPan;
        public Texture2D m_cursorOrbit;
        public Texture2D m_cursorFPS;



        //--- Private Variables ---//
        private Transform m_focusTarget;
        private Texture2D m_currentCursor;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_focusTarget = null;
        }



        //--- Methods ---//
        public void UpdateCamera(float _speedMultiplier)
        {
            // Get the mouse x, y, and scroll wheel as they are used in the various systems
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            // Choose what cursor to use based on the inputs. It should be the basic one by default
            m_currentCursor = null;



            //--- FPS Camera Functionality ---//
            {
                // Initiate the FPS camera mode by holding the right click
                if (Input.GetMouseButton(1))
                {
                    // Change the mouse cursor to the FPS icon
                    m_currentCursor = m_cursorFPS;
                }
            }



            //--- Mouse Picking Functionality ---//
            {
                // Clear the focus target by pressing the right mouse button
                if (Input.GetMouseButtonDown(1))
                    m_focusTarget = null;

                // Follow the focus target if it has moved
                if (m_focusTarget != null)
                {
                    // Move the pivot point so it stays with the focus target
                    m_pivotPoint.position = m_focusTarget.position;
                }
            }



            //--- Orbiting Functionality ---//
            {
                // Holding alt begins orbiting
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    // Change the mouse cursor to the orbit icon
                    m_currentCursor = m_cursorOrbit;

                    // Need to also hold the left mouse button to rotate the orbit
                    if (Input.GetMouseButton(0))
                        RotateOrbit(mouseX, mouseY);
                }
            }



            //--- Zooming Functionality ---//
            {
                // If the mouse wheel has moved at all, we should control the zoom
                if (mouseWheel != 0.0f)
                {
                    Zoom(mouseWheel, _speedMultiplier);
                }
            }



            //--- Panning Functionality ---//
            {
                // Pan by holding the middle click
                if (Input.GetMouseButton(2))
                {
                    // Change the mouse cursor to the grab icon
                    m_currentCursor = m_cursorPan;

                    // The scroll wheel has been pressed in so we should pan
                    Pan(mouseX, mouseY, _speedMultiplier);
                }
            }



            // Set the final selected cursor based on what action is being performed
            Vector2 cursorOffset = (m_currentCursor == null) ? Vector2.zero : new Vector2(m_currentCursor.width / 2.0f, m_currentCursor.height / 2.0f);
            Cursor.SetCursor(m_currentCursor, cursorOffset, CursorMode.Auto);
        }



        //--- Utility Functions ---//
        public void CheckForFocusTarget()
        {
            // If currently panning or orbiting, we shouldn't allow for changing focus
            // We can check this by determining what the current cursor is
            if (m_currentCursor != m_cursorOrbit && m_currentCursor != m_cursorPan)
            {
                // Create a ray that is fired from the mouse relative to the controllable camera
                Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);

                // Fire the ray and check for a hit
                if (Physics.Raycast(ray, out var raycastHit, m_maxFocusPickDist, m_pickLayers))
                {
                    // If we hit an object, move the pivot point to it and set it as the focus target
                    m_focusTarget = raycastHit.transform;
                    m_pivotPoint.position = m_focusTarget.position;
                }
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

            // Multiply the forward vector by the zoom amount to determine how much to move
            Vector3 zoomVec = camForward * _mouseWheel * Time.deltaTime * finalSpeed;

            // Calculate the new position
            Vector3 newCamPosition = m_cam.transform.position + zoomVec;

            // Use a raycast to determine if the new position is too close to the object
            // If so, move back a bit
            if (Physics.Raycast(newCamPosition, m_cam.transform.forward, out var hitObj, m_maxFocusPickDist, m_pickLayers))
            {
                // Determine the distance to the hit point
                Vector3 vecToHit = hitObj.point - newCamPosition;
                float dist = vecToHit.magnitude;

                // If the distance is too close, we should push back
                if (dist < m_closestZoomDistance)
                {
                    // Flip the vector, normalize it, and scale it to find out how far we need to move back
                    Vector3 reverseVec = vecToHit.normalized * -m_closestZoomDistance;

                    // Determine the new position by moving back from the hit point using the reversed vector
                    newCamPosition = hitObj.point + reverseVec;
                }
            }

            // Apply the movement to the camera
            m_cam.transform.position = newCamPosition;
        }
    }
}