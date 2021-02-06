using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

#endif

[Serializable]
public class UpdaterRef
{
    public string className;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UpdaterRef))]
    class UpdaterRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, new GUIContent("Building Updater"), property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var rect = new Rect(position.x, position.y, position.width, 20);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("className"), new GUIContent(""));
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }
    }
#endif
}
