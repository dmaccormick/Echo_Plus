using UnityEngine;
using Thesis.VisTrack;
using System.Collections.Generic;

namespace Thesis.Visualization.VisCam
{
    // This manages all of the player cameras that are created when loading in a VisTrack_Camera
    public class VisCam_PlayerCameraManager : MonoBehaviour
    {
        //--- Private Variables ---//
        private List<Camera> m_cameras;
        private int m_activeCamIdx;



        //--- Methods ---//
        public void Setup()
        {
            // Find all of the player cameras in the scene by getting them from the vis tracks
            VisTrack_Camera[] playerCams = GameObject.FindObjectsOfType<VisTrack_Camera>();

            // If there are no player cameras, we can just go ahead and disable this script
            if (playerCams == null)
            {
                // Disable the script and leave
                this.enabled = false;
                return;
            }

            // Otherwise, we should grab the camera objects from them
            m_cameras = new List<Camera>();
            foreach (var playerCam in playerCams)
                m_cameras.Add(playerCam.GetTargetCam());

            // The current camera is the first one by default
            m_activeCamIdx = 0;

            // Now, we should disable all of the cameras in the scene since we start with the other cameras active
            DisableAllCameras();
        }

        public void DisableAllCameras()
        {
            // Turn all of the cameras off
            foreach (var cam in m_cameras)
                cam.enabled = false;
        }

        public void NextCamera()
        {
            // Move to the next camera in the sequence
            m_activeCamIdx++;

            // Wrap around to the start if we reached the end of the list
            if (m_activeCamIdx >= m_cameras.Count)
                m_activeCamIdx = 0;

            // Enable the newly active camera
            EnableActiveCamera();
        }

        public void PrevCamera()
        {
            // Move to the previous camera in the sequence
            m_activeCamIdx--;

            // Wrap around to the end if we reached the start of the list
            if (m_activeCamIdx < 0)
                m_activeCamIdx = m_cameras.Count - 1;

            // Enable the newly active camera
            EnableActiveCamera();
        }

        public void EnableActiveCamera()
        {
            // Loop through all of the cameras and disable all of them except for the active one
            for (int i = 0; i < m_cameras.Count; i++)
                m_cameras[i].enabled = (i == m_activeCamIdx);
        }

        public void EnableCamera(Camera _cam)
        {
            // Return if the list is not setup yet
            if (m_cameras == null)
                return;

            // Find the camera in the list and activate it
            for (int i = 0; i < m_cameras.Count; i++)
            {
                // Enable the right camera and store its index
                if (m_cameras[i] == _cam)
                {
                    m_cameras[i].enabled = true;
                    m_activeCamIdx = i;
                }
                else
                {
                    // Disable all other cameras
                    m_cameras[i].enabled = false;
                }
            }
        }

        public void OnCamDestroyed(Camera _cam)
        {
            // Remove the camera from the list
            m_cameras.Remove(_cam);

            // If the current camera index is now too high, we should lower it
            while (m_activeCamIdx > m_cameras.Count)
                m_activeCamIdx--;
        }



        //--- Getters ---//
        public Camera[] GetAllCameras()
        {
            if (m_cameras == null)
                return null;
            else
                return this.m_cameras.ToArray();
        }
    }
}