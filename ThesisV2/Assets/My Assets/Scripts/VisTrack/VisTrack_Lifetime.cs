using System;
using System.Collections.Generic;
using UnityEngine;
using Thesis.Interface;

namespace Thesis.VisTrack
{
    public class VisTrack_Lifetime : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Lifetime
        {
            public Data_Lifetime(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

                // Check if the second token is TRUE or FALSE, use ToUpper to make it consistent
                string boolToUpper = tokens[1].ToUpper();
                m_isActive = (boolToUpper == "TRUE");
            }

            public static List<Data_Lifetime> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Lifetime> dataPoints = new List<Data_Lifetime>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach (string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Lifetime(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public bool m_isActive;
        }



        //--- Private Variables ---//
        private List<Data_Lifetime> m_dataPoints;



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // Create a list of data points by parsing the string
                m_dataPoints = Data_Lifetime.ParseDataList(_data);

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