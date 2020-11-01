using UnityEngine;
using System.Collections.Generic;

namespace Thesis.SO
{
    [System.Serializable]
    public struct AssetPathData
    {
        public AssetPathData(string _path, int _index)
        {
            this.m_path = _path;
            this.m_index = _index;
        }

        [SerializeField] public string m_path;
        [SerializeField] public int m_index;
    }

    [CreateAssetMenu(fileName = "AssetPaths", menuName = "Custom/AssetPaths", order = 1)]
    public class SO_AssetPaths : ScriptableObject
    {
        [SerializeField] public List<string> m_keys;
        [SerializeField] public List<AssetPathData> m_values;
    }
}
