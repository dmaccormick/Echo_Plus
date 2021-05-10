using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thesis.UI;

namespace Thesis.Study
{
    public class Study_VisOverride : MonoBehaviour
    {
        public InputField m_inStaticFile;
        public InputField m_inDynamicFile;

        public string m_folderPath;
        public string m_gameFileName;

        public int m_startFileIndex = 1;
        public int m_endFileIndex = 10;

        private UI_VisualizationManager m_visManager;

        private void Awake()
        {
            m_visManager = FindObjectOfType<UI_VisualizationManager>();
        }

        public void OnLoadAllFiles()
        {
            LoadStaticFile();
            LoadDynamicFiles();
        }

        public void LoadStaticFile()
        {
            m_inStaticFile.text = m_folderPath + "01_" + m_gameFileName + "_Static.log";

            m_visManager.OnLoadStaticFile();
        }

        public void LoadDynamicFiles()
        {
            for (int i = m_startFileIndex; i <= m_endFileIndex; i++)
            {
                m_inDynamicFile.text = m_folderPath + i.ToString("D2") + "_" + m_gameFileName + "_Dynamic.log";

                m_visManager.OnLoadDynamicFile();
            }
        }
    }

}
