using UnityEngine;
using System;
using System.Collections.Generic;
using Thesis.Interface;
using Thesis.Utility;

namespace Thesis.Visualization
{
    public class Visualization_ObjGenerator : MonoBehaviour
    {
        //--- Methods ---//
        public static bool GenerateVisObjects(List<Visualization_ObjParse> _parsedObjects)
        {
            // Loop through and generate actual visualization objects from the parsed ones
            foreach (Visualization_ObjParse objParse in _parsedObjects)
            {
                // Create a new gameobject with the name from the parser
                GameObject visObj = new GameObject(objParse.m_objName);

                // Give the object the visualization script
                Visualization_Object visObjComp = visObj.AddComponent<Visualization_Object>();

                // Attach the related tracks to the object and connect them to the visualization script
                foreach(KeyValuePair<string, string> trackInfo in objParse.m_trackData)
                {
                    // Add the track by name and get the interface reference from it
                    string trackNamespace = "Thesis.VisTrack";
                    string trackName = trackInfo.Key;
                    string fullTrackName = trackNamespace + "." + trackName;
                    Type trackType = Utility_Functions.GetTypeFromString(fullTrackName);
                    IVisualizable trackComp = visObj.AddComponent(trackType) as IVisualizable;

                    // If it failed to add properly, return false
                    if (trackComp == null)
                        return false;

                    // Initialize the track with the data that was parsed. Return false if it failed
                    if (!trackComp.InitWithString(trackInfo.Value))
                        return false;

                    // Register the track with the visualization object
                    visObjComp.AddTrack(trackComp);
                }
            }

            // If all the objects were generated correctly, return true
            return true;
        }
    }

}