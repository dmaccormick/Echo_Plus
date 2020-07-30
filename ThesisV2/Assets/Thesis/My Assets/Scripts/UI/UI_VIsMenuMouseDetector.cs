using UnityEngine;
using UnityEngine.EventSystems;

namespace Thesis.UI
{
    public class UI_VIsMenuMouseDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //--- Private Variables ---//
        private bool m_isMouseDetected;



        //--- Unity Methods ---//
        private void Awake()
        {
            // Init the private variables
            m_isMouseDetected = false;
        }



        //--- IPointerEnterHandler Functions ---//
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_isMouseDetected = true;
        }



        //--- IPointerExitHandler Functions ---//
        public void OnPointerExit(PointerEventData eventData)
        {
            m_isMouseDetected = false;
        }



        //--- Getters ---//
        public bool IsMouseDetected
        {
            get => m_isMouseDetected;
        }
    }
}
