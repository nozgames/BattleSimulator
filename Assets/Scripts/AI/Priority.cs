namespace BattleSimulator.AI
{
    public struct Priority
    {
        public static readonly Priority none = new Priority { value = 0.0f, weight = 0.0f };

        public float value;
        public float weight;

        public static bool operator >(Priority lhs, Priority rhs) => lhs.value > rhs.value;
        public static bool operator <(Priority lhs, Priority rhs) => lhs.value < rhs.value;
    }
}
