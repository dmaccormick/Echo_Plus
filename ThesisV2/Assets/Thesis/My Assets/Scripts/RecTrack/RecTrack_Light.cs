using UnityEngine;
using UnityEngine.Assertions;
using Thesis.Recording;
using Thesis.Interface;
using System.Collections.Generic;
using System.Text;

namespace Thesis.RecTrack
{
    // Currently only fully works with directional lights
    // TODO: Add full support for point and spot lights!
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Light : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Light
        {
            public Data_Light(float _timestamp, LightType _type, Color _colour, float _intensity)
            {
                this.m_timestamp = _timestamp;
                this.m_type = _type;
                this.m_colour = _colour;
                this.m_intensity = _intensity;
            }

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" 
                    + ((int)this.m_type).ToString() + "~"
                    + this.m_colour.ToString(_format) + "~"
                    + this.m_intensity.ToString();
            }

            public float m_timestamp;
            public LightType m_type;
            public Color m_colour;
            public float m_intensity;
        }



        //--- Public Variables ---//
        public Light m_targetLight;
        public string m_dataFormat = "F3";



        //--- Private Variables ---//
        private List<Data_Light> m_dataPoints;
        private LightType m_currentType;
        private Color m_currentColour;
        private float m_currentIntensity;



        //--- IRecordable Interface ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the targets are not null
            Assert.IsNotNull(m_targetLight, "Track Assert Failed [" + GetTrackName() + "] - " + "m_targetLight needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables 
            m_dataPoints = new List<Data_Light>();
            m_currentType = m_targetLight.type;
            m_currentColour = m_targetLight.color;
            m_currentIntensity = m_targetLight.intensity;

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
            // If any of the light settings have changed, update the values and record the change
            if (m_currentType != m_targetLight.type ||
                m_currentColour != m_targetLight.color ||
                m_currentIntensity != m_targetLight.intensity)
            {
                // Update the values
                m_currentType = m_targetLight.type;
                m_currentColour = m_targetLight.color;
                m_currentIntensity = m_targetLight.intensity;

                // Record the changes to the values
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Light(_currentTime, m_currentType, m_currentColour, m_currentIntensity));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // Add all of the datapoints to the string
            foreach (Data_Light data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Light";
        }

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values from this object
            m_targetLight = GetComponent<Light>();

            // If either one failed, try to grab from the children or the parent instead
            m_targetLight = (m_targetLight == null) ? GetComponentInChildren<Light>() : m_targetLight;
            m_targetLight = (m_targetLight == null) ? GetComponentInParent<Light>() : m_targetLight;
        }
    }
}
