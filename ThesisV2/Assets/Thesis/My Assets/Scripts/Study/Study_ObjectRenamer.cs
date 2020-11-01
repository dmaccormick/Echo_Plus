using UnityEngine;

namespace Thesis.Study
{
    [DefaultExecutionOrder(-100)]
    public class Study_ObjectRenamer : MonoBehaviour
    {
        public GameObject[] m_objectsToRename;

        private void Awake()
        {
            // Get the player ID and convert it to a string
            int participantID = PlayerPrefs.GetInt("ParticipantID");
            string participantIDStr = participantID.ToString("D2");

            // Rename all of the objects so they have the player ID appended at the end
            foreach(var obj in m_objectsToRename)
            {
                obj.name = obj.name + " " + participantIDStr;
            }
        }
    }

}