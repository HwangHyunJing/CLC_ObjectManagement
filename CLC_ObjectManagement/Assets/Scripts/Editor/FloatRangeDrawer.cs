using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// 어떤 형으로 사용할지를 명시
[CustomPropertyDrawer(typeof(SpawnZone.FloatRange))]

public class FloatRangeDrawer : PropertyDrawer
{
    // 말 그대로, inspector 상에서 뭔가를 그리는 과정
    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label)
    {
        // 기본 값을 미리 저장
        int originalIndentLevel = EditorGUI.indentLevel;
        float originalLabelWidth = EditorGUIUtility.labelWidth;

        EditorGUI.BeginProperty(position, label, property);
        // 관련이 있는 요소를 찾도록 함

        // label 성분을 앞으로 가져온 뒤, 성분과 라벨을 분리
        position = EditorGUI.PrefixLabel(
            position, GUIUtility.GetControlID(FocusType.Passive), label);
        // GUI 상에서 보이는 칸의 위치를 조정 (길이를 조정했다고 해야 하나)
        position.width = position.width / 2f;
        // 라벨 성분의 길이만 별도로 조정
        EditorGUIUtility.labelWidth = position.width / 2f;
        // 높이 성분에서 들여쓰기를 함. 줄 간격 벌리는 느낌
        EditorGUI.indentLevel = 1;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("min"));
        // 다음 칸으로 이동
        position.x += position.width;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("max"));
        EditorGUI.EndProperty();

        // 모든 수정이 끝나면 GUI상에 기존의 값을 돌려줌. 해당 코드가 없어도 자동으로 수행되기는 한다
        EditorGUI.indentLevel = originalIndentLevel;
        EditorGUIUtility.labelWidth = originalLabelWidth;
    }
}
