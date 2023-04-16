using System.Collections.Generic;
using UnityEngine;
using ZKnight.UFlowChart.Runtime;

namespace ZKnight.UFlowChart.Node
{
    [FlowChartNode("Root/PlayAnimation")]
    [ChatGPT("播放动作")]
    public class PlayAnimation : FlowChartNode
    {
        [FlowChartInput("播放动作的单位")]
        public Animator Anim;
        [FlowChartInput("动作名称")]
        public string AnimationName;
        [FlowChartStream]
        public FlowChartNode Next;
        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Anim.SetTrigger(AnimationName);
            return Next;
        }
    }
}
