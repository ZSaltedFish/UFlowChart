using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{

    public class FlowChartContentPanel : EditorPopupMenu
    {
        private class ButtonTree
        {
            public const float BUTTON_FIX_HEIGHT = 20, FOLD_FIX_HEIGHT = 20, SPACE_WIDTH = 30;

            public string Name;
            public string Path { get; private set; }
            public Dictionary<string, ButtonTree> SubTree = new Dictionary<string, ButtonTree>();
            public Type EndingType;
            public string EndingName;
            public bool IsShowing = false;
            public FlowChartContentPanel Parent;
            public int StackIndex { get; private set; }
            public int Count => SubTree.Count;

            private GUIStyle _buttonStyle;
            private GUIStyle _foldState;
            private bool _hide = false;

            public ButtonTree(string name, FlowChartContentPanel parent, int index)
            {
                Parent = parent;
                Name = name;
                StackIndex = index;
                _buttonStyle = new GUIStyle
                {
                    hover = Parent._hoverState,
                    normal = Parent._normalState,
                    fixedHeight = BUTTON_FIX_HEIGHT,
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = (int)(BUTTON_FIX_HEIGHT / 1.5f)
                };

                _foldState = new GUIStyle(EditorStyles.foldout)
                {
                    fixedHeight = FOLD_FIX_HEIGHT,
                    hover = Parent._hoverState,
                    normal = Parent._normalState,
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = (int)(FOLD_FIX_HEIGHT / 1.5f)
                };
            }

            public ButtonTree GetButtonTree(string name, FlowChartContentPanel parent, int index)
            {
                if (!SubTree.TryGetValue(name, out ButtonTree subTree))
                {
                    subTree = new ButtonTree(name, parent, index);
                    SubTree.Add(name, subTree);
                }
                return subTree;
            }

            public void CreateButton(Type type, string name, FlowChartContentPanel parent, int index)
            {
                if (SubTree.ContainsKey(name))
                {
                    Debug.LogError($"{Name}包含重复的节点{name}");
                    return;
                }
                SubTree.Add(name, new ButtonTree(name, parent, index)
                {
                    EndingType = type,
                    EndingName = name
                });
            }

            public void Draw()
            {
                if (_hide)
                {
                    return;
                }
                if (SubTree.Count > 0)
                {
                    float width = Parent.Size.x - SPACE_WIDTH * StackIndex;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(FOLD_FIX_HEIGHT), GUILayout.Width(width));
                    IsShowing = EditorGUI.Foldout(rect, IsShowing, Name, true, _foldState);
                    EditorGUILayout.EndHorizontal();
                    if (IsShowing)
                    {
                        List<string> keys = new List<string>(SubTree.Keys);
                        foreach (string key in keys)
                        {
                            ButtonTree sub = SubTree[key];
                            sub.Draw();
                        }
                    }
                }
                else
                {
                    DrawButton();
                }
            }

            private void DrawButton()
            {
                float width = Parent.Size.x - SPACE_WIDTH * StackIndex;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button(EndingName, _buttonStyle, GUILayout.Height(BUTTON_FIX_HEIGHT), GUILayout.Width(width)))
                {
                    if (EndingType == typeof(GameObject))
                    {
                        Parent.RunSubNodeEvent(EndingName);
                    }
                    else
                    {
                        Parent.RunEvent(EndingType, Path);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            public bool Filt(string search)
            {
                bool filt = false;
                if (SubTree.Count > 0)
                {
                    List<string> keys = new List<string>(SubTree.Keys);
                    foreach (string key in keys)
                    {
                        ButtonTree sub = SubTree[key];
                        filt = sub.Filt(search) || filt;
                    }
                }
                else
                {
                    filt |= EndingName.ToLower().Contains(search);
                }
                _hide = !filt;
                IsShowing = filt && !string.IsNullOrEmpty(search);
                return filt;
            }

            public void Sort()
            {
                Dictionary<ButtonTree, string> tree2Key = new Dictionary<ButtonTree, string>();
                foreach (var item in SubTree)
                {
                    tree2Key.Add(item.Value, item.Key);
                }
                List<ButtonTree> values = new List<ButtonTree>(SubTree.Values);
                values.Sort((a, b) =>
                {
                    if (a.Count > 0 && b.Count > 0) return 0;
                    if (a.Count > 0 && b.Count == 0) return 1;
                    if (a.Count == 0 && b.Count > 0) return -1;
                    return 0;
                });

                SubTree.Clear();
                foreach (var item in values)
                {
                    item.Sort();
                    SubTree.Add(tree2Key[item], item);
                }
            }

            public void CreateButton(Type type, string v1, FlowChartContentPanel flowChartContentPanel, int v2, string editorPath)
            {
                CreateButton(type, v1, flowChartContentPanel, v2);
                Path = editorPath;
            }
        }

        public Color BgColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);
        public Color RectColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        private Rect _drawRect = new Rect(Vector2.zero, Vector2.zero);
        private ButtonTree _contextTree;
        public List<Action<FlowChartDialog, Type, Vector2, string>> OnTypeSelected = new List<Action<FlowChartDialog, Type, Vector2, string>>();
        public List<Action<FlowChartDialog, string, Vector2>> OnSubNodeSelected = new List<Action<FlowChartDialog, string, Vector2>>();

        private Texture2D _hoverTexture;
        private GUIStyleState _hoverState;
        private GUIStyleState _normalState;
        private string _search = string.Empty;
        private GUIStyle _searchStyle;
        private Vector2 _v2;

        public FlowChartContentPanel() : base()
        {
            Style = null;

            const int size = 20;
            _hoverTexture = new Texture2D(size, size);
            Color color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    _hoverTexture.SetPixel(x, y, color);
                }
            }
            _hoverState = new GUIStyleState()
            {
                background = _hoverTexture
            };
            _normalState = new GUIStyleState()
            {
                textColor = Color.white
            };
            _searchStyle = new GUIStyle(EditorStyles.textField)
            {
                alignment = TextAnchor.MiddleLeft
            };
        }
        public override void InitFinish()
        {
            _contextTree = new ButtonTree("ROOT", this, 0);
            InitButtonTree();
            Size = new Vector2(250, 500);
            OnTypeSelected.Add(FlowChartController.CreateNewNode);
            OnSubNodeSelected.Add(FlowChartController.CreateSubNode);

            OnMouseDown.Add(RunoutEvent);
            OnMouseUp.Add(RunoutEvent);
            OnMouseClick.Add(RunoutEvent);
            OnMouseMove.Add(RunoutEvent);
        }

        private void RunoutEvent(EditorControl arg1, EditorEvent arg2)
        {
            arg2.Use();
        }

        private void InitButtonTree()
        {
            Type[] types = typeof(FlowChart).Assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(FlowChartNodeAttribute), false);
                foreach (FlowChartNodeAttribute item in attrs)
                {
                    string[] paths = item.EditorPath.Split('/');
                    if (item.Hidden && item.Abenden)
                    {
                        continue;
                    }
                    ButtonTree tree = _contextTree;
                    for (int i = 0; i < paths.Length - 1; ++i)
                    {
                        tree = tree.GetButtonTree(paths[i], this, i);
                    }

                    if (item.Overload)
                    {
                        NodeOverloadManager.INSTANCE.SetOverrideNode(item.EditorPath, type);
                        if (!tree.SubTree.ContainsKey(paths[paths.Length - 1]))
                        {
                            tree.CreateButton(type, paths[paths.Length - 1], this, paths.Length - 1, item.EditorPath);
                        }
                    }
                    else
                    {
                        tree.CreateButton(type, paths[paths.Length - 1], this, paths.Length - 1);
                    }
                }
            }

            ButtonTree subTree = _contextTree.SubTree["Sub Node"];
            Dictionary<string, GameObject> subDir = SubChartDirHelper.GetSubChart();
            foreach (var item in subDir)
            {
                subTree.CreateButton(typeof(GameObject), item.Key, this, 1);
            }
            _contextTree.Sort();
        }

        protected override void Draw()
        {
            ButtonTree tree = _contextTree;
            Rect searchRect = EditorGUILayout.GetControlRect(GUILayout.Height(30));
            searchRect.position = new Vector2((Size.x - searchRect.size.x) / 2, 5);
            searchRect.size = new Vector2(searchRect.size.x, 25);   
            EditorGUI.DrawRect(searchRect, RectColor);
            string search = EditorGUI.TextField(searchRect, _search, _searchStyle);

            if (search != _search)
            {
                _search = search;
                tree.Filt(_search.ToLower());
            }

            _v2 = EditorGUILayout.BeginScrollView(_v2);
            List<string> keys = new List<string>(tree.SubTree.Keys);
            _drawRect.size = Size;
            EditorGUI.DrawRect(_drawRect, BgColor);
            foreach (string key in keys)
            {
                ButtonTree sub = tree.SubTree[key];
                sub.Draw();
            }
            EditorGUILayout.EndScrollView();
        }

        private void RunEvent(Type type, string path)
        {
            foreach (var item in OnTypeSelected)
            {
                try
                {
                    item(Root as FlowChartDialog, type, Rect.position, path);
                }
                catch (Exception err)
                {
                    Debug.LogError(err);
                }
            }
            Hide();
        }

        private void RunSubNodeEvent(string endingName)
        {
            foreach (var item in OnSubNodeSelected)
            {
                try
                {
                    item((Root as FlowChartDialog), endingName, Rect.position);
                }
                catch (Exception err)
                {
                    Debug.LogError(err);
                }
            }
            Hide();
        }

        public override void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(_hoverTexture);
            base.Dispose();
        }
    }
}
