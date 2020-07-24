using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Thesis.External;

namespace Thesis.Visualization
{
    //--- Event Classes ---//
    [System.Serializable]
    public class OutlineColourChangeEvent : UnityEvent<Color>
    {
    }

    public class Visualization_ObjectSet : MonoBehaviour
    {
        //--- Public Variables ---//
        public OutlineColourChangeEvent m_onOutlineColourChanged;



        //--- Private Constants ---//
        private const float c_OUTLINE_WIDTH = 10.0f;



        //--- Private Variables ---//
        private List<Visualization_Object> m_objects;
        private string m_objectSetName;
        private bool m_isVisible;
        private bool m_hasOutline;
        private Color m_outlineColour;



        //--- Methods ---//
        public void Setup(string _name, List<Visualization_Object> _objects)
        {
            this.m_objects = _objects;
            this.m_objectSetName = _name;
            this.m_isVisible = true;
            this.m_hasOutline = false;
            this.m_outlineColour = Color.black;

            // Init the event
            m_onOutlineColourChanged = new OutlineColourChangeEvent();
        }

        public void EnableOutline(Color _outlineColor)
        {
            // If the outlines have already been enabled, back out
            if (this.m_hasOutline)
                return;

            // Set the internal data
            this.m_hasOutline = true;
            this.m_outlineColour = _outlineColor;

            // Add outlines to all of the child objects
            foreach(Visualization_Object visObj in m_objects)
            {
                // Add the 'Quick Outline' script written by Chris Nolet to the child object
                QuickOutline outlineComp = visObj.gameObject.AddComponent<QuickOutline>();

                // Setup the component
                outlineComp.OutlineMode = QuickOutline.Mode.OutlineAll;
                outlineComp.OutlineColor = this.m_outlineColour;
                outlineComp.OutlineWidth = c_OUTLINE_WIDTH;
            }

            // Trigger the event
            m_onOutlineColourChanged.Invoke(this.m_outlineColour);
        }

        public void UpdateOutlineColour(Color _outlineColor)
        {
            // If this object set doesn't have an outline, just back out
            if (!this.m_hasOutline)
                return;

            // Update the internal outline colour
            this.m_outlineColour = _outlineColor;

            // Add outlines to all of the child objects
            foreach (Visualization_Object visObj in m_objects)
            {
                // Get the 'Quick Outline' script written by Chris Nolet from the child object
                QuickOutline outlineComp = visObj.gameObject.GetComponent<QuickOutline>();

                // If there is an outline component, we should update the colour on it
                if (outlineComp != null)
                    outlineComp.OutlineColor = this.m_outlineColour;
            }

            // Trigger the event
            m_onOutlineColourChanged.Invoke(this.m_outlineColour);
        }

        public void StartVisualization(float _startTime)
        {
            // Loop through all of the objects and start their visualizations
            foreach (Visualization_Object visObj in m_objects)
                visObj.StartVisualization(_startTime);
        }

        public void UpdateVisualization(float _currentTime)
        {
            // Only update the visualization if actually currently visible
            if (m_isVisible)
            {
                // Update all of the objects in the set
                foreach (Visualization_Object visObj in m_objects)
                {
                    visObj.UpdateVisualization(_currentTime);
                }
            }
        }

        public void DestroyAllObjects()
        {
            // Destroy all of the objects in the set
            foreach (Visualization_Object visObj in m_objects)
                Destroy(visObj.gameObject);

            // Destroy this object last of all
            Destroy(this.gameObject);
        }

        public void ToggleAllObjectsActive(bool _isActive)
        {
            // Update the internal state which controls whether or not the UpdateVisualization() function will trigger
            m_isVisible = _isActive;

            // Toggle the parent to show / hide the objects
            this.gameObject.SetActive(_isActive);
        }



        //--- Getters ---//
        public float GetEarliestTimestamp()
        {
            // Set the start time to a very high number to start
            float startTime = Mathf.Infinity;

            // Loop through all of the objects and find which of them has the latest end time
            foreach (Visualization_Object visObj in m_objects)
                startTime = Mathf.Min(startTime, visObj.GetEarliestTrackTime());

            // Return the earliest time
            return startTime;
        }

        public float GetLatestTimestamp()
        {
            // Set the end time to a very low number to start
            float endTime = 0.0f;

            // Loop through all of the objects and find which of them has the latest end time
            foreach (Visualization_Object visObj in m_objects)
                endTime = Mathf.Max(endTime, visObj.GetLatestTrackTime());

            // Return the latest time
            return endTime;
        }

        public string GetSetName()
        {
            return m_objectSetName;
        }

        public bool GetHasOutline()
        {
            return m_hasOutline;
        }

        public Color GetOutlineColour()
        {
            return m_outlineColour;
        }

        public bool GetIsVisible()
        {
            return m_isVisible;
        }
    }

}