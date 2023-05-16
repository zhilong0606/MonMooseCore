#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public static class LocalizationTool
    {
        private const string UIPath = "Assets/Resources/Exporter/UI";
        private static List<string> m_strList = new List<string>();

        [MenuItem("Tools/UI/Export Localization")]
        private static void ExportLocalization()
        {
            List<DirectoryInfo> dirList = new List<DirectoryInfo>();
            dirList.Add(new DirectoryInfo(UIPath));
            while (dirList.Count > 0)
            {
                DirectoryInfo dirInfo = dirList[0];
                dirList.RemoveAt(0);
                if (dirInfo == null)
                {
                    continue;
                }
                dirList.AddRange(dirInfo.GetDirectories());
                FileInfo[] files = dirInfo.GetFiles();
                for (int i = 0; i < files.Length; ++i)
                {
                    if (!files[i].FullName.EndsWith(".prefab"))
                    {
                        continue;
                    }
                    string filePath = files[i].FullName.Replace("\\\\", "/");
                    filePath = filePath.Substring(filePath.IndexOf("Assets", StringComparison.Ordinal));
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
                    AnalyzeTextStrFromPrefab(prefab);
                }
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < m_strList.Count; ++i)
            {
                if (i != 0)
                {
                    sb.Append("\r\n");
                }
                sb.Append(m_strList[i]);
            }

            FileStream fs = new FileStream("C:/hhh.txt", FileMode.OpenOrCreate);
            byte[] buffers = Encoding.Default.GetBytes(sb.ToString());
            fs.Write(buffers, 0, buffers.Length);
            fs.Flush();
            fs.Close();
            m_strList.Clear();
        }

        private static void AnalyzeTextStrFromPrefab(GameObject prefab)
        {
            List<GameObject> objList = new List<GameObject>();
            objList.Add(prefab);
            while (objList.Count > 0)
            {
                GameObject obj = objList[0];
                objList.RemoveAt(0);
                for (int i = 0; i < obj.transform.childCount; ++i)
                {
                    objList.Add(obj.transform.GetChild(i).gameObject);
                }
                Text text = obj.GetComponent<Text>();
                if (text != null)
                {
                    AddExportStr(text.text);
                }
            }
        }

        private static void AddExportStr(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            float f;
            if (float.TryParse(str, out f))
            {
                return;
            }
            if (!m_strList.Contains(str))
            {
                m_strList.Add(str);
            }
        }
    }
}
#endif