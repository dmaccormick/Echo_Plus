using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Thesis.SO;

namespace Thesis.Editor
{
    [System.Serializable]
    public class Editor_HelperWindow : EditorWindow
    {
        //--- Private Variables ---//
        //[SerializeField] private static SceneAsset m_recScene;
        //[SerializeField] private static SceneAsset m_visScene;
        private readonly string m_scriptablePath = "Assets/Thesis/My Assets/Scripts/SO/SO_HelperWindowData.asset";
        private SO_HelperWindow m_scriptable;



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
            // Find or create the scriptable object if needed
            if (m_scriptable == null)
            {
                m_scriptable = AssetDatabase.LoadAssetAtPath(m_scriptablePath, typeof(SO_HelperWindow)) as SO_HelperWindow;

                if (m_scriptable == null)
                {
                    m_scriptable = ScriptableObject.CreateInstance(typeof(SO_HelperWindow)) as SO_HelperWindow;
                    AssetDatabase.CreateAsset(m_scriptable, m_scriptablePath);
                }
            }

            // Create the fields so the user can assign the scene objects
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Participant Info Scene:");
            m_scriptable.m_participantInfoScene = EditorGUILayout.ObjectField(m_scriptable.m_participantInfoScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Recording Scene:");
            m_scriptable.m_recScene = EditorGUILayout.ObjectField(m_scriptable.m_recScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Visualization Scene:");
            m_scriptable.m_visScene = EditorGUILayout.ObjectField(m_scriptable.m_visScene, typeof(SceneAsset), false) as SceneAsset;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Create the buttons to instantly load the recording and visualization scenes
            EditorGUI.BeginDisabledGroup(m_scriptable.m_participantInfoScene == null);
            if (GUILayout.Button("Load Participant Info Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string participantScenePath = AssetDatabase.GetAssetPath(m_scriptable.m_participantInfoScene);
                EditorSceneManager.OpenScene(participantScenePath, OpenSceneMode.Single);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(m_scriptable.m_recScene == null);
            if (GUILayout.Button("Load Recording Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string recScenePath = AssetDatabase.GetAssetPath(m_scriptable.m_recScene);
                EditorSceneManager.OpenScene(recScenePath, OpenSceneMode.Single);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(m_scriptable.m_visScene == null);
            if (GUILayout.Button("Load Visualization Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                string visScenePath = AssetDatabase.GetAssetPath(m_scriptable.m_visScene);
                EditorSceneManager.OpenScene(visScenePath, OpenSceneMode.Single);
            }
            EditorGUI.EndDisabledGroup();

            // Tell Unity that the ScriptableObject asset needs to be saved when closing / opening Unity
            if (GUI.changed)
                EditorUtility.SetDirty(m_scriptable);
        }
    }
}