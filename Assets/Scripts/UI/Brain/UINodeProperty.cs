using UnityEngine;
using BattleSimulator.Simulation;

namespace BattleSimulator.UI
{
    abstract class UINodeProperty : MonoBehaviour
    {
        [SerializeField] protected string _propertyName = null;

        public abstract void Read(Node node);

        public abstract void Write(Node node);

        protected NodeProperty GetProperty(Node node) => NodeInfo.Create(node).GetProperty(_propertyName);
    }
}
