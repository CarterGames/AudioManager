using System;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to aid with editor scripting where the API is really wordy...
    /// </summary>
    public static class SerializedPropertyHelper
    {
        /// <summary>
        /// Calls InsertArrayElementAtIndex()
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="index">The index.</param>
        public static void InsertIndex(this SerializedProperty property, int index)
        {
            property.InsertArrayElementAtIndex(index);
        }
        
        
        /// <summary>
        /// Calls DeleteArrayElementAtIndex()
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="index">The index.</param>
        public static void DeleteIndex(this SerializedProperty property, int index)
        {
            property.DeleteArrayElementAtIndex(index);
        }
        
        
        /// <summary>
        /// Calls GetArrayElementAtIndex()
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="index">The index.</param>
        /// <returns>The property at the index entered.</returns>
        public static SerializedProperty GetIndex(this SerializedProperty property, int index)
        {
            return property.GetArrayElementAtIndex(index);
        }
        
        
        /// <summary>
        /// Calls FindProperty()
        /// </summary>
        /// <param name="serializedObject">The target object.</param>
        /// <param name="propName">The name of the property.</param>
        /// <returns>The found property.</returns>
        public static SerializedProperty Fp(this SerializedObject serializedObject, string propName)
        {
            return serializedObject.FindProperty(propName);
        }
        
        
        /// <summary>
        /// Calls FindPropertyRelative()
        /// </summary>
        /// <param name="property">The target property.</param>
        /// <param name="propName">The name of the property.</param>
        /// <returns>The found property.</returns>
        public static SerializedProperty Fpr(this SerializedProperty property, string propName)
        {
            return property.FindPropertyRelative(propName);
        }
    }
}