using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Thesis.Editor
{
    public class Editor_HelperWindow : EditorWindow
    {
        //--- Private Variables ---//
        private static SceneAsset m_recScene;
        private static SceneAsset m_visScene;



        //--- Unity Methods ---//
        [MenuItem("Window/Echo Helper Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            Editor_HelperWindow window = (Editor_HelperWindow)EditorWindow.GetWindow(typeof(Editor_HelperWindow), false, "Echo Helper");
            window.Show();
        }

        private void OnGUI()
        {
            // Create the fields so the user can assign the scene objects
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Recording Scene:");
            m_recScene = EditorGUILayout.ObjectField(m_recScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Visualization Scene:");
            m_visScene = EditorGUILayout.ObjectField(m_visScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Create the buttons to instantly load the recording and visualization scenes
            EditorGUI.BeginDisabledGroup(m_recScene == null);
            if (GUILayout.Button("Load Recording Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string recScenePath = AssetDatabase.GetAssetPath(m_recScene);
                EditorSceneManager.OpenScene(recScenePath, OpenSceneMode.Single);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(m_visScene == null);
            if (GUILayout.Button("Load Visualization Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string visScenePath = AssetDatabase.GetAssetPath(m_visScene);
                EditorSceneManager.OpenScene(visScenePath, OpenSceneMode.Single);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}