namespace Thesis.Interface
{
    public interface IRecordable
    {
        /// <summary>
        /// Start the recording. Use this for any setup and for recording the very first data point
        /// </summary>
        /// <param name="_startTime"> The time to record as a timestamp for the start of the recording</param>
        void StartRecording(float _startTime);


        /// <summary>
        /// End the recording. Use this for any cleanup and for recording the final data point
        /// </summary>
        /// <param name="_endTime"> The time to record as a timestamp for the end of the recording</param>
        void EndRecording(float _endTime);


        /// <summary>
        /// Update the recording. Use this to handle recording timing. Doesn't need to be used much for one-shots
        /// </summary>
        /// <param name="_currentTime">The time to record as a timestamp if there are any datapoints now</param>
        void UpdateRecording(float _currentTime);


        /// <summary>
        /// Actually sample and record the data that is being tracked
        /// </summary>
        /// /// <param name="_currentTime">The time to record as a timestamp</param>
        void RecordData(float _currentTime);


        /// <summary>
        /// Convert all of the data into a single formatted string and return it
        /// </summary>
        /// 
        /// <returns>
        /// Return the data that was recorded by this track, formatted into a single string
        /// </returns>
        string GetData();

        /// <summary>
        /// Get the name of the track, should match the visualization version exactly
        /// </summary>
        /// <returns>
        /// Returns a string which is the name of the track
        /// </returns>
        string GetTrackName();



        void SetupDefault();
    }
}
