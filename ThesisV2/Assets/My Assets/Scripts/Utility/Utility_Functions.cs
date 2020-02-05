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
    }
}