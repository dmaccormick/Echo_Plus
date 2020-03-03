﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Text;
using System.Collections.Generic;
using Thesis.Interface;
using Thesis.Recording;

namespace Thesis.RecTrack
{
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Scale : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Scale
        {
            public Data_Scale(float _timeStamp, Vector3 _data)
            {
                this.m_timestamp = _timeStamp;
                this.m_data = _data;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + this.m_data.ToString(_format);
            }

            public float m_timestamp;
            public Vector3 m_data;
        }



        //--- Public Variables ---//
        public Recording_Settings m_recordingSettings;
        public Transform m_target;



        //--- Private Variables ---//
        private List<Data_Scale> m_dataPoints;



        //--- IRecordable Interfaces ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the target is set
            Assert.IsNotNull(m_target, "m_target needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables
            m_dataPoints = new List<Data_Scale>();

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
            // Handle the different styles of recording
            if (m_recordingSettings.m_recordingMethod == Recording_Method.On_Change)
            {
                // Get the previously recorded datapoint and the current data point
                Data_Scale lastDataPoint = m_dataPoints[m_dataPoints.Count - 1];
                Vector3 currentDataPoint = m_target.lossyScale;

                // Determine the difference between the data points
                float dataDifference = Vector3.Magnitude(currentDataPoint - lastDataPoint.m_data);

                // If the difference is significant enough, we should record the data
                if (dataDifference >= m_recordingSettings.m_changeMinThreshold)
                {
                    // If the object jumped far enough, we should double up the recording
                    // This way, the vis system doesn't simply lerp it across the time and it instead jumps properly
                    if (dataDifference >= m_recordingSettings.m_changeJumpThreshold)
                    {
                        // Double up the previous data point but go backwards one frame since that's where it was last frame, not this frame
                        float prevTime = _currentTime - Time.unscaledDeltaTime;
                        Vector3 prevData = lastDataPoint.m_data;
                        m_dataPoints.Add(new Data_Scale(prevTime, prevData));
                    }

                    // Record the current data point
                    RecordData(_currentTime);
                }
            }
            else if (m_recordingSettings.m_recordingMethod == Recording_Method.Every_X_Seconds)
            {
                // If enough time has passed, update the recording
                if (_currentTime >= m_recordingSettings.m_nextSampleTime)
                    RecordData(_currentTime);
            }
            else
            {
                // Always record data when doing the every frame recording
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Get the data point from the target
            Vector3 currentScl = m_target.lossyScale;

            // Add the datapoint to the list
            m_dataPoints.Add(new Data_Scale(_currentTime, currentScl));

            // Recalculate the next sample time
            m_recordingSettings.m_nextSampleTime = _currentTime + m_recordingSettings.m_sampleTime;
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Add all of the datapoints to the string with the requested format
            foreach (Data_Scale data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_recordingSettings.m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Scale";
        }

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values
            m_target = this.gameObject.transform;
        }
    }

}
