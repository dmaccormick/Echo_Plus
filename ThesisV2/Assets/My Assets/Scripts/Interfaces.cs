using UnityEngine;

namespace Thesis
{
    public interface IRecordable
    {
        /// <summary>
        /// Start the recording. Use this for any setup and for recording the very first data point
        /// </summary>
        public void StartRecording();


        /// <summary>
        /// End the recording. Use this for any cleanup and for recording the final data point
        /// </summary>
        public void EndRecording();


        /// <summary>
        /// Update the recording. Use this to handle recording timing. Doesn't need to be used much for one-shots
        /// </summary>
        public void UpdateRecording();


        /// <summary>
        /// Actually sample and record the data that is being tracked
        /// </summary>
        public void RecordData();


        /// <summary>
        /// Convert all of the data into a single formatted string and return it
        /// </summary>
        /// 
        /// <returns>
        /// Return the data that was recorded by this track, formatted into a single string
        /// </returns>
        public string GetData();
    }
}
