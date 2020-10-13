﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Recording;
using Thesis.Interface;
using System.Collections.Generic;
using System.Text;

namespace Thesis.RecTrack
{
    [RequireComponent(typeof(Recording_Object))]
    public class RecTrack_Renderables : MonoBehaviour, IRecordable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Renderables
        {
            public Data_Renderables(float _timestamp, Mesh _mesh, Material[] _materials, Color _colour)
            {
                this.m_timestamp = _timestamp;
                this.m_mesh = _mesh;
                this.m_materials = _materials;
                this.m_color = _colour;
            }

//            public string GetString(string _format)
//            {
//#if UNITY_EDITOR
//                return this.m_timestamp.ToString(_format) + "~" + 
//                    AssetDatabase.GetAssetPath(this.m_mesh) + "~" +
//                    AssetDatabase.GetAssetPath(this.m_material) + "~" +
//                    this.m_color.ToString(_format);
//#else
//                return null;
//#endif
//            }

            public string GetString(string _format)
            {
#if UNITY_EDITOR
                return this.m_timestamp.ToString(_format) + "~" +
                    AssetDatabase.GetAssetPath(this.m_mesh) + "~" +
                    this.m_color.ToString(_format) + "~" +
                    GetMaterialArrayStr();
#else
                return null;
#endif
            }

            private string GetMaterialArrayStr()
            {
                StringBuilder str = new StringBuilder();

                // Add the materials up to the last one with the ~ at the end
                for (int i = 0; i < m_materials.Length - 1; i++)
                    str.Append(AssetDatabase.GetAssetPath(this.m_materials[i]) + "~");

                // Add the final material without the ~ at the end
                str.Append(AssetDatabase.GetAssetPath(this.m_materials[m_materials.Length - 1]));

                return str.ToString();
            }

            public float m_timestamp;
            public Mesh m_mesh;
            public Material[] m_materials;
            public Color m_color;
        }



        //--- Public Variables ---//
        public MeshRenderer m_targetRenderer;
        public MeshFilter m_targetFilter;
        public string m_dataFormat = "F3";



        //--- Private Variables ---//
        private List<Data_Renderables> m_dataPoints;
        private Mesh m_currentMesh;
        private Material[] m_currentMaterials;
        private Color m_currentColour;



        //--- IRecordable Interface ---//
        public void StartRecording(float _startTime)
        {
            // Ensure the targets are not null
            Assert.IsNotNull(m_targetFilter, "m_targetFilter needs to be set for the track on object [" + this.gameObject.name + "]");
            Assert.IsNotNull(m_targetRenderer, "m_targetRenderer needs to be set for the track on object [" + this.gameObject.name + "]");

            // Init the private variables 
            // NOTE: Use the shared mesh and material to prevent a duplicate from being created and removing the mesh path references
            // NOTE: The meshes need to be marked as read and write in the import settings!
            m_dataPoints = new List<Data_Renderables>();
            m_currentMesh = m_targetFilter.sharedMesh;
            m_currentMaterials = m_targetRenderer.sharedMaterials;
            m_currentColour = m_targetRenderer.sharedMaterial.color;

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
            if (m_currentMesh != m_targetFilter.sharedMesh ||
                m_currentColour != m_targetRenderer.sharedMaterial.color ||
                !CheckIfMaterialsMatch(m_currentMaterials, m_targetRenderer.sharedMaterials))
            {
                // Update the values
                m_currentMesh = m_targetFilter.sharedMesh;
                m_currentMaterials = m_targetRenderer.sharedMaterials;
                m_currentColour = m_targetRenderer.sharedMaterial.color;

                // Record the changes to the values
                RecordData(_currentTime);
            }
        }

        public void RecordData(float _currentTime)
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling RecordData() on object [" + this.gameObject.name + "]");

            // Add a new data point to the list
            m_dataPoints.Add(new Data_Renderables(_currentTime, m_currentMesh, m_currentMaterials, m_currentColour));
        }

        public string GetData()
        {
            // Ensure the datapoints are setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints must be init before calling GetData() on object [" + this.gameObject.name + "]");

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

        public void SetupDefault()
        {
            // Setup this recording track by grabbing default values from this object
            m_targetFilter = GetComponent<MeshFilter>();
            m_targetRenderer = GetComponent<MeshRenderer>();

            // If either one failed, try to grab from the children instead
            m_targetFilter = (m_targetFilter == null) ? GetComponentInChildren<MeshFilter>() : m_targetFilter;
            m_targetRenderer = (m_targetRenderer == null) ? GetComponentInChildren<MeshRenderer>() : m_targetRenderer;
        }



        //--- Utility Functions ---//
        private bool CheckIfMaterialsMatch(Material[] _currentMaterials, Material[] _sharedMaterials)
        {
            if (_currentMaterials.Length != _sharedMaterials.Length)
                return false;

            for(int i = 0; i < _currentMaterials.Length; i++)
            {
                if (_currentMaterials[i] != _sharedMaterials[i])
                    return false;
            }

            return true;
        }
    }
}
