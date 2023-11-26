﻿using System.Collections.Generic;
using UnityEngine;

namespace ZKnight.UFlowChart.Runtime
{
    [FlowChartNode("Math/Divide", overload: true)]
    public class DivideVector2 : FlowChartNode
    {
        [FlowChartInput("A")]
        public Vector2 A;
        [FlowChartInput("B")]
        public float B;
        [FlowChartOutput("Result")]
        public Vector2 Result;
        [FlowChartStream]
        public FlowChartNode Next;

        public override FlowChartNode FlowChartContent(Dictionary<string, object> @params)
        {
            Result = A / B;
            return Next;
        }
    }
}