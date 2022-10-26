using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A copy of the Scarlet Library MinMaxFloatDrawer for the MinMaxFloat field used in the asset.
    /// </summary>
    [CustomPropertyDrawer(typeof(MinMaxFloat))]
    public class MinMaxFloatDrawer : PropertyDrawer
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Property Drawer Method
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            var elementOne = property.FindPropertyRelative("min");
            var elementTwo = property.FindPropertyRelative("max");
            
            EditorGUI.BeginChangeCheck();

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var left = new Rect(position.x, position.y, (position.width / 2) - 1.5f, EditorGUIUtility.singleLineHeight);
            var right = new Rect(position.x + (position.width / 2) + 1.5f, position.y, (position.width / 2) - 1.5f, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(left, elementOne, GUIContent.none);
            EditorGUI.PropertyField(right, elementTwo, GUIContent.none);
            
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();            
            
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}