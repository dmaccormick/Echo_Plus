using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;
using Thesis.FileIO;
using System.Collections.Generic;
using System.IO;

namespace Thesis.Visualization
{
    //--- Playstate Enum ---//
    public enum Playstate
    {
        Paused,
        Reverse,
        Forward,
    }



    //--- Event Classes ---//
    [System.Serializable] public class MultiFloatEvent : UnityEvent<float, float, float>
    {
    }

    [System.Serializable] public class PlaystateEvent : UnityEvent<Playstate>
    {
    }



    //--- Visualization Manager ---//
    // NOTE: The visualization manager must be placed first in the script execution order
    [DefaultExecutionOrder(-10)]
    public class Visualization_Manager : MonoBehaviour
    {
        //--- Public Variables ---//
        public UnityEvent<float, float, float> m_onTimeUpdated;
        public UnityEvent<Playstate> m_onPlaystateUpdated;
        public List<Color> m_outlineColours;



        //--- Private Variables ---//
        private Visualization_ObjectSet m_staticObjectSet;
        private List<Visualization_ObjectSet> m_dynamicObjectSets;
        private Playstate m_playstate;
        private float m_startTime = Mathf.Infinity;
        private float m_endTime = 0.0f;
        private float m_currentTime = 0.0f;
        private float m_playbackSpeed = 1.0f;

 

        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the events
            m_onTimeUpdated = new MultiFloatEvent();
            m_onPlaystateUpdated = new PlaystateEvent();
        }

        private void Update()
        {
            // If in playmode, we should update the timer
            if (m_playstate == Playstate.Forward)
            {
                // Move the time forward
                m_currentTime += (Time.deltaTime * m_playbackSpeed);

                // If the time has reached the end, we should pause the playback and cap the timer
                if (m_currentTime >= m_endTime)
                {
                    // Cap the time
                    m_currentTime = m_endTime;

                    // Pause the playback
                    m_playstate = Playstate.Paused;

                    // Trigger the event associated with updating the playback state
                    m_onPlaystateUpdated.Invoke(m_playstate);
                }

                // Trigger the time update event
                m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

                // Update the visualization to the current point in time
                UpdateVisualization();
            }
            else if (m_playstate == Playstate.Reverse)
            {
                // Move the time backwards
                m_currentTime -= (Time.deltaTime * m_playbackSpeed);

                // If the time has reached the start, we should pause the playback and cap the timer
                if (m_currentTime <= m_startTime)
                {
                    // Cap the time
                    m_currentTime = m_startTime;

                    // Pause the playback
                    m_playstate = Playstate.Paused;

                    // Trigger the event associated with updating the playback state
                    m_onPlaystateUpdated.Invoke(m_playstate);
                }

                // Trigger the time update event
                m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

                // Update the visualization to the current point in time
                UpdateVisualization();
            }
        }



        //--- Loading Methods ---//
        public bool LoadStaticData(string _staticFilePath)
        {
            // We should pause the playback now and therefore trigger the event
            m_playstate = Playstate.Paused;
            m_onPlaystateUpdated.Invoke(m_playstate);

            // Determine the name of the visualization objects from the file path
            string fileName = Path.GetFileName(_staticFilePath);

            // Read all of the data from the static file
            string staticData = FileIO_FileReader.ReadFile(_staticFilePath);

            // If the file didn't read correctly, return false
            if (staticData == null)
                return false;

            // Send the data to the parser and get the list of objects back
            List<Visualization_ObjParse> parsedStaticObjects = Visualization_LogParser.ParseLogFile(staticData);

            // If the parse failed, return false
            if (parsedStaticObjects == null)
                return false;

            // Clear any previously loaded static objects so we only ever have one set of static objects
            if (m_staticObjectSet != null)
                m_staticObjectSet.DestroyAllObjects();

            // Generate the actual objects contained in the set from the list of parsed objects
            m_staticObjectSet = Visualization_ObjGenerator.GenerateObjectSet(parsedStaticObjects, "Static Objects (" + fileName + ")");

            // If the object generation failed, return false
            if (m_staticObjectSet == null)
                return false;

            // Look for the new start and end times
            m_startTime = CalcNewStartTime();
            m_endTime = CalcNewEndTime();
            m_currentTime = m_startTime;

            // Tell the set to start the visualization
            m_staticObjectSet.StartVisualization(m_startTime);

            // Trigger the update event since the times have been changed and everything is setup
            m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

            // Return true if everything parsed correctly
            return true;
        }

        public bool LoadDynamicData(string _dynamicFilePath)
        {
            // We should pause the playback now and therefore trigger the event
            m_playstate = Playstate.Paused;
            m_onPlaystateUpdated.Invoke(m_playstate);

            // Determine the name of the visualization objects from the file path
            string fileName = Path.GetFileName(_dynamicFilePath);

            // If this is the first dynamic object set added, need to setup the outer list
            if (m_dynamicObjectSets == null)
                m_dynamicObjectSets = new List<Visualization_ObjectSet>();

            // Read all of the data from the dynamic file
            string dynamicData = FileIO_FileReader.ReadFile(_dynamicFilePath);

            // If the file didn't read correctly, return false
            if (dynamicData == null)
                return false;

            // Send the data to the parser and get the list of objects back
            List<Visualization_ObjParse> parsedDynamicObjects = Visualization_LogParser.ParseLogFile(dynamicData);

            // If the parse failed, return false
            if (parsedDynamicObjects == null)
                return false;

            // Generate an actual object set from the list and hold onto it for now
            Visualization_ObjectSet newObjectSet = Visualization_ObjGenerator.GenerateObjectSet(parsedDynamicObjects, "Dynamic Objects(" + fileName + ")");

            // Return false if the object generation failed
            if (newObjectSet == null)
                return false;

            // Add the object set into the outer list for the dynamic object sets
            m_dynamicObjectSets.Add(newObjectSet);

            // Look for the new start and end times
            m_startTime = CalcNewStartTime();
            m_endTime = CalcNewEndTime();
            m_currentTime = m_startTime;

            // Start the visualization for the newly added dynamic object set
            newObjectSet.StartVisualization(m_startTime);

            // Add an outline to the new object set. Use the next one in the list and then wrap if need be
            // Have to -1 so we start at 0
            int nextColourIndex = (m_dynamicObjectSets.Count - 1) % m_outlineColours.Count;
            Color nextColour = m_outlineColours[nextColourIndex];
            newObjectSet.EnableOutline(nextColour);

            // Trigger the update event since the times have been changed and everything is setup
            m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

            // Return true if everything parsed correctly
            return true;
        }

