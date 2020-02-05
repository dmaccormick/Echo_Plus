using UnityEngine;
using Thesis.Interface;

namespace Thesis.VisTrack
{
    public class VisTrack_Renderables : MonoBehaviour, IVisualizable
    {
        //--- IVisualizable Interface ---// 
        public void InitWithString(string _data)
        {
            throw new System.NotImplementedException();
        }

        public void StartVisualization()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateVisualization(float _time)
        {
            throw new System.NotImplementedException();
        }

        public string GetTrackName()
        {
            throw new System.NotImplementedException();
        }
    }

}