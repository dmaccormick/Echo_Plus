using UnityEngine;

namespace Thesis.Recording
{
    public enum Recording_Method
    {
        On_Change,          // Only record a data point if there is a noticeable difference from the previous data point
        Every_X_Seconds,    // Record a new data point at a specific time interval, no matter what the data point is
        Every_Frame         // Record a new data point every single frame, no matter what
    }

    [System.Serializable]
    public class Recording_Settings
    {
        [Header("Generic Controls")]
        public Recording_Method m_recordingMethod = Recording_Method.On_Change;
        public string m_dataFormat = "F3";

        [Header("On Change Controls")]
        public float m_changeMinThreshold = 0.1f;
        public float m_changeJumpThreshold = 1.0f;

        [Header("Every X Seconds Controls")]
        public float m_sampleTime = 0.25f;
        [HideInInspector] public float m_nextSampleTime = 0.0f;
    }
}