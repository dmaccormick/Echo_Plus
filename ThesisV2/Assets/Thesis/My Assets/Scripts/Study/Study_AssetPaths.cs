using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Collections.Generic;
using Thesis.SO;

public class Study_AssetPaths : MonoBehaviour
{
    public SO_AssetPaths m_scriptable;
    private Dictionary<string, AssetPathData> m_paths;

    // Use this for initialization
    void Awake()
    {
        InitDictionary();
    }

#if UNITY_EDITOR
    [ContextMenu("Find Paths")]
    public void FindAllPaths()
    {
        //m_scriptable.m_paths = new Dictionary<string, string>();
        m_paths = new Dictionary<string, AssetPathData>();
        m_scriptable.m_keys = new List<string>();
        m_scriptable.m_values = new List<AssetPathData>();
        string[] allAssetGUIDs = AssetDatabase.FindAssets("t:Material t:Mesh t:Texture2D t:Animation t:AnimatorController t:AvatarMask");
        
        foreach(var assetGUID in allAssetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);

            if (assetPath.Contains("Package"))
                continue;

            //var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            var subObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for(int i = 0; i < subObjs.Length; i++)
            {
                var obj = subObjs[i];

                if (!m_paths.ContainsKey(obj.ToString()))
                {
                    m_paths.Add(obj.ToString(), new AssetPathData(assetPath, i));
                    m_scriptable.m_keys.Add(obj.ToString());
                    m_scriptable.m_values.Add(new AssetPathData(assetPath, i));
                }
            }
        }

        Debug.Log(m_paths.Count + "\t" + m_scriptable.m_keys.Count + "\t" + m_scriptable.m_values.Count);
        Assert.IsTrue(m_scriptable.m_keys.Count == m_scriptable.m_values.Count, "The number of keys and values in the scriptable MUST match!");

        EditorUtility.SetDirty(m_scriptable);
    }
#endif

    public void InitDictionary()
    {
        m_paths = new Dictionary<string, AssetPathData>();
        for (int i = 0; i < m_scriptable.m_keys.Count; i++)
        {
            m_paths.Add(m_scriptable.m_keys[i], m_scriptable.m_values[i]);
        }
    }

    public string GetPathForObject(Object _obj)
    {
        if (_obj == null)
            return "";

        string objName = _obj.ToString();

        if (_obj.ToString().Contains("(Instance)"))
        {
            int instanceStartIdx = objName.IndexOf('(');
            objName = objName.Substring(0, instanceStartIdx - 1);
        }

        if (!m_paths.ContainsKey(objName))
        {
            Debug.Log("No matching key for object: " + objName);
            return "";
        }

        return m_paths[objName].m_path;
    }

    public int GetIndexForObject(Object _obj)
    {
        if (_obj == null)
            return -1;

        string objName = _obj.ToString();

        if (_obj.ToString().Contains("(Instance)"))
        {
            int instanceStartIdx = objName.IndexOf('(');
            objName = objName.Substring(0, instanceStartIdx - 1);
        }

        if (!m_paths.ContainsKey(objName))
        {
            Debug.Log("No matching key for object: " + objName);
            return -1;
        }

        return m_paths[objName].m_index;
    }
}
