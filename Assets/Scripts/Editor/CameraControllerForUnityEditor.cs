using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraControllerForUnity))]
public class CameraControllerForUnityEditor : Editor
{
    SerializedProperty mode, thirdLimitAngleY, firstLimitAngleY, mobaLimitDistance;
    SerializedProperty offset, cameraRot, cameraMove, cameraZoom, enableUIInput, roOrMove;
    SerializedProperty autoRotate, orthographicSizeLimit, xAngle, yAngle, targetorthographicSize;
    SerializedProperty targetdis, fovlimit, pc_mouseRot, followtarget;
    private SerializedProperty reversalXY;
    GUIStyle toggleStyle;
    enum Hand
    {
        left, right
    }
    void OnEnable()
    {
        mode = serializedObject.FindProperty("mode");
        thirdLimitAngleY = serializedObject.FindProperty("thirdLimitAngleY");
        firstLimitAngleY = serializedObject.FindProperty("firstLimitAngleY");
        mobaLimitDistance = serializedObject.FindProperty("mobaLimitDistance");
        offset = serializedObject.FindProperty("offset");
        targetdis = serializedObject.FindProperty("targetdis");
        fovlimit = serializedObject.FindProperty("fovlimit");
        pc_mouseRot = serializedObject.FindProperty("pc_mouseRot");
        reversalXY = serializedObject.FindProperty("reversalXY");
        cameraRot = serializedObject.FindProperty("cameraRot");
        cameraMove = serializedObject.FindProperty("cameraMove");
        cameraZoom = serializedObject.FindProperty("cameraZoom");
        enableUIInput = serializedObject.FindProperty("enableUIInput");
        roOrMove = serializedObject.FindProperty("roOrMove");
        autoRotate = serializedObject.FindProperty("autoRotate");
        orthographicSizeLimit = serializedObject.FindProperty("orthographicSizeLimit");
        xAngle = serializedObject.FindProperty("xAngle");
        yAngle = serializedObject.FindProperty("yAngle");
        targetorthographicSize = serializedObject.FindProperty("targetorthographicSize");
        followtarget = serializedObject.FindProperty("followtarget");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        EditorGUI.EndDisabledGroup();
        if (toggleStyle == null)
            toggleStyle = new GUIStyle("Button");
        GUILayoutOption[] gUILayoutOption = new GUILayoutOption[] { GUILayout.MaxHeight(24), GUILayout.Width(42) };
        EditorGUILayout.BeginHorizontal();
        string tip = cameraMove.boolValue ? "允许移动" : "不允许移动";
        string icon = cameraMove.boolValue ? "d_MoveTool On" : "d_MoveTool";
        cameraMove.boolValue = GUILayout.Toggle(cameraMove.boolValue,
            EditorGUIUtility.TrIconContent(icon, tip),
            toggleStyle,
            gUILayoutOption);
        tip = cameraRot.boolValue ? "允许旋转" : "不允许旋转";
        icon = cameraRot.boolValue ? "d_RotateTool On" : "d_RotateTool";
        cameraRot.boolValue = GUILayout.Toggle(cameraRot.boolValue,
            EditorGUIUtility.TrIconContent(icon, tip),
            toggleStyle,
            gUILayoutOption);
        tip = cameraZoom.boolValue ? "允许拉伸" : "不允许拉伸";
        icon = cameraZoom.boolValue ? "d_ScaleTool On" : "d_ScaleTool";
        cameraZoom.boolValue = GUILayout.Toggle(cameraZoom.boolValue,
            EditorGUIUtility.TrIconContent("d_ScaleTool", tip),
            toggleStyle,
            gUILayoutOption);
        tip = enableUIInput.boolValue ? "击穿UI控制相机" : "启用UI遮挡";
        icon = enableUIInput.boolValue ? "d_scenepicking_pickable_hover" : "d_scenepicking_notpickable";
        enableUIInput.boolValue = GUILayout.Toggle(enableUIInput.boolValue,
            EditorGUIUtility.TrIconContent(icon, tip),
            toggleStyle,
            gUILayoutOption);
        tip = autoRotate.boolValue ? "自动回正" : "不启用回正";
        icon = autoRotate.boolValue ? "IN LockButton on@2x" : "IN LockButton@2x";
        autoRotate.boolValue = GUILayout.Toggle(autoRotate.boolValue,
            EditorGUIUtility.TrIconContent(icon, tip),
            toggleStyle,
            gUILayoutOption);
        EditorGUILayout.EndHorizontal();

        using (new GUILayout.VerticalScope("GroupBox"))
        {
            EditorGUILayout.PropertyField(mode, new GUIContent("模式"));
            if (mode.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(thirdLimitAngleY, new GUIContent("限制俯仰角度"));
                EditorGUILayout.PropertyField(mobaLimitDistance, new GUIContent("限制拉伸"));
                using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
                    EditorGUILayout.PropertyField(targetdis, new GUIContent("当前距焦点"));
            }
            else if (mode.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(firstLimitAngleY, new GUIContent("限制俯仰角度"));
                EditorGUILayout.PropertyField(fovlimit, new GUIContent("限制光圈"));
            }
            else if (mode.enumValueIndex == 2)
            {
                EditorGUILayout.PropertyField(mobaLimitDistance, new GUIContent("限制拉伸"));
                using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
                    EditorGUILayout.PropertyField(targetdis, new GUIContent("当前距焦点"));
            }
            EditorGUILayout.PropertyField(reversalXY, new GUIContent("翻转鼠标XY"));
        }
        using (new GUILayout.VerticalScope("GroupBox"))
        {
            EditorGUILayout.HelpBox("在触屏上，通过切换roOrMove实现单指旋转/移动场景。在PC上通常为true，因为鼠标左右键可以区分两个功能，当然，想要移动/旋转全交给一个按键也可以使用这个变量", MessageType.Info);

            EditorGUILayout.PropertyField(roOrMove, new GUIContent("启用旋转而非移动"));
            pc_mouseRot.intValue = (int)(Hand)EditorGUILayout.EnumPopup("PC端旋转", (Hand)pc_mouseRot.intValue);
            EditorGUILayout.LabelField("PC端平移: 中键");
        }

        using (new GUILayout.VerticalScope("GroupBox"))
        {
            EditorGUILayout.PropertyField(orthographicSizeLimit, new GUIContent("正交限制"));
            using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
            {
                EditorGUILayout.PropertyField(targetorthographicSize, new GUIContent("当前"));
            }
        }
        using (new GUILayout.VerticalScope("GroupBox"))
        {
            using (new EditorGUI.DisabledGroupScope(!Application.isPlaying))
            {
                EditorGUILayout.PropertyField(followtarget, new GUIContent("跟追对象"));
                EditorGUILayout.PropertyField(offset, new GUIContent("偏移"));
                using (var c = new EditorGUI.ChangeCheckScope())
                {
                    var v2 = EditorGUILayout.Vector2Field("当前角度", new Vector2(yAngle.floatValue, xAngle.floatValue));
                    if (c.changed)
                    {
                        yAngle.floatValue = v2.x;
                        xAngle.floatValue = v2.y;
                    }
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}