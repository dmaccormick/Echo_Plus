using UnityEngine;
using System.Collections.Generic;

namespace Thesis.SO
{
    [CreateAssetMenu(fileName = "AssetPaths", menuName = "Custom/AssetPaths", order = 1)]
    public class SO_AssetPaths : ScriptableObject
    {
        [SerializeField] public List<string> m_keys;
        [SerializeField] public List<string> m_values;
    }
}
