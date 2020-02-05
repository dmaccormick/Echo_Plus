using UnityEngine;
using UnityEngine.Assertions;
using System.Text;
using System.Collections.Generic;
using Thesis.Interface;
using Thesis.Recording;

namespace Thesis.RecTrack
{
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Rotation : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Rotation
        {
            public Data_Rotation(float _timeStamp, Quaternion _data)
            {
                this.m_timestamp = _timeStamp;
                this.m_data = _data;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + this.m_data.ToString(_format);
            }

            public float m_timestamp;
            public Quaternion m_data;
        }



        //--- Public Variables ---//
        public Transform m_target;
        public string m_dataFormat = "F3";
        public float m_sampleTime = 0.25f;



        //--- Private Variables ---//
        private List<Data_Rotation> m_dataPoints;
        private float m_deltaSampleTime;



        //--- IRecordable Interfaces ---//
        public void StartRecording()
        {
            // Ensure the target is set
            Assert.IsNotNull(m_target, "m_target needs to be set for the track");

            // Init the private variables
            m_dataPoints = new List<Data_Rotation>();
            m_deltaSampleTime = 0.0f;

            // Record the first data point
            RecordData();
        }

        public void EndRecording()
        {
            // Record the final data point
            RecordData();
        }

        public void UpdateRecording(float _elapsedTime)
        {
            // Increase the timer since the last sample time
            m_deltaSampleTime += _elapsedTime;

            // If enough time has passed, update the recording
            if (m_deltaSampleTime >= m_sampleTime)
            {
                RecordData();
            }
        }

        public void RecordData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData()");

            // Get the current timestamp
            // TODO: Replace with a timer from the recording manager since if the game is paused, this will show 0
            float currentTime = Time.time;

            // Get the data point from the target
            Quaternion currentRot = m_target.rotation;

            // Add the datapoint to the list
            m_dataPoints.Add(new Data_Rotation(currentTime, currentRot));

            // Reset the time since the last data sample
            m_deltaSampleTime = 0.0f;
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData()");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Add all of the datapoints to the string with the requested format
            foreach (Data_Rotation data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Rotation";
        }
    }
}