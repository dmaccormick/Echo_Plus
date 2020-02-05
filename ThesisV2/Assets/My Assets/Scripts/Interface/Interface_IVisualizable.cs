namespace Thesis.Interface
{
    public interface IVisualizable
    {
        bool InitWithString(string _data);

        void StartVisualization();

        void UpdateVisualization(float _time);

        string GetTrackName();
    }
}
