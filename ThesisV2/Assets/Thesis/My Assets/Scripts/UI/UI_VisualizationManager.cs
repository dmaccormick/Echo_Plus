using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Thesis.Visualization;
using Thesis.Visualization.VisCam;
using System.Collections.Generic;
using System.IO;

namespace Thesis.UI
{
    public class UI_VisualizationManager : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Controls")]
        public Visualization_Manager m_visManager;
        public VisCam_CameraControls m_camControls;
        public Visualization_Metrics m_metrics;

        [Header("Information Panel")]
        public GameObject m_pnlInfoParent;
        public GameObject m_pnlInfoOrbitCam;
        public GameObject m_pnlInfoFPSCam;
        public GameObject m_pnlInfoNoCam;

        [Header("Toolbar Top Left Elements")]
        public Button m_btnOpenSettings;
        public Button m_btnOpenObjectList;
        public Button m_btnToggleCamControls;
        public Image m_imgCamIcon;
        public Sprite[] m_sprCamIcons;

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
        public Transform m_objectListParent;
        public GameObject m_objectListElementPrefab;

        [Header("Camera List UI Elements")]
        public GameObject m_pnlCameraList;
        public Transform m_cameraListParent;
        public GameObject m_cameraListElementPrefab;

        [Header("Camera Quick Switch UI Elements")]
        public GameObject m_pnlQuickSelect;

        [Header("Key Object List UI Elements")]
        public GameObject m_pnlKeyObjectList;
        public Transform m_keyObjectListParent;
        public GameObject m_keyObjectListElementPrefab;
        public Toggle m_keyObjectTglFocusTarget;
        public GameObject m_keyObjectTglFocusTargetIndicator;

        [Header("Colour Selector UI Elements")]
        public GameObject m_pnlColourSelector;
        public UI_ColourSelector m_scriptColourSelector;

        [Header("EXE File Path")]
        public string m_exeLogFileFolderName;
        public int m_firstDynamicIndex;
        public int m_lastDynamicIndex;



        //--- Unity Methods ---//
        private void OnEnable()
        {
            // Register the visualization manager events
            m_visManager.m_onPlaystateUpdated.AddListener(this.OnVisPlaystateUpdated);
            m_visManager.m_onTimeUpdated.AddListener(this.OnVisTimeUpdated);
            m_visManager.m_onKeyObjectListUpdated.AddListener(this.OnKeyObjectListUpdated);
        }

        private void OnDisable()
        {
            // Unregister the visualization manager events
            m_visManager.m_onPlaystateUpdated.RemoveListener(this.OnVisPlaystateUpdated);
            m_visManager.m_onTimeUpdated.RemoveListener(this.OnVisTimeUpdated);
            m_visManager.m_onKeyObjectListUpdated.RemoveListener(this.OnKeyObjectListUpdated);
        }



        //--- Toolbar Top Left Callbacks ---//
        public void OnToggleSettings()
        {
            // Toggle the settings UI panel
            bool isPanelActive = m_pnlSettings.gameObject.activeSelf;
            m_pnlSettings.gameObject.SetActive(!isPanelActive);

            // Hide the other menus
            m_pnlObjectList.gameObject.SetActive(false);
            m_pnlCameraList.gameObject.SetActive(false);

            // Toggle the camera controls depending on if the menu is open or not
            m_camControls.SetMenuOpen(m_pnlSettings.gameObject.activeSelf);

            // Toggle the info panel depending on if the menu is open or not
            m_pnlInfoParent.SetActive(!m_pnlSettings.gameObject.activeSelf);
        }

        public void OnToggleObjectList()
        {
            // Toggle the object list UI panel
            bool isPanelActive = m_pnlObjectList.gameObject.activeSelf;
            m_pnlObjectList.gameObject.SetActive(!isPanelActive);

            // Hide the other menus
            m_pnlSettings.gameObject.SetActive(false);
            m_pnlCameraList.gameObject.SetActive(false);

            // Toggle the camera controls depending on if the menu is open or not
            m_camControls.SetMenuOpen(m_pnlObjectList.gameObject.activeSelf);

            // Toggle the info panel depending on if the menu is open or not
            m_pnlInfoParent.SetActive(!m_pnlObjectList.gameObject.activeSelf);
        }
        
