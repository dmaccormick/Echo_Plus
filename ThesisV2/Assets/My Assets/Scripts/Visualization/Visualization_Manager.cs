using UnityEngine;
using Thesis.FileIO;
using System.Collections.Generic;

namespace Thesis.Visualization
{
    // NOTE: The visualization manager must be placed first in the script execution order
    [DefaultExecutionOrder(-10)]
    public class Visualization_Manager : MonoBehaviour
    {
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

            // TODO: Generate actual objects from the list of parsed objects
            // ...

            // TODO: Return false if the object generation failed
            // ...

            // Return true if everything parsed correctly
            return true;
        }

        public bool LoadDynamicData(string _dynamicFilePath)
        {
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

            // TODO: Generate actual objects from the list of parsed objects
            // ...

            // TODO: Return false if the object generation failed
            // ...

            // Return true if everything parsed correctly
            return true;
        }
    }
}