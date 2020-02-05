using UnityEngine;
using UnityEngine.UI;
using Thesis.Visualization;

namespace Thesis.UI
{
    public class UI_VisualizationManager : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Controls")]
        public Visualization_Manager m_visManager;

        [Header("Toolbar UI Elements")]
        public Button m_btnOpenSettings;

        [Header("Settings UI Elements")]
        public GameObject m_pnlSettings;



        //--- Toolbar Callbacks ---//
        public void OnToggleSettings()
        {
            // Toggle the settings UI panel
            bool isPanelActive = m_pnlSettings.gameObject.activeSelf;
            m_pnlSettings.gameObject.SetActive(!isPanelActive);
        }
    }
}