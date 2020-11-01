using UnityEngine;

namespace Thesis.Test
{
    public class Test_Spawner : MonoBehaviour
    {
        //--- Public Variables ---//
        public GameObject m_spawnObject;
        public KeyCode m_selectedKey;

        private int m_numSpawned = 0;



        //--- Unity Methods ---//
        private void Update()
        {
            // Spawn an object by pressing the given key
            if (Input.GetKeyUp(m_selectedKey))
            {
                Instantiate(m_spawnObject, this.transform.position, this.transform.rotation, null);
                m_numSpawned++;

                if (m_numSpawned >= 10)
                    FindObjectOfType<Study_AutomaticRecording>().StopRecordingAndSave();
            }
        }
    }
}