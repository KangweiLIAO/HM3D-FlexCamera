using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {
    CameraSpawnController control;

    private void OnEnable() {
        control = target as CameraSpawnController; // assign target to a var
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI(); // default call
        EditorGUILayout.Space();
        if (GUILayout.Button("Combine meshes")) {
            control.Combine();
        }
        EditorGUILayout.Space();
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter the play mode to spawn debug points or cameras", MessageType.Warning);
            EditorGUILayout.Space();
        } else {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max camera count:");
            control.maxCameraSpawn = EditorGUILayout.IntField(control.maxCameraSpawn);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (control.state == CameraSpawnController.State.Idle) {
                // if controller state is idle
                if (GUILayout.Button("Spawn Debug Points")) {
                    control.state = CameraSpawnController.State.Debuging;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Spawn Cameras")) {
                    control.CleanAllCaches(); // clear all cubemap caches
                    control.state = CameraSpawnController.State.Spawning;
                }
            }
            if (control.state == CameraSpawnController.State.Debuging) {
                // if controller state is debugging
                if (GUILayout.Button("Stop Spawning Debug Points")) {
                    control.state = CameraSpawnController.State.Finish;
                }
            }
        }
    }
}
