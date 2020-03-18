using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Thesis.Recording;

namespace Thesis.UI
{
    public class UI_RecordingManager : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Controls")]
        public Recording_Manager m_recManager;

        [Header("Toolbar UI Elements")]
        public Button m_btnOpenSettings;
        public Button m_btnStartRecording;
        public Button m_btnStopRecording;
        public Button m_btnSaveRecording;

        [Header("Settings UI Elements")]
        public GameObject m_pnlSettings;
        public InputField m_inStaticSaveLoc;
        public InputField m_inDynamicSaveLoc;



        //--- Toolbar UI Callbacks ---//
        public void OnToggleSettings()
        {
            // Toggle the settings UI panel
            bool isPanelActive = m_pnlSettings.gameObject.activeSelf;
            m_pnlSettings.gameObject.SetActive(!isPanelActive);
        }

        public void OnStartRecording()
        {
            // Tell the recording manager to start recording
            m_recManager.StartRecording();
        }

        public void OnStopRecording()
        {
            // Tell the recording manager to stop recording
            m_recManager.StopRecording();
        }

        public void OnSaveRecording()
        {
            // Tell the recording manager to save the static file
            string staticPath = m_inStaticSaveLoc.text;
            bool staticSaveWorked = m_recManager.SaveStaticData(staticPath);

            // Tell the recording manager to save the dynamic file as well
            string dynamicPath = m_inDynamicSaveLoc.text;
            bool dynamicSaveWorked = m_recManager.SaveDynamicData(dynamicPath);

#if UNITY_EDITOR
            // Show a dialog to indicate if the saving worked
            if (staticSaveWorked && dynamicSaveWorked)
            {
                EditorUtility.DisplayDialog("Save Successful", "Saving the static file and the dynamic file worked correctly!", "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Save Failed", "There was an error when saving the static file and the dynamic file!", "Continue");
            }
#endif
        }
    }
}