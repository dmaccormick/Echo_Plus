using UnityEngine;

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
        private VisCam_Combined m_combinedCam;
        private bool m_menuOpen;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_activeCam = VisCam_CamName.Orbit;
            m_orbitCam = GetComponent<VisCam_OrbitCam>();
            m_fpsCam = GetComponent<VisCam_FPSCam>();
            m_combinedCam = GetComponent<VisCam_Combined>();
            m_menuOpen = false;
        }

        private void Update()
        {
            // Only control the camera if actually able to do so. Can't move the camera if another menu is open
            if (m_activeCam != VisCam_CamName.None && !m_menuOpen)
            {
                // Determine the current height of the camera. Use absolute value so it considers being below the level as well
                float camHeight = Mathf.Abs(m_cam.transform.position.y);

                // Calculate the speed multiplier depending on the height of the camera
                // Under the interval, the camera moves at the base speed
                // Above the interval, the multiplier is applied
                // The multiplier is how many intervals the camera is currently at
                // Ex: If the interval is 10m, the camera is at base speed 10m and under. At 50m, it moves 5x faster
                float speedMultiplier = (camHeight < m_heightMultiplerInterval) ? 1.0f : camHeight / m_heightMultiplerInterval;

                // Update the active camera script and pass it the speed multiplier
                if (m_activeCam == VisCam_CamName.Orbit)
                    //m_orbitCam.UpdateCamera(speedMultiplier);
                    m_combinedCam.UpdateCamera(speedMultiplier);
                else
                    m_fpsCam.UpdateCamera(speedMultiplier);
            }
            else
            {
                // TEMP: Consider the combined camera to be the none setting
                if (m_activeCam == VisCam_CamName.None && !m_menuOpen)
                {
                    // Determine the current height of the camera. Use absolute value so it considers being below the level as well
                    float camHeight = Mathf.Abs(m_cam.transform.position.y);

                    // Calculate the speed multiplier depending on the height of the camera
                    // Under the interval, the camera moves at the base speed
                    // Above the interval, the multiplier is applied
                    // The multiplier is how many intervals the camera is currently at
                    // Ex: If the interval is 10m, the camera is at base speed 10m and under. At 50m, it moves 5x faster
                    float speedMultiplier = (camHeight < m_heightMultiplerInterval) ? 1.0f : camHeight / m_heightMultiplerInterval;

                    m_combinedCam.UpdateCamera(speedMultiplier);
                }
            }
        }



        //--- Methods ---//
        public VisCam_CamName CycleActiveCamera()
        {
            // If the current cam is the FPS cam, we should release the pivot
            // If it is the orbit cam, we should hide the orbit target indicator
            if (m_activeCam == VisCam_CamName.Fps)
            {
                m_fpsCam.ReleasePivot();
            }
            else if (m_activeCam == VisCam_CamName.Orbit)
            {
                // Hide the target and disable follow so that we don't get jolted back when switching back to the orbit camera
                m_orbitCam.HidePickingTargetIcons();
                m_orbitCam.StopFollowing();
            }

            // Switch to the next camera in the list
            int currentCamIndex = (int)m_activeCam;
            currentCamIndex++;

            // Wrap the cam index if need
            if (currentCamIndex > (int)VisCam_CamName.None)
                currentCamIndex = 0;

            // Update the active camera
            m_activeCam = (VisCam_CamName)currentCamIndex;

            // If the new cam is the FPS cam, we should grab the orbit cam's pivot and switch their relationship
            if (m_activeCam == VisCam_CamName.Fps)
                m_fpsCam.GrabPivot();

            // Return the newly selected active camera type
            return m_activeCam;
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



        //--- Getters ---//
        public VisCam_CamName GetActiveCamera()
        {
            return m_activeCam;
        }
    }

}