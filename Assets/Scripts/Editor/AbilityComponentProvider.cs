using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using BattleSimulator.Abilities;
using System.Reflection;

namespace BattleSimulator
{
    using IProvider = FilterWindow.IProvider;
    using Element = FilterWindow.Element;
    using GroupElement = FilterWindow.GroupElement;

    class AbilityComponentProvider : IProvider
    {
        class AbilityComponentElement : Element
        {
            public Type type;

            public AbilityComponentElement(int level, string label, Type type)
            {
                this.level = level;
                this.type = type;
                // TODO: Add support for custom icons
                content = new GUIContent(label);
            }
        }

        class PathNode : IComparable<PathNode>
        {
            public List<PathNode> nodes = new List<PathNode>();
            public string name;
            public Type type;

            public int CompareTo(PathNode other)
            {
                return name.CompareTo(other.name);
            }
        }

        public Vector2 position { get; set; }

        Ability m_Target;
        AbilityEditor m_TargetEditor;

        public AbilityComponentProvider(Ability target, AbilityEditor targetEditor)
        {
            m_Target = target;
            m_TargetEditor = targetEditor;
        }

        public void CreateComponentTree(List<Element> tree)
        {
            tree.Add(new GroupElement(0, "Abilities"));

            var types = TypeCache.GetTypesDerivedFrom<AbilityComponent>();
            var rootNode = new PathNode();

            foreach (var t in types)
            {
                if (t.IsAbstract)
                    continue;

                string path = string.Empty;

                // Look for a AbilityComponentMenu attribute
                var attrs = t.GetCustomAttributes(false);

                bool skipComponent = false;
                foreach (var attr in attrs)
                {
                    var attrMenu = attr as AbilityComponentMenuAttribute;
                    if (attrMenu != null)
                        path = attrMenu.path;
                }

                var usage = t.GetCustomAttribute<AbilityComponentUsageAttribute>(true);

                if (skipComponent)
                    continue;

                // Skip components that have already been added to the ability
                if (!(usage?.allowMultiple??false) && m_Target.Has(t))
                    continue;

                // If no attribute or in case something went wrong when grabbing it, fallback to a
                // beautified class name
                if (string.IsNullOrEmpty(path))
                    path = ObjectNames.NicifyVariableName(t.Name);

                // Prep the categories & types tree
                AddNode(rootNode, path, t);
            }

            // Recursively add all elements to the tree
            Traverse(rootNode, 1, tree);
        }

        public bool GoToChild(Element element, bool addIfComponent)
        {
            if (element is AbilityComponentElement)
            {
                var e = (AbilityComponentElement)element;
                m_TargetEditor.AddComponent(e.type);
                return true;
            }

            return false;
        }

        void AddNode(PathNode root, string path, Type type)
        {
            var current = root;
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var child = current.nodes.Find(x => x.name == part);

                if (child == null)
                {
                    child = new PathNode { name = part, type = type };
                    current.nodes.Add(child);
                }

                current = child;
            }
        }

        void Traverse(PathNode node, int depth, List<Element> tree)
        {
            node.nodes.Sort();

            foreach (var n in node.nodes)
            {
                if (n.nodes.Count > 0) // Group
                {
                    tree.Add(new GroupElement(depth, n.name));
                    Traverse(n, depth + 1, tree);
                } else // Element
                {
                    tree.Add(new AbilityComponentElement(depth, n.name, n.type));
                }
            }
        }
    }
}
