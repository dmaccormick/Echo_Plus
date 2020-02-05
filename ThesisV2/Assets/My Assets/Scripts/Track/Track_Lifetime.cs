using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Recording;
using Thesis.Misc;
using System.Collections.Generic;
using System.Text;

namespace Thesis.Track
{
    [RequireComponent(typeof(Recording_Object))]
    public class Track_Lifetime : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Lifetime
        {
            public Data_Lifetime(float _timestamp, bool _isActive)
            {
                this.m_timestamp = _timestamp;
                this.m_isActiveFlag = _isActive;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + m_isActiveFlag.ToString();
            }

            public float m_timestamp;
            public bool m_isActiveFlag;
        }



        //--- Public Variables ---//
        public string m_dataFormat = "F3";



        //--- Private Variables ---//
        private List<Data_Lifetime> m_dataPoints;
        private bool m_isActive;
        private bool m_isRecording = false; // Cannot start recording right away, has to wait



        //--- Unity Methods ---//
        private void OnEnable()
        {
            // Only track this event if recording is active
            if (m_isRecording)
            {
                // The object is now active
                m_isActive = true;

                // We should record the change
                RecordData();
            }
        }

        private void OnDisable()
        {
            // Only track this event if recording is active
            if (m_isRecording)
            {
                // The object is now disabled so it is not active
                m_isActive = false;

                // We should record the change
                RecordData();
            }
        }

        private void OnDestroy()
        {
            // Only track this event if recording is active
            if (m_isRecording)
            {
                // The object has been destroyed so it is not active
                m_isActive = false;

                // We should record the change
                RecordData();
            }
        }



        //--- IRecordable Interface ---//
        public void StartRecording()
        {
            // Init the private variables 
            // NOTE: Use the shared mesh and material to prevent a duplicate from being created and removing the mesh path references
            // NOTE: The meshes need to be marked as read and write in the import settings!
            m_dataPoints = new List<Data_Lifetime>();

            // Recording is now active
            // NOTE: This track needs this because its triggers are outside of the normal update track loop
            m_isRecording = true;

            // The object is active for now
            m_isActive = true;

            // Record the first data point
            RecordData();
        }

        public void EndRecording()
        {
            // No longer recording
            m_isRecording = false;

            // Record the final data point
            RecordData();
        }

        public void UpdateRecording(float _elapsedTime)
        {
            // This class does not have anything here
            // TODO: Split the interface so we don't have this non-implemented function
        }

        public void RecordData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData()");

            // Get the current timestamp
            // TODO: Replace with a timer from the recording manager since if the game is paused, this will show 0
            float currentTime = Time.time;

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Lifetime(currentTime, m_isActive));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData()");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Add all of the datapoints to the string
            foreach (Data_Lifetime data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Lifetime";
        }
    }
}
