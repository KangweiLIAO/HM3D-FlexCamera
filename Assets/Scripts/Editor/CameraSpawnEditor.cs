using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {

    CameraSpawnController controller;

    private void OnEnable() {
        controller = (CameraSpawnController)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Combine meshes")) {
            controller.Combine();
        }
        EditorGUILayout.Space();
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter the play mode to spawn debug points or cameras", MessageType.Warning);
            EditorGUILayout.Space();
        } else {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max camera count:");
            controller.setMaxCameraSpawn = EditorGUILayout.IntField(controller.setMaxCameraSpawn);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (controller.startDebugging) {
                if (GUILayout.Button("Stop Spawning Debug Points")) {
                    controller.startDebugging = false;
                }
            } else {
                if (GUILayout.Button("Spawn Debug Points")
                    && !(controller.getState == CameraSpawnController.State.Spawning)) {
                    controller.startDebugging = true;
                }
            }
            EditorGUILayout.Space();
            if (controller.getState == CameraSpawnController.State.Spawning) {
                if (GUILayout.Button("Stop Cameras Spawning")) {
                    controller.getState = CameraSpawnController.State.Idle;
                }
            } else {
                if (GUILayout.Button("Spawn Cameras") && !controller.startDebugging) {
                    controller.getState = CameraSpawnController.State.Spawning;
                }
            }
        }
    }
}
