namespace BattleSimulator.AI
{
    public class FloatPort : Port
    {
        private float _value;

        public FloatPort (Node node, PortFlow flow, float value = 0.0f) : base(node,flow)
        {
            _value = value;
        }

        public void Write(float value) => _value = value;

        protected override float ReadFloat () => _value;

        protected override bool ReadBoolean() => _value != 0.0f;
    }
}
