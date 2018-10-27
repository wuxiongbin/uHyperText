using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SymbolLabel), true)]
    public class SymbolLabelEditor : SymbolTextEditor
    {
        protected SerializedProperty m_MaxElement;

        protected override void OnGUIFontData()
        {
            EditorGUILayout.PropertyField(m_FontData);
        }
        protected override void OnGUIOther()
        {
            if (m_MaxElement == null)
                m_MaxElement = serializedObject.FindProperty("m_MaxElement");

            EditorGUILayout.PropertyField(m_MaxElement);
        }
    }
}