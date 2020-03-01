﻿using UnityEngine;

namespace Thesis.Visualization.VisCam
{
    public enum VisCam_CamName
    {
        Orbit,
        Fps,
        None
    }

    public class VisCam_CameraControls : MonoBehaviour
    {
        //--- Public Variables ---//
        public Camera m_cam;
        public float m_heightMultiplerInterval;



        //--- Private Variables ---//
        private VisCam_CamName m_activeCam;
        private VisCam_OrbitCam m_orbitCam;
        private VisCam_FPSCam m_fpsCam;
        private bool m_menuOpen;



        //--- Unity Methods ---//
        private void Start()
        {
            // Init the private variables
            m_activeCam = VisCam_CamName.Orbit;
            m_orbitCam = GetComponent<VisCam_OrbitCam>();
            m_fpsCam = GetComponent<VisCam_FPSCam>();
            m_menuOpen = false;
        }

        private void Update()
        {
            // Only control the camera if actually able to do so. Can't move the camera if another menu is open
            if (m_activeCam != VisCam_CamName.None && !m_menuOpen)
            {
                // Determine the current height of the camera
                float camHeight = m_cam.transform.position.y;

                // Calculate the speed multiplier depending on the height of the camera
                // Under the interval, the camera moves at the base speed
                // Above the interval, the multiplier is applied
                // The multiplier is how many intervals the camera is currently at
                // Ex: If the interval is 10m, the camera is at base speed 10m and under. At 50m, it moves 5x faster
                float speedMultiplier = (camHeight < m_heightMultiplerInterval) ? 1.0f : camHeight / m_heightMultiplerInterval;

                // Update the active camera script and pass it the speed multiplier
                if (m_activeCam == VisCam_CamName.Orbit)
                    m_orbitCam.UpdateCamera(speedMultiplier);
                else
                    m_fpsCam.UpdateCamera(speedMultiplier);
            }
        }



        //--- Setters ---//
        public void SetActiveCamera(VisCam_CamName _activeCam)
        {
            this.m_activeCam = _activeCam;
        }

        public void SetMenuOpen(bool _menuOpen)
        {
            this.m_menuOpen = _menuOpen;
        }
    }

}