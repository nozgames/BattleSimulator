using System;
using UnityEngine;
using Unity.Mathematics;

namespace BattleSimulator
{
    public abstract class UnitActionPriority : MonoBehaviour
    {
        [SerializeField] private float _weight = 1.0f;

        [NonSerialized]
        public int target;

        [NonSerialized]
        public float priority;

        public abstract float CalculatePriority(Unit unit);
    }
}
