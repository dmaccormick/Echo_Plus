namespace Thesis.Interface
{
    public interface IVisualizable
    {
        /// <summary>
        /// Setup the visualization track with a string containing all of the data formatted for this track
        /// </summary>
        /// <param name="_data"> The full set of data for this track. Should be formatted in the right style for this track
        bool InitWithString(string _data);

        /// <summary>
        /// Perform necessary setup that is needed for the actual visualization, such as finding the visualization target
        /// </summary>
        /// <param name="_startTime"> The timestamp for the beginning of the visualization
        void StartVisualization(float _startTime);

        /// <summary>
        /// Animate the visualization to match the given time. Grab the relevant data point and apply it however is necessary
        /// </summary>
        /// <param name="_currentTime"> The timestamp for the current part of the visualization. Use it to get the relevant data point
        void UpdateVisualization(float _currentTime);

        /// <summary>
        /// Return the index to the nearest data point BEFORE the given time (ie: if time is 1.5s and the nearest data points are at 1.2s and 2.0s, return the index for the 1.2s one)
        /// </summary>
        /// <param name="_time"> The timestamp to search for
        int FindDataPointForTime(float _time);

        /// <summary>
        /// Return the name of the track, needs to EXACTLY match the corresponding recording track one
        /// </summary>
        /// 
        /// <returns>
        /// Return the name of the track
        /// </returns>
        string GetTrackName();

        /// <summary>
        /// Get the timestamp for the earliest recorded data point. Necessary to see when the visualziation should start
        /// </summary>
        /// 
        /// <returns>
        /// Return the first datapoint's timestamp
        /// </returns>
        float GetFirstTimestamp();

        /// <summary>
        /// Get the timestamp for the latest recorded data point. Necessary to see when the visualziation should end
        /// </summary>
        /// 
        /// <returns>
        /// Return the last datapoint's timestamp
        /// </returns>
        float GetLastTimestamp();
    }
}
