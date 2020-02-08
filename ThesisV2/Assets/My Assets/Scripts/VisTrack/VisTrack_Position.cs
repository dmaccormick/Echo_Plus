using System;
using System.Collections.Generic;
using UnityEngine;
using Thesis.Interface;
using Thesis.Utility;

namespace Thesis.VisTrack
{
    public class VisTrack_Position : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Position
        {
            public Data_Position(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

                // The second token is the vector3 so we need to parse that specifically
                this.m_data = Utility_Functions.ParseVector3(tokens[1]);
            }

            public static List<Data_Position> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Position> dataPoints = new List<Data_Position>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach(string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Position(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public Vector3 m_data;
        }



        //--- Private Variables ---//
        private List<Data_Position> m_dataPoints;



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // Create a list of data points by parsing the string
                m_dataPoints = Data_Position.ParseDataList(_data);

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
            throw new System.NotImplementedException();
        }

        public void UpdateVisualization(float _time)
        {
            throw new System.NotImplementedException();
        }

        public int FindDataPointForTime(float _time)
        {
            throw new System.NotImplementedException();
        }

        public string GetTrackName()
        {
            return "Position";
        }

        public float GetFirstTimestamp()
        {
            throw new NotImplementedException();
        }
    }

}