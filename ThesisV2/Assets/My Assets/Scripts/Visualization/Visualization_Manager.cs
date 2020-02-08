using UnityEngine;
using Thesis.FileIO;
using System.Collections.Generic;

namespace Thesis.Visualization
{
    // NOTE: The visualization manager must be placed first in the script execution order
    [DefaultExecutionOrder(-10)]
    public class Visualization_Manager : MonoBehaviour
    {
        //--- Private Variables ---//
        private List<Visualization_Object> m_staticObjects; // TODO: Create object set class that allows for hiding / activating of the whole set
        private List<List<Visualization_Object>> m_dynamicObjects; 



        //--- Loading Methods ---//
        public bool LoadStaticData(string _staticFilePath)
        {
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

            // Generate actual objects from the list of parsed objects
            m_staticObjects = Visualization_ObjGenerator.GenerateVisObjects(parsedStaticObjects, "Static Objects");

            // Return false if the object generation failed
            if (m_staticObjects == null)
                return false;

            // Loop through all of the static objects and start their visualizations
            foreach (Visualization_Object visObj in m_staticObjects)
                visObj.StartVisualization(GetNewStartTime());

            // Return true if everything parsed correctly
            return true;
        }

        public bool LoadDynamicData(string _dynamicFilePath)
        {
            // If this is the first dynamic object list added, need to setup the outer list
            if (m_dynamicObjects == null)
                m_dynamicObjects = new List<List<Visualization_Object>>();

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

            // Generate actual objects from the list of parsed objects
            m_dynamicObjects.Add(Visualization_ObjGenerator.GenerateVisObjects(parsedDynamicObjects, "Dynamic Objects"));

            // Return false if the object generation failed
            if (m_dynamicObjects == null)
                return false;

            // Loop through all of the newly added dynamic objects and start their visualizations
            foreach (Visualization_Object visObj in m_dynamicObjects[m_dynamicObjects.Count - 1])
                visObj.StartVisualization(GetNewStartTime());

            // Return true if everything parsed correctly
            return true;
        }



        //--- Utility Functions ---//
        private float GetNewStartTime()
        {
            // Set the start time to a very high number to start
            float startTime = Mathf.Infinity;

            // If the static objects are setup, see which of them has the earliest start time
            if (m_staticObjects != null)
                startTime = Mathf.Min(startTime, GetEarliestTimeFromVisObjSet(m_staticObjects));

            // Do the same for each of the dynamic object lists if they are setup
            if (m_dynamicObjects != null)
            {
                foreach(List<Visualization_Object> dynamicObjectSet in m_dynamicObjects)
                    startTime = Mathf.Min(startTime, GetEarliestTimeFromVisObjSet(dynamicObjectSet));
            }

            // Return the earliest time
            return startTime;
        }

        private float GetEarliestTimeFromVisObjSet(List<Visualization_Object> _objectSet)
        {
            // Set the start time to a very high number to start
            float startTime = Mathf.Infinity;

            // Loop through all of the objects and find which of them has the earliest start time
            foreach (Visualization_Object visObj in _objectSet)
                startTime = Mathf.Min(startTime, visObj.GetEarliestTrackTime());

            // Return the earliest time
            return startTime;
        }
    }
}