namespace BattleSimulator.UI
{
    class SelectNodeCommand : Command
    {
        private UINode _uinode;

        public SelectNodeCommand (UINode uinode)
        {
            _uinode = uinode;
        }

        public override void OnExecute()
        {
            _uinode.uigraph.SelectNode(_uinode);
        }

        public override void OnRedo() => OnExecute();
    
        public override void OnUndo()
        {
            _uinode.uigraph.UnselectNode(_uinode);
        }
    }
}
