namespace Thesis.Interface
{
    public interface IVisualizable
    {
        bool InitWithString(string _data);

        void StartVisualization(float _startTime);

        void UpdateVisualization(float _time);

        int FindDataPointForTime(float _time);

        string GetTrackName();

        float GetFirstTimestamp();

        float GetLastTimestamp();
    }
}
