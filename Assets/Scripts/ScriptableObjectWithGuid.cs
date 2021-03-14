using UnityEngine;

namespace BattleSimulator
{
    public class ScriptableObjectWithGuid : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private string _guid = null;

        public System.Guid guid { get; private set; }

        public void OnAfterDeserialize()
        {
            if (System.Guid.TryParse(_guid, out var parsed))
                guid = parsed;
        }

        public void OnBeforeSerialize()
        {
            if (string.IsNullOrEmpty(_guid))
            {
                guid = System.Guid.NewGuid();
                _guid = guid.ToString();
            }                
        }
    }
}
