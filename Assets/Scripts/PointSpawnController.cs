using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PointSpawnController : MonoBehaviour {
    public bool isConvex = false;
    public bool combineMeshes = false;
    [Range(5, 200)]
    public int pointDensity = 10; // points spawning rate (per second)

    private MeshFilter mf;
    private List<Point> points;

    private bool _spawning = false;
    public bool spawnStatus { get { return _spawning; } set { _spawning = value; } }
    public double meshArea {
        get {
            if (!Application.isPlaying) return 0;
            else return mf.mesh.GetTotalArea();
        }
    }

    struct Point {
        public Point(Vector3 pos) {
            this.pos = pos;
        }
        public Vector3 pos;
    }

    void Awake() {
        mf = gameObject.GetComponent<MeshFilter>();
        if (!mf) {
            // create a mesh filter component for root object
            mf = gameObject.AddComponent<MeshFilter>();
        }
    }

    // Start is called before the first frame update
    void Start() {
        points = new List<Point>();
        if (combineMeshes) {
            // if combine meshes checked
            Mesh cm = Resources.Load("CombinedMeshes/" + gameObject.name) as Mesh;
            if (cm) {
                // if combined mesh exist assign to root mesh filter
                mf.mesh = cm;
                Debug.Log("Combined mesh for " + gameObject.name + " loaded");
            } else {
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];
                for (int i = 0; i < meshFilters.Length; i++) {
                    if (meshFilters[i].sharedMesh) {
                        combine[i].mesh = meshFilters[i].sharedMesh;
                        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    }
                }
                if (combine.Length > 0) {
                    mf.mesh = new Mesh();   // create a new mesh to apply combined mesh
                    // set UInt32 to contains large scale of verties:
                    mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mf.mesh.CombineMeshes(combine);
                    CreateCombinedMeshAssets(mf.mesh);
                }
            }
        }
    }

    void FixedUpdate() {
        if (_spawning) {
            SpawnPoints(Mathf.CeilToInt(pointDensity * Time.fixedDeltaTime));
        }
    }

    void OnDrawGizmos() {
        if (points == null || points.Count == 0)
            return;

        foreach (Point p in points) {
            Gizmos.color = Color.red; // The_Helper.InterpolateColor(Color.red, Color.green, p.pos.magnitude); 
            Gizmos.DrawSphere(transform.TransformPoint(p.pos), transform.lossyScale.magnitude / 100);
        }
    }

    void CreateCombinedMeshAssets(Mesh mesh) {
        // find asset with name CombinedMesh in folder "Assets/Generated":
        string[] guids = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Resources/CombinedMeshes/" });
        foreach (var asset in guids) {
            // delete the old combined meshes
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(mesh, "Assets/Resources/CombinedMeshes/" + gameObject.name + ".mat");
    }

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
