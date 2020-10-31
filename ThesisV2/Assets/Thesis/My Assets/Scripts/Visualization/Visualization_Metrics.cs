using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Thesis.Visualization
{
    public class Visualization_Metrics : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("File Information")]
        public string m_baseFileName;
        public string m_playerID;
        public string m_genreName;



        //--- Private Variables ---//
        [Header("Controls")]
        private bool m_shouldBeTracking = false;

        [Header("Cameras")]
        private int m_numTimesSwitchedToControllableCam = 0; // Count for how many times the user selected a new camera from the side panel that WAS the controllable one
        private int m_numTimesSwitchedToOtherCam = 0; // Count for how many times the user selected a new camera from the side panel that was NOT the controllable one
        private int m_numTotalCameraChanges = 0; // The total count for how many times the user changed cameras, a sum of the above two values
        private float m_timeSpentInControllableCam = 0.0f; // The amount of time (in seconds) that the user spent in the controllable camera
        private float m_timeSpentInOtherCams = 0; // The amount of time (in seconds) that the user spent in any other camera (static OR dynamic)
        private float m_totalDistanceMovedInControllableCam = 0; // The distance (in unity units (m)) that the user flew around while in the controllable camera

        [Header("Object Sets")]
        private int m_numTimesOutlinesChanged = 0; // The number of times the outline panel was opened
        private int m_numTimesSetSolod = 0; // The number of times solo buttons were pressed
        private int m_numTimesSetHidden = 0; // The number of times sets were hidden using the side panel
        private int m_numTimesSetVisible = 0; // The number of times sets were made visible using the side panel (when previously hidden)

        [Header("Focus Targeting")]
        private int m_keyObjectTargetChanges = 0; // The number of times the user focused onto a key target (either using the side panel or clicking in the scene)
        private int m_nonKeyObjectTargetChanges = 0; // The number of times the user focused on a dynamic but NOT key target (can only happen by clicking in the scene)
        private int m_numTimesFocusTargetVisibilityToggled = 0; // The number of times the user toggled the visibility of the focus target using the button in the key object panel header

        [Header("Timescale Controls")] 
        private int m_numTimesSpeedSetTo0_1x = 0; // The number of times the user pressed the 0.1x speed button
        private int m_numTimesSpeedSetTo0_5x = 0; // The number of times the user pressed the 0.5x speed button
        private int m_numTimesSpeedSetTo1x = 0; // The number of times the user pressed the 1x speed button
        private int m_numTimesSpeedSetTo2x = 0; // The number of times the user pressed the 2x speed button
        private int m_numTimesSpeedSetTo5x = 0; // The number of times the user pressed the 5x speed button
        private int m_numTimesSpeedButtonPressed = 0; // The total number of times a speed button was pressed (sum of the above numbers)
        private float m_timeSpentScrubbing = 0.0f; // The total amount of time (in seconds) that the user was scrubbing along the timeline

        [Header("UI Panels")]
        private int m_numTimesCamListPanelToggled = 0; // The number of times the user toggled the side camera panel
        private int m_numTimesKeyObjectsPanelToggled = 0; // The number of times the user toggled the side key object panel
        private int m_numTimesCamControlsPanelToggled = 0; // The number of times the user toggled the bottom camera controls panel
        private int m_numTimesSetsPanelToggled = 0; // The number of times the user toggled the side object sets panel



        //--- Unity Methods ---//
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
                WriteFile();
        }



        //--- Control Methods ---//
        public void StartTracking()
        {
            m_shouldBeTracking = true;
        }



        //--- File Methods ---//
        public void WriteFile()
        {
            // Create the full file path
            string folderPath = Application.dataPath + "/MetricLogs/";
            string fileName = m_baseFileName + m_playerID + "_" + m_genreName;
            string fileExtension = ".csv";
            string fullFilePath = folderPath + fileName + fileExtension;

            // Attempt to save the data to the file
            try
            {
                // Open the file
                StreamWriter file = new StreamWriter(fullFilePath);

                // Write all of the information
                WriteColumnNames(file);
                WriteData(file);

                // Close the file
                file.Close();

                Debug.Log("File saved correctly to: " + fullFilePath);
            }
            catch(Exception e)
            {
                Debug.LogError("Error when saving Echo metrics file: " + e.Message);
            }
        }

        public void WriteColumnNames(StreamWriter _file)
        {
            // Create a string builder that we can use to slowly build up the text line
            StringBuilder strBuild = new StringBuilder();

            // Participant information
            strBuild.Append("ParticipantID,");
            strBuild.Append("Genre,");

            // Camera information
            strBuild.Append("NumTimesChangedToControlCam,");
            strBuild.Append("NumTimesChangedToOtherCam,");
            strBuild.Append("TotalNumCameraChanges,");
            strBuild.Append("TimeInControlCam(s),");
            strBuild.Append("TimeInOtherCams,");
            strBuild.Append("DistMovedInControlCam(m),");

            // Object set information
            strBuild.Append("NumOutlineChanges,");
            strBuild.Append("NumSetsSolod,");
            strBuild.Append("NumSetsMadeHidden,");
            strBuild.Append("NumSetsMadeVisible,");

            // Focus targeting information
            strBuild.Append("NumTimesKeyTargetChanged,");
            strBuild.Append("NumTimesNonKeyTargetChanged,");
            strBuild.Append("NumTimesFocusVisibilityToggled,");

            // Timescale information
            strBuild.Append("NumTimesSpeedTo0.1x,");
            strBuild.Append("NumTimesSpeedTo0.5x,");
            strBuild.Append("NumTimesSpeedTo1.0x,");
            strBuild.Append("NumTimesSpeedTo2.0x,");
            strBuild.Append("NumTimesSpeedTo5.0x,");
            strBuild.Append("NumTimesSpeedChanged,");
            strBuild.Append("TimeSpentScrubbing(s),");

            // UI Panel Information
            strBuild.Append("NumTimesCamListToggled,");
            strBuild.Append("NumTimesKeyObjListToggled,");
            strBuild.Append("NumTimesCamControlsToggled,");
            strBuild.Append("NumTimesSetsListToggled,");

            // Write the entire line to the file
            _file.WriteLine(strBuild.ToString());
        }

        public void WriteData(StreamWriter _file)
        {
            // Create a string builder that we can use to slowly build up the text line
            StringBuilder strBuild = new StringBuilder();

            // Participant information
            strBuild.Append(m_playerID.ToString() + ",");
            strBuild.Append(m_genreName.ToString() + ",");

            // Camera information
            strBuild.Append(m_numTimesSwitchedToControllableCam.ToString() + ",");
            strBuild.Append(m_numTimesSwitchedToOtherCam.ToString() + ",");
            strBuild.Append(m_numTotalCameraChanges.ToString() + ",");
            strBuild.Append(m_timeSpentInControllableCam.ToString() + ",");
            strBuild.Append(m_timeSpentInOtherCams.ToString() + ",");
            strBuild.Append(m_totalDistanceMovedInControllableCam.ToString() + ",");

            // Object set information
            strBuild.Append(m_numTimesOutlinesChanged.ToString() + ",");
            strBuild.Append(m_numTimesSetSolod.ToString() + ",");
            strBuild.Append(m_numTimesSetHidden.ToString() + ",");
            strBuild.Append(m_numTimesSetVisible.ToString() + ",");

            // Focus targeting information
            strBuild.Append(m_keyObjectTargetChanges.ToString() + ",");
            strBuild.Append(m_nonKeyObjectTargetChanges.ToString() + ",");
            strBuild.Append(m_numTimesFocusTargetVisibilityToggled.ToString() + ",");

            // Timescale information
            strBuild.Append(m_numTimesSpeedSetTo0_1x.ToString() + ",");
            strBuild.Append(m_numTimesSpeedSetTo0_5x.ToString() + ",");
            strBuild.Append(m_numTimesSpeedSetTo1x.ToString() + ",");
            strBuild.Append(m_numTimesSpeedSetTo2x.ToString() + ",");
            strBuild.Append(m_numTimesSpeedSetTo5x.ToString() + ",");
            strBuild.Append(m_numTimesSpeedButtonPressed.ToString() + ",");
            strBuild.Append(m_timeSpentScrubbing.ToString() + ",");

            // UI Panel Information
            strBuild.Append(m_numTimesCamListPanelToggled.ToString() + ",");
            strBuild.Append(m_numTimesKeyObjectsPanelToggled.ToString() + ",");
            strBuild.Append(m_numTimesCamControlsPanelToggled.ToString() + ",");
            strBuild.Append(m_numTimesSetsPanelToggled.ToString() + ",");

            // Write the entire line to the file
            _file.WriteLine(strBuild.ToString());
        }



        //--- Camera Methods ---//
        public void IncreaseNumChangesToControllableCam()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesSwitchedToControllableCam++;
                m_numTotalCameraChanges++;
            }
        }

        public void IncreaseNumChangesToOtherCams()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesSwitchedToOtherCam++;
                m_numTotalCameraChanges++;
            }
        }

        public void IncreaseTimeSpentInControllableCam(float _deltaTime)
        {
            if (m_shouldBeTracking)
            {
                m_timeSpentInControllableCam += _deltaTime;
            }
        }

        public void IncreaseTimeSpentInOtherCams(float _deltaTime)
        {
            if (m_shouldBeTracking)
            {
                m_timeSpentInOtherCams += _deltaTime;
            }
        }

        public void IncreaseMovementAmountInControllableCam(float _deltaDist)
        {
            if (m_shouldBeTracking)
            {
                m_totalDistanceMovedInControllableCam += _deltaDist;
            }
        }



        //--- Object Set Methods ---//
        public void IncreaseOutlineChangeCount()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesOutlinesChanged++;
            }
        }

        public void IncreaseNumTimesSetSolod()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesSetSolod++;
            }
        }

        public void IncreaseNumTimesSetHidden()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesSetHidden++;
            }
        }

        public void IncreaseNumTimesSetVisible()
        {
            if (m_shouldBeTracking)
            {
                m_numTimesSetVisible++;
            }
        }
    }
}