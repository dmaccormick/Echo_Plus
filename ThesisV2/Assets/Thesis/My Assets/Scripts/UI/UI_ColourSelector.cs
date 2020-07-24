using UnityEngine;
using UnityEngine.UI;
using Thesis.Visualization;

namespace Thesis.UI
{
    public class UI_ColourSelector : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Colour Icon")]
        public Image m_imgColourIcon;

        [Header("Colour Sliders")]
        public Slider m_sldHue;
        public Slider m_sldSat;
        public Slider m_sldVal;

        [Header("Line Renderer")]
        public LineRenderer m_lineRenderer;



        //--- Private Variables ---//
        private Visualization_ObjectSet m_refSet;



        //--- Methods ---//
        public void OpenForObject(GameObject _callObj, Visualization_ObjectSet _refSet)
        {
            // Store the reference internally
            m_refSet = _refSet;

            // Draw a line from the palette to the calling object to help show what called it
            m_lineRenderer.SetPosition(0, _callObj.transform.position);
            m_lineRenderer.SetPosition(1, this.transform.position);
            m_lineRenderer.startColor = _refSet.GetOutlineColour();
            m_lineRenderer.endColor = _refSet.GetOutlineColour();

            // Update the colour of the image to match the set's current colour
            m_imgColourIcon.color = _refSet.GetOutlineColour();

            // Update the sliders to match the current colour values
            Color.RGBToHSV(m_refSet.GetOutlineColour(), out float H, out float S, out float V);
            m_sldHue.value = H;
            m_sldSat.value = S;
            m_sldVal.value = V;
        }
    }
}