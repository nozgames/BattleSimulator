using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Rendering;
using System;

using BattleSimulator.Abilities;

namespace BattleSimulator
{
    [CustomEditor(typeof(Ability), true)]
    class AbilityEditor : Editor
    {
        class Styles
        {
            public static readonly GUIContent Components =
                new GUIContent("Components", "Components to include in this ability.");

            public static readonly GUIContent MissingFeature = new GUIContent("Missing Component",
                "Missing component, due to compilation issues or missing files. you can attempt auto fix or choose to remove the component.");

            public static GUIStyle BoldLabelSimple;

            static Styles()
            {
                BoldLabelSimple = new GUIStyle(EditorStyles.label);
                BoldLabelSimple.fontStyle = FontStyle.Bold;
            }
        }

        private SerializedProperty m_components;
        private SerializedProperty m_FalseBool;
        private Ability _ability;
        List<Editor> m_Editors = new List<Editor>();

        private void OnEnable()
        {
            _ability = serializedObject.targetObject as Ability;
            m_components = serializedObject.FindProperty("_components");
            UpdateEditorList();
        }

        private void OnDisable()
        {
            ClearEditorsList();
        }

        public override void OnInspectorGUI()
        {
            if (m_components == null)
                OnEnable();
            else if (m_components.arraySize != m_Editors.Count)
                UpdateEditorList();

            GUI.enabled = false;
            EditorGUILayout.TextField("GUID", _ability.guid.ToString());
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            serializedObject.Update();
            DrawComponentList();
        }

        private void DrawComponentList()
        {
            EditorGUILayout.LabelField(Styles.Components, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (m_components.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No Ability Components added", MessageType.Info);
            } else
            {
                var indent = EditorGUI.indentLevel;
                var wait = false;

                //Draw List
                CoreEditorUtils.DrawSplitter();
                for (int i = 0; i < m_components.arraySize; i++)
                {
                    SerializedProperty renderFeaturesProperty = m_components.GetArrayElementAtIndex(i);

                    if (renderFeaturesProperty.objectReferenceValue is WaitComponent waitComponent && waitComponent.isActive)
                    {
                        wait = true;
                        EditorGUI.indentLevel = indent;
                    } else if (wait)
                    {
                        EditorGUI.indentLevel = indent + 1;
                    }


                    DrawComponent(i, ref renderFeaturesProperty);
                    CoreEditorUtils.DrawSplitter();
                }
            }
            EditorGUILayout.Space();

            using (var hscope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(EditorGUIUtility.TrTextContent("Add Component"), EditorStyles.miniButton))
                {
                    var r = hscope.rect;
                    var pos = new Vector2(r.x + r.width / 2f, r.yMax + 18f);
                    FilterWindow.Show(pos, new AbilityComponentProvider(_ability, this));
                }
            }
        }