        public bool DeleteObjectSet(Visualization_ObjectSet _deletedSet)
        {
            // We should pause the playback now and therefore trigger the event
            m_playstate = Playstate.Paused;
            m_onPlaystateUpdated.Invoke(m_playstate);

            // Check if the object set in question is the loaded static one
            if (m_staticObjectSet == _deletedSet)
            {
                // Destroy the static set
                m_staticObjectSet.DestroyAllObjects();

                // Remove the reference to the set
                m_staticObjectSet = null;

                // Look for the new start and end times
                m_startTime = CalcNewStartTime();
                m_endTime = CalcNewEndTime();
                m_currentTime = m_startTime;

                // Trigger the update event since the times have been changed
                m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

                // Return true to indicate that the deletion was successful
                return true;
            }

            // Otherwise, check if it is one of the dynamic ones
            if (m_dynamicObjectSets != null && m_dynamicObjectSets.Contains(_deletedSet))
            {
                // Remove the set from the list
                m_dynamicObjectSets.Remove(_deletedSet);

                // Destroy the set objects
                _deletedSet.DestroyAllObjects();

                // Look for the new start and end times
                m_startTime = CalcNewStartTime();
                m_endTime = CalcNewEndTime();
                m_currentTime = m_startTime;

                // Trigger the update event since the times have been changed
                m_onTimeUpdated.Invoke(m_startTime, m_currentTime, m_endTime);

                // Return true to indicate that the deletion was successful
                return true;
            }

            // If we reached this point, the deletion failed so return false
            return false;
        }



        //--- Playback Methods ---//
        public void ReversePlayback()
        {
            // Update the playstate
            m_playstate = Playstate.Reverse;
        }

        public void PausePlayback()
        {
            // Update the playstate
            m_playstate = Playstate.Paused;
        }

        public void PlayForward()
        {
            // Update the playstate
            m_playstate = Playstate.Forward;
        }

        public void UpdateVisualization()
        {
            // Update the dynamic objects. Don't update the static ones since they are static
            if (m_dynamicObjectSets != null)
            {
                // Loop through all of the dynamic object sets and update them
                foreach (Visualization_ObjectSet objSet in m_dynamicObjectSets)
                    objSet.UpdateVisualization(m_currentTime);
            }
        }



        //--- Setters ---//
        public void SetCurrentTime(float _time)
        {
            // Avoid checking if the start and end time are invalid
            if (m_startTime == Mathf.Infinity && m_endTime == 0.0f)
                return;

            // Ensure the given time is in the range of the start and end values
            Assert.IsTrue(_time <= m_endTime, "The new time cannot be larger than the end time");
            Assert.IsTrue(_time >= m_startTime, "The new time cannot be smaller than the start time");

            // Update the current time
            m_currentTime = _time;

            // Update the visualization
            UpdateVisualization();
        }

        public void SetPlaybackSpeed(float _newSpeed)
        {
            // Update the playback speed
            m_playbackSpeed = _newSpeed;
        }



        //--- Getters ---//
        public float GetStartTime()
        {
            return m_startTime;
        }

        public float GetEndTime()
        {
            return m_endTime;
        }

        public float GetCurrentTime()
        {
            return m_currentTime;
        }

        public Visualization_ObjectSet GetStaticObjectSet()
        {
            return m_staticObjectSet;
        }

        public List<Visualization_ObjectSet> GetDynamicObjectSets()
        {
            return m_dynamicObjectSets;
        }



        //--- Utility Functions ---//
        private float CalcNewStartTime()
        {
            // Set the start time to a very high number to start
            float startTime = Mathf.Infinity;

            // If the static objects are setup, see which of them has the earliest start time
            if (m_staticObjectSet != null)
                startTime = Mathf.Min(startTime, m_staticObjectSet.GetEarliestTimestamp());

            // Do the same for each of the dynamic object sets if they are setup
            if (m_dynamicObjectSets != null)
            {
                foreach (Visualization_ObjectSet dynamicObjectSet in m_dynamicObjectSets)
                    startTime = Mathf.Min(startTime, dynamicObjectSet.GetEarliestTimestamp());
            }

            // Return the earliest time
            return startTime;
        }

        private float CalcNewEndTime()
        {
            // Set the end time to a very low number to start
            float endTime = 0.0f;

            // If the static objects are setup, see which of them has the latest start time
            if (m_staticObjectSet != null)
                endTime = Mathf.Max(endTime, m_staticObjectSet.GetLatestTimestamp());

            // Do the same for each of the dynamic object sets if they are setup
            if (m_dynamicObjectSets != null)
            {
                foreach (Visualization_ObjectSet dynamicObjectSet in m_dynamicObjectSets)
                    endTime = Mathf.Max(endTime, dynamicObjectSet.GetLatestTimestamp());
            }

            // Return the latest time
            return endTime;
        }
    }
}