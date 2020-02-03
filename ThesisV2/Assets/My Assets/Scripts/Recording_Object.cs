using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Thesis
{
    public class Recording_Object : MonoBehaviour
    {
        //--- Public Variables ---//
        public List<MonoBehaviour> m_trackComponents;



        //--- Private Variables ---//
        private List<IRecordable> m_trackInterfaces;



        //--- Messages TO The Recording Manager ---//
        public void RegisterObject()
        {
            // TODO: Contact the recording manager and tell it this object now exists
            // ...
        }

        public void UnregisterObject()
        {
            // TODO: Contact the recording manager and tell it this object is being destroyed
            // ...

            // TODO: Pass off the information this object recorded before it is completely destroyed
            // ...
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
        }

        public string GetAllTrackData()
        {
            // Use a stringbuilder for efficiency in concatenation
            StringBuilder builder = new StringBuilder();

            // TODO: Add object header information
            // ...

            // Add all of the string data from the tracks together into one set of data
            foreach (IRecordable track in m_trackInterfaces)
                builder.Append(track.GetData());

            // TODO: Add object footer information
            // ...

            // Return the compiled data
            return builder.ToString();
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