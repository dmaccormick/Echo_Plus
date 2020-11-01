using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using Thesis.Interface;
using Thesis.Utility;
using Thesis.Visualization;

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

                // The second token is the path to the mesh and its submesh ID (-1 if not a submesh)
                var meshTokens = tokens[1].Split(',');
                string meshPath = meshTokens[0];
                int subMeshIndex = int.Parse(meshTokens[1]);

#if UNITY_EDITOR
                // If not a submesh, just load the mesh directly
                // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
                if (subMeshIndex == -1)
                {
                    m_mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;
                }
                else
                {
                    var subAssets = AssetDatabase.LoadAllAssetsAtPath(meshPath);
                    m_mesh = subAssets[subMeshIndex] as Mesh;
                }

                // The third token is the colour which is a vector3 so parse that
                m_color = Utility_Functions.ParseColor(tokens[2]);

                // Remaining tokens are the materials so load them too
                m_materials = new List<Material>();
                for (int i = 3; i < tokens.Length; i++)
                    m_materials.Add(AssetDatabase.LoadAssetAtPath(tokens[i], typeof(Material)) as Material);

#else
                // Convert the mesh path to a resources based path instead of an asset path
                string meshResourcePath = Utility_Functions.ConvertAssetToResourcePath(meshPath);

                // If not a submesh, just load the mesh directly
                // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
                if (subMeshIndex == -1)
                {
                    m_mesh = Resources.Load(meshResourcePath, typeof(Mesh)) as Mesh;
                }
                else
                {
                    var subAssets = Resources.LoadAll(meshPath);
                    m_mesh = subAssets[subMeshIndex] as Mesh;
                }

                // The third token is the colour which is a vector3 so parse that
                m_color = Utility_Functions.ParseColor(tokens[2]);
                
                // Convert all of the materials and load them in as well
                m_materials = new List<Material>();
                for (int i = 3; i < tokens.Length; i++)
                {
                    string matResourcePath = Utility_Functions.ConvertAssetToResourcePath(tokens[i]);
                    m_materials.Add(Resources.Load(matResourcePath, typeof(Material)) as Material);
                }
#endif
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
            //public Material m_material;
            public List<Material> m_materials;
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

            // Only configure the mesh collider if this object is dynamic
            if (GetComponent<Visualization_Object>().IsDynamic)
            {
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
        }

        public void UpdateVisualization(float _time)
        {
            // Get the relevant data point for the given time
            int dataIdx = FindDataPointForTime(_time);
            Data_Renderables dataPoint = m_dataPoints[dataIdx];

            // Apply the data point to the visualization
            m_targetFilter.sharedMesh = dataPoint.m_mesh;

            // Need to merge the list of materials with the outline materials to prevent overriding and losing the outlines
            if (TryGetComponent<Thesis.External.QuickOutline>(out var outline))
            {
                List<Material> combinedMaterials = new List<Material>();
                combinedMaterials.AddRange(dataPoint.m_materials);
                combinedMaterials.Add(outline.outlineMaskMaterial);
                combinedMaterials.Add(outline.outlineFillMaterial);
                m_targetRenderer.sharedMaterials = combinedMaterials.ToArray();
            }
            else
            {
                m_targetRenderer.sharedMaterials = dataPoint.m_materials.ToArray();
            }
            
            m_targetRenderer.material.color = dataPoint.m_color;
        }

        public int FindDataPointForTime(float _time)
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

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
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the first data point
            return m_dataPoints[0].m_timestamp;
        }

        public float GetLastTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            // Return the timestamp for the last data point
            return m_dataPoints[m_dataPoints.Count - 1].m_timestamp;
        }
    }
}