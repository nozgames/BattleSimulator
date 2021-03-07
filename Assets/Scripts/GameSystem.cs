﻿using UnityEngine;

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

#if false            

            var action = new ActionNode("test");
            var distance = new DistanceNode();
            var distanceToPriority = new FloatToPriority();
            distanceToPriority.min.Write(10);
            distanceToPriority.max.Write(0);
            distanceToPriority.input.ConnectTo(distance.output);
            distanceToPriority.weight.Write(1);
            action.priorityPort.ConnectTo(distanceToPriority.output);

            // TODO: need float to priority node

            var graph = new Graph();
            graph.AddNode(action);
            graph.AddNode(distanceToPriority);
            graph.AddNode(distance);

            var unit = new AI.Target();
            var context = new Context(unit);
            // TODO: need to pass all units into the context
            // TODO: contexts can be reused or be a struct
            graph.Execute(context);
#endif            

            try
            {
                _graph = UIManager.LoadGraph(Path.Combine(Application.dataPath, "AI", "Graphs", "test.aigraph"));
            } 
            catch (Exception e)
            {
                _graph = UIManager.NewGraph();
            }
        }

        private void OnDisable()
        {
            _graph.graph.Save(Path.Combine(Application.dataPath, "AI", "Graphs", "test.aigraph"));
        }

        private UIGraph _graph;

        private void Update()
        {
            // Update all units
            Unit.UpdateAll();
        }
    }
}
