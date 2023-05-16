using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MonMoose.Core
{
    public static class ProtoUtility
    {
        public static void RunProtoc(string ilPath, string ilFolderPath, string exportFolderPath)
        {
            string protocPath;
            FilePathUtility.TryGetAbsolutePath(@"..\Tools\ProtoStructureGenerator\protoc.exe", out protocPath);
            string argStr = string.Format("--csharp_out={0} {1} --proto_path={2}", exportFolderPath, ilPath, ilFolderPath);
            string errorMsg;
            if (!RunExe(protocPath, argStr, out errorMsg))
            {
                throw new Exception(errorMsg);
            }
        }

        public static bool RunExe(string fileName, string argument, out string errorMsg)
        {
            bool result = false;
            if (string.IsNullOrEmpty(fileName))
            {
                errorMsg = "File Name is Empty";
                return false;
            }
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = fileName;
            startInfo.Arguments = argument;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            errorMsg = string.Empty;
            try
            {
                if (process.Start())
                {
                    errorMsg = process.StandardError.ReadToEnd();
                    if (string.IsNullOrEmpty(errorMsg))
                    {
                        result = true;
                    }
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                errorMsg = e.Message;
            }
            finally
            {
                if (process != null)
                {
                    process.Close();
                }
            }
            return result;
        }
    }
}
