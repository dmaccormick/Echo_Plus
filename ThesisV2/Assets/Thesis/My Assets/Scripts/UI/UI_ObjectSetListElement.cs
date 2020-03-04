using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using Thesis.Visualization;

namespace Thesis.UI
{
    //--- Event Classes ---//
    [System.Serializable]
    public class ObjectSetEvent : UnityEvent<Visualization_ObjectSet>
    {
    }



    //--- Main Class ---//
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
        public Text m_txtOutlineLabel;

        [Header("Misc")]
        public Text m_txtSetName;

        [Header("Events")]
        public ObjectSetEvent m_onDeleteSet;



        //--- Private Variables ---//
        private Visualization_ObjectSet m_refObjectSet;



        //--- Initialization Methods ---//
        public void InitWithObjectSet(Visualization_ObjectSet _refObjectSet)
        {
            // Store a reference to the object set so we can update its values
            m_refObjectSet = _refObjectSet;

            // Use the object set's current values to setup the UI
            m_tglVisibility.isOn = _refObjectSet.GetIsVisible();
            Color.RGBToHSV(_refObjectSet.GetOutlineColour(), out float Hue, out float S, out float V);
            m_sldOutlineHue.value = Hue;
            m_imgOutlineColour.color = Color.HSVToRGB(Hue, 1.0f, 1.0f);
            m_txtSetName.text = _refObjectSet.GetSetName();

            // If the set doesn't have an outline, we should remove the controls for it
            if (!_refObjectSet.GetHasOutline())
            {
                m_sldOutlineHue.gameObject.SetActive(false);
                m_imgOutlineColour.gameObject.SetActive(false);
                m_txtOutlineLabel.gameObject.SetActive(false);
            }
        }



        //--- Visibility Methods ---//
        public void OnToggleVisibilityControl(bool _isVisible)
        {
            // Select the correct sprite based on the value
            Sprite selectedSprite = (_isVisible) ? m_sprVisibility_On : m_sprVisibility_Off;

            // Assign the sprite to the icon
            m_imgVisibilityIcon.sprite = selectedSprite;

            // Update the visibility of the actual set itself
            m_refObjectSet.ToggleAllObjectsActive(_isVisible);
        }



        //--- Outline Controls ---//
        public void OnChangeOutlineHue(float _newHueValue)
        {
            // Determine the new colour using HSV and a full saturation and value
            Color newColor = Color.HSVToRGB(_newHueValue, 1.0f, 1.0f);

            // Apply the color to the indicator image
            m_imgOutlineColour.color = newColor;

            // Update the outline colour of the actual set itself
            m_refObjectSet.UpdateOutlineColour(newColor);
        }



        //--- Delete Controls ---//
        public void OnDeletePressed()
        {
            // Show a confirm dialog box to make sure they actually want to delete it
            if (EditorUtility.DisplayDialog("Delete Set?", "Are you sure you want to delete this set? This action cannot be undone.", "Delete Set", "Cancel"))
            {
                // They pressed the "Delete Set" button so we should trigger the action to perform the deletion
                m_onDeleteSet.Invoke(m_refObjectSet);
            }
        }
    }
}