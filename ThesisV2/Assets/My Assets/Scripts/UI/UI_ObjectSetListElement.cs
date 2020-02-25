using UnityEngine;
using UnityEngine.UI;
using Thesis.Visualization;

namespace Thesis.UI
{
    public class UI_ObjectSetListElement : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Visibility Controls")]
        public Toggle m_tglVisibility;
        public Image m_imgVisibilityIcon;
        public Sprite m_sprVisibility_On;
        public Sprite m_sprVisibility_Off;

        [Header("Outline Controls")]
        public Slider m_sldOutlineHue;
        public Image m_imgOutlineColour;



        //--- Private Variables ---//
        private Visualization_ObjectSet m_refObjectSet;



        //--- Initialization Methods ---//
        public void InitWithObjectSet(Visualization_ObjectSet _refObjectSet)
        {
            // Store a reference to the object set so we can update its values
            m_refObjectSet = _refObjectSet;

            // TODO: Use the object set's current values to setup the UI
            // ...
        }



        //--- Visibility Methods ---//
        public void OnToggleVisibilityControl(bool _isVisible)
        {
            // Select the correct sprite based on the value
            Sprite selectedSprite = (_isVisible) ? m_sprVisibility_On : m_sprVisibility_Off;

            // Assign the sprite to the icon
            m_imgVisibilityIcon.sprite = selectedSprite;
        }



        //--- Outline Controls ---//
        public void OnChangeOutlineHue(float _newHueValue)
        {
            // Determine the new colour using HSV and a full saturation and value
            Color newColor = Color.HSVToRGB(_newHueValue, 1.0f, 1.0f);

            // Apply the color to the indicator image
            m_imgOutlineColour.color = newColor;
        }
    }

}