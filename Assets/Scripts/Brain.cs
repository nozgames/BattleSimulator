using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if false

    Unit
        Actions

    Action
        Priority
        TargetFilters
        TargetPriorities

    PriorityCalculator
       

    Action Calculating Priority
        - Need Unit
        - Need Target (optional)

    Target calculating priority
        - Apply filters
        - Need Unit
        - Need Target

    Target Finder


#endif




namespace BattleSimulator
{
    [Serializable]
    class BrainPriority
    {
        
    }

    [Serializable]
    class BrainHealthPriority : BrainPriority
    {

    }

    [Serializable]
    class BrainSmilePriority : BrainPriority
    {

    }

    [Serializable]
    class BrainAction
    {
        [SerializeReference]
        [SelectImplementation(typeof(BrainPriority))]
        [SerializeField] private List<BrainPriority> _priorities;
    }

    [CreateAssetMenu(fileName = "New Brain", menuName = "BattleSimulator/Brain")]
    class Brain : ScriptableObject
    {                
        [SerializeField] private List<BrainAction> _actions;
    }

    public class SelectImplementationAttribute : PropertyAttribute
    {
        public Type FieldType;

        public SelectImplementationAttribute(Type fieldType)
        {
            FieldType = fieldType;
        }
    }

    [CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
    public class SelectImplementationDrawer : PropertyDrawer
    {
        private Type[] _implementations;
        private int _implementationTypeIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_implementations == null || GUILayout.Button("Refresh implementations"))
            {
                _implementations = GetImplementations((attribute as SelectImplementationAttribute).FieldType)
                    .Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
            }

            EditorGUILayout.LabelField($"Found {_implementations.Count()} implementations");

            _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
                _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());

            if (GUILayout.Button("Create instance"))
            {
                property.managedReferenceValue = Activator.CreateInstance(_implementations[_implementationTypeIndex]);
            }
            EditorGUILayout.PropertyField(property, true);
        }

        public static Type[] GetImplementations(Type interfaceType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
        }
    }
}
