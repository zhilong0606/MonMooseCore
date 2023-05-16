#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public enum E_ExtractType
    {
        E_Default,
        E_Horizontal,
        E_Vertical,
    };

    public class UIAtlasTool
    {
        [MenuItem("Assets/图集工具/分离alpha")]
        static public void ExtractAlpha()
        {
            string strFolder;
            if (Selection.objects == null || Selection.objects.Length == 0)
            {

            }
            else
            {
                EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section.....", 0);
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    strFolder = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
                    EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section....." + strFolder, i * 1.0f / Selection.objects.Length);
                    if (!ExtractAlpha(strFolder, ""))
                    {
                        ExtractAlphaInFolder(strFolder, i * 1.0f / Selection.objects.Length, 1.0f / Selection.objects.Length);
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Assets/图集工具/分离alpha水平排列")]
        static public void ExtractAlpha_ForceHorizontal()
        {
            string strFolder;
            if (Selection.objects == null || Selection.objects.Length == 0)
            {

            }
            else
            {
                EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section.....", 0);
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    strFolder = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
                    EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section....." + strFolder, i * 1.0f / Selection.objects.Length);
                    if (!ExtractAlpha(strFolder, "", E_ExtractType.E_Horizontal))
                    {
                        ExtractAlphaInFolder(strFolder, i * 1.0f / Selection.objects.Length, 1.0f / Selection.objects.Length, E_ExtractType.E_Horizontal);
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Assets/图集工具/分离alpha竖直排列")]
        static public void ExtractAlpha_ForceVertical()
        {
            string strFolder;
            if (Selection.objects == null || Selection.objects.Length == 0)
            {

            }
            else
            {
                EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section.....", 0);
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    strFolder = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
                    EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section....." + strFolder, i * 1.0f / Selection.objects.Length);
                    if (!ExtractAlpha(strFolder, "", E_ExtractType.E_Vertical))
                    {
                        ExtractAlphaInFolder(strFolder, i * 1.0f / Selection.objects.Length, 1.0f / Selection.objects.Length, E_ExtractType.E_Vertical);
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        static public void ExtractAlphaInFolder(string strFolder, float baseProgress, float deltaProgress, E_ExtractType eType = E_ExtractType.E_Default)
        {
            int ic = Application.dataPath.LastIndexOf('/');
            if (ic <= 0)
                return;

            string strPath = Application.dataPath.Substring(0, ic + 1) + strFolder;

            string desFolder = strPath + "/rgb/";
            if (!System.IO.Directory.Exists(desFolder))
            {
                System.IO.Directory.CreateDirectory(desFolder);
            }
            desFolder = FileUtil.GetProjectRelativePath(desFolder);
            Debug.Log(desFolder);

            string[] findfiles = Directory.GetFiles(strPath, "*.*");
            for (int i = 0; i < findfiles.Length; i++)
            {
                EditorUtility.DisplayProgressBar("ExtractAlpha", "Build File....." + findfiles[i], baseProgress + deltaProgress * i / findfiles.Length);
                if (findfiles[i].EndsWith(".png") || findfiles[i].EndsWith(".tga"))
                {
                    findfiles[i] = findfiles[i].Replace('\\', '/');

                    findfiles[i] = FileUtil.GetProjectRelativePath(findfiles[i]);
                    ExtractAlpha(findfiles[i], desFolder, eType);
                }
            }
        }

        static public bool ExtractAlpha(string strFile, string strDesFolder, E_ExtractType eType = E_ExtractType.E_Default)
        {
            int idot = strFile.LastIndexOf('.');
            if (idot <= 0)
                return false;

            string ext = strFile.Substring(idot);
            if (ext != ".png" && ext != ".tga")
                return false;

            string strFileName;


            int iiv = strFile.LastIndexOf('/');
            strFileName = strFile.Substring(iiv + 1, idot - iiv - 1);
            if (strFileName.Length <= 0)
                return false;

            string desPath;
            if (strDesFolder != null && strDesFolder.Length != 0)
            {
                desPath = strDesFolder;
            }
            else
            {
                desPath = strFile.Substring(0, iiv);
                desPath += "/rgb/";
                int ic = Application.dataPath.LastIndexOf('/');
                if (ic <= 0)
                    return false;

                string strFolder = Application.dataPath.Substring(0, ic + 1) + desPath;

                if (!System.IO.Directory.Exists(strFolder))
                {
                    System.IO.Directory.CreateDirectory(strFolder);
                }
            }

            TextureImporter texim = (TextureImporter)(TextureImporter.GetAtPath(strFile));
            if (texim == null)
                return false;
            bool bReadable = texim.isReadable;
            texim.isReadable = true;
            texim.textureType = TextureImporterType.Default;
            texim.npotScale = TextureImporterNPOTScale.None;
            texim.mipmapEnabled = false;
            TextureImporterFormat tf = texim.textureFormat;
            texim.textureFormat = TextureImporterFormat.RGBA32;

            //TextureImporterPlatformSettings platSettings = new TextureImporterPlatformSettings();
            //platSettings.format = TextureImporterFormat.RGBA32;
            texim.SetPlatformTextureSettings("Standalone", 2048, TextureImporterFormat.RGBA32);
            AssetDatabase.ImportAsset(strFile);

            Texture2D tex = AssetDatabase.LoadAssetAtPath(strFile, typeof(Texture2D)) as Texture2D;
            if (tex == null)
            {
                texim.isReadable = bReadable;
                texim.textureFormat = TextureImporterFormat.RGBA32;
                AssetDatabase.ImportAsset(strFile);
                return false;
            }
            Texture2D ctex = null;
            if (tex.format == TextureFormat.RGBA32 || tex.format == TextureFormat.ARGB32)
            {
                if (E_ExtractType.E_Default == eType)
                {
                    if (tex.width < tex.height)
                    {
                        //为了消除双线性采样，多采样到alpha的问题，左右分布的图片，往原图右侧补一空像素，x2最终就是多两像素
                        ctex = new Texture2D(tex.width * 2 + 2, tex.height, TextureFormat.RGB24, texim.mipmapEnabled);

                        for (int i = 0; i < tex.width; i++)
                        {
                            for (int j = 0; j < tex.height; j++)
                            {
                                Color c = tex.GetPixel(i, j);
                                if (c.a < 0.001f)
                                {
                                    ctex.SetPixel(i, j, new Color(0, 0, 0));
                                }
                                else
                                {
                                    ctex.SetPixel(i, j, new Color(c.r, c.g, c.b));
                                }
                            }
                        }
                        //补一像素
                        for (int i = tex.width; i < tex.width + 1; i++)
                        {
                            for (int j = 0; j < tex.height; j++)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                        }

                        for (int i = tex.width + 1; i < tex.width * 2 + 1; i++)
                        {
                            for (int j = 0; j < tex.height; j++)
                            {
                                Color c = tex.GetPixel(i - tex.width - 1, j);
                                ctex.SetPixel(i, j, new Color(c.a, c.a, c.a));
                            }
                        }
                        //补一像素
                        for (int i = tex.width * 2 + 1; i < tex.width * 2 + 2; i++)
                        {
                            for (int j = 0; j < tex.height; j++)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                        }

                    }
                    else
                    {
                        //为了消除双线性采样，多采样到alpha冗余像素的问题，上下分布的图片，往原图上侧补一空像素，x2最终就是多两像素
                        //左下角 为贴图空间 (0,0) 点
                        ctex = new Texture2D(tex.width, tex.height * 2 + 2, TextureFormat.RGB24, texim.mipmapEnabled);
                        for (int j = 0; j < tex.height; j++)
                        {
                            for (int i = 0; i < tex.width; i++)
                            {
                                Color c = tex.GetPixel(i, j);
                                if (c.a < 0.001f)
                                {
                                    ctex.SetPixel(i, j, new Color(0, 0, 0));
                                }
                                else
                                {
                                    ctex.SetPixel(i, j, new Color(c.r, c.g, c.b));
                                }
                                //if (j < tex.height/2 && i < tex.width/2)
                                //{
                                //    ctex.SetPixel(i, j, new Color(1, 0, 0));
                                //}
                            }
                        }
                        //补一像素
                        for (int j = tex.height; j < tex.height + 1; j++)
                        {
                            for (int i = 0; i < tex.width; i++)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                        }

                        for (int j = tex.height + 1; j < tex.height * 2 + 1; j++)
                        {
                            for (int i = 0; i < tex.width; i++)
                            {
                                Color c = tex.GetPixel(i, j - tex.height - 1);
                                ctex.SetPixel(i, j, new Color(c.a, c.a, c.a));
                            }
                        }
                        //补一像素
                        for (int j = tex.height * 2 + 1; j < tex.height * 2 + 2; j++)
                        {
                            for (int i = 0; i < tex.width; i++)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                        }
                    }
                }
                else if (E_ExtractType.E_Horizontal == eType)
                {
                    ctex = new Texture2D(tex.width * 2, tex.height, TextureFormat.RGB24, texim.mipmapEnabled);
                    for (int i = 0; i < tex.width; i++)
                    {
                        for (int j = 0; j < tex.height; j++)
                        {
                            Color c = tex.GetPixel(i, j);
                            if (c.a < 0.001f)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                            else
                            {
                                ctex.SetPixel(i, j, new Color(c.r, c.g, c.b));
                            }
                        }
                    }
                    for (int i = tex.width; i < tex.width * 2; i++)
                    {
                        for (int j = 0; j < tex.height; j++)
                        {
                            Color c = tex.GetPixel(i - tex.width, j);
                            ctex.SetPixel(i, j, new Color(c.a, c.a, c.a));
                        }
                    }
                }
                else if (E_ExtractType.E_Vertical == eType)
                {
                    ctex = new Texture2D(tex.width, tex.height * 2, TextureFormat.RGB24, texim.mipmapEnabled);
                    for (int i = 0; i < tex.width; i++)
                    {
                        for (int j = 0; j < tex.height; j++)
                        {
                            Color c = tex.GetPixel(i, j);
                            if (c.a < 0.001f)
                            {
                                ctex.SetPixel(i, j, new Color(0, 0, 0));
                            }
                            else
                            {
                                ctex.SetPixel(i, j, new Color(c.r, c.g, c.b));
                            }
                        }
                    }
                    for (int i = 0; i < tex.width; i++)
                    {
                        for (int j = tex.height; j < tex.height * 2; j++)
                        {
                            Color c = tex.GetPixel(i, j - tex.height);
                            ctex.SetPixel(i, j, new Color(c.a, c.a, c.a));
                        }
                    }
                }

                ctex.Apply();

                byte[] bytes = ctex.EncodeToPNG();

                System.IO.File.WriteAllBytes((desPath + "/" + strFileName + ".png"), bytes);
            }
            //retore import
            texim.isReadable = bReadable;
            texim.textureFormat = tf;
            AssetDatabase.ImportAsset(strFile);

            //AssetDatabase.Refresh();
            return true;
        }

        [MenuItem("Assets/图集工具/删除alpha")]
        static public void RemoveAlpha()
        {
            string strFolder;
            if (Selection.objects == null || Selection.objects.Length == 0)
            {

            }
            else
            {
                EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section.....", 0);
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    strFolder = AssetDatabase.GetAssetPath(Selection.objects[i]);
                    EditorUtility.DisplayProgressBar("ExtractAlpha", "Build Section....." + strFolder, i * 1.0f / Selection.objects.Length);
                    if (!RemoveAlpha(strFolder, ""))
                    {
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
            }
        }

        static public bool RemoveAlpha(string strFile, string strDesFolder)
        {
            int idot = strFile.LastIndexOf('.');
            if (idot <= 0)
                return false;

            string ext = strFile.Substring(idot);
            if (ext != ".png" && ext != ".tga")
                return false;

            string strFileName;


            int iiv = strFile.LastIndexOf('/');
            strFileName = strFile.Substring(iiv + 1, idot - iiv - 1);
            if (strFileName.Length <= 0)
                return false;

            string desPath;
            if (strDesFolder != null && strDesFolder.Length != 0)
            {
                desPath = strDesFolder;
            }
            else
            {
                desPath = strFile.Substring(0, iiv);
            }

            TextureImporter texim = (TextureImporter)(TextureImporter.GetAtPath(strFile));
            if (texim == null)
                return false;
            bool bReadable = texim.isReadable;
            texim.isReadable = true;
            TextureImporterFormat tf = texim.textureFormat;
            texim.textureCompression = TextureImporterCompression.Uncompressed;
            texim.textureFormat = TextureImporterFormat.RGBA32;

            AssetDatabase.ImportAsset(strFile);

            Texture2D tex = AssetDatabase.LoadAssetAtPath(strFile, typeof(Texture2D)) as Texture2D;
            if (tex == null)
            {
                texim.isReadable = bReadable;
                texim.textureFormat = tf;
                AssetDatabase.ImportAsset(strFile);
                return false;
            }

            if (tex.format == TextureFormat.RGBA32 || tex.format == TextureFormat.ARGB32)
            {
                Texture2D ctex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, texim.mipmapEnabled);
                ctex.SetPixels(tex.GetPixels());
                ctex.Apply();

                byte[] bytes = ctex.EncodeToPNG();

                System.IO.File.WriteAllBytes((desPath + "/" + strFileName + ext), bytes);
            }
            //retore import
            texim.textureCompression = TextureImporterCompression.Compressed;
            texim.textureFormat = TextureImporterFormat.Automatic;
            texim.isReadable = false;
            texim.textureFormat = tf;
            AssetDatabase.ImportAsset(strFile);

            //AssetDatabase.Refresh();
            return true;
        }
    }
}
#endif