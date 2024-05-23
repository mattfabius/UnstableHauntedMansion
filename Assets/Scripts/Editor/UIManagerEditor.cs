using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fabius
{
    [CustomEditor(typeof(UIManager))]
    public class UIManagerEditor : Editor
    {
        private SerializedProperty startScreen;
        private SerializedProperty currentScreenIndex;

        private int selectedScreenIndex = 0;
        string[] screenOptions;

        private void OnEnable()
        {
            startScreen = serializedObject.FindProperty("startScreen");
            selectedScreenIndex = startScreen.intValue;
            currentScreenIndex = serializedObject.FindProperty("currentScreenIndex");
        }
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                selectedScreenIndex = currentScreenIndex.intValue;
            }

            UIManager uiManager = (UIManager)serializedObject.targetObject;
            screenOptions = new string[uiManager.screens.Count];
            for (int i = 0; i < uiManager.screens.Count; i++)
            {
                screenOptions[i] = uiManager.screens[i].name;
            }

            EditorGUI.BeginChangeCheck();
            selectedScreenIndex = EditorGUILayout.Popup("Start Screen", selectedScreenIndex, screenOptions);
            if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
            {
                startScreen.intValue = selectedScreenIndex;
                serializedObject.ApplyModifiedProperties();
                uiManager.SetStartScreen();
            }

            base.OnInspectorGUI();
        }
    }
}