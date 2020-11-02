using UnityEngine;
using Thesis.Recording;

public class Study_AutomaticRecording : MonoBehaviour
{
    public string m_genreName;
    private Recording_Manager m_recMan;

    private string m_baseFilePath;

    private void Awake()
    {
        m_recMan = FindObjectOfType<Recording_Manager>();

        int participantID = PlayerPrefs.GetInt("ParticipantID");
        string participantIDStr = participantID.ToString("D2");
        m_baseFilePath = Application.dataPath + "/GameLogs/" + participantIDStr + "_" + m_genreName;
    }

    private void Start()
    {
        StartRecording();
    }

    public void StartRecording()
    {
        m_recMan.StartRecording();
    }

    public void StopRecording()
    {
        m_recMan.StopRecording();
    }

    public void StopRecordingAndSave()
    {
        StopRecording();

        string staticFilePath = m_baseFilePath + "_Static.log";
        string dynamicFilePath = m_baseFilePath + "_Dynamic.log";

        Debug.Log("Saving!");

        m_recMan.SaveStaticData(staticFilePath);
        m_recMan.SaveDynamicData(dynamicFilePath);
    }
}
