using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;

namespace WXB
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(SymbolText), true)]
    public class SymbolTextEditor : TextEditor
    {
        protected SerializedProperty m_Text;
        protected SerializedProperty m_FontData;
        protected SerializedProperty m_SegmentElement;
        protected SerializedProperty m_MinLineHeight;
        protected SerializedProperty m_isCheckFontY;
        protected SerializedProperty m_LineAlignment;
        protected SerializedProperty wordSpacing;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Text = serializedObject.FindProperty("m_Text");
            m_FontData = serializedObject.FindProperty("m_FontData");
            m_SegmentElement = serializedObject.FindProperty("m_SegmentElement");
            m_MinLineHeight = serializedObject.FindProperty("m_MinLineHeight");
            m_isCheckFontY = serializedObject.FindProperty("m_isCheckFontY");
            m_LineAlignment = serializedObject.FindProperty("m_LineAlignment");
            wordSpacing = serializedObject.FindProperty("wordSpacing");
        }

        protected virtual void OnGUIFontData()
        {
            EditorGUILayout.PropertyField(m_Text);
            EditorGUILayout.PropertyField(m_FontData);
        }

        protected virtual void OnGUIOther()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnGUIFontData();
            AppearanceControlsGUI();
            RaycastControlsGUI();

            EditorGUILayout.PropertyField(m_LineAlignment);

            // 元素分割类型
            {
                List<string> alles = new List<string>();
                alles.Add("Empty");
                alles.AddRange(ESFactory.GetAllName());
                int current = alles.IndexOf(m_SegmentElement.stringValue);
                if (current == -1)
                    current = 0;

                int[] optionValues = new int[alles.Count];
                for (int i = 0; i < optionValues.Length; ++i)
                    optionValues[i] = i;

                current = EditorGUILayout.IntPopup("Element Segment", current, alles.ToArray(), optionValues);
                if (current <= 0)
                    m_SegmentElement.stringValue = null;
                else
                    m_SegmentElement.stringValue = alles[current];
            }

            // 字间距+
            EditorGUILayout.PropertyField(wordSpacing);
            //if (wordSpacing.floatValue < 0)
            //    wordSpacing.floatValue = 0;

            // 最小行高
            EditorGUILayout.PropertyField(m_MinLineHeight);

            // 是否开启字体高度修正
            EditorGUILayout.PropertyField(m_isCheckFontY);

            OnGUIOther();

            if (serializedObject.ApplyModifiedProperties())
            {
                SymbolText st = target as SymbolText;
                st.SetAllDirty();
            }
        }
    }
}