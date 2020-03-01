using UnityEngine;

namespace Thesis.Visualization.VisCam
{
    public class VisCam_FPSCam : MonoBehaviour
    {
        //--- Public Variables ---//
        public Camera m_cam;
        public float m_movementSpeed;
        public float m_sprintMultiplier;
        public float m_rotationSpeed;



        //--- Methods ---//
        public void UpdateCamera(float _speedMultiplier)
        {
            // If "sprinting", the movement speeds should be multiplied
            float finalMoveSpeed = (Input.GetKey(KeyCode.LeftShift)) ? m_movementSpeed * m_sprintMultiplier : m_movementSpeed;

            // Also, should consider the speed multiplier that comes from the height of the camera
            finalMoveSpeed *= _speedMultiplier;

            // X and Z movement comes from WASD
            float xMovement = Input.GetAxisRaw("Horizontal") * finalMoveSpeed * Time.deltaTime;
            float zMovement = Input.GetAxisRaw("Vertical") * finalMoveSpeed * Time.deltaTime;
            float yMovement = 0.0f;

            // Y movement comes from space and LCTRL
            if (Input.GetKey(KeyCode.Space))
                yMovement = finalMoveSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftControl))
                yMovement = -finalMoveSpeed * Time.deltaTime;

            // Move along all of the axes
            this.transform.position += transform.TransformVector(new Vector3(xMovement, yMovement, zMovement));

            // The user can rotate the camera by holding down left click
            if (Input.GetMouseButton(0))
            {
                // Rotate yaw and pitch, but not roll
                float yawMovement = Input.GetAxisRaw("Mouse X") * m_rotationSpeed;
                float pitchMovement = -Input.GetAxisRaw("Mouse Y") * m_rotationSpeed; // Inverted to fix actual inversion

                // Perform the rotations
                // This has an issue where gimbal lock is a possibility
                this.transform.localEulerAngles += new Vector3(pitchMovement, yawMovement, 0.0f);
            }
        }
    }
}