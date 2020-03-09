using UnityEngine;
using System;

namespace Thesis.Utility
{
    public class Utility_Functions : MonoBehaviour
    {
        //--- Methods ---//
        public static Type GetTypeFromString(string _string)
        {
            // Based off this: https://stackoverflow.com/questions/11107536/convert-string-to-type-in-c-sharp
            // Try to get the type from the current assembly
            Type objType = Type.GetType(_string);

            // If not found in this assembly, we need to through others
            if (objType == null)
            {
                // Loop through all of the assemblies
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Check the iterated assembly for the type
                    objType = asm.GetType(_string);

                    // If the type was found, exit the tloop
                    if (objType != null)
                        break;
                }
            }

            // Return the type
            return objType;
        }

        public static Vector3 ParseVector3(string _str)
        {
            // Based off this: https://answers.unity.com/questions/1134997/string-to-vector3.html
            // Start by removing the parentheses
            int openBracketIndex = _str.IndexOf('(') + 1;
            int closeBracketIndex = _str.IndexOf(')');
            int substrLength = closeBracketIndex - openBracketIndex;
            _str = _str.Substring(openBracketIndex, substrLength);

            // Split the rest of the string on the commas to get the individual floats
            string[] tokens = _str.Split(',');

            // Create a new vector3 and parse the individual floats into it
            Vector3 newVec = new Vector3();
            newVec.x = float.Parse(tokens[0]);
            newVec.y = float.Parse(tokens[1]);
            newVec.z = float.Parse(tokens[2]);

            // Return the created vector
            return newVec;
        }

        public static Color ParseColor(string _str)
        {
            // Start by removing the parentheses
            int openBracketIndex = _str.IndexOf('(') + 1;
            int closeBracketIndex = _str.IndexOf(')');
            int substrLength = closeBracketIndex - openBracketIndex;
            _str = _str.Substring(openBracketIndex, substrLength);

            // Split the rest of the string on the commas to get the individual floats
            string[] tokens = _str.Split(',');

            // Create a new colour and parse the individual floats into it
            Color newColor = new Color();
            newColor.r = float.Parse(tokens[0]);
            newColor.g = float.Parse(tokens[1]);
            newColor.b = float.Parse(tokens[2]);
            newColor.a = float.Parse(tokens[3]);

            // Return the created colour
            return newColor;
        }

        public static Quaternion ParseQuaternion(string _str)
        {
            // Start by removing the parentheses
            int openBracketIndex = _str.IndexOf('(') + 1;
            int closeBracketIndex = _str.IndexOf(')');
            int substrLength = closeBracketIndex - openBracketIndex;
            _str = _str.Substring(openBracketIndex, substrLength);

            // Split the rest of the string on the commas to get the individual floats
            string[] tokens = _str.Split(',');

            // Create a new quaternion and parse the individual floats into it
            Quaternion newQuat = new Quaternion();
            newQuat.x = float.Parse(tokens[0]);
            newQuat.y = float.Parse(tokens[1]);
            newQuat.z = float.Parse(tokens[2]);
            newQuat.w = float.Parse(tokens[3]);

            // Return the created quaternion
            return newQuat;
        }

        public static string RemoveIDString(string _name)
        {
            // Find the last '_' in the name, since that is where the ID starts
            int underscoreIdx = _name.LastIndexOf('_');

            // Shorten the string to cut off anything at the underscore and beyond
            string nameWithoutID = _name.Substring(0, underscoreIdx);

            // Return the shortened name
            return nameWithoutID;
        }

        public static string GetFileNameFromSetName(string _setName)
        {
            // Find the last '(' since that is where the file name starts
            int openBracketIdx = _setName.LastIndexOf('(');
            int length = _setName.Length - openBracketIdx - 2; // Subtract an extra value to compensate for the end bracket

            // Shorten the string
            string fileName = _setName.Substring(openBracketIdx + 1, length);

            // Return the shortened string
            return fileName;
        }
    }
}