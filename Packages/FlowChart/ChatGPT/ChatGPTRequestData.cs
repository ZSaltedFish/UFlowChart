using System;
using System.Collections.Generic;

namespace Knight.UFlowChart.ChatGPT
{
    [Serializable]
    public class ChatGPTRequestData
    {
        public List<Message> messages;
        public string model;
        public float temperature;

        public ChatGPTRequestData(string model, float temperature)
        {
            messages = new List<Message>();
            this.model = model;
            this.temperature = temperature;
        }

        public ChatGPTRequestData AppendChat(Message msg)
        {
            messages.Add(msg);
            return this;
        }

        public void Clear()
        {
            messages.Clear();
        }
    }
}
