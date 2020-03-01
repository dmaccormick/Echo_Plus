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
        private float m_nextSampleTime;



        //--- IRecordable Interfaces ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the target is set
            Assert.IsNotNull(m_target, "m_target needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables
            m_dataPoints = new List<Data_Rotation>();
            m_nextSampleTime = 0.0f;

            // Record the first data point
            RecordData(_startTime);
        }

        public void EndRecording(float _endTime)
        {
            // Record the final data point
            RecordData(_endTime);
        }

        public void UpdateRecording(float _currentTime)
        {
            // If enough time has passed, update the recording
            if (m_nextSampleTime >= m_sampleTime)
            {
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Get the data point from the target
            Quaternion currentRot = m_target.rotation;

            // Add the datapoint to the list
            m_dataPoints.Add(new Data_Rotation(_currentTime, currentRot));

            // Recalculate the next sample time
            m_nextSampleTime = _currentTime + m_sampleTime;
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

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

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values
            m_target = this.gameObject.transform;
        }
    }
}