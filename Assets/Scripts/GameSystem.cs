using UnityEngine;

using BattleSimulator.AI;
using BattleSimulator.UI;
using System.IO;
using System;

namespace BattleSimulator
{
    public class GameSystem : MonoBehaviour
    {
        private GameSystem _instance = null;

        private void Awake()
        {
            _instance = this;

            try
            {
                _graph = UIManager.LoadGraph(Path.Combine(Application.dataPath, "AI", "Graphs", "test.aigraph"));
            } 
            catch 
            {
                _graph = UIManager.NewGraph();
            }
        }

        private void OnApplicationQuit()
        {
            _graph.ToGraph().Save(Path.Combine(Application.dataPath, "AI", "Graphs", "test.aigraph"));
        }

        private UIGraph _graph;

        private void Update()
        {
            // Update all units
            Unit.UpdateAll();
        }
    }
}
