using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraSpawnController : MonoBehaviour {
    [Range(5, 200)]
    public int pointSpawnPerSec = 10; // points spawning rate (per second)
    public bool isConvex = false;

    private MeshFilter mf;
    private List<Point> points;

    private bool _spawning = false; // for spawning debug points
    private bool _combining = false; // for combine sub-meshes

    public bool startSpawning { get { return _spawning; } set { _spawning = value; } }
    public bool startCombineMesh { get { return _combining; } set { _combining = value; } }
    public double meshArea {
        get {
            if (!mf.sharedMesh) return -1;
            return mf.sharedMesh.GetTotalArea();
        }
    }

    /// <summary>
    /// Struct to store the debug points' properties
    /// </summary>
    struct Point {
        public Vector3 pos;
        public Point(Vector3 pos) {
            this.pos = pos;
        }
    }

    private void Awake() {
        mf = gameObject.GetComponent<MeshFilter>();
        if (!mf) {
            // create a mesh filter component for root object
            mf = gameObject.AddComponent<MeshFilter>();
        }
        points = new List<Point>();
    }

    void Start() {

    }

    void FixedUpdate() {
        if (_spawning) {
            SpawnPoints(Mathf.CeilToInt(pointSpawnPerSec * Time.fixedDeltaTime));
            //foreach (Point p in points) {
            //    // initiate a camera at the point

            //    // capture the cube map at the point

            //    // spawn a sphere/cube to apply the cubemap

            //    // delete the camera

            //}
        }
    }

    void OnDrawGizmos() {
        if (points == null || points.Count == 0)
            return;

        foreach (Point p in points) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.TransformPoint(p.pos), transform.lossyScale.magnitude / 100);
        }
    }
    /// <summary>
    /// Store the given mesh in resource folder
    /// </summary>
    /// <param name="mesh">A mesh, usually is a combined mesh</param>
    void CreateCombinedMeshAssets(Mesh mesh) {
        // Find asset with name CombinedMesh in folder "Assets/Generated":
        string[] guids = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Resources/CombinedMeshes/" });
        if (guids.Length > 0)
            AssetDatabase.DeleteAsset(guids[0]); // Delete the old combined meshes if there is one
        try {
            AssetDatabase.CreateAsset(mesh, "Assets/Resources/CombinedMeshes/" + gameObject.name + ".mat");
        } catch {
            Debug.LogError("Failed to create");
        }
    }

    /// <summary>
    /// Combine submeshes under the parent object (which this script bind) and store the 
    /// combined mesh in resource folder
    /// </summary>
    public void Combine() {
        Mesh cm = Resources.Load("CombinedMeshes/" + gameObject.name) as Mesh;
        if (cm) {
            // If combined mesh exist assign to root mesh filter
            mf.sharedMesh = cm;
            Debug.Log("Combined mesh for " + gameObject.name + " loaded");
        } else {
            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            // -1 since the first elem in meshFilters[] is the parent object:
            CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
            for (int i = 0; i < combine.Length; i++) {
                if (meshFilters[i + 1].sharedMesh) {
                    combine[i].mesh = meshFilters[i + 1].sharedMesh;
                    combine[i].transform = meshFilters[i + 1].transform.localToWorldMatrix;
                }
            }
            if (combine.Length > 0) {
                mf.sharedMesh = new Mesh();   // Create a new mesh to apply combined mesh
                mf.sharedMesh.Clear();
                // Set UInt32 to contains large scale of verties:
                mf.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mf.sharedMesh.CombineMeshes(combine);
                CreateCombinedMeshAssets(mf.sharedMesh);
            }
        }
    }

    /// <summary>
    /// Spawn and display the debug points where the camera will be spawned on
    /// </summary>
    /// <param name="numRays"></param>
    public void SpawnPoints(int numRays) {
        if (!Application.isPlaying) {
            Debug.LogError("Please enter play mode in order to start spawning");
            return;
        }
        if (!mf) {
            Debug.LogError("A MeshFilter component is required for points spawning in the object " + gameObject.name);
            return;
        }
        for (int i = 0; i < numRays; i++) {
            Vector3 point = isConvex ?
                mf.mesh.GetRandomPointInConvex() : mf.mesh.GetRandomPointInNonConvex(mf.mesh.GetCenter());
            points.Add(new Point(point));
        }
    }
}
