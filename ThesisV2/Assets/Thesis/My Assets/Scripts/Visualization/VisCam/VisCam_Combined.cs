using UnityEngine;
using UnityEngine.Events;
using Thesis.UI;

namespace Thesis.Visualization.VisCam
{
    //--- Event Classes ---//
    [System.Serializable]
    public class TargetChangeEvent : UnityEvent<Transform>
    {
    }

    public class VisCam_Combined : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Generic Controls")]
        public Camera m_cam;
        public float m_sprintMultiplier;

        [Header("FPS Cam Controls")]
        public float m_fpsMoveSpeed;
        public float m_fpsRotSpeed;

        [Header("Pan Controls")]
        public float m_panningSpeed;
        public bool m_invertPan;

        [Header("Rotation Controls")]
        public Transform m_pivotObj;
        public float m_rotationSpeed;
        public bool m_invertYaw;
        public bool m_invertPitch;

        [Header("Zoom Controls")]
        public float m_zoomSpeed;
        public float m_closestZoomDistance;
        public float m_focusDistance;

        [Header("Pick Controls")]
        public LayerMask m_pickLayers;
        public float m_maxFocusPickDist;
        public float m_pickCooldown; // How long after letting go of alt before we can pick again (prevents accidentally picking a new target right after orbiting)
        public VisCam_QuickFocus m_quickFocus;
        public GameObject m_pickIndicator;

        [Header("Mouse Cursors")]
        public Texture2D m_cursorPan;
        public Texture2D m_cursorPanDisabled;
        public Texture2D m_cursorOrbit;
        public Texture2D m_cursorFPS;

        [Header("Events")]
        public TargetChangeEvent m_onFocusTargetChanged;



        //--- Private Variables ---//
        private UI_VIsMenuMouseDetector m_mouseDetector;
        private Transform m_focusTarget;
        private Texture2D m_currentCursor;
        private Transform m_pivotParent;
        private bool m_canPick;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the events
            m_onFocusTargetChanged = new TargetChangeEvent();

