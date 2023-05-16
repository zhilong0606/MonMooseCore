#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class CommonTool
    {
        private static readonly List<string> m_resoucePathPrefixStrList = new List<string>()
        {
        };

        [MenuItem("Assets/CommonTool/Get Resource Path %k", false, 1)]
        static void GetAssetPath()
        {
            string path = string.Empty;
            if (Selection.activeObject != null)
            {
                Object selectedObj = Selection.activeObject;
                if (AssetDatabase.Contains(selectedObj))
                {
                    path = AssetDatabase.GetAssetPath(selectedObj);
                    for (int i = 0; i < m_resoucePathPrefixStrList.Count; ++i)
                    {
                        string prefix = m_resoucePathPrefixStrList[i];

                        if (path.StartsWith(prefix))
                        {
                            path = path.Substring(prefix.Length);
                            break;
                        }
                    }
                }
            }
            EditorGUIUtility.systemCopyBuffer = path;
        }
    }
}
#endif