using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
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
        private GameObject m_targetObj;
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

        public void StartVisualization(float _startTime)
        {
            // Init the target
            m_targetObj = this.gameObject;

            // Apply the initial visualization
            UpdateVisualization(_startTime);
        }

        public void UpdateVisualization(float _time)
        {
            // Get the relevant data point for the given time
            int dataIdx = FindDataPointForTime(_time);
            
            // For the lifetime track, we can actually go before the object exists. In that case, the above index is -1
            if (dataIdx == -1)
            {
                // If the index is negative, the object didn't exist at the time of the recording so it should be hidden by default
                m_targetObj.SetActive(false);
            }
            else
            {
                // Otherwise, get the datapoint that matches the index
                Data_Lifetime dataPoint = m_dataPoints[dataIdx];

                // Otherwise, use the value from the datapoint to mark the object's existence
                m_targetObj.SetActive(dataPoint.m_isActive);
            }
        }

        public int FindDataPointForTime(float _time)
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints has to be setup for before looking for a data point");
            Assert.IsTrue(m_dataPoints.Count >= 1, "m_dataPoints cannot be empty");

            // For the lifetime track, we can actually go BEFORE the object should exist so return -1 if that is the case
            if (_time < m_dataPoints[0].m_timestamp)
                return -1;

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
            return "Lifetime";
        }

        public float GetFirstTimestamp()
        {
            // Ensure the datapoints are actually setup
            Assert.IsNotNull(m_dataPoints, "m_dataPoints has to be setup for before looking for a data point");
            Assert.IsTrue(m_dataPoints.Count >= 1, "m_dataPoints cannot be empty");

            // Return the timestamp for the first data point
            return m_dataPoints[0].m_timestamp;
        }
    }
}