using System;
using UnityEditor;
using UnityEngine;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartDialog : EditorControlDialog
    {
        public FlowChartModel Model;

        private static string[] _FILTER = new string[] { "Prefab", "prefab" };
        public FlowChartCanvas FCanvas;
        public EditorToolBar ToolBar;
        public override string XMLNodePath => "Packages/com.zknight.uflowchart/Editor/ControlXmls/FlowChartDialog.xml";
        public ChartNodeInfoManager Manager;
        public string AssetPath { get; private set; }
        private GameObject _srcObj;
        public override void Start()
        {
            base.Start();
            Model = new FlowChartModel();
            wantsMouseMove = true;

            EditorPopupMenu filePopup = NodeFactoryXML.CreateEditorControl<EditorPopupMenu>();
            filePopup.AddMenu("Save", Save);
            filePopup.AddMenu("Open", obj => OpenPanel());
            ToolBar.AddItem("Files", filePopup);

            EditorPopupMenu toolPopup = NodeFactoryXML.CreateEditorControl<EditorPopupMenu>();
            toolPopup.AddMenu("Sub Chart Dir", SubChartDir);
            ToolBar.AddItem("Tool", toolPopup);
        }

        private void SubChartDir(object obj)
        {
            string path = EditorUtility.OpenFolderPanel("Sub Node Directory", FlowChartConfig.INSTANCE.SubNodePath, "");
            if (!string.IsNullOrEmpty(path))
            {
                FlowChartConfig.INSTANCE.SubNodePath = path;
                FlowChartConfig.INSTANCE.Save();
                SubChartDirHelper.Init();
            }
        }

        private void OpenPanel()
        {
            string str = EditorUtility.OpenFilePanelWithFilters("Open Flow Chart", "", _FILTER);
            if (!string.IsNullOrEmpty(str))
            {
                OpenPrefab(str);
            }
        }

        private void OpenPrefab(string str)
        {
            FCanvas.Clear();
            Manager = new ChartNodeInfoManager();
            _srcObj = AssetDatabase.LoadAssetAtPath<GameObject>(str);
            Manager.ReadFromGameObject(_srcObj);
            FCanvas.InitNode(Manager.Nodes.ToArray());
        }

        public void OpenPrefab(GameObject go)
        {
            FCanvas.Clear();
            Manager = new ChartNodeInfoManager();
            _srcObj = go;
            AssetPath = AssetDatabase.GetAssetPath(go);
            Manager.ReadFromGameObject(_srcObj);
            FCanvas.InitNode(Manager.Nodes.ToArray());
        }

        public void CreateNew()
        {
            FCanvas.Clear();
            Manager = new ChartNodeInfoManager();
            FCanvas.InitNode(Manager.Nodes.ToArray());
        }

        public void Save(object obj = null)
        {
            try
            {
                if (_srcObj == null)
                {
                    string str = EditorUtility.SaveFilePanel("Save Flow Chart", "", "FlowChart", "Prefab");
                    if (!string.IsNullOrEmpty(str))
                    {
                        GameObject go = Manager.Generate();
                        _srcObj = PrefabUtility.SaveAsPrefabAsset(go, str);
                        AssetPath = AssetDatabase.GetAssetPath(_srcObj);
                        DestroyImmediate(go);
                    }
                }
                else
                {
                    GameObject go = Manager.Generate();
                    _srcObj = EditorResHelper.ReplacePrefabAsset(AssetPath, go);
                    DestroyImmediate(go);
                }
                Debug.Log($"存储成功");
            }
            catch (Exception err)
            {
                Debug.LogError(err);
            }
        }
    }
}
