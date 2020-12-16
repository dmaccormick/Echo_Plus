using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Text;
using Thesis.FileIO;

namespace Thesis.Recording
{
    // NOTE: The recording manager must be placed first in the script execution order
    [DefaultExecutionOrder(-10)]
    public class Recording_Manager : MonoBehaviour
    {
        //--- Data Struct ---//
        private class Recording_ObjectData
        {
            public Recording_ObjectData(Recording_Object _objectRef)
            {
                m_objectRef = _objectRef;
                m_exportedData = "";
            }

            public void OnObjectMarkedDone()
            {
                // Get the data from the object before the reference becomes null
                m_exportedData = m_objectRef.GetAllTrackData();
            }

            public string GetExportedData()
            {
                // If the object was marked done recording, the data was already exported. Otherwise, we need to grab it now
                if (m_exportedData == "")
                    m_exportedData = m_objectRef.GetAllTrackData();

                // Return the exported data
                return m_exportedData;
            }

            public Recording_Object m_objectRef;
            public string m_exportedData;
        }



        //--- Public Variables ---//
        public bool m_useUnscaledDeltaTime;



        //--- Private Variables ---//
        private Dictionary<string, Recording_ObjectData> m_staticObjects;
        private Dictionary<string, Recording_ObjectData> m_dynamicObjects;
        private long m_nextUniqueId;
        private bool m_isRecording;
        private float m_currentTime;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the first unique ID to simply be 0
            m_nextUniqueId = 0;

            // Setup the recording process
            Setup();
        }

        private void Update()
        {
            // If the system is recording, we need to be update the recording objects throughout the scene
            if (m_isRecording)
                UpdateRecording();
        }



        //--- Registration Methods ---//
        public void Setup()
        {
            // The system is not recording by default
            m_isRecording = false;

            // When it does record, it should start from the beginning of the timer
            m_currentTime = 0.0f;

            // Init the static and dynamic lists
            m_staticObjects = new Dictionary<string, Recording_ObjectData>();
            m_dynamicObjects = new Dictionary<string, Recording_ObjectData>();
        }

        public void RegisterObject(Recording_Object _newObject)
        {
            // If the system is not currently recording, don't register the object
            // If the object still exists when we start recording, we will register it then
            if (!m_isRecording)
                return;

            // Ensure the object has not already been registered
            string objID = _newObject.GetUniqueID();
            if (objID != null)
            {
                // If the object has been registered already, just back out
                if (m_dynamicObjects.ContainsKey(objID) || m_staticObjects.ContainsKey(objID))
                    return;
            }

            // Give the object the next unique ID as a string like "_#ID#"
            _newObject.SetUniqueID(GetNextUniqueID());

            // Now, sort the object into either the dynamic or static list depending on its setting
            var selectedList = (_newObject.m_isStatic) ? m_staticObjects : m_dynamicObjects;
            selectedList.Add(_newObject.GetUniqueID(), new Recording_ObjectData(_newObject));

            // We need to set up the object now
            _newObject.SetupObject();

            // Tell the object to begin recording
            _newObject.StartRecording(m_currentTime);
        }

        public void MarkObjectDoneRecording(Recording_Object _doneObject)
        {
            // If the recording hasn't started yet, just back out. The object is being destroyed before recording has started
            if (!m_isRecording)
                return;

            // Ensure the object has already been registered
            string objID = _doneObject.GetUniqueID();
            Assert.IsTrue(m_dynamicObjects.ContainsKey(objID) || m_staticObjects.ContainsKey(objID), "The object [" + _doneObject.gameObject.name + "] was never registered");

            // Going to find the object reference in the relevant list
            string deadObjID = _doneObject.GetUniqueID();
            Recording_ObjectData objDataElement;

            // Attempt to get the reference to the object from each of the lists
            if (!m_staticObjects.TryGetValue(deadObjID, out objDataElement))
            {
                if (!m_dynamicObjects.TryGetValue(deadObjID, out objDataElement))
                {
                    // If we got here then neither list had the refernce which is a problem
                    Debug.LogError("Error: Neither the static or dynamic list had a reference to the object [" + _doneObject.gameObject.name + "]");
                    return;
                }
            }

            // Tell the object element that the referenced object is now done recording
            objDataElement.OnObjectMarkedDone();
        }



