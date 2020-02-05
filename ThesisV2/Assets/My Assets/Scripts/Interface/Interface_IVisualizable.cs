namespace Thesis.Interface
{
    public interface IVisualizable
    {
        void InitWithString(string _data);

        void StartVisualization();

        void UpdateVisualization(float _time);

        string GetTrackName();
    }
}
