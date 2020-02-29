using UnityEngine;

namespace Thesis.Test
{
    public class Test_Mover : MonoBehaviour
    {
        //--- Public Variables ---//
        public float m_moveSpeed;
        public float m_rotSpeed;
        public float m_sclSpeed;



        //--- Unity Methods ---//
        private void Update()
        {
            // Move the character
            HandleMovement();

            // Rotate the character
            HandleRotation();

            // Scale the character
            HandleScaling();
        }



        //--- Methods ---//
        public void HandleMovement()
        {
            // Get the horizontal and vertical movement axes
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Move according to the given axes
            float moveX = h * m_moveSpeed * Time.deltaTime;
            float moveZ = v * m_moveSpeed * Time.deltaTime;
            this.transform.position += new Vector3(moveX, 0.0f, moveZ);
        }

        public void HandleRotation()
        {
            // Spin left using the Q button and right using the E button
            if (Input.GetKey(KeyCode.Q))
                this.transform.Rotate(this.transform.up, -m_rotSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.E))
                this.transform.Rotate(this.transform.up, m_rotSpeed * Time.deltaTime);
        }

        public void HandleScaling()
        {
            // Scale up using the T button and down using the G button
            if (Input.GetKey(KeyCode.T))
                this.transform.localScale += Vector3.one * m_sclSpeed * Time.deltaTime;
            else if (Input.GetKey(KeyCode.G))
                this.transform.localScale -= Vector3.one * m_sclSpeed * Time.deltaTime;
        }
    }
}