        public void OnToggleCameraList()
        {
            // Toggle the camera list UI panel
            bool isPanelActive = m_pnlCameraList.gameObject.activeSelf;
            m_pnlCameraList.gameObject.SetActive(!isPanelActive);

            // Hide the other menus
            m_pnlSettings.gameObject.SetActive(false);
            m_pnlObjectList.gameObject.SetActive(false);

            // Toggle the camera controls depending on if the menu is open or not
            m_camControls.SetMenuOpen(m_pnlCameraList.gameObject.activeSelf);

            // Toggle the info panel depending on if the menu is open or not
            m_pnlInfoParent.SetActive(!m_pnlCameraList.gameObject.activeSelf);

            // Create the camera list UI if the panel is now active
            if (m_pnlCameraList.gameObject.activeSelf)
                CreateCameraListUI();
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
            // Decide if we actually should load the file after all
            bool proceedWithLoading = true;

            // Since there can only be one static file, we should confirm if the user wants to overwrite any currently loaded static files
            if (m_visManager.GetStaticObjectSet() != null)
            {
#if UNITY_EDITOR
                // Show a dialog box that lets the user cancel loading if they want to
                proceedWithLoading = EditorUtility.DisplayDialog("Overwrite Current Static Objects?", "Only one static object set can be loaded at a time. If you load this file, it will overwrite the static objects you already have loaded. Do you wish to proceed?", "Yes, Overwrite The Static Objects", "Cancel");
#endif
            }

            // Load the file if requested to do so
            if (proceedWithLoading)
            {
#if UNITY_EDITOR
                // Get the static file location from the input field
                string staticPath = m_inStaticLoadLoc.text;
#else
                // If in build mode, use the log files that are directly beside the EXE
                string staticPath = Path.GetDirectoryName(Application.dataPath);
                staticPath = Path.Combine(new string[] { staticPath, "Phase1Data", m_exeLogFileFolderName, "Logs", "StaticData_" + m_exeLogFileFolderName + ".log"});
#endif

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

                    // Setup the player camera manager system
                    GameObject.FindObjectOfType<VisCam_PlayerCameraManager>().Setup();

                    // Update the UI elements for the list of loaded object sets and the cameras
                    CreateObjectListUI();
                    CreateCameraListUI();

                    // Start the metric tracking if it hasn't begun yet
                    m_metrics.StartTracking();

#if UNITY_EDITOR
                    // Show a message that the file loaded correctly
                    EditorUtility.DisplayDialog("Static File Load Successful", "The static log file data loaded correctly!", "Continue");
#endif
                }
                else
                {
#if UNITY_EDITOR
                    // Show a message that the file failed to load correctly
                    EditorUtility.DisplayDialog("Static File Load Failed", "The static log file failed to load!", "Continue");
#endif
                }
            }
        }

        public void OnLoadDynamicFile()
        {
#if UNITY_EDITOR
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

                // Setup the player camera manager system
                GameObject.FindObjectOfType<VisCam_PlayerCameraManager>().Setup();
                OnCameraEnabled(m_camControls.GetActiveCameraRef());

                // Update the UI elements for the list of loaded object sets and the cameras
                CreateObjectListUI();
                CreateCameraListUI();

                // Start the metric tracking if it hasn't begun yet
                m_metrics.StartTracking();

                // Show a message that the file loaded correctly
                EditorUtility.DisplayDialog("Dynamic File Load Successful", "The dynamic log file data loaded correctly!", "Continue");
            }
            else
            {
                // Show a message that the file failed to load correctly
                EditorUtility.DisplayDialog("Dynamic File Load Failed", "The dynamic log file failed to load!", "Continue");
            }
#else
            // Load all of the dynamic files for this dataset
            for (int i = m_firstDynamicIndex; i <= m_lastDynamicIndex; i++)
            {
                // If in build mode, use the log files that are directly beside the EXE
                string dynamicPath = Path.GetDirectoryName(Application.dataPath);
                dynamicPath = Path.Combine(new string[] { dynamicPath, "Phase1Data", m_exeLogFileFolderName, "Logs", "DynamicData_" + m_exeLogFileFolderName  + "_" + i.ToString() + ".log" });

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

                    // Setup the player camera manager system
                    GameObject.FindObjectOfType<VisCam_PlayerCameraManager>().Setup();
                    OnCameraEnabled(m_camControls.GetActiveCameraRef());

                    // Update the UI elements for the list of loaded object sets and the cameras
                    CreateObjectListUI();
                    CreateCameraListUI();
                }
            }