        //--- Recording Methods ---//
        public void StartRecording()
        {
            // Setup again for the next recording
            Setup();

            // Recording has now started
            m_isRecording = true;

            // Find all of the recording objects in the scene
            Recording_Object[] recordingObjects = GameObject.FindObjectsOfType<Recording_Object>();

            // Register all of them
            foreach (Recording_Object recObj in recordingObjects)
                RegisterObject(recObj);

            // Loop through and start the recordings on all of the dynamic objects
            foreach (Recording_ObjectData dynamicObjData in m_dynamicObjects.Values)
            {
                // Skip over the destroyed ones
                // TODO: Put the destroyed ones in a separate list to prevent branching!
                if (dynamicObjData.m_objectRef == null)
                    continue;

                // Start the recording of the live objects
                dynamicObjData.m_objectRef.StartRecording(m_currentTime);
            }

            // Loop through all of the static objects and grab their data in one shot
            foreach (Recording_ObjectData staticObjData in m_staticObjects.Values)
            {
                // Skip over the destroyed ones
                // TODO: Put the destroyed ones in a separate list to prevent branching!
                if (staticObjData.m_objectRef == null)
                    continue;

                // Start the recording which will trigger the first data point to be recorded
                staticObjData.m_objectRef.StartRecording(m_currentTime);

                // Since these objects are static, we can just mark them as done recording right now to get their data out
                MarkObjectDoneRecording(staticObjData.m_objectRef);
            }
        }

        public void UpdateRecording()
        {
            // Increase the current time
            // Can use the UNSCALED delta time to account for slow motion in the game
            // Alternatively, can use the regular delta time
            m_currentTime += (m_useUnscaledDeltaTime) ? Time.unscaledDeltaTime : Time.deltaTime;

            // Loop through and update the recordings on all of the dynamic objects, not the static ones
            foreach (Recording_ObjectData dynamicObjData in m_dynamicObjects.Values)
            {
                // Skip over the destroyed ones
                // TODO: Put the destroyed ones in a separate list to prevent branching!
                if (dynamicObjData.m_objectRef == null)
                    continue;

                // Update the recording of the live objects, using unscaled dt in case the game has slow motion
                dynamicObjData.m_objectRef.UpdateRecording(m_currentTime);
            }
        }

        public void StopRecording()
        {
            // Loop through and stop the recordings on all of the dynamic objects
            foreach (Recording_ObjectData dynamicObjData in m_dynamicObjects.Values)
            {
                // Skip over the destroyed ones
                // TODO: Put the destroyed ones in a separate list to prevent branching!
                if (dynamicObjData.m_objectRef == null)
                    continue;

                // Stop the recording of the live objects. The static ones are already stopped
                dynamicObjData.m_objectRef.EndRecording(m_currentTime);
            }

            // The system is no longer recording
            m_isRecording = false;
        }



        //--- Saving Methods ---//
        public bool SaveStaticData(string _staticFilePath)
        {
            // Gather all of the data from the static objects and combine it
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Recording_ObjectData recObj in m_staticObjects.Values)
                stringBuilder.Append(recObj.GetExportedData());

            // Write all of the data to the static file path and return if it worked or not
            return FileIO_FileWriter.WriteFile(_staticFilePath, stringBuilder.ToString());
        }

        public bool SaveDynamicData(string _dynamicFilePath)
        {
            // Gather all of the data from the static objects and combine it
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Recording_ObjectData recObj in m_dynamicObjects.Values)
                stringBuilder.Append(recObj.GetExportedData());

            // Write all of the data to the static file path and return if it worked or not
            return FileIO_FileWriter.WriteFile(_dynamicFilePath, stringBuilder.ToString());
        }



        //--- Getters ---//
        public string GetNextUniqueID()
        {
            // Return the ID string like "_#ID#" and increment the value for next time
            return "_" + m_nextUniqueId++;
        }

        public float GetCurrentTime()
        {
            return m_currentTime;
        }
    }
}