using UnityEngine;
using UnityEngine.UI;
using Thesis.Visualization;
using Thesis.Utility;
using Thesis.Visualization.VisCam;

namespace Thesis.UI
{
    public class UI_KeyObjListElement : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Selection Indicator")]
        public Image m_imgSelectedIndicator;

        [Header("Outline Controls")]
        public Image m_imgOutlineColour;

        [Header("Misc")]
        public Text m_txtObjName;
        public Text m_txtSetName;



        //--- Private Variables ---//
        private GameObject m_refObj;
        private Visualization_ObjectSet m_parentSet;



        //--- Initialization Method ---//
        public void InitWithObject(GameObject _refObj)
        {
            // Store the data internally
            this.m_refObj = _refObj;
            m_parentSet = m_refObj.GetComponentInParent<Visualization_ObjectSet>();

            // Indicate the object's name and set name as well
            m_txtObjName.text = Utility_Functions.RemoveIDString(m_refObj.name);
            m_txtSetName.text = m_parentSet.GetSetName();

            // Use the object set's current values to setup the UI
            Color.RGBToHSV(m_parentSet.GetOutlineColour(), out float Hue, out float S, out float V);
            m_imgOutlineColour.color = Color.HSVToRGB(Hue, 1.0f, 1.0f);

            // By default, this object is not selected in the quick select
            m_imgSelectedIndicator.gameObject.SetActive(false);

            // Hook into the quick focus camera's focus change event
            FindObjectOfType<VisCam_Combined>().m_onFocusTargetChanged.AddListener(this.OnFocusTargetChanged);
        }

        private void OnDestroy()
        {
            // Unhook from the event
            //FindObjectOfType<VisCam_QuickFocus>().m_onFocusTargetChanged.RemoveListener(this.OnFocusTargetChanged);
        }



        //--- Focus Methods ---//
        public void OnFocusSelected()
        {
            // Toggle the focus in the quick select
            FindObjectOfType<VisCam_QuickFocus>().ToggleFocusTarget(m_refObj);
        }

        public void OnFocusTargetChanged(Transform _newTarget)
        {
            // Toggle the indicator UI based on if this is the new target or not
            m_imgSelectedIndicator.gameObject.SetActive(_newTarget == m_refObj.transform);
        }



        //--- Colour Palette Methods ---//
        public void OnColourPaletteSelected()
        {
            // Open the colour selector window
            FindObjectOfType<UI_VisualizationManager>().OpenColourSelector(this.gameObject, m_parentSet);
        }
    }
}
