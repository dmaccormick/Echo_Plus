using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Thesis.Interface;
using Thesis.Utility;

namespace Thesis.VisTrack
{
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

                // The second token is the path to the mesh so we need to get the mesh itself from the asset database
                m_mesh = AssetDatabase.LoadAssetAtPath(tokens[1], typeof(Mesh)) as Mesh;

                // The third token is the path to the material so load that too
                m_material = AssetDatabase.LoadAssetAtPath(tokens[2], typeof(Material)) as Material;

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

        public void StartVisualization()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateVisualization(float _time)
        {
            throw new System.NotImplementedException();
        }

        public string GetTrackName()
        {
            throw new System.NotImplementedException();
        }
    }

}