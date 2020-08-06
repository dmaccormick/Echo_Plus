using UnityEngine;
using System;
using System.IO;

namespace Echo.FileIO
{
    public class FileIO_FileReader : MonoBehaviour
    {
        //--- Methods ---//
        public static string ReadFile(string _filePath)
        {
            try
            {
                // Open the file for reading
                StreamReader reader = new StreamReader(File.OpenRead(_filePath));

                // Raed all of the file contents into a string
                string fileContents = reader.ReadToEnd();

                // Return the file contents if everything worked correctly
                return fileContents;
            }
            catch(Exception e)
            {
                // Output an error message and return null indicating the file failed to load
                Debug.LogError("Error reading file: " + e.Message);
                return null;
            }
        }
    }

}