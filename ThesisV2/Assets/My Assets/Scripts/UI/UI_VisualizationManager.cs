using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Thesis.Visualization;

namespace Thesis.UI
{
    public class UI_VisualizationManager : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Controls")]
        public Visualization_Manager m_visManager;

        [Header("Toolbar UI Elements")]
        public Button m_btnOpenSettings;

        [Header("Settings UI Elements")]
        public GameObject m_pnlSettings;
        public InputField m_inStaticLoadLoc;
        public Button m_btnLoadStaticFile;
        public Text m_txtStaticFileIndicator;
        public InputField m_inDynamicLoadLoc;
        public Button m_btnLoadDynamicFile;
        public Text m_txtDynamicFileIndicator;
        public Button m_btnClearLoadedFiles;



        //--- Toolbar Callbacks ---//
        public void OnToggleSettings()
        {
            // Toggle the settings UI panel
            bool isPanelActive = m_pnlSettings.gameObject.activeSelf;
            m_pnlSettings.gameObject.SetActive(!isPanelActive);
        }



        //--- Settings Callbacks ---//
        public void OnLoadStaticFile()
        {
            // Get the static file location from the input field
            string staticPath = m_inStaticLoadLoc.text;

            // Tell the visualization manager to load the static data
            bool loadSuccess = m_visManager.LoadStaticData(staticPath);

            // Show a dialog box to indicate if the load worked
            if (loadSuccess)
            {
                EditorUtility.DisplayDialog("Static File Load Successful", "The static log file data loaded correctly!", "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Static File Load Failed", "The static log file failed to load!", "Continue");
            }
        }

        public void OnLoadDynamicFile()
        {
            // Get the dynamic file location from the input field
            string dynamicPath = m_inDynamicLoadLoc.text;

            // Tell the visualization manager to load the dynamic data
            bool loadSuccess = m_visManager.LoadDynamicData(dynamicPath);

            // Show a dialog box to indicate if the load worked
            if (loadSuccess)
            {
                EditorUtility.DisplayDialog("Dynamic File Load Successful", "The dynamic log file data loaded correctly!", "Continue");
            }
            else
            {
                EditorUtility.DisplayDialog("Dynamic File Load Failed", "The dynamic log file failed to load!", "Continue");
            }
        }

        public void OnClearLoadedFiles()
        {

        }
    }
}