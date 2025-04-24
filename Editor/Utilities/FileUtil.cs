using System.IO;
using UnityEditor;
using UnityEngine;

namespace ToolkitEditor
{
    public static class FileUtil
    {
        public static string GetAbsolutePath(Object asset)
        {
            return Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(asset).Substring("/Assets".Length));
		}

        public static void Move(string srcPath, string dstPath, bool overridePath = true)
        {
            if (!File.Exists(srcPath))
                return;

            if (File.Exists(dstPath))
            {
                if (!overridePath)
                    return;

                File.Delete(dstPath);
            }

            File.Move(srcPath, dstPath);
        }
    }
}