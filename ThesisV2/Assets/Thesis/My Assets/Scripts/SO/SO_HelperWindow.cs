using UnityEngine;
using UnityEditor;

namespace Thesis.SO
{
    public class SO_HelperWindow : ScriptableObject
    {
#if UNITY_EDITOR
        public SceneAsset m_participantInfoScene;
        public SceneAsset m_recScene;
        public SceneAsset m_visScene;
#endif
    }
}