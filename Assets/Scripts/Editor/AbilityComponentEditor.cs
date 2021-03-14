using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

using BattleSimulator.Abilities;

namespace BattleSimulator
{
    [CustomEditor(typeof(AbilityComponent), true)]
    class AbilityComponentEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel += 2;
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel -= 2;
        }
    }
}
