using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Recording;
using Thesis.Interface;
using System.Collections.Generic;
using System.Text;

namespace Thesis.RecTrack
{
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Lifetime : MonoBehaviour, IRecordable
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
        private Recording_Manager m_recManager;
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
                RecordData(m_recManager.GetCurrentTime());
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
                RecordData(m_recManager.GetCurrentTime());
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
                RecordData(m_recManager.GetCurrentTime());
            }
        }



        //--- IRecordable Interface ---//
        public void StartRecording(float _startTime)
        {
            // Init the private variables 
            // NOTE: This track needs to reference the manager and get its current time directly since it records data outside of the normal loop
            m_recManager = GameObject.FindObjectOfType<Recording_Manager>();
            m_dataPoints = new List<Data_Lifetime>();

            // Recording is now active
            // NOTE: This track needs this because its triggers are outside of the normal update track loop
            m_isRecording = true;

            // The object is active for now
            m_isActive = true;

            // Record the first data point
            RecordData(_startTime);
        }

        public void EndRecording(float _endTime)
        {
            // No longer recording
            m_isRecording = false;

            // Record the final data point
            RecordData(_endTime);
        }

        public void UpdateRecording(float _currentTime)
        {
            // This class does not have anything here
            // TODO: Split the interface so we don't have this non-implemented function
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Lifetime(_currentTime, m_isActive));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

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

        public void SetupDefault()
        {
            // There is nothing really to setup for the lifetime
        }
    }
}
