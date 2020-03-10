using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Thesis.Visualization;
using Thesis.Utility;

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
        public Image m_imgOutlineColour;
        public Text m_txtOutlineLabel;

        [Header("Misc")]
        public Text m_txtCamName;

        [Header("Events")]
        public CameraEvent m_onActivateCamera;



        //--- Private Variables ---//
        private Camera m_refCamera;



        //--- Initialization Method ---//
        public void InitWithCam(Camera _refCamera, bool _hasParentSet)
        {
            // Store the data internally
            this.m_refCamera = _refCamera;

            // Toggle the icon to show if the camera is active or not
            this.m_imgCamSelectIndicator.gameObject.SetActive(m_refCamera.enabled);

            // Grab the camera's parent set type if there is one and use it to setup the rest of the UI elements
            if (_hasParentSet)
            {
                // Grab the parent set
                Visualization_ObjectSet parentSet = _refCamera.gameObject.GetComponentInParent<Visualization_ObjectSet>();

                // Set the label that shows the name of the camera
                string camName = Utility_Functions.RemoveIDString(_refCamera.gameObject.name);
                string fullName = "\"" + camName + "\" (" + Utility_Functions.GetFileNameFromSetName(parentSet.GetSetName()) + ")";
                m_txtCamName.text = fullName;

                // Setup the outline information if the parent set has an outline, otherwise hide it
                if (parentSet.GetHasOutline())
                {
                    // Use the object set's current values to setup the outline information
                    Color.RGBToHSV(parentSet.GetOutlineColour(), out float Hue, out float S, out float V);
                    m_imgOutlineColour.color = Color.HSVToRGB(Hue, 1.0f, 1.0f);
                }
                else
                {
                    // Hide the outline controls
                    m_imgOutlineColour.gameObject.SetActive(false);
                    m_txtOutlineLabel.gameObject.SetActive(false);
                }
            }
            else
            {
                // Hide the outline controls
                m_imgOutlineColour.gameObject.SetActive(false);
                m_txtOutlineLabel.gameObject.SetActive(false);

                // Set the label that shows the name of the camera
                string camName = "Controllable Camera";
                m_txtCamName.text = camName;
            }
        }



        //--- Camera Selection Methods ---//
        public void OnSelectCamera()
        {
            // Trigger the event and pass the camera along with it
            m_onActivateCamera.Invoke(m_refCamera);
        }

        public void UpdateActiveIcon()
        {
            // If the ref camera doesn't exist, destroy this and return
            if (m_refCamera == null)
            {
                Destroy(this.gameObject);
                return;
            }

            // Toggle the icon to show if the camera is active or not
            this.m_imgCamSelectIndicator.gameObject.SetActive(m_refCamera.enabled);
        }
    }
}