            // Init the private variables
            m_mouseDetector = FindObjectOfType<UI_VIsMenuMouseDetector>();
            FocusTarget = null;
            m_canPick = true;
        }



        //--- Methods ---//
        public void UpdateCamera(float _speedMultiplier)
        {
            // Get the mouse x, y, and scroll wheel as they are used in the various systems
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");

            // Determine if the mouse is currently hovering over the main window or not
            bool mouseOverWindow = m_mouseDetector.IsMouseDetected;

            // Choose what cursor to use based on the inputs. It should be the basic one by default
            m_currentCursor = null;

            //--- FPS Camera Functionality ---//
            {
                // Grab the pivot when pressing the right click down and release it when letting go
                if (Input.GetMouseButtonDown(1))
                    GrabPivot();
                else if (Input.GetMouseButtonUp(1))
                    ReleasePivot();

                // Now, move in the FPS camera mode by holding the right click
                if (Input.GetMouseButton(1))
                {
                    // Change the mouse cursor to the FPS icon
                    m_currentCursor = m_cursorFPS;

                    // Perform the FPS movements
                    UpdateFPSMovement(_speedMultiplier);
                }
            }

            //--- Mouse Picking Functionality ---//
            {
                // Clear the focus target by pressing C
                if (Input.GetKeyDown(KeyCode.C))
                {
                    // Clear the target
                    FocusTarget = null;

                    // Update the quick selection ui
                    m_quickFocus.RemoveTempTarget();
                }

                // Follow the focus target if it has moved
                if (m_focusTarget != null)
                {
                    // Move the pivot point so it stays with the focus target
                    m_pivotObj.position = m_focusTarget.position;
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

                // If letting go of the orbit, temporarily disable picking to prevent accidentally picking right away
                if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
                {
                    // Disable picking for now
                    m_canPick = false;

                    // Reactivate it shortly
                    Invoke("EnablePicking", m_pickCooldown);
                }
            }

            //--- Zooming Functionality ---//
            {
                // If the mouse wheel has moved at all, we should control the zoom
                // We should only be able to do this if hovering over the main window
                if (mouseOverWindow && mouseWheel != 0.0f)
                {
                    Zoom(mouseWheel, _speedMultiplier);
                }

                // Press 'F' to zoom and focus on the targeted object
                if (m_focusTarget != null && Input.GetKeyDown(KeyCode.F))
                    ZoomToFocus();
            }

            //--- Panning Functionality ---//
            {
                // Pan by holding the middle click
                if (Input.GetMouseButton(2))
                {
                    // If panning is enabled, pan. Otherwise, show the disabled panning icon
                    // Panning is enabled when not currently focusing on a target
                    if (m_focusTarget == null)
                    {
                        // Change the mouse cursor to the grab icon
                        m_currentCursor = m_cursorPan;

                        // The scroll wheel has been pressed in so we should pan
                        Pan(mouseX, mouseY, _speedMultiplier);
                    }
                    else
                    {
                        // Change the mouse cursor to the disabled grab icon
                        m_currentCursor = m_cursorPanDisabled;
                    }
                }
            }

            // If there is a focus target, move the indicator to it and billboard to the camera
            if (m_focusTarget != null)
            {
                m_pickIndicator.transform.position = m_focusTarget.position;
                m_pickIndicator.SetActive(true);
                m_pickIndicator.transform.forward = m_cam.transform.position - m_pickIndicator.transform.position;
            }
            else
                m_pickIndicator.SetActive(false);

            // Set the final selected cursor based on what action is being performed
            Vector2 cursorOffset = (m_currentCursor == null) ? Vector2.zero : new Vector2(m_currentCursor.width / 2.0f, m_currentCursor.height / 2.0f);
            Cursor.SetCursor(m_currentCursor, cursorOffset, CursorMode.Auto);
        }



        //--- Methods ---//
        public void CheckForFocusTarget()
        {
            // If the camera is not currently active, back out
            if (!m_cam.enabled)
                return;

            // If currently panning or orbiting, we shouldn't allow for changing focus
            // We can check this by determining what the current cursor is
            if (m_currentCursor != m_cursorOrbit && m_currentCursor != m_cursorPan)
            {
                // If we can't pick since the cooldown isn't over, return
                if (!m_canPick)
                    return;

                // Create a ray that is fired from the mouse relative to the controllable camera
                Ray ray = m_cam.ScreenPointToRay(Input.mousePosition);

                // Fire the ray and check for a hit
                if (Physics.Raycast(ray, out var raycastHit, m_maxFocusPickDist, m_pickLayers))
                {
                    // If we hit an object, set is as the new focus target
                    SetNewFocusTarget(raycastHit.transform);

                    // Tell the selection UI about the new pick target
                    m_quickFocus.AddTempTarget(m_focusTarget);
                }
            }
        }

        public void SetNewFocusTarget(Transform _target)
        {
            // Set the focus target
            FocusTarget = _target;

            // If the target exists, move the pivot point to it
            if (m_focusTarget != null)
            {
                // Move the pivot to the target
                m_pivotObj.position = m_focusTarget.position;

                // Move the camera so that it is no longer offset from the pivot
                // Offsetting can occur when moving with the FPS cam
                float distFromPivot = Vector3.Distance(m_pivotObj.position, m_cam.transform.position);
                m_cam.transform.position = m_pivotObj.position + m_pivotObj.TransformVector(new Vector3(0.0f, 0.0f, -distFromPivot));
            }
        }

        public Transform ToggleFocusTarget(Transform _target)
        {
            // If it is the same target, turn it off
            // Otherwise, set it as the new focus target
            if (m_focusTarget == _target)
                SetNewFocusTarget(null);
            else
                SetNewFocusTarget(_target);

            // Return the focus target
            return m_focusTarget;
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
            m_pivotObj.position += transformedDir;
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
            m_pivotObj.Rotate(Vector3.right, pitch, Space.Self);
            m_pivotObj.Rotate(Vector3.up, yaw, Space.World);
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

        public void GrabPivot()
        {
            // Store a reference to the pivot's parent so we can re-establish their connection afterwards
            m_pivotParent = m_pivotObj.parent;

            // Move the camera so it is a direct child of the pivot's current parent
            m_cam.transform.parent = m_pivotParent;

            // Make the orbit cam's pivot a child of the camera so it moves around with us as we move the FPS cam
            m_pivotObj.parent = m_cam.transform;
        }

        public void ReleasePivot()
        {
            // Return if we haven't grabbed the pivot yet
            if (m_pivotParent == null)
                return;

            // Put the pivot back as a child of its original parent
            m_pivotObj.parent = m_pivotParent;

            // Put the camera back as a child of the pivot
            m_cam.transform.parent = m_pivotObj;

            // Clear the reference to the pivot's parent
            m_pivotParent = null;
        }

        public void UpdateFPSMovement(float _speedMultiplier)
        {
            // If "sprinting", the movement speeds should be multiplied
            float finalMoveSpeed = (Input.GetKey(KeyCode.LeftShift)) ? m_fpsMoveSpeed * m_sprintMultiplier : m_fpsMoveSpeed;

            // Also, should consider the speed multiplier that comes from the height of the camera
            finalMoveSpeed *= _speedMultiplier;

            // Get the movement axes
            float hAxis = Input.GetAxisRaw("Horizontal");
            float vAxis = Input.GetAxisRaw("Vertical");

            // X and Z movement comes from WASD
            float xMovement = hAxis * finalMoveSpeed * Time.deltaTime;
            float zMovement = vAxis * finalMoveSpeed * Time.deltaTime;
            float yMovement = 0.0f;

            // Y movement comes from space and LCTRL
            if (Input.GetKey(KeyCode.Space))
                yMovement = finalMoveSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftControl))
                yMovement = -finalMoveSpeed * Time.deltaTime;

            // Move along all of the axes, relative to the camera
            Vector3 movementVec = new Vector3(xMovement, yMovement, zMovement);
            Vector3 transformedMovement = m_cam.transform.TransformDirection(movementVec);
            m_cam.transform.position += transformedMovement;

            // The user can rotate the camera by holding down right click
            if (Input.GetMouseButton(1))
            {
                // Get the mouse x and y
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Rotate yaw and pitch, but not roll
                // Need to invert the pitch to fix actual inversion
                float yawMovement = mouseX * m_fpsRotSpeed;
                float pitchMovement = -mouseY * m_fpsRotSpeed;

                // Perform the rotations
                Vector3 eulerRotations = new Vector3(pitchMovement, yawMovement, 0.0f);
                Vector3 currentEuler = m_cam.transform.localRotation.eulerAngles;
                Vector3 newRot = eulerRotations + currentEuler;

                // Clamp the pitch to [-89, 89] to prevent flipping
                // Code adapted from here: https://answers.unity.com/questions/1382504/mathfclamp-negative-rotation-for-the-10th-million.html
                newRot = new Vector3(Mathf.Clamp((newRot.x <= 180) ? newRot.x : -(360 - newRot.x), -89.0f, 89.0f), newRot.y, newRot.z);

                // Apply the rotation
                m_cam.transform.rotation = Quaternion.Euler(newRot);
            }
        }

        public void EnablePicking()
        {
            // Turn picking back on
            m_canPick = true;
        }

        public void ZoomToFocus()
        {
            // Put the pivot directly back on the target
            m_pivotObj.position = m_focusTarget.position;

            // Calculate the vector from the target to the camera's current position
            //Vector3 vTargetToCam = m_cam.transform.position - m_focusTarget.position;

            // Scale it down to the focus distance set in the inspector
            //Vector3 vTargetToCamScaled = vTargetToCam.normalized * m_focusDistance;

            // Calculate the new position by offsetting from the target by the calculated vector
            //Vector3 newCamPos = m_focusTarget.position + vTargetToCamScaled;

            // Move the camera to the new position
            //m_cam.transform.position = newCamPos;
            m_cam.transform.position = m_pivotObj.position + m_pivotObj.TransformVector(new Vector3(0.0f, 0.0f, -m_focusDistance));
        }

        public void SetIndicatorVisible(bool _visible)
        {
            m_pickIndicator.SetActive(_visible);
        }



        //--- Setters ---//
        public Transform FocusTarget
        {
            get => m_focusTarget;
            set
            {
                // Set the target
                m_focusTarget = value;

                // Send the new target out in the event
                m_onFocusTargetChanged.Invoke(m_focusTarget);
            }
        }
    }
}