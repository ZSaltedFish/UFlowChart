using System;
using UnityEditor;
using UnityEngine;
using ZKnight.UFlowChart.ChatGPT;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Editor
{
    public class ChatGPTCommand : EditorWindow
    {
        public bool HasInitialize => _inited;
        public string APIKey = "Your Token";
        private static ChatGPTInstance GPT = new ChatGPTInstance();
        private bool _initing = false;
        private bool _inited => GPT.MessageHistory.Count != 0;
        private bool _initedFaild = false;

        public bool Inited => _inited;
        public bool InitedFaild => _initedFaild;

        private string _message;

        public void Init()
        {
            var head = ChatGPTCreator.GetGPTHead();
            GPT.SetAIPKey(APIKey);
            GPT.Send(head, InitReturn, null);
            _initing = true;
        }

        private void InitReturn(bool arg1, string arg2)
        {
            if (arg1)
            {
                _initing = false;
            }
            else
            {
                _initedFaild = true;
            }
        }

        public void Send(string value, Action<bool, string> resultAction)
        {
            if (!_inited && _initedFaild)
            {
                return;
            }
            GPT.Send(value, resultAction, null);
        }

        public void OnGUI()
        {
            APIKey = EditorGUILayout.TextField("ChatGPT Token", APIKey);
            if (_initing)
            {
                EditorGUILayout.LabelField("初始化中");
                return;
            }

            if (!_inited)
            {
                if (GUILayout.Button("Init"))
                {
                    Init();
                }
                return;
            }

            if (_initedFaild)
            {
                EditorGUILayout.LabelField("初始化失败");
                return;
            }

            _message = EditorGUILayout.TextArea(_message, GUILayout.Height(200));
            if (GUILayout.Button("Send"))
            {
                Send(_message, SendCallback);
            }

            if (GUILayout.Button("Re Init"))
            {
                Init();
            }
        }

        private FlowChartDialog _dialog;

        private void SendCallback(bool arg1, string arg2)
        {
            if (arg1)
            {
                _dialog.GPTCallback(arg2);
            }
        }

        public static void OpenWindow(FlowChartDialog dialog)
        {
            var window = GetWindow<ChatGPTCommand>();
            window._dialog = dialog;
            window.Show();
        }
    }
}
