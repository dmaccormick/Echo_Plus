using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Thesis.Visualization
{
    //--- Parsed Object Class ---//
    public class Visualization_ObjParse
    {
        public Visualization_ObjParse()
        {
            this.m_objName = null;
            this.m_isKeyObj = false;
            this.m_trackData = new Dictionary<string, string>();
        }

        public string m_objName;
        public bool m_isKeyObj;
        public Dictionary<string, string> m_trackData;
    }



    //--- Log Parser Class ---//
    public class Visualization_LogParser : MonoBehaviour
    {
        //--- Methods ---//
        public static List<Visualization_ObjParse> ParseLogFile(string _logContents)
        {
            try
            {
                // Create a list to hold all of the objects as we parse them, and a temp holding object
                List<Visualization_ObjParse> parsedObjects = new List<Visualization_ObjParse>();
                Visualization_ObjParse currentParsedObject = new Visualization_ObjParse();
                string currentTrackName = null;
                StringBuilder currentTrackData = new StringBuilder();

                // Split up the log contents into the individual lines
                string[] fileLines = _logContents.Split('\n');

                // Loop through all of the lines and handle them one by one
                foreach (string line in fileLines)
                {
                    // Ensure the line isn't empty
                    if (line == "")
                        continue;

                    // Start by removing the \r at the end of the line
                    string lineTrimmed = line.TrimEnd(new char[] { '\r' });

                    // Also remove any \t at the start of the line
                    lineTrimmed = lineTrimmed.TrimStart(new char[] { '\t' });

                    // Split the line into the individual tokens
                    string[] tokens = lineTrimmed.Split('~');
                    string firstToken = tokens[0];

                    // Ensure the token isn't empty
                    if (firstToken == "")
                        continue;

                    // Depending on what the first token is, we need to handle the line differently
                    if (firstToken == "OBJ_START")
                    {
                        // Create a new parse object
                        currentParsedObject = new Visualization_ObjParse();

                        // The second token is the name of the object so we should set that
                        currentParsedObject.m_objName = tokens[1];

                        // The next token should be if the object is a key focus object or not
                        // If it is, it can be quick selected
                        currentParsedObject.m_isKeyObj = (tokens.Length >= 3 && tokens[2] == "True");
                    }
                    else if (firstToken == "OBJ_END")
                    {
                        // The object is complete so we should add it to the list
                        parsedObjects.Add(currentParsedObject);
                    }
                    else if (firstToken == "TRK_START")
                    {
                        // Grab the name of the track since it is the second token
                        currentTrackName = tokens[1];

                        // Reset the track data so we can start compiling it
                        currentTrackData.Clear();

                        // Add the track to the dictionary
                        currentParsedObject.m_trackData.Add(currentTrackName, "");
                    }
                    else if (firstToken == "TRK_END")
                    {
                        // Add the track data to the dictionary
                        currentParsedObject.m_trackData[currentTrackName] = currentTrackData.ToString();

                        // Reset the track name
                        currentTrackName = null;
                    }
                    else
                    {
                        // This line should be a datapoint so we can just add it to the track data to be parsed later
                        // Need to add a newline at the end so we have something to split on later
                        currentTrackData.Append(lineTrimmed + "\n");
                    }
                }

                // Return the list of parsed objects
                return parsedObjects;
            }
            catch(Exception e)
            {
                // If something went wrong, output an error and return null
                Debug.LogError("Error parsing file: " + e.Message);
                return null;
            }
        }
    }
}