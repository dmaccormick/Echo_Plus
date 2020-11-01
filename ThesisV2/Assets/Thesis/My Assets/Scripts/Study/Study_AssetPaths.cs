using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Collections.Generic;
using Thesis.SO;

public class Study_AssetPaths : MonoBehaviour
{
    public SO_AssetPaths m_scriptable;
    private Dictionary<string, string> m_paths;

    // Use this for initialization
    void Awake()
    {
        InitDictionary();
    }

    [ContextMenu("Find Paths")]
    public void FindAllPaths()
    {
        //m_scriptable.m_paths = new Dictionary<string, string>();
        m_paths = new Dictionary<string, string>();
        m_scriptable.m_keys = new List<string>();
        m_scriptable.m_values = new List<string>();
        string[] allAssetGUIDs = AssetDatabase.FindAssets("t:Material t:Mesh t:Texture2D t:Animation t:AnimatorController t:AvatarMask");
        
        foreach(var assetGUID in allAssetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);

            if (assetPath.Contains("Package"))
                continue;

            //var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            var subObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            foreach(var obj in subObjs)
            {
                if (!m_paths.ContainsKey(obj.name))
                {
                    m_paths.Add(obj.name, assetPath);
                    m_scriptable.m_keys.Add(obj.name);
                    m_scriptable.m_values.Add(assetPath);
                }
            }
        }

        Debug.Log(m_paths.Count + "\t" + m_scriptable.m_keys.Count + "\t" + m_scriptable.m_values.Count);
        Assert.IsTrue(m_scriptable.m_keys.Count == m_scriptable.m_values.Count, "The number of keys and values in the scriptable MUST match!");

#if UNITY_EDITOR
        EditorUtility.SetDirty(m_scriptable);
#endif
    }

    public void InitDictionary()
    {
        m_paths = new Dictionary<string, string>();
        for (int i = 0; i < m_scriptable.m_keys.Count; i++)
        {
            m_paths.Add(m_scriptable.m_keys[i], m_scriptable.m_values[i]);
        }
    }

    public string GetPathForObject(Object _obj)
    {
        string objName = _obj.name;

        if (_obj.name.Contains("(Instance)"))
        {
            int instanceStartIdx = objName.IndexOf('(');
            objName = objName.Substring(0, instanceStartIdx - 1);
        }

        if (!m_paths.ContainsKey(objName))
        {
            Debug.Log("No matching key for object: " + objName);
            return "";
        }

        return m_paths[objName];
    }
}
