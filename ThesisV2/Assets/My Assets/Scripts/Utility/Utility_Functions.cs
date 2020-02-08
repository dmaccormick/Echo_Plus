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

        public static Quaternion ParseQuaternion(string _str)
        {
            // Start by removing the parentheses
            int openBracketIndex = _str.IndexOf('(') + 1;
            int closeBracketIndex = _str.IndexOf(')');
            int substrLength = closeBracketIndex - openBracketIndex;
            _str = _str.Substring(openBracketIndex, substrLength);

            // Split the rest of the string on the commas to get the individual floats
            string[] tokens = _str.Split(',');

            // Create a new vector3 and parse the individual floats into it
            Quaternion newQuat = new Quaternion();
            newQuat.x = float.Parse(tokens[0]);
            newQuat.y = float.Parse(tokens[1]);
            newQuat.z = float.Parse(tokens[2]);
            newQuat.w = float.Parse(tokens[3]);

            // Return the created vector
            return newQuat;
        }
    }
}