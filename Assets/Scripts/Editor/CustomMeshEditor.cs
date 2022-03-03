using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class CustomMeshEditor : Editor {

    private const string EDITOR_NML_KEY = "_normals_length";
    private const string EDITOR_BBX_KEY = "_bounding_box";
    private Mesh mesh;
    private MeshFilter mf;
    private Vector3[] verts;
    private Vector3[] normals;
    private float normalsLength = 1f;
    private bool showBoundingBox = false;

    private void OnEnable() {
        mf = target as MeshFilter;
        if (mf != null) {
            mesh = mf.sharedMesh;
        }
        normalsLength = EditorPrefs.GetFloat(EDITOR_NML_KEY);
        showBoundingBox = EditorPrefs.GetBool(EDITOR_BBX_KEY);
    }

    private void OnSceneGUI() {
        if (mesh == null) {
            return;
        } else {
            Handles.matrix = mf.transform.localToWorldMatrix;
            Handles.color = Color.yellow;
            verts = mesh.vertices;
            normals = mesh.normals;
            int len = mesh.vertexCount;

            // Draw the bounding box of selected mesh:
            if (showBoundingBox) {
                Handles.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
            }

            if (normalsLength != 0) {
                for (int i = 0; i < len; i++) {
                    Handles.DrawLine(verts[i], verts[i] + normals[i] * normalsLength);
                }
            }
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        showBoundingBox = EditorGUILayout.Toggle("Show bounding box", showBoundingBox);
        normalsLength = EditorGUILayout.FloatField("Normals length", normalsLength);
        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetFloat(EDITOR_NML_KEY, normalsLength);
            EditorPrefs.SetBool(EDITOR_BBX_KEY, showBoundingBox);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate 90\u00B0")) {
            RotateMeshX(90f);
        }
        if (GUILayout.Button("Rotate -90\u00B0")) {
            RotateMeshX(-90f);
        }
        EditorGUILayout.EndHorizontal();
    }

    void RotateMeshX(float degree) {
        // Fixes mesh's rotation after importing it from HM3D.
        MeshFilter meshFilter = (MeshFilter)target;
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length];
        Quaternion rotation = Quaternion.Euler(degree, 0f, 0f);
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertex = vertices[i];
            newVertices[i] = rotation * vertex;
        }
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        EditorUtility.SetDirty(meshFilter);
    }
}