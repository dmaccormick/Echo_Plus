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