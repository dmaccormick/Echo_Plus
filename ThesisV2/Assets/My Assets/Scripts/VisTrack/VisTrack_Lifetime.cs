using UnityEngine;
using Thesis.Interface;

namespace Thesis.VisTrack
{
    public class VisTrack_Lifetime : MonoBehaviour, IVisualizable
    {
        //--- IVisualizable Interface ---// 
        public bool InitWithString(string _data)
        {
            //throw new System.NotImplementedException();
            return true;
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