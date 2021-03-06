
using UnityEngine;

namespace BattleSimulator.UI
{
    class MoveNodeCommand : Command
    {
        private UINode _uinode;
        private Vector2 _position;
        private Vector2 _undo;

        public MoveNodeCommand (UINode uinode, Vector2 position)
        {
            _uinode = uinode;
            _undo = uinode.position;
            _position = position;
        }

        public override bool CanMerge(Command command)
        {
            var move = command as MoveNodeCommand;
            if (move == null || move._uinode != _uinode)
                return false;

            return true;
        }

        public override void Merge (Command command)
        {
            var move = command as MoveNodeCommand;
            if (move == null || move._uinode != _uinode)
                return;

            _position = move._position;
            Execute(); 
        }

        public override void OnExecute()
        {
            _uinode.MoveTo(_position);   
        }

        public override void OnRedo()
        {
            _uinode.MoveTo(_position);
        }

        public override void OnUndo()
        {
            _uinode.MoveTo(_undo);
        }
    }
}
