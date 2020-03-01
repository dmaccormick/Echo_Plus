using UnityEngine;

namespace Thesis.Visualization.VisCam
{
    public class VisCam_FPSCam : MonoBehaviour
    {
        //--- Public Variables ---//
        public Camera m_cam;
        public Transform m_orbitCamPivot;
        public float m_movementSpeed;
        public float m_sprintMultiplier;
        public float m_rotationSpeed;



        //--- Private Variables ---//
        private Transform m_orbitCamPivotParent;



        //--- Methods ---//
        public void GrabPivot()
        {
            // Store a reference to the pivot's parent so we can re-establish their connection afterwards
            m_orbitCamPivotParent = m_orbitCamPivot.parent;

            // Move the camera so it is a direct child of the pivot's current parent
            m_cam.transform.parent = m_orbitCamPivotParent;

            // Make the orbit cam's pivot a child of the camera so it moves around with us as we move the FPS cam
            m_orbitCamPivot.parent = m_cam.transform;
        }

        public void ReleasePivot()
        {
            // Return if we haven't grabbed the pivot yet
            if (m_orbitCamPivotParent == null)
                return;

            // Put the pivot back as a child of its original parent
            m_orbitCamPivot.parent = m_orbitCamPivotParent;

            // Put the camera back as a child of the pivot
            m_cam.transform.parent = m_orbitCamPivot;

            // Clear the reference to the pivot's parent
            m_orbitCamPivotParent = null;
        }

        public void UpdateCamera(float _speedMultiplier)
        {
            // If "sprinting", the movement speeds should be multiplied
            float finalMoveSpeed = (Input.GetKey(KeyCode.LeftShift)) ? m_movementSpeed * m_sprintMultiplier : m_movementSpeed;

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

            //// Also, move the orbit camera's pivot point the same amount, to prevent issues when switching back to orbit cam
            //m_orbitCamPivot.transform.position += transformedMovement;

            // TODO: Try switching it so that the pivot point is actually a child of the CAMERA instead
            // That way, it stays in the same place relative to the camera and switches back afterwards

            // The user can rotate the camera by holding down left click
            if (Input.GetMouseButton(0))
            {
                // Get the mouse x and y
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                // Rotate yaw and pitch, but not roll
                // Need to invert the pitch to fix actual inversion
                float yawMovement = mouseX * m_rotationSpeed;
                float pitchMovement = -mouseY * m_rotationSpeed;

                // Perform the rotations
                m_cam.transform.Rotate(Vector3.up, yawMovement, Space.Self);
                //m_cam.transform.Rotate(Vector3.right, pitchMovement, Space.Self);
            }
        }
    }
}