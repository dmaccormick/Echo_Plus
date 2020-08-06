using UnityEngine;
using UnityEditor;
using Echo.Recording;

namespace Echo.Editor
{
    [CustomEditor(typeof(Recording_Object))]
    [CanEditMultipleObjects]
    public class Editor_RecordingObject : UnityEditor.Editor
    {
        //--- Unity Methods ---//
        public override void OnInspectorGUI()
        {
            // Update the object
            this.serializedObject.Update();

            // Create a box that surrounds the quick setup features
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                // Put a bit of space to make it easier to read
                EditorGUILayout.Space();

                // Add a label to identify the buttons
                EditorGUILayout.LabelField("Quick Setup (Removes Current Settings)", EditorStyles.boldLabel);

                // Put a bit of space to make it easier to read
                EditorGUILayout.Space();

                // Create the buttons for the default setup options in a horizontal box
                EditorGUILayout.BeginHorizontal();
                {
                    // Create the button for setting the object up as a static object
                    if (GUILayout.Button("Setup Default Static"))
                    {
                        // Get all of the targets in case multiple objects are selected
                        Object[] targetObjs = this.targets;

                        // Setup all of the targets
                        foreach(Object targetObject in targetObjs)
                        {
                            Recording_Object targetComp = targetObject as Recording_Object;
                            targetComp.SetupDefaultStatic();
                        }
                    }

                    // Create the button for setting the object up as a dynamic object
                    if (GUILayout.Button("Setup Default Dynamic"))
                    {
                        // Get all of the targets in case multiple objects are selected
                        Object[] targetObjs = this.targets;

                        // Setup all of the targets
                        foreach (Object targetObject in targetObjs)
                        {
                            Recording_Object targetComp = targetObject as Recording_Object;
                            targetComp.SetupDefaultDynamic();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                // Put a bit of space to make it easier to read
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            // Draw the rest of the default inspector
            DrawDefaultInspector();

            // Apply the modified values
            this.serializedObject.ApplyModifiedProperties();
        }
    }
}