        private void DrawComponent(int index, ref SerializedProperty componentProperty)
        {
            var effectComponentObjectRef = componentProperty.objectReferenceValue;
            if (null == effectComponentObjectRef)
            {
                CoreEditorUtils.DrawHeaderToggle(Styles.MissingFeature, componentProperty, m_FalseBool, pos => OnContextClick(pos, index));
                m_FalseBool.boolValue = false; // always make sure false bool is false
                EditorGUILayout.HelpBox(Styles.MissingFeature.tooltip, MessageType.Error);
                return;
            }

            bool hasChangedProperties = false;
            string title = effectComponentObjectRef.GetType().Name;
            //ObjectNames.GetInspectorTitle(effectComponentObjectRef);

            // Get the serialized object for the editor script & update it
            Editor rendererFeatureEditor = m_Editors[index];
            SerializedObject serializedComponentEditor = rendererFeatureEditor.serializedObject;
            serializedComponentEditor.Update();

            // Foldout header
            EditorGUI.BeginChangeCheck();
            SerializedProperty activeProperty = serializedComponentEditor.FindProperty("_active");
            bool displayContent = CoreEditorUtils.DrawHeaderToggle(title, componentProperty, activeProperty, pos => OnContextClick(pos, index));
            hasChangedProperties |= EditorGUI.EndChangeCheck();

            // ObjectEditor
            if (displayContent)
            {
#if false
                //EditorGUI.BeginChangeCheck();
                //SerializedProperty nameProperty = serializedComponentEditor.FindProperty("_name");
                //nameProperty.stringValue = ValidateName(EditorGUILayout.DelayedTextField(Styles.PassNameField, nameProperty.stringValue));
                //EditorGUILayout.TextField(Styles.ComponentNameField, )
                if (EditorGUI.EndChangeCheck())
                {
                    hasChangedProperties = true;

                    // We need to update sub-asset name
                    rendererFeatureObjRef.name = nameProperty.stringValue;
                    AssetDatabase.SaveAssets();

                    // Triggers update for sub-asset name change
                    ProjectWindowUtil.ShowCreatedAsset(target);
                }
#endif

                EditorGUI.BeginChangeCheck();
                rendererFeatureEditor.OnInspectorGUI();
                hasChangedProperties |= EditorGUI.EndChangeCheck();

                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            }

            // Apply changes and save if the user has modified any settings
            if (hasChangedProperties)
            {
                serializedComponentEditor.ApplyModifiedProperties();
                serializedObject.ApplyModifiedProperties();
                ForceSave();
            }
        }

        private void OnContextClick(Vector2 position, int id)
        {
            var menu = new GenericMenu();

            if (id == 0)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveComponent(id, -1));

            if (id == m_components.arraySize - 1)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveComponent(id, 1));

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveComponent(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        public void AddComponent(Type type)
        {
            serializedObject.Update();

            ScriptableObject component = CreateInstance(type);
            component.name = type.Name;
            Undo.RegisterCreatedObjectUndo(component, "Add Ability Component");

            // Store this new effect as a sub-asset so we can reference it safely afterwards
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(target))
                AssetDatabase.AddObjectToAsset(component, target);

            // Grow the list first, then add - that's how serialized lists work in Unity
            m_components.arraySize++;
            SerializedProperty componentProp = m_components.GetArrayElementAtIndex(m_components.arraySize - 1);
            componentProp.objectReferenceValue = component;

            UpdateEditorList();
            serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            if (EditorUtility.IsPersistent(target))
                ForceSave();

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveComponent(int id)
        {
            var property = m_components.GetArrayElementAtIndex(id);
            var component = property.objectReferenceValue;
            property.objectReferenceValue = null;

            Undo.SetCurrentGroupName(component == null ? "Remove Component" : $"Remove {component.name}");

            // remove the array index itself from the list
            m_components.DeleteArrayElementAtIndex(id);
            UpdateEditorList();
            serializedObject.ApplyModifiedProperties();

            // Make sure the component is removed from the asset
            if (EditorUtility.IsPersistent(target))
                AssetDatabase.RemoveObjectFromAsset(component);

            // Destroy the setting object after ApplyModifiedProperties(). If we do it before, redo
            // actions will be in the wrong order and the reference to the setting object in the
            // list will be lost.
            if (component != null)
                Undo.DestroyObjectImmediate(component);

            // Force save / refresh
            ForceSave();
        }

        private void MoveComponent(int id, int offset)
        {
            Undo.SetCurrentGroupName("Move Component");
            serializedObject.Update();
            m_components.MoveArrayElement(id, id + offset);
            UpdateEditorList();
            serializedObject.ApplyModifiedProperties();

            ForceSave();
        }

        private void UpdateEditorList()
        {
            ClearEditorsList();
            for (int i = 0; i < m_components.arraySize; i++)
                m_Editors.Add(CreateEditor(m_components.GetArrayElementAtIndex(i).objectReferenceValue));
        }

        /// <summary>
        /// Clear the editors list by destroying all of the editor
        /// </summary>
        private void ClearEditorsList()
        {
            for (int i = m_Editors.Count - 1; i >= 0; --i)
                DestroyImmediate(m_Editors[i]);

            m_Editors.Clear();
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(target);
        }
    }
}
