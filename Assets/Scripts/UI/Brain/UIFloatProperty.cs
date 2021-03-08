using UnityEngine;

namespace BattleSimulator.UI
{
    class UIFloatProperty : UINodeProperty
    {
        [SerializeField] private TMPro.TMP_InputField _input = null;

        public override void Read(AI.Node node)
        {
            GetProperty(node)?.propertyInfo.SetValue(node, float.TryParse(_input.text, out var result) ? result : 0.0f);
        }

        public override void Write(AI.Node node)
        {
            _input.text = (GetProperty(node)?.propertyInfo.GetValue(node) ?? 0.0f).ToString();            
        }
    }
}
