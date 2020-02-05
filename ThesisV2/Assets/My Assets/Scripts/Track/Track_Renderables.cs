using UnityEngine;
using UnityEngine.Assertions;
using Thesis.Recording;
using Thesis.Misc;
using System.Collections.Generic;
using System.Text;

namespace Thesis.Track
{
    [RequireComponent(typeof(Recording_Object))]
    public class Track_Renderables : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Renderables
        {
            public Data_Renderables(float _timestamp, Mesh _mesh, Material _material, Color _colour)
            {
                this.m_timestamp = _timestamp;
                this.m_mesh = _mesh;
                this.m_material = _material;
                this.m_color = _colour;
            }

            public string GetString()
            {
                return null;
            }

            public float m_timestamp;
            public Mesh m_mesh;
            public Material m_material;
            public Color m_color;
        }



        //--- Public Variables ---//
        public MeshRenderer m_targetRenderer;
        public MeshFilter m_targetFilter;



        //--- Private Variables ---//
        private List<Data_Renderables> m_dataPoints;
        private Mesh m_currentMesh;
        private Material m_currentMaterial;
        private Color m_currentColour;



        //--- IRecordable Interface ---//
        public void StartRecording()
        {
            // Ensure the targets are not null
            Assert.IsNotNull(m_targetFilter, "m_targetFilter needs to be set for the track");
            Assert.IsNotNull(m_targetRenderer, "m_targetRenderer needs to be set for the track");

            // Init the private variables 
            m_dataPoints = new List<Data_Renderables>();
            m_currentMesh = m_targetFilter.sharedMesh;
            m_currentMaterial = m_targetRenderer.sharedMaterial;
            m_currentColour = m_targetRenderer.material.color;

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
            // If any of the renderables have changed, update the values and record the change
            if (m_currentMesh != m_targetFilter.sharedMesh ||
                m_currentMaterial != m_targetRenderer.sharedMaterial ||
                m_currentColour != m_targetRenderer.material.color)
            {
                // Update the values
                m_currentMesh = m_targetFilter.sharedMesh;
                m_currentMaterial = m_targetRenderer.sharedMaterial;
                m_currentColour = m_targetRenderer.material.color;

                // Record the changes to the values
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

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Renderables(currentTime, m_currentMesh, m_currentMaterial, m_currentColour));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData()");

            // Use a string builder to compile the data string efficiently
            StringBuilder stringBuilder = new StringBuilder();

            // TODO: Add the track header information
            // ...

            // Add all of the datapoints to the string
            foreach (Data_Renderables data in m_dataPoints)
                stringBuilder.AppendLine(data.GetString());

            // TODO: Add the track footer information
            // ...

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }
    }
}
