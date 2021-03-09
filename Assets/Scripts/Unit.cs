using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace BattleSimulator
{
    public struct UnitState
    {
        public float health;
        public int target;
        public int team;
        public float3 position;
        public quaternion rotation;
    }

    public class Unit : Target 
    {
        [SerializeField] private float _health = 100.0f;
        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private float _size = 1.0f;
        [SerializeField] private int _team = 0;

        private AI.Graph _graph;

        private static List<Unit> _units = new List<Unit>();

        private UnitAction[] _actions;

        private int _index;


        public int Team => _team;

        public Target Target { get; set; }

        public float size => _size;

        public Vector3 avoidance { get; set; }

        public float avoidanceWeight { get; set; }

        public float NormalizedHealth => 1.0f;

        public UnitState State {
            get => new UnitState {
                health = 0,
                team = _team,
                target = -1,
                position = transform.position,
                rotation = transform.rotation
            };
            set {
           
                transform.position = value.position;
                transform.rotation = value.rotation;
            }
        }

        public bool HasTarget => Target != null;

        private void Awake()
        {
            // Find all actions this unit is capable of
            _actions = GetComponentsInChildren<UnitAction>();

            //_graph = new AI.Graph();
            //_graph.Load(System.IO.Path.Combine(Application.dataPath, "AI", "Graphs", "test.aigraph"));
        }

        private void OnEnable()
        {
            _index = _units.Count;
            _units.Add(this);
        }

        private void OnDisable()
        {
            if (_index == _units.Count - 1)
                _units.RemoveAt(_index);
            else
            {
                _units[_index] = _units[_units.Count - 1];
                _units[_index]._index = _index;
                _units.RemoveAt(_units.Count - 1);
            }

            _index = -1;
        }

        /// <summary>
        /// Returns the direction to the current target
        /// </summary>
        /// <returns>Normalized direction</returns>
        public Vector3 DirectionToTarget() => DirectionToTarget(Target);

        /// <summary>
        /// Returns the direction to the given target
        /// </summary>
        /// <param name="target">Target</param>
        /// <returns>Normalized direction</returns>
        public Vector3 DirectionToTarget(Target target) => (target.transform.position - transform.position).normalized;

        public static void SetGraph (AI.Graph graph)
        {
            foreach (var unit in _units)
                unit._graph = graph;
        }

        public static void UpdateAll ()
        {
            // Copy the units into a states array
            var unitStates = new NativeArray<UnitState>(_units.Count, Allocator.Temp);
  //          for (int i = 0; i < _units.Count; i++)
    //            unitStates[i] = _units[i].State;

            var avoidanceSystem = new AvoidanceSystem();
            avoidanceSystem.OnUpdate();

            var aiunits = new AI.Target[_units.Count];
            for(int i=0; i<_units.Count; i++)
            {
                aiunits[i] = new AI.Target();
                aiunits[i].health = _units[i]._health;
                aiunits[i].maxHealth = _units[i]._health;
                aiunits[i].position = _units[i].transform.position;
                aiunits[i].team = _units[i].Team;
            }

            // All the unit brains can think in parallel as brain thinking should 
            // not change any unit data
            for (int i = 0; i < _units.Count; i++)
            {
                var unit = _units[i];
                var aicontext = new AI.Context(aiunits[i], aiunits);
                var aitarget = unit._graph.Execute(aicontext);

                unit.Target = null;
                if (aitarget != null)
                {
                    for(int j=0; j<aiunits.Length; j++)
                    {
                        if(aiunits[j] == aitarget)
                        {
                            unit.Target = _units[j];
                            break;
                        }
                    }
                }

                if (unit.Target == null)
                    continue;

                var dir = unit.avoidance;
                var enemy = unit.Target;
                if (enemy.DistanceTo(unit) <= ((Unit)enemy).size + unit.size)
                    dir = Vector3.zero;

                unit.transform.rotation = Quaternion.LookRotation(unit.DirectionToTarget(enemy), Vector3.up);

                dir.Normalize();

                unit.transform.position += dir * unit._speed * Time.deltaTime;

#if false
                var enemy = FindClosestUnit(unit.transform.position, unit.FilterEnemy);
                unit.Target = enemy;
                var dir = unit.avoidance;
                if (enemy.DistanceTo(unit) <= enemy.size + unit.size)
                    //dir += unit.DirectionToTarget(enemy) * (1.0f - unit.avoidanceWeight);
                //else if (enemy.DistanceTo(unit) > (enemy.size + unit.size) * 0.99f)
                    dir = Vector3.zero;

                unit.transform.rotation = Quaternion.LookRotation(unit.DirectionToTarget(enemy), Vector3.up);

                dir.Normalize();

                unit.transform.position += dir * unit._speed * Time.deltaTime;
#endif
            }

            foreach (var unit in _units)
            {
                // TODO: figure out the active action

//                foreach (var action in unit._actions)
  //                  action.Perform(Time.deltaTime);
            }

            // Copy the states back into the units
            //for (int i = 0; i < _units.Count; i++)
//                _units[i].State = unitStates[i];

            unitStates.Dispose();
        }

        private void Think()
        {
            // Update all action priorities
            foreach (var action in _actions)
                action.CalculatePriority();

            var bestAction = (UnitAction)null;
            var bestActionPriority = 0.0f;
            foreach (var action in _actions)
            {
                if(action.priority > bestActionPriority)
                {
                    bestActionPriority = action.priority;
                    bestAction = action;
                }
            }

            if (null == bestAction)
                return;

            if(!bestAction.isInRange)
            {
                // TODO: retreat if too close
                // TODO: move toward if too far
            }

            bestAction.Perform(Time.deltaTime);
        }

        public static Unit[] GetUnits() => _units.ToArray();

        /// <summary>
        /// Returns all units within the given radius
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="filter"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int GetUnitsInRadius(Vector3 position, float radius, Func<Unit, bool> filter, List<Unit> result)
        {
            result.Clear();
            foreach (var unit in _units)
            {
                if (!filter(unit))
                    continue;

                var unitDistance = unit.DistanceTo(position);
                if (unitDistance <= radius)
                    result.Add(unit);
            }

            return result.Count;
        }

        /// <summary>
        /// Find the closest unit to the given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public static Unit FindClosestUnit(Vector3 position, Func<Unit, bool> filter)
        {
            var bestDistance = float.MaxValue;
            var bestUnit = (Unit)null;

            foreach (var unit in _units)
            {
                if (!filter(unit))
                    continue;

                var unitDistance = unit.DistanceTo(position);
                if (unitDistance < bestDistance)
                {
                    bestDistance = unitDistance;
                    bestUnit = unit;
                }
            }

            return bestUnit;
        }

        public bool FilterTeammate (Unit filter)
        {
            return filter.Team == Team;
        }

        public bool FilterEnemy (Unit filter)
        {
            return filter.Team != Team;
        }
    }
}
