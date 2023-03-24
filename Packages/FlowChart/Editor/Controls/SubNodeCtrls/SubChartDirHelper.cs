using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public static class SubChartDirHelper
    {
        private static Dictionary<string, GameObject> _SUB_CHART;

        public static void Init()
        {
            _SUB_CHART = new Dictionary<string, GameObject>();
            string dir = FlowChartConfig.INSTANCE.SubNodePath;
            string[] paths = Directory.GetFiles(dir);

            foreach (string tPath in paths)
            {
                string path = tPath.ToLower();
                if (!path.EndsWith(".prefab"))
                {
                    continue;
                }
                FileInfo info = new FileInfo(path);
                string newPath = path.Replace('\\', '/');
                int index = newPath.IndexOf("assets");
                newPath = newPath.Substring(index);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPath);
                _SUB_CHART.Add(info.Name.Remove(info.Name.IndexOf(".prefab")), prefab);
            }
        }

        public static Dictionary<string, GameObject> GetSubChart()
        {
            Init();
            return _SUB_CHART;
        }
    }
}
