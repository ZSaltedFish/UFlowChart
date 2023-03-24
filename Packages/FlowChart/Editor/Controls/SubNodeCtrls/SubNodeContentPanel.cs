using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;
using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class SubNodeContentPanel : EditorPopupMenu
    {
        public VariableParam Params;
        private static Dictionary<string, Type> _TYPE_DICT;
        private string _textFilter;
        private Vector2 _v;
        private List<string> _filter = new List<string>();

        public List<Action<Type>> OnTypeSelected = new List<Action<Type>>();

        public override void InitFinish()
        {
            base.InitFinish();
            Size = new Vector2(200, 500);
            if (_TYPE_DICT == null)
            {
                InitDICT();
            }
        }

        private void InitDICT()
        {
            _TYPE_DICT = AssemblyHelper.AcceptableTypes;
        }

        protected override void Draw()
        {
            string temp = EditorGUILayout.TextField(_textFilter, GUILayout.Height(25));
            if (temp != _textFilter)
            {
                _textFilter = temp.ToLower();
                _filter.Clear();
                if (!string.IsNullOrEmpty(_textFilter))
                {
                    foreach (var item in _TYPE_DICT)
                    {
                        string keyLower = item.Key.ToLower();
                        if (keyLower == _textFilter)
                        {
                            _filter.Insert(0, item.Key);
                        }
                        else if (item.Key.ToLower().Contains(_textFilter))
                        {
                            _filter.Add(item.Key);
                        }
                    }
                }
            }

            if (_filter.Count > 0 && _filter.Count <= 100)
            {
                _v = EditorGUILayout.BeginScrollView(_v);
                foreach (string name in _filter)
                {
                    if (GUILayout.Button(name, GUILayout.Width(200)))
                    {
                        foreach (Action<Type> action in OnTypeSelected)
                        {
                            try
                            {
                                action(_TYPE_DICT[name]);
                            }
                            catch (Exception err)
                            {
                                Debug.LogError(err);
                            }
                        }
                        Hide();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
