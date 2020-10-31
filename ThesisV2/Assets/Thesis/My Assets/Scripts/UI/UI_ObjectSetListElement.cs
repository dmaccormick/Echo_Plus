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
        public Image m_imgOutlineColour;

        [Header("Misc")]
        public Text m_txtSetName;
        public Text m_txtStaticOrDynamic;

        [Header("Solo Controls")]
        public Button m_btnSolo;
        public Text m_txtStartSolo;
        public Text m_txtEndSolo;
        public Image m_imgSoloIndicator;

        [Header("Events")]
        public ObjectSetEvent m_onDeleteSet;



        //--- Private Variables ---//
        private Visualization_ObjectSet m_refObjectSet;
        private Visualization_Metrics m_metrics;
        private bool m_isSoloed;
        private bool m_canSolo;



        //--- Initialization Methods ---//
        public void InitWithObjectSet(Visualization_ObjectSet _refObjectSet)
        {
            // Init the private variables
            m_isSoloed = false;
            m_canSolo = false;
            m_metrics = FindObjectOfType<Visualization_Metrics>();

            // Store a reference to the object set so we can update its values
            m_refObjectSet = _refObjectSet;

            // Use the object set's current values to setup the UI
            m_tglVisibility.isOn = _refObjectSet.GetIsVisible();
            Color.RGBToHSV(_refObjectSet.GetOutlineColour(), out float Hue, out float S, out float V);
            //m_sldOutlineHue.value = Hue;
            m_imgOutlineColour.color = Color.HSVToRGB(Hue, 1.0f, 1.0f);
            m_txtSetName.text = _refObjectSet.GetSetName();
            m_txtStaticOrDynamic.text = (_refObjectSet.GetHasOutline()) ? "Dynamic" : "Static";

            // If the set doesn't have an outline, we should remove the controls for it
            if (!_refObjectSet.GetHasOutline())
            {
                //m_sldOutlineHue.gameObject.SetActive(false);
                m_imgOutlineColour.gameObject.SetActive(false);
                //m_txtOutlineLabel.gameObject.SetActive(false);
                m_btnSolo.gameObject.SetActive(false);

                // Hide all of the solo controls
                m_canSolo = false;
                m_txtStartSolo.gameObject.SetActive(false);
                m_txtEndSolo.gameObject.SetActive(false);
                m_imgSoloIndicator.gameObject.SetActive(false);
            }
            else
            {
                // Connect to the set's colour change event
                m_refObjectSet.m_onOutlineColourChanged.AddListener(this.OnSetColourUpdated);
                m_canSolo = true;
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

            // Update the metrics
            if (_isVisible)
                m_metrics.IncreaseNumTimesSetVisible();
            else
                m_metrics.IncreaseNumTimesSetHidden();
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
#if UNITY_EDITOR
            // Show a confirm dialog box to make sure they actually want to delete it
            if (EditorUtility.DisplayDialog("Delete Set?", "Are you sure you want to delete this set? This action cannot be undone.", "Delete Set", "Cancel"))
            {
                // They pressed the "Delete Set" button so we should trigger the action to perform the deletion
                m_onDeleteSet.Invoke(m_refObjectSet);
            }
#endif
        }



        //--- Solo Controls ---//
        public void OnSoloPressed()
        {
            if (m_canSolo)
            {
                // Swap the solo state
                m_isSoloed = !m_isSoloed;

                // Tell the visualization manager that this set was told to solo
                if (m_isSoloed)
                {
                    FindObjectOfType<Visualization_Manager>().SetSoloSet(this.m_refObjectSet);
                    m_metrics.IncreaseNumTimesSetSolod();
                }
                else
                    FindObjectOfType<Visualization_Manager>().SetSoloSet(null);
            }
        }

        public void ShowSoloState(Visualization_ObjectSet _soloedSet)
        {
            if (m_canSolo)
            {
                // If there are not any solo'd sets, revert to the basic state
                // If this set is solo'd, change the text on the button
                // If another set is solo'd, show the solo'd indicator
                if (_soloedSet == null)
                {
                    m_txtStartSolo.gameObject.SetActive(true);
                    m_txtEndSolo.gameObject.SetActive(false);
                    m_imgSoloIndicator.gameObject.SetActive(false);
                    m_tglVisibility.enabled = true;

                    // Determine if this set should be visible according to the eye icon state
                    bool shouldBeVisible = (m_imgVisibilityIcon.sprite == m_sprVisibility_On);
                    m_refObjectSet.ToggleAllObjectsActive(shouldBeVisible);
                }
                else if (_soloedSet == this.m_refObjectSet)
                {
                    m_txtStartSolo.gameObject.SetActive(false);
                    m_txtEndSolo.gameObject.SetActive(true);
                    m_imgSoloIndicator.gameObject.SetActive(false);
                    m_tglVisibility.enabled = false;

                    // This set is being solo'd so it should always be visible
                    m_refObjectSet.ToggleAllObjectsActive(true);
                }
                else
                {
                    m_txtStartSolo.gameObject.SetActive(false);
                    m_txtEndSolo.gameObject.SetActive(false);
                    m_imgSoloIndicator.gameObject.SetActive(true);
                    m_tglVisibility.enabled = false;

                    // Another set is being solo'd so this should never be visible
                    m_refObjectSet.ToggleAllObjectsActive(false);
                }
            }
        }



        //--- Colour Palette Methods ---//
        public void OnColourPaletteSelected()
        {
            // Open the colour selector window
            FindObjectOfType<UI_VisualizationManager>().OpenColourSelector(this.gameObject, m_refObjectSet);
        }

        public void OnSetColourUpdated(Color _newColor)
        {
            // Update the colour indicator
            m_imgOutlineColour.color = _newColor;
        }
    }
}