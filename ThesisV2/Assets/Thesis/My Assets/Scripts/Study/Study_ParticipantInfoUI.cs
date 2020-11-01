using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Thesis.Study
{
    public class Study_ParticipantInfoUI : MonoBehaviour
    {
        public InputField m_inParticipantID;
        public Button m_btnStartSession;
        public string m_sceneToLoad;

        private int m_parsedID;

        private void Awake()
        {
            m_parsedID = -1;
        }
        
        public void OnIDChanged(string _text)
        {
            if (int.TryParse(_text, out var newParsedID))
            {
                if (newParsedID > 0)
                {
                    m_parsedID = newParsedID;
                    m_btnStartSession.interactable = true;
                }
                else
                    m_btnStartSession.interactable = false;
            }
            else
            {
                m_btnStartSession.interactable = false;
            }
        }

        public void OnStartSession()
        {
            PlayerPrefs.SetInt("ParticipantID", m_parsedID);
            SceneManager.LoadScene(m_sceneToLoad);
        }
    }
}