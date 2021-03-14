using UnityEngine;
using BattleSimulator.Simulation;

namespace BattleSimulator.UI
{
    class UIFloatProperty : UINodeProperty
    {
        [SerializeField] private TMPro.TMP_InputField _input = null;

        public override void Read(Node node)
        {
            GetProperty(node)?.propertyInfo.SetValue(node, float.TryParse(_input.text, out var result) ? result : 0.0f);
        }

        public override void Write(Node node)
        {
            _input.text = (GetProperty(node)?.propertyInfo.GetValue(node) ?? 0.0f).ToString();            
        }
    }
}
