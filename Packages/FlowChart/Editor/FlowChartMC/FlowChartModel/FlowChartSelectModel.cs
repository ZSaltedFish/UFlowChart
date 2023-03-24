using System.Collections.Generic;

namespace ZKnight.UFlowChart.Editor
{
    public class FlowChartSelectModel
    {
        public List<IFlowChartOperator> SelectedNodes;
        public IFlowChartOperator SelectedLine;

        public FlowChartSelectModel()
        {
            SelectedNodes = new List<IFlowChartOperator>();
        }

        public void ReleaseSelected()
        {
            foreach (IFlowChartOperator item in SelectedNodes)
            {
                item.SetUnselected();
            }
            SelectedNodes.Clear();
            SelectedLine?.SetUnselected();
            SelectedLine = null;
        }

        public void SetSelectedNode(IFlowChartOperator node)
        {
            SelectedNodes.Add(node);
            node.SetSelected();
        }

        public void SetSelectedLine(IFlowChartOperator line)
        {
            SelectedLine = line;
            line.SetSelected();
        }
    }
}
