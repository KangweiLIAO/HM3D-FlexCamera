using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraSpawnController : MonoBehaviour {
    [SerializeField]
    private GameObject startPoint;
    [SerializeField]
    private GameObject captureCameraPrefab;
    [SerializeField]
    private GameObject cubemapSpawnPrefab;
    [Range(1, 200)]
    public int pointSpawnPerSec = 10; // points spawning rate (per second)

    public List<Point> points;

    private CameraViewController viewController;

    private MeshFilter combinedMF;

    private int totalCapture = 0;
    private int limitHelper = 0;

    private bool _debugging = false; // for spawning debug points
    public bool startDebugging { get => _debugging; set => _debugging = value; }

    private State _state = State.Idle; // for spawning cameras
    public State getState { get => _state; set => _state = value; }

    private int maxCameraSpawn = 20;
    public int setMaxCameraSpawn { get => maxCameraSpawn; set => maxCameraSpawn = value; }

    /// <summary>
    /// Struct to store the debug points' properties
    /// </summary>
    public struct Point {
        public Vector3 pos;
        public Point(Vector3 pos) {
            this.pos = pos;
        }
    }

    public enum State {
        Idle,
        Spawning,
        Finish
    }

    void Start() {
        viewController = GameObject.Find("Camera Controller").GetComponent<CameraViewController>();
        combinedMF = gameObject.GetComponent<MeshFilter>();
        if (!combinedMF) {
            // create a mesh filter component for root object
            combinedMF = gameObject.AddComponent<MeshFilter>();
        }
        points = new List<Point>();
        Clean();
    }

    void FixedUpdate() {
        if (_debugging) {
            SpawnPoints(Mathf.CeilToInt(pointSpawnPerSec * Time.fixedDeltaTime));
        }
        SpawnCameras();
    }

    void OnDrawGizmos() {
        if (points == null || points.Count == 0)
            return;

        foreach (Point p in points) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.TransformPoint(p.pos), new Vector3(1, 1, 1));
        }
    }

    void OnApplicationQuit() {
        Clean();
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
    /// Combine submeshes under the house parent object (which this script bind) and store the 
    /// combined mesh in resource folder
    /// </summary>
    public void Combine() {
        Mesh cm = Resources.Load("CombinedMeshes/" + gameObject.name) as Mesh;
        if (cm) {
            // If combined mesh exist assign to root mesh filter
            combinedMF.sharedMesh = cm;
            Debug.Log("Combined mesh for " + gameObject.name + " detected and loaded");
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
                combinedMF.sharedMesh = new Mesh();   // Create a new mesh to apply combined mesh
                combinedMF.sharedMesh.Clear();
                // Set UInt32 to contains large scale of verties:
                combinedMF.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combinedMF.sharedMesh.CombineMeshes(combine);
                CreateCombinedMeshAssets(combinedMF.sharedMesh);
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
        if (!combinedMF) {
            Debug.LogError("A MeshFilter component is required for points spawning in the object " + gameObject.name);
            return;
        }
        for (int i = 0; i < numRays; i++) {
            Vector3 point = combinedMF.mesh.GetRandomPointInConvex();
            points.Add(new Point(point));

        }
    }

    void SpawnCameraAt(Point p, int limitPerLine) {
        // Create a temporary camera to capture cubemap at a random point
        GameObject camObj = Instantiate(captureCameraPrefab);
        camObj.name = "tmp_camera_" + totalCapture;
        Camera tmpCam = camObj.GetComponent<Camera>();
        camObj.transform.position = p.pos;// move camera to a random point

        // Create a sphere/cube to apply cubemap on
        GameObject cubeInst = Instantiate(cubemapSpawnPrefab);
        cubeInst.name = "cubemap_cube_" + totalCapture;
        cubeInst.transform.GetChild(0).name = "cubemap_camera_" + totalCapture;
        if (limitHelper < limitPerLine) {
            startPoint.transform.position += startPoint.transform.forward * 2;
        } else {
            // next line of prefabs
            startPoint.transform.position -= startPoint.transform.forward * 2 * (limitPerLine - 1);
            startPoint.transform.position += startPoint.transform.right * 2;
            limitHelper = 0;
        }
        cubeInst.transform.position = startPoint.transform.position;

        CubemapController mapControl = cubeInst.GetComponent<CubemapController>();
        mapControl.targetCam = tmpCam;
        mapControl.cubemapIndex = totalCapture;
        mapControl.CaptureCubemapTexture(); // capture cubemap base on tmp camera
        Destroy(camObj); // Destroy tmp camera to avoid redundancy

        MeshRenderer mr = cubeInst.GetComponent<MeshRenderer>();
        mr.material = mapControl.cubemapMaterial;
        limitHelper++;
    }

    public void SpawnCameras(int limitPerLine = 10) {
        if (_state == State.Finish) {
            viewController.InitCameras();
            _state = State.Idle;
        }
        if (_state == State.Spawning && totalCapture < maxCameraSpawn) {
            SpawnPoints(Mathf.CeilToInt(Time.fixedDeltaTime));
            foreach (Point p in points) {
                SpawnCameraAt(p, limitPerLine);
                totalCapture++;
            }
            //points.Clear();
        } else if (!(_state == State.Idle)) {
            _state = State.Finish;
        }
    }

    /// <summary>
    /// Clean all materials and cubemaps
    /// </summary>
    public void Clean() {
        // find asset with name Cubemap, type of renderTexture in folder "Assets/Textures":
        string[] textureGuids = AssetDatabase.FindAssets("cubemap_ t:renderTexture", new[] { "Assets/Textures" });
        foreach (var asset in textureGuids) {
            // delete the old cubemap textures
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        string[] materialGuids = AssetDatabase.FindAssets("material_ t:material", new[] { "Assets/Materials" });
        foreach (var asset in materialGuids) {
            // delete the old cubemap materials
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
    }
}
