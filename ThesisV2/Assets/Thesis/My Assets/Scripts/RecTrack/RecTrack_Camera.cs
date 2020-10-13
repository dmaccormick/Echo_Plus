using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Recording;
using Thesis.Interface;
using System.Collections.Generic;
using System.Text;

namespace Thesis.RecTrack
{
    // Currently only works with perspective cameras
    // TODO: Add support for ortho cameras!
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Camera : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Camera
        {
            public Data_Camera(float _timestamp, float _fov, float _clipClose, float _clipFar)
            {
                this.m_timestamp = _timestamp;
                this.m_fov = _fov;
                this.m_clipClose = _clipClose;
                this.m_clipFar = _clipFar;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + m_fov.ToString() + "~" + m_clipClose.ToString() + "~" + m_clipFar.ToString();
            }

            public float m_timestamp;
            public float m_fov;
            public float m_clipClose;
            public float m_clipFar;
        }



        //--- Public Variables ---//
        public Camera m_targetCam;
        public string m_dataFormat = "F3";



        //--- Private Variables ---//
        private List<Data_Camera> m_dataPoints;
        private float m_currentFov;
        private float m_currentClipClose;
        private float m_currentClipFar;



        //--- IRecordable Interface ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the targets are not null
            Assert.IsNotNull(m_targetCam, "m_targetCam needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables 
            m_dataPoints = new List<Data_Camera>();
            m_currentFov = m_targetCam.fieldOfView;
            m_currentClipClose = m_targetCam.nearClipPlane;
            m_currentClipFar = m_targetCam.farClipPlane;

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
            // If any of the renderables have changed, update the values and record the change
            if (m_currentFov != m_targetCam.fieldOfView ||
                m_currentClipClose != m_targetCam.nearClipPlane ||
                m_currentClipFar != m_targetCam.farClipPlane)
            {
                // Update the values
                m_currentFov = m_targetCam.fieldOfView;
                m_currentClipClose = m_targetCam.nearClipPlane;
                m_currentClipFar = m_targetCam.farClipPlane;

                // Record the changes to the values
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Camera(_currentTime, m_currentFov, m_currentClipClose, m_currentClipFar));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Add all of the datapoints to the string
            foreach (Data_Camera data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Camera";
        }

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values from this object
            m_targetCam = GetComponent<Camera>();

            // If either one failed, try to grab from the children or the parent instead
            m_targetCam = (m_targetCam == null) ? GetComponentInChildren<Camera>() : m_targetCam;
            m_targetCam = (m_targetCam == null) ? GetComponentInParent<Camera>() : m_targetCam;
        }
    }
}
