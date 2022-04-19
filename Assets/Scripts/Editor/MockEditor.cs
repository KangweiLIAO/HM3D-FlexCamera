using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubemapController))]
public class MockEditor : Editor {
    CubemapController control;
    void OnEnable() {
        control = target as CubemapController;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (control.testMode == true) {
            if (GUILayout.Button("Capture cubemap texture")) {
                control.CaptureCubemap();
            }
            if (GUILayout.Button("Capture panoramic image")) {
                control.CapturePanoramicImage();
            }
        }
    }
}
