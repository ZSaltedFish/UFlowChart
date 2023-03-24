using ZKnight.ZXMLui;

namespace ZKnight.UFlowChart.Editor
{
    public class NodeSelectedCtrl : EditorControl
    {
        public int SrcNodeID;
        public EditorImage OutLine;
        public EditorImage Selected;

        public bool IsOutline
        {
            get => OutLine.Active;
            set
            {
                OutLine.Active = value;
            }
        }

        public bool IsSelected
        {
            get => Selected.Active;
            set
            {
                Selected.Active = value;
            }
        }
        public override string XMLNodePath => "Packages/com.zknight.uflowchart/Editor/ControlXmls/NodeSelectedCtrl.xml";
    }
}
