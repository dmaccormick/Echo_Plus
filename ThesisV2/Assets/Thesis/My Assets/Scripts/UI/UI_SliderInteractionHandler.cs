using UnityEngine;
using UnityEngine.EventSystems;
using Thesis.Study;

namespace Thesis.UI
{
    public class UI_SliderInteractionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        //--- Private Variables ---//
        private Study_Metrics m_metrics;
        private bool m_isMouseOver;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_isMouseOver = false;
            m_metrics = FindObjectOfType<Study_Metrics>();
        }

        private void Update()
        {
            // If the mouse is over the slider and the user is pressing the left mouse button, they are scrubbing
            // We can update the metrics accordingly
            if (m_isMouseOver && Input.GetMouseButton(0))
                m_metrics.IncreaseTimeSpentScrubbing(Time.deltaTime);
        }



        //--- IPointerDownHandler Interface ---//
        public void OnPointerDown(PointerEventData eventData)
        {
            m_isMouseOver = true;
        }



        //--- IPointerUpHandler Interface ---///
        public void OnPointerUp(PointerEventData eventData)
        {
            m_isMouseOver = false;
        }
    }

}
