
using System.Collections.Generic;

namespace BattleSimulator.UI
{
    internal class GroupCommand : Command
    {
        private List<Command> _commands = new List<Command>(4);

        public void Add (Command command)
        {
            _commands.Add(command);
        }

        public override void OnExecute()
        {
            foreach (var command in _commands)
                command.OnExecute();
        }

        public override void OnRedo() => OnExecute();

        public override void OnUndo()
        {
            for (var i = _commands.Count - 1; i >= 0; i--)
                _commands[i].OnUndo();
        }

        public override bool CanMerge (Command command)
        {
            var group = command as GroupCommand;
            if (group == null)
                return false;

            if (group._commands.Count != _commands.Count)
                return false;

            for (int i = 0; i < _commands.Count; i++)
                if (!_commands[i].CanMerge(group._commands[i]))
                    return false;

            return true;
        }

        public override void Merge (Command command)
        {
            var group = command as GroupCommand;
            if (group == null)
                return;

            if (group._commands.Count != _commands.Count)
                return;

            for (int i = 0; i < _commands.Count; i++)
                _commands[i].Merge(group._commands[i]);
        }
    }
}
