using UnityEngine;

using BattleSimulator.Simulation;
using BattleSimulator.UI;
using System.IO;
using System;

namespace BattleSimulator
{
    public class GameSystem : MonoBehaviour
    {
        private static GameSystem _instance = null;
        [SerializeField] private GameObject _unitsPrefab = null;
        [SerializeField] private GameObject _units = null;
        [SerializeField] private ScriptableObjectDatabase _unitDatabase = null;

        public Material[] teamMaterials;

        private void Awake()
        {
            _instance = this;

            try
            {
                _graph = UIManager.LoadGraph(Path.Combine(Application.dataPath, "Resources", "AI", "Graphs", "test.aigraph"));
            } 
            catch 
            {
                _graph = UIManager.NewGraph(_unitDatabase.GetRecord<UnitDef>(0));
            }

            if (_units == null)
                _units = Instantiate(_unitsPrefab);
        }

        private void OnApplicationQuit()
        {
            _graph.ToGraph().Save(Path.Combine(Application.dataPath, "Resources", "AI", "Graphs", "test.aigraph"));
        }

        private UIGraph _graph;

        public static ScriptableObjectDatabase unitDatabase => _instance._unitDatabase;

        private void Update()
        {
            // Update all units
            if (!_graph.isActiveAndEnabled)
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    DestroyImmediate(_units);
                    _units = Instantiate(_unitsPrefab);
                    Unit.SetGraph(_graph.ToGraph());
                }

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    _graph.gameObject.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Unit.SetGraph(_graph.ToGraph());
                _graph.gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (!_graph.isActiveAndEnabled)
                Unit.UpdateAll();
        }
    }
}
