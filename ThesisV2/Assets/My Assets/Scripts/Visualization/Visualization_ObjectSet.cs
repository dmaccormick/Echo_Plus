using UnityEngine;
using System.Collections.Generic;

namespace Thesis.Visualization
{
    public class Visualization_ObjectSet : MonoBehaviour
    {
        //--- Private Variables ---//
        private List<Visualization_Object> m_objects;
        private string m_objectSetName;
        private bool m_isVisible;
        private Color m_outlineColour;



        //--- Methods ---//
        public void Setup(string _name, List<Visualization_Object> _objects)
        {
            this.m_objects = _objects;
            this.m_objectSetName = _name;
            this.m_isVisible = true;
            this.m_outlineColour = Color.HSVToRGB(Random.value, 1.0f, 1.0f);
        }

        public void StartVisualization(float _startTime)
        {
            // Loop through all of the objects and start their visualizations
            foreach (Visualization_Object visObj in m_objects)
                visObj.StartVisualization(_startTime);
        }

        public void UpdateVisualization()
        {
            // Only update if visible
        }

        public void EndVisualization()
        {

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
    }

}