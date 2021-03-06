namespace BattleSimulator.UI
{
    internal abstract class Command
    {
        public bool isExecuted { get; private set; }

        public UINode[] selection { get; set; }

        public void Execute ()
        {
            isExecuted = true;
            OnExecute();
        }

        public void Undo()
        {
            isExecuted = false;
            OnUndo();
        }

        public void Redo()
        {
            isExecuted = true;
            OnRedo();
        }

        public virtual bool CanMerge(Command command) => false;

        public virtual void Merge(Command command) { }

        public abstract void OnExecute();

        public abstract void OnRedo();

        public abstract void OnUndo();

        public virtual void Dispose ()
        {

        }
    }
}
