using UnityEngine;
using UnityEditor;
using Thesis.Recording;

namespace Thesis.Editor
{
    [CustomEditor(typeof(Recording_Object))]
    public class Editor_RecordingObject : UnityEditor.Editor
    {
        //--- Private Variables ---//
        private SerializedObject m_targetObj;
        private Recording_Object m_targetComp;


        //--- Unity Methods ---//
        private void OnEnable()
        {
            // Init the private variables
            m_targetObj = this.serializedObject;
            m_targetComp = this.target as Recording_Object;
        }

        public override void OnInspectorGUI()
        {
            // Update the object
            m_targetObj.Update();

            // Create the buttons for the default setup options in a horizontal box
            EditorGUILayout.BeginHorizontal();
            {
                // Create the button for setting the object up as a static object
                if (GUILayout.Button("Setup Default Static"))
                {
                    m_targetComp.SetupDefaultStatic();
                }

                // Create the button for setting the object up as a dynamic object
                if (GUILayout.Button("Setup Default Dynamic"))
                {
                    m_targetComp.SetupDefaultDynamic();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Draw the rest of the default inspector
            DrawDefaultInspector();

            // Apply the modified values
            m_targetObj.ApplyModifiedProperties();
        }
    }
}