#endif
        }



        //--- Object List Methods ---//
        public void CreateObjectListUI()
        {
            // Clear the current list elements
            ClearObjectListUI();

            // Get the up to date static and dynamic file sets
            Visualization_ObjectSet staticSet = m_visManager.GetStaticObjectSet();
            List<Visualization_ObjectSet> dynamicSets = m_visManager.GetDynamicObjectSets();

            // If there is a static set loaded, we should create a UI list element for it
            if (staticSet != null)
                CreateObjectListElement(staticSet);

            // If there are dynamic sets loaded, we should create UI list elements for all of them
            if (dynamicSets != null)
            {
                foreach (Visualization_ObjectSet objSet in dynamicSets)
                    CreateObjectListElement(objSet);
            }
        }

        public void ClearObjectListUI()
        {
            // Delete all of the object list elements currently in the UI
            for (int i = 0; i < m_objectListParent.childCount; i++)
            {
                // Get the element
                Transform child = m_objectListParent.GetChild(i);

                // Get the list element component from the child and unregister from the event before destroying it
                UI_ObjectSetListElement uiComp = child.gameObject.GetComponent<UI_ObjectSetListElement>();
                uiComp.m_onDeleteSet.RemoveAllListeners();

                // Detroy the element
                Destroy(child.gameObject);
            }
        }

        public void CreateObjectListElement(Visualization_ObjectSet _targetSet)
        {
            // Instantiate a new list element under the list parent
            GameObject listElement = Instantiate(m_objectListElementPrefab, m_objectListParent);

            // Grab the list element component and set it up
            UI_ObjectSetListElement uiComp = listElement.GetComponent<UI_ObjectSetListElement>();
            uiComp.InitWithObjectSet(_targetSet);

            // Register for the deletion event
            uiComp.m_onDeleteSet.AddListener(OnSetListDeleted);
        }

        public void OnSetListDeleted(Visualization_ObjectSet _deletedSet)
        {
            // Delete the set from the visualization manager
            if (!m_visManager.DeleteObjectSet(_deletedSet))
            {
#if UNITY_EDITOR
                // If the deletion failed for some reason, show a dialog box indicating that
                EditorUtility.DisplayDialog("Set Deletion Failed!", "There was an error when trying to delete the set!", "Continue");
#endif
            }

            // If there are no more dynamic sets, we should hide the timeline controls
            if (m_visManager.GetDynamicObjectSets().Count == 0)
            {
                m_pnlTimeLineControls.SetActive(false);
                m_pnlSpeedControls.SetActive(false);
            }

            // Rebuild the object set UI
            CreateObjectListUI();

            // Switch back to the main camera
            m_camControls.m_cam.enabled = true;
            //m_pnlQuickSelect.SetActive(true);
            FindObjectOfType<VisCam_PlayerCameraManager>().DisableAllCameras();
        }



        //--- Camera List Methods ---//
        public void CreateCameraListUI()
        {
            // Clear the current list elements
            ClearCameraListUI();

            // Get the main controllable camera and create a UI element for it
            Camera controllableCam = GameObject.FindObjectOfType<VisCam_CameraControls>().GetActiveCameraRef();
            CreateCameraListElement(controllableCam, false);

            // Create UI elements for any loaded player cameras as well
            var playerCamManager = GameObject.FindObjectOfType<VisCam_PlayerCameraManager>();
            if (playerCamManager != null)
            {
                Camera[] playerCams = playerCamManager.GetAllCameras();
                if (playerCams != null && playerCams.Length > 0)
                {
                    foreach (var cam in playerCams)
                        CreateCameraListElement(cam, true);
                }
                else
                {
                    // If there are no player cams, we should ensure the controllable one is active
                    OnCameraEnabled(controllableCam);
                }
            }
            else
            {
                // No player cam manager so the controllable camera should be the active one
                OnCameraEnabled(controllableCam);
            }
        }

        public void ClearCameraListUI()
        {
            // Delete all of the camera list elements currently in the UI
            for (int i = 0; i < m_cameraListParent.childCount; i++)
            {
                // Get the element
                Transform child = m_cameraListParent.GetChild(i);

                // Get the list element component from the child and unregister from the event before destroying it
                UI_CameraListElement uiComp = child.gameObject.GetComponent<UI_CameraListElement>();
                uiComp.m_onActivateCamera.RemoveAllListeners();

                // Detroy the element
                Destroy(child.gameObject);
            }
        }

        public void CreateCameraListElement(Camera _cam, bool _hasParentSet)
        {
            // Instantiate a new list element under the list parent
            GameObject listElement = Instantiate(m_cameraListElementPrefab, m_cameraListParent);

            // Grab the list element UI component and set it up
            UI_CameraListElement uiComp = listElement.GetComponent<UI_CameraListElement>();
            uiComp.InitWithCam(_cam, _hasParentSet);

            // Register for the camera activation event
            uiComp.m_onActivateCamera.AddListener(OnCameraEnabled);
        }

        public void OnCameraEnabled(Camera _cam)
        {
            // Activate the given camera
            _cam.enabled = true;

            // If the camera is not the controllable one, we should disable that one
            Camera controllableCam = m_camControls.GetActiveCameraRef();
            if (_cam != controllableCam)
            {
                controllableCam.enabled = false;
                m_camControls.SetMenuOpen(true); // using this as a way of disabling the camera controls for now
                FindObjectOfType<VisCam_Combined>().SetIndicatorVisible(false);

                // Update the metric counter
                m_metrics.IncreaseNumChangesToOtherCams();

                // Hide the quick selection UI
               // m_pnlQuickSelect.SetActive(false);
            }
            else
            {
                m_camControls.SetMenuOpen(false); // using this as a way of enabling the camera controls
                FindObjectOfType<VisCam_Combined>().SetIndicatorVisible(true);

                // Update the metric counter
                m_metrics.IncreaseNumChangesToControllableCam();

                // Show the quick selection UI
                //m_pnlQuickSelect.SetActive(true);
            }

            // Disable all of the unused cameras and activate the right one
            GameObject.FindObjectOfType<VisCam_PlayerCameraManager>().EnableCamera(_cam);

            // Update all of the camera UI elements so they show the icon or not
            foreach (var camUI in GameObject.FindObjectsOfType<UI_CameraListElement>())
                camUI.UpdateActiveIcon();
        }



        //--- Key Object List Methods ---//
        public void ClearKeyObjectList()
        {
            // Delete all of the key object list elements currently in the UI
            for (int i = 0; i < m_keyObjectListParent.childCount; i++)
            {
                // Get the element
                Transform child = m_keyObjectListParent.GetChild(i);

                //// Get the list element component from the child and unregister from the event before destroying it
                //UI_CameraListElement uiComp = child.gameObject.GetComponent<UI_CameraListElement>();
                //uiComp.m_onActivateCamera.RemoveAllListeners();

                // Detroy the element
                Destroy(child.gameObject);
            }
        }

        public void CreateKeyObjectListElement(GameObject _keyObj)
        {
            // Instantiate a new list element under the list parent
            GameObject listElement = Instantiate(m_keyObjectListElementPrefab, m_keyObjectListParent);

            // Grab the list element UI component and set it up
            UI_KeyObjListElement uiComp = listElement.GetComponent<UI_KeyObjListElement>();
            uiComp.InitWithObject(_keyObj);
        }

        public void ToggleFocusTargetVisibility(bool _isVisible)
        {
            // Tell the vis camera to update the visibility
            m_camControls.ToggleFocusTargetVisibility(_isVisible);

            // Toggle the visibility of the disabled indicator
            m_keyObjectTglFocusTargetIndicator.SetActive(!_isVisible);
        }



        //--- Colour Selector Methods ---//
        public void OpenColourSelector(GameObject _callingUIElement, Visualization_ObjectSet _setToChange)
        {
            // Activate the colour selector
            m_pnlColourSelector.SetActive(true);

            // Update it so it references the right object and set
            m_scriptColourSelector.OpenForObject(_callingUIElement, _setToChange);
        }




        //--- Vis Manager Event Callbacks ---//
        public void OnVisPlaystateUpdated(Playstate _newPlaystate)
        {
            // Toggle the reverse, pause, and forward buttons to match the new playstate
            m_btnReversePlayback.interactable = (_newPlaystate != Playstate.Reverse);
            m_btnPausePlayback.interactable = (_newPlaystate != Playstate.Paused);
            m_btnForwardPlayback.interactable = (_newPlaystate != Playstate.Forward);
        }

        public void OnVisTimeUpdated(float _startTime, float _currentTime, float _endTime)
        {
            // Ensure none of the values are infinity
            float startTime = (_startTime == Mathf.Infinity) ? 0.0f : _startTime;
            float currentTime = (_currentTime == Mathf.Infinity) ? 0.0f : _currentTime;
            float endTime = (_endTime == Mathf.Infinity) ? 0.0f : _endTime;

            // If the slider start needs to be adjusted to match the new value, do so
            if (m_sldTimeline.minValue != startTime)
                m_sldTimeline.minValue = startTime;

            // If the slider end needs to be adjusted to match the new value, do so
            if (m_sldTimeline.maxValue != endTime)
                m_sldTimeline.maxValue = endTime;

            // Move the slider handle to match the new time
            m_sldTimeline.value = currentTime;

            // Update the text above the slider handle and at the start and end of the timeline
            m_txtStartTime.text = startTime.ToString("F2");
            m_txtCurrentTime.text = currentTime.ToString("F2");
            m_txtEndTime.text = endTime.ToString("F2");
        }

        public void OnKeyObjectListUpdated(List<GameObject> _keyObjs)
        {
            // Clear the current list
            ClearKeyObjectList();

            // Create new elements for all of the key objects
            foreach (var keyObject in _keyObjs)
                CreateKeyObjectListElement(keyObject);
        }
    }
}