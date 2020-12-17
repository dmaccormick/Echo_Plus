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
        public class Data_Renderables
        {
            public Data_Renderables(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

                // The second token is the path to the mesh and its submesh name
                var meshTokens = tokens[1].Split(',');
                string meshPath = meshTokens[0];
                string meshName = meshTokens[1];

#if UNITY_EDITOR
                // If not a submesh, just load the mesh directly
                // If it is a submesh, we need to load all of the assets at the path and grab the one from the right index
                if (meshName == "NONE")
                {
                    // Try to load the asset anyways and see if it works
                    m_mesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;

                    if (m_mesh == null)
                        Debug.LogWarning("Cannot find mesh at path: " + meshPath);
                }
                else
                {
                    // Find the mesh within the subassets that has a matching name
                    var subAssets = AssetDatabase.LoadAllAssetsAtPath(meshPath);
                   
                    foreach(var subAsset in subAssets)
                    {
                        if (subAsset.GetType() == typeof(Mesh))
                        {
                            if (subAsset.name == meshName)
                            {
                                var meshConversionAttempt = subAsset as Mesh;

                                if (meshConversionAttempt == null)
                                    Debug.LogWarning("Failed to convert mesh that matched name: " + meshName);
                                else
                                    m_mesh = meshConversionAttempt;
                            }
                        }
                    }
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
            public List<Material> m_materials;
            public Color m_color;
        }



        //--- Private Variables ---//
        private MeshFilter m_targetFilter;
        private MeshRenderer m_targetRenderer;
        private List<Data_Renderables> m_dataPoints;
        private int m_lastDataIndex = 0;
        private float m_lastTime = 0.0f;



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
            
            m_targetRenderer.sharedMaterial.color = dataPoint.m_color;
        }

        public int FindDataPointForTime(float _time)
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints has to be setup for before looking for a data point on object [" + this.gameObject.name + "]");
            Assert.IsTrue(m_dataPoints.Count >= 1, "Track Assert Failed [" + GetTrackName() + "] - " + "m_dataPoints cannot be empty on object [" + this.gameObject.name + "]");

            if (m_dataPoints.Count > 1)
            {
                // If we are PAST the last data point, return the final datapoint again
                if (_time >= m_dataPoints[m_dataPoints.Count - 1].m_timestamp)
                    return m_dataPoints.Count - 1;

                // Determine if we are going forward or backward in time
                float timeDiff = _time - m_lastTime;
                m_lastTime = _time;
                
                // No difference in time so just return the same data index
                if (timeDiff == 0.0f)
                {
                    return m_lastDataIndex;
                }
                // Positive difference in time means we moved forward so we should search that direction first
                else if (timeDiff > 0.0f)
                {
                    // Search forward, including the current data point 
                    int index = SearchForward(true, _time);

                    // If it somehow wasn't found, now search backwards, but don't include the current point since we already searched it
                    if (index == -1)
                    {
                        index = SearchBackward(false, _time);

                        // If we STILL haven't found it, just return the current index again
                        if (index == -1)
                            return 0;
                        else
                            return index;
                    }
                    else
                    {
                        return index;
                    }
                }
                // Negative difference in time means we moved backward so we should search that direction first
                else if (timeDiff < 0.0f)
                {
                    // Search backward, including the current data point 
                    int index = SearchBackward(true, _time);

                    // If it somehow wasn't found, now search forwards, but don't include the current point since we already searched it
                    if (index == -1)
                    {
                        index = SearchForward(false, _time);

                        // If we STILL haven't found it, just return the first index
                        if (index == -1)
                            return 0;
                        else
                            return index;
                    }
                    else
                    {
                        return index;
                    }
                }
            }

            // If we get to here, just return 0
            return 0;
        }

        private int SearchForward(bool _includeLastIndex, float _time)
        {
            // If we should include the last used index, start there. Otherwise, start at the next one
            int startIndex = (_includeLastIndex) ? m_lastDataIndex : m_lastDataIndex + 1;

            // Loop through and check all of the datapoints until we find a match
            for (int i = startIndex; i < m_dataPoints.Count; i++)
            {
                if (CheckDataAtIndex(i, _time))
                    return i;
            }

            // Return -1 if we couldn't find anything
            return -1;
        }

        private int SearchBackward(bool _includeLastIndex, float _time)
        {
            // If we should include the last used index, start there. Otherwise, start at the previous one
            int startIndex = (_includeLastIndex) ? m_lastDataIndex : m_lastDataIndex - 1;

            // Loop through and check all of the datapoints until we find a match
            for (int i = startIndex; i >= 0; i--)
            {
                if (CheckDataAtIndex(i, _time))
                    return i;
            }

            // Return -1 if we couldn't find anything
            return -1;
        }

        public bool CheckDataAtIndex(int _index, float _time)
        {
            if (_index >= m_dataPoints.Count - 1)
                return false;

            // Get the datapoint at the current index and next index
            var thisDataPoint = m_dataPoints[_index];
            var nextDataPoint = m_dataPoints[_index + 1];

            // If this datapoint is BEFORE OR AT the time and the next one is AFTER the time, then we are at the right data point
            return (thisDataPoint.m_timestamp <= _time && nextDataPoint.m_timestamp > _time);
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