using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Echo.Utility;

namespace Echo.Visualization.VisCam
{
    public class VisCam_QuickFocus : MonoBehaviour
    {
        //--- Public Variables ---//
        [Header("Quick Focus UI")]
        public GameObject m_pnlQuickFocus;
        public Button m_btnNextFocus;
        public Button m_btnPrevFocus;
        public Text m_txtFocusName;



        //--- Private Variables ---//
        private List<Transform> m_focusTargets;
        private VisCam_Combined m_camControls;
        private int m_focusTargetIdx;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_focusTargets = new List<Transform>();
            m_focusTargets.Add(null); // The first element of the list is connected to the player selection which is nothing by default
            m_camControls = FindObjectOfType<VisCam_Combined>();
            m_focusTargetIdx = 0;
        }



        //--- Methods ---//
        public void ActivateUI()
        {
            // Start showing the UI
            m_pnlQuickFocus.SetActive(true);
        }

        public void UpdateUI()
        {
            // Get the current target from the list
            Transform currentTarget = m_focusTargets[m_focusTargetIdx];

            // If the target is null, just show blank information
            // If the target is player selected, show that 
            // Otherwise, show the set that the target is from
            if (currentTarget == null)
            {
                // Indicate that nothing is selected
                m_txtFocusName.text = "---";
            }
            else if (currentTarget == m_focusTargets[0])
            {
                // Indicate that the target is the one that was selected by the user
                m_txtFocusName.text = "\"" + Utility_Functions.RemoveIDString(currentTarget.name) + "\" (User Selected)";
            }
            else
            {
                // Indicate the focus target's name and set name as well
                string targetName = "\"" + Utility_Functions.RemoveIDString(currentTarget.name) + "\"";
                string fullSetName = currentTarget.GetComponentInParent<Visualization_ObjectSet>().GetSetName();
                string shortSetName = Utility_Functions.GetFileNameFromSetName(fullSetName);
                m_txtFocusName.text = targetName + " (" + shortSetName + ")";
            }

            // Toggle the next and prev buttons to be active or not, depending on how many focus items there are in the list
            m_btnPrevFocus.interactable = (m_focusTargets.Count > 1);
            m_btnNextFocus.interactable = (m_focusTargets.Count > 1);
        }

        public void AddFocusTarget(Transform _target)
        {
            // Add the target to the list
            m_focusTargets.Add(_target);

            // Update the UI
            UpdateUI();
        }

        public void AddTempTarget(Transform _tempTarget)
        {
            // If the list already contains the target object, switch to it
            // Otherwise, use the temp target slot
            if (m_focusTargets.Contains(_tempTarget))
            {
                // Switch the focus index to be the index of the element in the list
                m_focusTargetIdx = m_focusTargets.FindIndex(target => (target == _tempTarget));
            }
            else
            {
                // Put the temp target into the first slot and then update the UI
                m_focusTargets[0] = _tempTarget;

                // Switch over to the first target immediately
                m_focusTargetIdx = 0;
            }
            
            // Update the UI
            UpdateUI();
        }

        public void RemoveTempTarget()
        {
            // Clear the temp target from the first slot
            m_focusTargets[0] = null;

            // Switch to the empty first target immediately to clear any focus
            m_focusTargetIdx = 0;

            // Update the camera and the UI
            OnFocusTargetChanged();
        }

        public void OnNextClicked()
        {
            // Move to the next target in the list
            m_focusTargetIdx++;

            // Wrap the index if need be
            if (m_focusTargetIdx >= m_focusTargets.Count)
                m_focusTargetIdx = 0;

            // Update the camera and the UI
            OnFocusTargetChanged();
        }

        public void OnPrevClicked()
        {
            // Move to the previous target in the list
            m_focusTargetIdx--;

            // Wrap the index if need be
            if (m_focusTargetIdx < 0)
                m_focusTargetIdx = m_focusTargets.Count - 1;

            // Update the camera and the UI
            OnFocusTargetChanged();
        }

        public void OnFocusTargetChanged()
        {
            // Tell the camera to focus on the new object
            m_camControls.SetNewFocusTarget(m_focusTargets[m_focusTargetIdx]);

            // Update the UI
            UpdateUI();
        }

        public void RemoveFocusTarget(Transform _target)
        {
            // Remove the focus target from the list
            m_focusTargets.Remove(_target);

            // Reset the focus target back to the first one
            m_focusTargetIdx = 0;

            // Update the camera and the UI
            OnFocusTargetChanged();
        }
    }

}