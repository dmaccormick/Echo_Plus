﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Interface;
using Thesis.Utility;
using System.IO;

namespace Thesis.VisTrack
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class VisTrack_Renderables : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        public struct Data_Renderables
        {
            public Data_Renderables(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

#if UNITY_EDITOR
                // The second token is the path to the mesh so we need to get the mesh itself from the asset database
                m_mesh = AssetDatabase.LoadAssetAtPath(tokens[1], typeof(Mesh)) as Mesh;

                // The third token is the path to the material so load that too
                m_material = AssetDatabase.LoadAssetAtPath(tokens[2], typeof(Material)) as Material;
#else
                // Convert the mesh and mat file paths to be resources based paths instead
                string meshResourcePath = Utility_Functions.ConvertAssetToResourcePath(tokens[1]);
                string matResourcePath = Utility_Functions.ConvertAssetToResourcePath(tokens[2]);

                // Load the mesh and material from the resource folders
                m_mesh = Resources.Load(meshResourcePath, typeof(Mesh)) as Mesh;
                m_material = Resources.Load(matResourcePath, typeof(Material)) as Material;
#endif

                // The fourth token is the colour which is a vector3 so parse that
                m_color = Utility_Functions.ParseColor(tokens[3]);
            }

            public static List<Data_Renderables> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Renderables> dataPoints = new List<Data_Renderables>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach (string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Renderables(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public Mesh m_mesh;
            public Material m_material;
            public Color m_color;
        }



        //--- Private Variables ---//
        private MeshFilter m_targetFilter;
        private MeshRenderer m_targetRenderer;
        private List<Data_Renderables> m_dataPoints;



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // Create a list of data points by parsing the string
                m_dataPoints = Data_Renderables.ParseDataList(_data);

                // If everything worked correctly, return true
                return true;
            }
            catch (Exception _e)
            {
                // If something went wrong, output an error and return false
                Debug.LogError("Error in InitWithString(): " + _e.Message);
                return false;
            }
        }

        public void StartVisualization(float _startTime)
        {
            // Init the targets
            m_targetFilter = GetComponent<MeshFilter>();
            m_targetRenderer = GetComponent<MeshRenderer>();

            // Apply the initial visualization
            UpdateVisualization(_startTime);

            // Setup the mesh collider so it is ready for mouse picking and uses the correct mesh
            // Use all of the cooking options
            MeshCollider meshCollider = this.GetComponent<MeshCollider>();
            meshCollider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.UseFastMidphase | MeshColliderCookingOptions.WeldColocatedVertices;
            meshCollider.sharedMesh = m_targetFilter.sharedMesh;
            meshCollider.convex = false;

            // We need to set the object to be on the focus picking layer so that we can focus it with the camera controls
            // NOTE: This feature REQUIRES that this layer is added and is exactly this
            this.gameObject.layer = LayerMask.NameToLayer("FocusTargetPicking");
        }

        public void UpdateVisualization(float _time)
        {
            // Get the relevant data point for the given time
            int dataIdx = FindDataPointForTime(_time);
            Data_Renderables dataPoint = m_dataPoints[dataIdx];

            // Apply the data point to the visualization
            m_targetFilter.sharedMesh = dataPoint.m_mesh;
            m_targetRenderer.sharedMaterial = dataPoint.m_material;
            m_targetRenderer.sharedMaterial.color = dataPoint.m_color;
        }

        public int FindDataPointForTime(float _time)
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Start by setting the selected index to 0 in case there is only one point
            int selectedIndex = 0;

            // Loop through all of the data and find the nearest point BEFORE the given time
            for (selectedIndex = 0; selectedIndex < m_dataPoints.Count - 1; selectedIndex++)
            {
                // Get the datapoint at the current index and next index
                var thisDataPoint = m_dataPoints[selectedIndex];
                var nextDataPoint = m_dataPoints[selectedIndex + 1];

                // If this datapoint is BEFORE OR AT the time and the next one is AFTER the time, then we are at the right data point
                if (thisDataPoint.m_timestamp <= _time && nextDataPoint.m_timestamp > _time)
                    break;
            }

            // Return the selected index
            return selectedIndex;
        }

        public string GetTrackName()
        {
            return "Renderables";
        }

        public float GetFirstTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the first data point
            return m_dataPoints[0].m_timestamp;
        }

        public float GetLastTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the last data point
            return m_dataPoints[m_dataPoints.Count - 1].m_timestamp;
        }
    }
}