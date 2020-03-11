﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Thesis.Interface;
using Thesis.Visualization.VisCam;

namespace Thesis.Visualization
{
    public class Visualization_Object : MonoBehaviour
    {
        //--- Private Variables ---//
        private List<IVisualizable> m_tracks;
        private bool m_isKeyObj;



        //--- Unity Methods ---//
        private void OnDestroy()
        {
            // If this object is a key object, we should de-register with the quick focus selector system
            if (m_isKeyObj)
            {
                var quickFocus = FindObjectOfType<VisCam_QuickFocus>();
                if (quickFocus != null)
                    quickFocus.RemoveFocusTarget(this.transform);
            }
        }



        //--- Methods ---//
        public void Setup(bool _isKeyObj)
        {
            // Init the private variables
            m_tracks = new List<IVisualizable>();
            m_isKeyObj = _isKeyObj;
        }

        public void AddTrack(IVisualizable _newTrack)
        {
            // Ensure the track list is setup first
            Assert.IsNotNull(m_tracks, "m_tracks has to be setup before adding a new track on object [" + this.gameObject.name + "]");

            // Add the track to the list
            m_tracks.Add(_newTrack);
        }

        public void StartVisualization(float _startTime)
        {
            // Start the visualization on all of the tracks
            foreach (IVisualizable track in m_tracks)
                track.StartVisualization(_startTime);

            // If this object is a key object, we should register with the quick focus selector system
            if (m_isKeyObj)
            {
                var quickFocus = FindObjectOfType<VisCam_QuickFocus>();
                if (quickFocus != null)
                    quickFocus.AddFocusTarget(this.transform);
            } 
        }

        public void UpdateVisualization(float _currentTime)
        {
            // Update the visualization on all of the tracks
            foreach (IVisualizable track in m_tracks)
                track.UpdateVisualization(_currentTime);
        }

        public float GetEarliestTrackTime()
        {
            // Set the start time to a very high number to start
            float startTime = Mathf.Infinity;

            // Loop through all of the tracks and find which of them has the earliest start time
            foreach (IVisualizable track in m_tracks)
                startTime = Mathf.Min(startTime, track.GetFirstTimestamp());

            // Return the earliest time
            return startTime;
        }

        public float GetLatestTrackTime()
        {
            // Set the end time to a very low number to start
            float endTime = 0.0f;

            // Loop through all of the tracks and find which of them has the latest end time
            foreach (IVisualizable track in m_tracks)
                endTime = Mathf.Max(endTime, track.GetLastTimestamp());

            // Return the latest time
            return endTime;
        }
    }
}