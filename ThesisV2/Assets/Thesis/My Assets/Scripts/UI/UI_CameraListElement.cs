using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Thesis.VisTrack;

namespace Thesis.UI
{
    //--- Event Classes ---//
    [System.Serializable]
    public class CameraEvent : UnityEvent<Camera>
    {
    }

    public class UI_CameraListElement : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Camera Selection Indicator")]
        public Image m_imgCamSelectIndicator;

        [Header("Outline Controls")]
        public Slider m_sldOutlineHue;
        public Image m_imgOutlineColour;
        public Text m_txtOutlineLabel;

        [Header("Misc")]
        public Text m_txtCamName;

        [Header("Events")]
        public CameraEvent m_onActivateCamera;



        //--- Private Variables ---//
        private Camera m_refCamera;
        private VisTrack_Camera m_refCamTrack;



        //--- Initialization Method ---//
        public void InitWithCam(Camera _refCamera)
        {
            // Store the data internally
            this.m_refCamTrack = null;
            this.m_refCamera = _refCamera;

            // Toggle the icon to show if the camera is active or not
            this.m_imgCamSelectIndicator.gameObject.SetActive(m_refCamera.enabled);
        }

        public void InitWithCamTrack(VisTrack_Camera _refCamTrack)
        {
            // Store the data internally
            this.m_refCamTrack = _refCamTrack;
            this.m_refCamera = _refCamTrack.GetTargetCam();

            // Toggle the icon to show if the camera is active or not
            this.m_imgCamSelectIndicator.gameObject.SetActive(m_refCamera.enabled);
        }



        //--- Camera Selection Methods ---//
        public void OnSelectCamera()
        {
            // Trigger the event and pass the camera along with it
            m_onActivateCamera.Invoke(m_refCamera);
        }

        public void UpdateActiveIcon()
        {
            // Toggle the icon to show if the camera is active or not
            this.m_imgCamSelectIndicator.gameObject.SetActive(m_refCamera.enabled);
        }
    }
}
