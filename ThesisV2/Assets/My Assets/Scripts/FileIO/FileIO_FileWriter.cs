using UnityEngine;
using System;
using System.IO;

namespace Thesis.FileIO
{
    public class FileIO_FileWriter
    {
        public static bool WriteFile(string _filePath, string _fileContents)
        {
            try
            {
                // Create the file writer
                StreamWriter writer = new StreamWriter(File.Open(_filePath, FileMode.Create));

                // Write the full contents of the file
                writer.Write(_fileContents);

                // Close the file for safety
                writer.Close();

                // Return true to indicate that the file saved correctly
                return true;
            }
            catch(Exception e)
            {
                // Output an error message and return false indicating the file failed to save
                Debug.LogError("Error writing file: " + e.Message);
                return false;
            }
        }
    }
}