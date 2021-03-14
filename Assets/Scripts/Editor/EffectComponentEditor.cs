using UnityEditor;

using BattleSimulator.Effects;

namespace BattleSimulator
{
    [CustomEditor(typeof(EffectComponent), true)]
    class EffectComponentEditor : Editor
    {
        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

        public override void OnInspectorGUI()
        {
            EditorGUI.indentLevel+=2;
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            serializedObject.ApplyModifiedProperties();
            EditorGUI.indentLevel -= 2;
            ;
        }
    }
}
