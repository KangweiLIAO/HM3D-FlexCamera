using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraSpawnController : MonoBehaviour {

    [SerializeField]
    private GameObject camerasPivot;
    [SerializeField]
    private GameObject captureCameraPrefab;
    [SerializeField]
    private GameObject cubemapSpawnPrefab;

    private MeshFilter combinedMF;
    private CameraViewController viewController;

    public List<CameraPoint> points;
    private int cameraSpawnCount;
    private int totalCapture = 0;
    private int currInstNum = 0;

    private bool _debugging = false; // for spawning debug points
    public bool startDebugging { get => _debugging; set => _debugging = value; }

    private State _state = State.Idle; // for spawning cameras
    public State getState { get => _state; set => _state = value; }
    public int setMaxCameraSpawn { get => cameraSpawnCount; set => cameraSpawnCount = value; }


    /// <summary>
    /// The state of this script
    /// </summary>
    public enum State {
        Idle,
        Spawning,
        Finish
    }

    /// <summary>
    /// Struct to store a camera's status
    /// </summary>
    public struct CameraPoint {
        public Camera camera;
        public Vector3 pos;

        public CameraPoint(Vector3 pos, Camera camera = null, bool fisheye = false) {
            this.pos = pos;
            this.camera = camera;
        }
    }


    void Start() {
        viewController = GameObject.Find("Camera Controller").GetComponent<CameraViewController>();
        combinedMF = gameObject.GetComponent<MeshFilter>();
        if (!combinedMF) {
            // create a mesh filter component for root object
            combinedMF = gameObject.AddComponent<MeshFilter>();
        }
        points = new List<CameraPoint>();
        CleanAllCaches();
    }

    void FixedUpdate() {
        if (_debugging) {
            SpawnCameraDebugPoints(Mathf.CeilToInt(Time.fixedDeltaTime));
        }
        SpawnCameras();
    }

    void OnDrawGizmos() {
        if (points == null || points.Count == 0)
            return;

        foreach (CameraPoint p in points) {
            Gizmos.color = Color.red;
            if (!p.camera) {
                Gizmos.DrawSphere(transform.TransformPoint(p.pos), 0.05f);
            } else {
                Gizmos.DrawWireSphere(transform.TransformPoint(p.pos),
                    cubemapSpawnPrefab.GetComponent<SphereCollider>().radius);
            }
        }
    }

    void OnApplicationQuit() {
        CleanAllCaches();
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
                // Store assets to disk
                string[] guids = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Resources/CombinedMeshes/" });
                if (guids.Length > 0)
                    AssetDatabase.DeleteAsset(guids[0]); // Delete the old combined meshes if there is one
                try {
                    AssetDatabase.CreateAsset(combinedMF.sharedMesh, "Assets/Resources/CombinedMeshes/" 
                        + gameObject.name + ".mat");
                } catch {
                    Debug.LogError("Failed to create");
                }
            }
        }
    }

    /// <summary>
    /// Spawn and display the debug points where the camera will be spawned on
    /// </summary>
    /// <param name="numRays"></param>
    public void SpawnCameraDebugPoints(int numRays) {
        if (!combinedMF) {
            Debug.LogError(" A MeshFilter component should be bind to " + gameObject.name);
            return;
        }
        for (int i = 0; i < numRays; i++) {
            Vector3 point = combinedMF.mesh.GetRandomPointInConvex();
            points.Add(new CameraPoint(point));
        }
    }

    /// <summary>
    /// Spawn a regular camera at point p with a cubemap instance
    /// </summary>
    /// <param name="p">Spawning point</param>
    /// <param name="instPerRow">Number of cubemap instance per row</param>
    void SpawnCameraAt(CameraPoint p, int instPerRow, float gap = 5) {
        // Create a temporary camera to capture cubemap at a random point
        GameObject camObj = Instantiate(captureCameraPrefab);
        camObj.name = "tmp_camera_" + totalCapture;
        Camera tmpCam = camObj.GetComponent<Camera>();
        camObj.transform.position = p.pos;// move camera to the random point

        // Create instance to apply the captured cubemap
        GameObject mapInst = Instantiate(cubemapSpawnPrefab);
        mapInst.name = "cubemap_cube_" + totalCapture;
        mapInst.transform.GetChild(0).name = "cubemap_camera_" + totalCapture;
        if (currInstNum < instPerRow) {
            camerasPivot.transform.position += camerasPivot.transform.forward * gap;
        } else {
            // next line of prefabs
            camerasPivot.transform.position -= camerasPivot.transform.forward * gap * (instPerRow - 1);
            camerasPivot.transform.position += camerasPivot.transform.right * gap;
            currInstNum = 0;
        }
        mapInst.transform.position = camerasPivot.transform.position;

        CubemapController mapControl = mapInst.GetComponent<CubemapController>();
        mapControl.targetCam = tmpCam;
        mapControl.cubemapIndex = totalCapture;
        mapControl.CaptureCubemapTexture(); // capture cubemap base on tmp camera
        Destroy(camObj); // Destroy tmp camera to avoid redundancy

        MeshRenderer mr = mapInst.GetComponent<MeshRenderer>();
        mr.material = mapControl.cubemapMaterial;
        currInstNum++;
    }

    /// <summary>
    /// Spawn cameras on random points inside the object where this script binds to
    /// </summary>
    public void SpawnCameras(int instPerRow = 10) {
        if (_state == State.Finish) {
            viewController.InitCameras();
            _state = State.Idle;
        }
        if (_state == State.Spawning && totalCapture < cameraSpawnCount-1) {
            SpawnCameraDebugPoints(Mathf.CeilToInt(Time.fixedDeltaTime));
            foreach (CameraPoint p in points) {
                SpawnCameraAt(p, instPerRow);
                totalCapture++;
            }
            //points.Clear();
        } else if (!(_state == State.Idle)) {
            _state = State.Finish;
        }
    }

    /// <summary>
    /// Clean all cubemap materials and cubemaps
    /// </summary>
    public void CleanAllCaches() {
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
