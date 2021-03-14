using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using BattleSimulator.Effects;

namespace BattleSimulator
{
    using IProvider = FilterWindow.IProvider;
    using Element = FilterWindow.Element;
    using GroupElement = FilterWindow.GroupElement;

    class EffectComponentProvider : IProvider
    {
        class EffectComponentElement : Element
        {
            public Type type;

            public EffectComponentElement(int level, string label, Type type)
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

        Effect m_Target;
        EffectEditor m_TargetEditor;

        public EffectComponentProvider(Effect target, EffectEditor targetEditor)
        {
            m_Target = target;
            m_TargetEditor = targetEditor;
        }

        public void CreateComponentTree(List<Element> tree)
        {
            tree.Add(new GroupElement(0, "Effects"));

            var types = TypeCache.GetTypesDerivedFrom<EffectComponent>();
            var rootNode = new PathNode();

            foreach (var t in types)
            {
                // Skip components that have already been added to the effect
                if (m_Target.Has(t))
                    continue;

                string path = string.Empty;

                // Look for a EffectComponentMenu attribute
                var attrs = t.GetCustomAttributes(false);

                bool skipComponent = false;
                foreach (var attr in attrs)
                {
                    var attrMenu = attr as EffectComponentMenuAttribute;
                    if (attrMenu != null)
                        path = attrMenu.path;

#if false
                    var attrDeprecated = attr as EffectComponentDeprecated;
                    if (attrDeprecated != null)
                        skipComponent = true;
#endif
                }

                if (skipComponent)
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
            if (element is EffectComponentElement)
            {
                var e = (EffectComponentElement)element;
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
                    tree.Add(new EffectComponentElement(depth, n.name, n.type));
                }
            }
        }
    }
}
