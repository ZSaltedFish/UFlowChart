using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartConfig
    {
        public const string PATH = "Packages/com.zknight.uflowchart/Resources/Config.txt";
        public string SubNodePath = "Assets/FlowChart/SubCharts";

        private static FlowChartConfig _ins;
        public static FlowChartConfig INSTANCE
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new FlowChartConfig();
                }
                return _ins;
            }
        }

        private FlowChartConfig()
        {
            Load();
        }

        public void Load()
        {
            if (!File.Exists(PATH))
            {
                File.Create(PATH);
            }
            using StreamReader reader = new StreamReader(PATH);
            string[] datas = reader.ReadToEnd().Split('\t');
            if (datas.Length > 0) SubNodePath = datas[0];
        }

        public void Save()
        {
            using StreamWriter writer = new StreamWriter(PATH);
            StringBuilder builder = new StringBuilder();
            builder.Append(SubNodePath).Append('\t');
            writer.Write(builder.ToString());
        }
    }
}
