using UnityEngine;
using BattleSimulator.AI;

namespace BattleSimulator.UI
{
    abstract class UINodeProperty : MonoBehaviour
    {
        [SerializeField] protected string _propertyName = null;

        public abstract void Read(AI.Node node);

        public abstract void Write(AI.Node node);

        protected NodeProperty GetProperty(Node node) => NodeInfo.Create(node).GetProperty(_propertyName);
    }
}
