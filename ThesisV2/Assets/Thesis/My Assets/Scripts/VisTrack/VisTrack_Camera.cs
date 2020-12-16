using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Thesis.Interface;
using Thesis.Visualization.VisCam;

namespace Thesis.VisTrack
{
    [RequireComponent(typeof(Camera))]
    public class VisTrack_Camera : MonoBehaviour, IVisualizable
    {
        //--- Data Struct ---//
        public struct Data_Camera
        {
            public Data_Camera(string _dataStr)
            {
                // Split the data string
                string[] tokens = _dataStr.Split('~');

                // The first token is the timestamp so just parse the float
                m_timestamp = float.Parse(tokens[0]);

                // The second token is the FOV
                m_fov = float.Parse(tokens[1]);

                // The third token is the near clip plane distance
                m_clipClose = float.Parse(tokens[2]);

                // The fourth token is the far clip plane distance
                m_clipFar = float.Parse(tokens[3]);
            }

            public static List<Data_Camera> ParseDataList(string _data)
            {
                // Create a list to hold the parsed data
                List<Data_Camera> dataPoints = new List<Data_Camera>();

                // Split the string into individual lines which each are one data point
                string[] lines = _data.Split('\n');

                // Create new data points from each of the lines
                foreach (string line in lines)
                {
                    // If the line is empty, do nothing
                    if (line == null || line == "")
                        continue;

                    // Otherwise, create a new data point
                    dataPoints.Add(new Data_Camera(line));
                }

                // Return the list of data points
                return dataPoints;
            }

            public float m_timestamp;
            public float m_fov;
            public float m_clipClose;
            public float m_clipFar;
        }



        //--- Private Variables ---//
        private Camera m_targetCam;
        private List<Data_Camera> m_dataPoints;
        private int m_lastDataIndex = 0;
        private float m_lastTime = 0.0f;



        //--- Unity Methods ---//
        public void OnDestroy()
        {
            // Look for the player camera manager and tell it to remove this camera
            var camManager = FindObjectOfType<VisCam_PlayerCameraManager>();
            if (camManager != null)
                camManager.OnCamDestroyed(m_targetCam);
        }



        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            try
            {
                // Create a list of data points by parsing the string
                m_dataPoints = Data_Camera.ParseDataList(_data);

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
            m_targetCam = GetComponent<Camera>();

            // Apply the initial visualization
            UpdateVisualization(_startTime);
        }

        public void UpdateVisualization(float _time)
        {
            // Get the relevant data point for the given time
            int dataIdx = FindDataPointForTime(_time);
            Data_Camera dataPoint = m_dataPoints[dataIdx];

            // Apply the data point to the visualization
            m_targetCam.fieldOfView = dataPoint.m_fov;
            m_targetCam.nearClipPlane = dataPoint.m_clipClose;
            m_targetCam.farClipPlane = dataPoint.m_clipFar;
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
            return "Camera";
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



        //--- Getters ---//
        public Camera GetTargetCam()
        {
            return this.m_targetCam;
        }
    }
}