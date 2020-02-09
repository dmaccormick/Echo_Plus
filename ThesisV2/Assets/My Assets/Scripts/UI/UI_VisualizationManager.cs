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
        public GameObject m_pnlTimeLineControls;
        public Slider m_sldTimeline;
        public Text m_txtStartTime;
        public Text m_txtEndTime;
        public Text m_txtCurrentTime;
        public Button m_btnReversePlayback;
        public Button m_btnPausePlayback;
        public Button m_btnForwardPlayback;

        [Header("Toolbar Speed Elements")]
        public GameObject m_pnlSpeedControls;
        public Button m_btnTenthSpeed;
        public Button m_btnHalfSpeed;
        public Button m_btnNormalSpeed;
        public Button m_btnTwiceSpeed;
        public Button m_btnFiveSpeed;

        [Header("Settings UI Elements")]
        public GameObject m_pnlSettings;
        public InputField m_inStaticLoadLoc;
        public Button m_btnLoadStaticFile;
        public InputField m_inDynamicLoadLoc;
        public Button m_btnLoadDynamicFile;

        [Header("Loaded Objects UI Elements")]
        public GameObject m_pnlObjectList;



        //--- Unity Methods ---//
        private void OnEnable()
        {
            // Register the visualization manager events
            m_visManager.m_onPlaystateUpdated.AddListener(this.OnVisPlaystateUpdated);
            m_visManager.m_onTimeUpdated.AddListener(this.OnVisTimeUpdated);
        }

        private void OnDisable()
        {
            // Unregister the visualization manager events
            m_visManager.m_onPlaystateUpdated.RemoveListener(this.OnVisPlaystateUpdated);
            m_visManager.m_onTimeUpdated.RemoveListener(this.OnVisTimeUpdated);
        }



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



        //--- Toolbar Timeline Callbacks ---//
        public void OnSliderTimelineUpdated(float _newValue)
        {
            // Update the time in the visualization manager
            m_visManager.SetCurrentTime(_newValue);

            // Update the text above the slider handle
            m_txtCurrentTime.text = _newValue.ToString("F2");
        }

        public void OnReversePlayback()
        {
            // Reverse the playback in the visualization manager
            m_visManager.ReversePlayback();
        }

        public void OnPause()
        {
            // Pause the playback in the visualization manager
            m_visManager.PausePlayback();
        }

        public void OnForwardPlayback()
        {
            // Start playing forward in the visualization manager
            m_visManager.PlayForward();
        }



        //--- Toolbar Speed Callbacks ---//
        public void OnSetPlaybackSpeed(float _newSpeed)
        {
            // Update the playback speed in the visualization manager
            m_visManager.SetPlaybackSpeed(_newSpeed);
        }



        //--- Settings Callbacks ---//
        public void OnLoadStaticFile()
        {
            // Get the static file location from the input field
            string staticPath = m_inStaticLoadLoc.text;

            // Tell the visualization manager to load the static data
            bool loadSuccess = m_visManager.LoadStaticData(staticPath);

            // Handle the results of the loading
            if (loadSuccess)
            {
                // Update the time indicators to match the new values
                m_txtStartTime.text = m_visManager.GetStartTime().ToString("F2");
                m_txtEndTime.text = m_visManager.GetEndTime().ToString("F2");
                m_txtCurrentTime.text = m_visManager.GetCurrentTime().ToString("F2");

                // Update the slider so that its values match the start and end time
                m_sldTimeline.minValue = m_visManager.GetStartTime();
                m_sldTimeline.maxValue = m_visManager.GetEndTime();

                // Show the timeline and speed controls
                m_pnlTimeLineControls.SetActive(true);
                m_pnlSpeedControls.SetActive(true);

                // Show a message that the file loaded correctly
                EditorUtility.DisplayDialog("Static File Load Successful", "The static log file data loaded correctly!", "Continue");
            }
            else
            {
                // Show a message that the file failed to load correctly
                EditorUtility.DisplayDialog("Static File Load Failed", "The static log file failed to load!", "Continue");
            }
        }

        public void OnLoadDynamicFile()
        {
            // Get the dynamic file location from the input field
            string dynamicPath = m_inDynamicLoadLoc.text;

            // Tell the visualization manager to load the dynamic data
            bool loadSuccess = m_visManager.LoadDynamicData(dynamicPath);

            // Handle the results of the loading
            if (loadSuccess)
            {
                // Update the time indicators to match the new values
                m_txtStartTime.text = m_visManager.GetStartTime().ToString("F2");
                m_txtEndTime.text = m_visManager.GetEndTime().ToString("F2");
                m_txtCurrentTime.text = m_visManager.GetCurrentTime().ToString("F2");

                // Update the slider so that its values match the start and end time
                m_sldTimeline.minValue = m_visManager.GetStartTime();
                m_sldTimeline.maxValue = m_visManager.GetEndTime();

                // Show the timeline and speed controls
                m_pnlTimeLineControls.SetActive(true);
                m_pnlSpeedControls.SetActive(true);

                // Show a message that the file loaded correctly
                EditorUtility.DisplayDialog("Dynamic File Load Successful", "The dynamic log file data loaded correctly!", "Continue");
            }
            else
            {
                // Show a message that the file failed to load correctly
                EditorUtility.DisplayDialog("Dynamic File Load Failed", "The dynamic log file failed to load!", "Continue");
            }
        }



        //--- Vis Manager Event Callbacks ---//
        public void OnVisPlaystateUpdated(Playstate _newPlaystate)
        {
            // Toggle the reverse, pause, and forward buttons to match the new playstate
            m_btnReversePlayback.interactable = (_newPlaystate != Playstate.Reverse);
            m_btnPausePlayback.interactable = (_newPlaystate != Playstate.Paused);
            m_btnForwardPlayback.interactable = (_newPlaystate != Playstate.Forward);
        }

        public void OnVisTimeUpdated(float _newTime)
        {
            // Move the slider handle to match the new time
            m_sldTimeline.value = _newTime;

            // Update the text above the slider handle
            m_txtCurrentTime.text = _newTime.ToString("F2");
        }
    }
}