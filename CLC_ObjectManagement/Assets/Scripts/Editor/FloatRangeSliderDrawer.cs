using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position, SerializedProperty property, GUIContent label)
    {
        // 별도의 indent는 없음
        int originalIndentLevel = EditorGUI.indentLevel;

        EditorGUI.BeginProperty(position, label, property);

        //
        position = EditorGUI.PrefixLabel(
            position, GUIUtility.GetControlID(FocusType.Passive), label
            );
        EditorGUI.indentLevel = 0;

        SerializedProperty minProperty = property.FindPropertyRelative("min");
        SerializedProperty maxProperty = property.FindPropertyRelative("max");
        float minValue = minProperty.floatValue;
        float maxValue = maxProperty.floatValue;

        // 새로운 slider의 너비 관련 설정
        float fieldWidth = position.width / 4f - 4f;    // float 영역 넓이
        float sliderWidth = position.width / 2f;        // slider 영역 넓이(길이)

        // slider의 최소값이 표시될 자리
        position.width = fieldWidth;
        minValue = EditorGUI.FloatField(position, minValue);
        position.x += fieldWidth + 4f; // 커서를 field 만큼 이동, 그리고 빈칸 4f 만큼 이동
        position.width = sliderWidth; // 슬라이드 입력을 위해 너비 변경

        // attribute에 저장해둔 slider의 limit을 가져옴
        FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        // float 값들을 이용해서 slider 오브젝트를 생성
        EditorGUI.MinMaxSlider(
            position, ref minValue, ref maxValue, limit.Min, limit.Max
            );

        // slider의 최대값이 표시될 자리
        position.x += sliderWidth + 4f;
        position.width = fieldWidth;
        maxValue = EditorGUI.FloatField(position, maxValue);

        // 직접 입력으로 인해 min이 max를 넘어서는 현상을 막기 위함
        if(minValue < limit.Min)
        {
            minValue = limit.Min;
        }
        else if (minValue > limit.Max)
        {
            minValue = limit.Max;
        }
        if(maxValue > limit.Max)
        {
            maxValue = limit.Max;
        }
        else if(maxValue < minValue)
        {
            maxValue = minValue;
        }
        

        minProperty.floatValue = minValue;
        maxProperty.floatValue = maxValue;

        EditorGUI.EndProperty();

        EditorGUI.indentLevel = originalIndentLevel;
    }
}
