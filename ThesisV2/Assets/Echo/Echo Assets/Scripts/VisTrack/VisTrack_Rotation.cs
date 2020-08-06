using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Echo.Interface;
using Echo.Utility;


namespace Echo.VisTrack
{
    public class VisTrack_Rotation : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        [System.Serializable]
        public struct Data_Rotation
        {
            public Data_Rotation(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

                // The second token is the quaternion so we need to parse that specifically
                this.m_data = Utility_Functions.ParseQuaternion(tokens[1]);
            }

            public static List<Data_Rotation> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Rotation> dataPoints = new List<Data_Rotation>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach (string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Rotation(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public Quaternion m_data;
        }



        //--- Private Variables ---//
        private Transform m_targetTransform;
        private List<Data_Rotation> m_dataPoints;



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // Create a list of data points by parsing the string
                m_dataPoints = Data_Rotation.ParseDataList(_data);

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
            m_targetTransform = this.transform;

            // Apply the initial visualization
            UpdateVisualization(_startTime);
        }

        public void UpdateVisualization(float _time)
        {
            // Create a variable to hold the interpolated data 
            Quaternion finalData;

            // Get the data point before the given time and the index for the datapoint after
            int prevDataIdx = FindDataPointForTime(_time);
            int nextDataIdx = prevDataIdx + 1;
            Data_Rotation prevDataPoint = m_dataPoints[prevDataIdx];

            // Set the result data to be the data point before the time by default
            finalData = prevDataPoint.m_data;

            // If the datapoint after exists, overwrite the result data to be the interpolated result between them
            if (nextDataIdx < m_dataPoints.Count)
            {
                // Grab the next data point
                Data_Rotation nextDataPoint = m_dataPoints[nextDataIdx];

                // Calculate the slerp T param between the before and after points
                float slerpT = Mathf.InverseLerp(prevDataPoint.m_timestamp, nextDataPoint.m_timestamp, _time);

                // Set the final data to be a slerp'd value between the two points
                finalData = Quaternion.Slerp(prevDataPoint.m_data, nextDataPoint.m_data, slerpT);
            }

            // Apply the data point to the visualization
            m_targetTransform.rotation = finalData;
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
            return "Rotation";
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