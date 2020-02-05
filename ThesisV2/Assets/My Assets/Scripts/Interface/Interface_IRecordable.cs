namespace Thesis.Interface
{
    public interface IRecordable
    {
        /// <summary>
        /// Start the recording. Use this for any setup and for recording the very first data point
        /// </summary>
        void StartRecording();


        /// <summary>
        /// End the recording. Use this for any cleanup and for recording the final data point
        /// </summary>
        void EndRecording();


        /// <summary>
        /// Update the recording. Use this to handle recording timing. Doesn't need to be used much for one-shots
        /// </summary>
        /// <param name="_elapsedTime">The time since the last frame, UNSCALED to account for games with slow motion</param>
        void UpdateRecording(float _elapsedTime);


        /// <summary>
        /// Actually sample and record the data that is being tracked
        /// </summary>
        void RecordData();


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
    }
}
