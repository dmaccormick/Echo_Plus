using UnityEngine;

public class Test_CameraFollow : MonoBehaviour
{
    //--- Public Variables ---//
    public Transform m_target;
    public Vector3 m_offset;



    //--- Unity Methods ---//
    void LateUpdate()
    {
        // Move to follow the target's position, but offset from it by the set amount
        this.transform.position = m_target.position + m_offset;
    }
}
