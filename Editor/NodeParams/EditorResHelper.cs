using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public class EditorResHelper
    {
        public const string CONFIG_RES_PATH = "Assets/Bundles";


        /// <summary>
        /// 获取文件夹内所有的预制路径
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="subDire">是否获取子文件夹</param>
        /// <returns></returns>
        public static List<string> GetAllPath(string srcPath, bool subDire)
        {
            List<string> paths = new List<string>();
            string[] files = Directory.GetFiles(srcPath);
            foreach (string str in files)
            {
                if (str.EndsWith(".prefab"))
                {
                    paths.Add(str);
                }
            }
            if (subDire)
            {
                foreach (string subPath in Directory.GetDirectories(srcPath))
                {
                    List<string> subFiles = GetAllPath(subPath, true);
                    paths.AddRange(subFiles);
                }
            }
            return paths;
        }

        /// <summary>
        /// 获取文件夹内所有资源路径
        /// </summary>
        /// <param name="srcPath">源文件夹</param>
        /// <param name="subDire">是否获取子文件夹</param>
        /// <returns></returns>
        public static List<string> GetAllResourcePath(string srcPath, bool subDire)
        {
            List<string> paths = new List<string>();
            string[] files = Directory.GetFiles(srcPath);
            foreach (string str in files)
            {
                if (str.EndsWith(".meta"))
                {
                    continue;
                }
                paths.Add(str);
            }
            if (subDire)
            {
                foreach (string subPath in Directory.GetDirectories(srcPath))
                {
                    List<string> subFiles = GetAllResourcePath(subPath, true);
                    paths.AddRange(subFiles);
                }
            }
            return paths;
        }

        public static void DeleteConfig(params int[] ids)
        {
            foreach (int id in ids)
            {
                AssetDatabase.DeleteAsset($"{CONFIG_RES_PATH}/{id}.prefab");
            }
            AssetDatabase.Refresh();
        }

        public static bool WriteData(byte[] bytes, string path)
        {
            try
            {
                using FileStream file = new FileStream(path, FileMode.Create);
                file.Write(bytes, 0, bytes.Length);
                return false;
            }
            catch (Exception err)
            {
                Debug.LogError(err);
                return false;
            }
        }

        public static GameObject ReplacePrefabAsset(GameObject srcAsset)
        {
            GameObject temp = UnityEngine.Object.Instantiate(srcAsset);
            string path = AssetDatabase.GetAssetPath(srcAsset);
            GameObject newAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(temp, path, InteractionMode.AutomatedAction);
            UnityEngine.Object.DestroyImmediate(temp);
            return newAsset;
        }

        public static GameObject ReplacePrefabAsset(string path, GameObject go)
        {
            GameObject newAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(go, path, InteractionMode.AutomatedAction);
            return newAsset;
        }
    }
}
