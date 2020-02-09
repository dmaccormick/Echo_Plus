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

        [Header("Toolbar Top Left Elements")]
        public Button m_btnOpenSettings;
        public Button m_btnOpenObjectList;
        public Button m_btnToggleCamControls;
        public Image m_imgCamControlIndicator;

        [Header("Toolbar Timeline Elements")]
        public Slider m_sldTimeline;
        public Text m_txtStartTime;
        public Text m_txtEndTime;
        public Text m_txtCurrentTime;

        [Header("Toolbar Speed Elements")]
        public Button m_btnTenthSpeed;
        public Button m_btnHalfSpeed;
        public Button m_btnNormalSpeed;
        public Button m_btnTwiceSpeed;
        public Button m_btnFiveSpeed;

        [Header("Settings UI Elements")]
        public GameObject m_pnlSettings;
        public InputField m_inStaticLoadLoc;
        public Button m_btnLoadStaticFile;
        public Text m_txtStaticFileIndicator;
        public InputField m_inDynamicLoadLoc;
        public Button m_btnLoadDynamicFile;
        public Text m_txtDynamicFileIndicator;
        public Button m_btnClearLoadedFiles;

        [Header("Loaded Objects UI Elements")]
        public GameObject m_pnlObjectList;



        //--- Toolbar Top Left Callbacks ---//
        public void OnToggleSettings()
        {
            // Toggle the settings UI panel
            bool isPanelActive = m_pnlSettings.gameObject.activeSelf;
            m_pnlSettings.gameObject.SetActive(!isPanelActive);

            // Hide the object list menu
            m_pnlObjectList.gameObject.SetActive(false);
        }

        public void OnToggleObjectList()
        {
            // Toggle the object list UI panel
            bool isPanelActive = m_pnlObjectList.gameObject.activeSelf;
            m_pnlObjectList.gameObject.SetActive(!isPanelActive);

            // Hide the settings menu
            m_pnlSettings.gameObject.SetActive(false);
        }
        
        public void OnToggleCameraControls()
        {
            // Toggle the indicator for if the camera controls are active
            bool isCamIndicatorActive = m_imgCamControlIndicator.gameObject.activeSelf;
            m_imgCamControlIndicator.gameObject.SetActive(!isCamIndicatorActive);

            // TODO: Toggle the actual camera control scripts
            // ...
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