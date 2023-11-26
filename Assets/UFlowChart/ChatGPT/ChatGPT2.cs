using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Unity.EditorCoroutines.Editor;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Knight.UFlowChart.ChatGPT
{
    public class ChatGPT2
    {
        public const string SETTING_FOLDER = "com.zknight.uflowchart/ChatGPT/ChatGPTSettings";
        public const string HISTORY_FILE_NAME = "ChatHistory.json";

        public const string ChatGPTUrl = "https://api.openai.com/v1/chat/completions";
        public const string DefaultModel = "gpt-3.5-turbo";
        public const string DefaultToken = "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";
        public const string DefaultUserID = "User";
        public const float DefaultTemperature = 0f;
        public string UserName;
        public string ChatGPTToken;
        public string UserID;

        private List<Message> _messagesHistory;
        private ChatGPTRequestData _requestData;
        private UnityWebRequest _webRequest;

        public float ChatGPTRandomness
        {
            get => _requestData.temperature;
            set => _requestData.temperature = Mathf.Clamp(value, 0f, 2f);
        }

        public bool IsRequesting => _webRequest != null && !_webRequest.isDone;
        public float RequestProgress => IsRequesting ? (_webRequest.uploadProgress + _webRequest.downloadProgress) / 2f : 0f;

        private ChatGPT2(string token = DefaultToken, string userID = DefaultUserID, string model = DefaultModel, float temperature = DefaultTemperature)
        {
            ChatGPTToken = string.IsNullOrWhiteSpace(token) ? DefaultToken : token;
            UserID = string.IsNullOrWhiteSpace(userID) ? DefaultUserID : userID;
            _messagesHistory = new List<Message>();
            _requestData = new ChatGPTRequestData(model, temperature);
        }

        public void SetToken(string token)
        {
            ChatGPTToken = token;
        }

        /// <summary>
        /// Continue last conversation
        /// </summary>
        public void RestoreChatHistory()
        {
            var path = Path.Combine(SETTING_FOLDER, HISTORY_FILE_NAME);
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            var data = JsonConvert.DeserializeObject<ChatGPTRequestData>(asset.text);
            _requestData.messages = data.messages;
        }

        public void SaveChatHistory()
        {
            var path = Path.Combine(SETTING_FOLDER, HISTORY_FILE_NAME);
            var json = JsonConvert.SerializeObject(_requestData);
            var text = new TextAsset(json);
            AssetDatabase.CreateAsset(text, path);
        }

        public void Send(string message, Action<bool, string> onComplete, Action<float> onProgressUpdate)
        {
            EditorCoroutineUtility.StartCoroutine(Request(message, onComplete, onProgressUpdate), this);
        }

        public IEnumerator Request(string input, Action<bool, string> onComplete, Action<float> onProgressUpdate)
        {
            var msg = new Message()
            {
                role = UserID,
                content = input
            };

            _requestData.AppendChat(msg);
            _messagesHistory.Add(msg);

            using (_webRequest = new UnityWebRequest(ChatGPTUrl, "POST"))
            {
                var jsonDt = JsonConvert.SerializeObject(_requestData);
                Debug.Log(jsonDt);
                var bodyRaw = Encoding.UTF8.GetBytes(jsonDt);
                _webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                _webRequest.downloadHandler = new DownloadHandlerBuffer();
                _webRequest.SetRequestHeader("Content-Type", "application/json");
                _webRequest.SetRequestHeader("Authorization", $"Bearer {DefaultToken}");
                _webRequest.certificateHandler = new ChatGPTWebRequestCert();

                var req = _webRequest.SendWebRequest();
                while (!_webRequest.isDone)
                {
                    onProgressUpdate?.Invoke((_webRequest.downloadProgress + _webRequest.uploadProgress) / 2f);
                    yield return null;
                }

                if (_webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"ChatGPT Request Faild: {_webRequest.result}, {_webRequest.responseCode}");
                    onComplete?.Invoke(false, string.Empty);
                }
                else
                {
                    var json = _webRequest.downloadHandler.text;
                    Debug.Log(json);

                    try
                    {
                        var result = JsonConvert.DeserializeObject<ChatCompletion>(json);
                        var lastChoiceIdx = result.choices.Count - 1;
                        var replyMsg = result.choices[lastChoiceIdx].message;
                        replyMsg.content = replyMsg.content.Trim();
                        _messagesHistory.Add(replyMsg);
                        onComplete?.Invoke(true, replyMsg.content);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"ChatGPT download data format ERROR: {e.Message}");
                        onComplete?.Invoke(false, e.Message);
                    }
                }

                _webRequest = null;
            }
        }

        public void NewChat()
        {
            _requestData.Clear();
            _messagesHistory.Clear();
        }

        public bool IsSelfMessage(Message msg)
        {
            return UserID.CompareTo(msg.role) == 0;
        }

        private static ChatGPT2 _INSTANCE;
        public static ChatGPT2 CHAT_GPT_INSTANCE
        {
            get
            {
                if (_INSTANCE == null)
                {
                    _INSTANCE = new ChatGPT2();
                }

                return _INSTANCE;
            }
        }
    }
}
