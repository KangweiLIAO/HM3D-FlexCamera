using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PointSpawnController : MonoBehaviour {
    public bool isConvex = false;
    public bool combineMeshes = false;
    [Range(5, 100)]
    public int pointDensity = 10; // points spawning rate (per second)

    private MeshFilter mf;
    private List<Point> points;

    private bool spawning = false;

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
        if (spawning) {
            Populate(Mathf.CeilToInt(pointDensity * Time.fixedDeltaTime), 10);
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
            // delete the old cubemap textures
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(mesh, "Assets/Resources/CombinedMeshes/" + gameObject.name + ".mat");
        //Debug.Log(AssetDatabase.GetAssetPath(mesh));    // Print the path of the saved asset
    }

    void Populate(int numRays, float duration = 0) {
        if (duration <= 0)
            duration = Time.deltaTime;
        if (numRays <= 0)
            numRays = 1;
        Vector3 center = mf.mesh.bounds.center;
        for (int i = 0; i < numRays; i++) {
            Vector3 point = isConvex ? mf.mesh.GetRandomPointInsideConvex() : mf.mesh.GetRandomPointInsideNonConvex(center);
            points.Add(new Point(point));
        }
    }

    public void SpawnPoints() {
        //Debug.Log("Spawn points");
        spawning = true;
        if (!mf) {
            Debug.LogError("A MeshFilter component is required for points spawning in the object " + gameObject.name);
            return;
        }
        Vector3[] verts = mf.mesh.vertices;
        int[] triangles = mf.mesh.triangles;

        double areaSum = 0;
        List<double> areas = new List<double>();
        for (int i = 0; i < triangles.Length; i += 3) {
            double area = 0.0;
            Vector3 corner = verts[triangles[i]];
            Vector3 edge1 = verts[triangles[i + 1]] - corner;
            Vector3 edge2 = verts[triangles[i + 2]] - corner;
            area += Vector3.Cross(edge1, edge2).magnitude;
            areas.Add(area / 2); // add each triangle's area into the list
            areaSum += area;
        }
        Debug.Log("Total surface area: " + areaSum / 2);

    }

    public void StopSpawnPoints() {
        if (spawning) spawning = !spawning;
    }
}
