using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Thesis.Misc;

namespace Thesis.Recording
{
    public class Recording_Object : MonoBehaviour
    {
        //--- Public Variables ---//
        public List<MonoBehaviour> m_trackComponents;
        public bool m_isStatic;



        //--- Private Variables ---//
        private Recording_Manager m_recManager;
        private List<IRecordable> m_trackInterfaces;
        private string m_uniqueID;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Find the recording manager
            m_recManager = GameObject.FindObjectOfType<Recording_Manager>();

            // Message the recording manager and tell it that this object now exists
            RegisterObject();
        }

        private void OnDestroy()
        {
            // Message the recording manager and tell it that this object no longer exists
            UnregisterObject();
        }



        //--- Messages TO The Recording Manager ---//
        public void RegisterObject()
        {
            // Contact the recording manager and tell it this object now exists
            m_recManager.RegisterObject(this);
        }

        public void UnregisterObject()
        {
            // Contact the recording manager and tell it this object is being destroyed
            m_recManager.MarkObjectDoneRecording(this);
        }



        //--- Messages FROM The Recording Manager ---//
        public void SetupObject()
        {
            // Convert the track components into the relevant interface scripts
            ConvertTrackComps();
        }

        public void StartRecording()
        {
            // Loop through all of the tracks and tell them to start recording
            foreach (IRecordable track in m_trackInterfaces)
                track.StartRecording();
        }

        public void UpdateRecording(float _elapsedTime)
        {
            // Loop through all of the tracks and tell them to update
            foreach (IRecordable track in m_trackInterfaces)
                track.UpdateRecording(_elapsedTime);
        }

        public void EndRecording()
        {
            // Loop through all of the tracks and tell them to finish recording
            foreach (IRecordable track in m_trackInterfaces)
                track.EndRecording();

            // Unregister the object from the recording manager now
            m_recManager.MarkObjectDoneRecording(this);
        }

        public string GetAllTrackData()
        {
            // Use a stringbuilder for efficiency in concatenation
            StringBuilder builder = new StringBuilder();

            // Add object header information
            builder.AppendLine("OBJ_START~" + this.gameObject.name + this.m_uniqueID);

            // Add all of the string data from the tracks together into one set of data
            foreach (IRecordable track in m_trackInterfaces)
            {
                // First, add the track header information
                builder.AppendLine("\tTRK_START~" + track.GetTrackName());

                // Now, add all of the actual data that the track recorded
                builder.Append(track.GetData());

                // Finally, add the track footer information
                builder.AppendLine("\tTRK_END~" + track.GetTrackName());
            }

            // Add object footer information
            builder.AppendLine("OBJ_END~" + this.gameObject.name + this.m_uniqueID);

            // Return the compiled data
            return builder.ToString();
        }



        //--- Setters ---//
        public void SetUniqueID(string _uniqueID)
        {
            // Set the unique ID for only this object, which will later be appended to its name on export
            this.m_uniqueID = _uniqueID;
        }



        //--- Getters ---//
        public string GetUniqueID()
        {
            // Return this object's unique ID
            return this.m_uniqueID;
        }



        //--- Utility Functions ---//
        private void ConvertTrackComps()
        {
            // Start by setting up the interface list object
            m_trackInterfaces = new List<IRecordable>();

            // Try to convert all of the components over to the interfaces
            foreach (MonoBehaviour trackComp in m_trackComponents)
            {
                // Convert the component to the interface
                IRecordable trackInterface = trackComp as IRecordable;

                // Add to the list of interfaces if it worked, output an error if it didn't
                if (trackInterface != null)
                {
                    m_trackInterfaces.Add(trackInterface);
                }
                else
                {
                    Debug.LogError("Error: A component in the track list is NOT actually a track");
                }
            }
        }
    }
}