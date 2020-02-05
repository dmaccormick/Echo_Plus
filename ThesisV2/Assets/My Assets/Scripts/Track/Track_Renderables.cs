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

            public string GetString(string _format)
            {
                return this.m_timestamp.ToString(_format) + "~" + 
                    AssetDatabase.GetAssetPath(this.m_mesh) + "~" +
                    AssetDatabase.GetAssetPath(this.m_material) + "~" +
                    this.m_color.ToString(_format);
            }

            public float m_timestamp;
            public Mesh m_mesh;
            public Material m_material;
            public Color m_color;
        }



        //--- Public Variables ---//
        public MeshRenderer m_targetRenderer;
        public MeshFilter m_targetFilter;
        public string m_dataFormat = "F3";



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
            // NOTE: Use the shared mesh and material to prevent a duplicate from being created and removing the mesh path references
            // NOTE: The meshes need to be marked as read and write in the import settings!
            m_dataPoints = new List<Data_Renderables>();
            m_currentMesh = m_targetFilter.sharedMesh;
            m_currentMaterial = m_targetRenderer.sharedMaterial;
            m_currentColour = m_targetRenderer.sharedMaterial.color;

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
                m_currentColour != m_targetRenderer.sharedMaterial.color)
            {
                // Update the values
                m_currentMesh = m_targetFilter.sharedMesh;
                m_currentMaterial = m_targetRenderer.sharedMaterial;
                m_currentColour = m_targetRenderer.sharedMaterial.color;

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

            // Add all of the datapoints to the string
            foreach (Data_Renderables data in m_dataPoints)
                stringBuilder.AppendLine("\t\t" + data.GetString(m_dataFormat));

            // Return the full set of data grouped together
            return stringBuilder.ToString();
        }

        public string GetTrackName()
        {
            return "Renderables";
        }
    }
}
