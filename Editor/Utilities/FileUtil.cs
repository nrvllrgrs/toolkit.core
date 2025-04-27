using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

namespace ToolkitEditor
{
    public static class FileUtil
    {
        public static string GetAbsolutePath(Object asset)
        {
            if (asset == null)
                return string.Empty;

            return Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(asset).Substring("/Assets".Length));
		}

        public static string GetRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (!path.StartsWith(Application.dataPath, System.StringComparison.OrdinalIgnoreCase))
                return path;

            return "Assets" + path.Substring(Application.dataPath.Length);
        }

        public static T GetAssetAtAbsolutePath<T>(string path)
            where T : Object
        {
            string relativePath = GetRelativePath(path);
            if (relativePath == null)
                return null;

            return AssetDatabase.LoadAssetAtPath<T>(relativePath);
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