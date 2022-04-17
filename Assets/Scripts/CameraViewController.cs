using UnityEngine;

public class CameraViewController : MonoBehaviour {
    [Range(1, 100)]
    public float rotateSpeed = 25f;

    public Camera[] cameras;

    private int currCamIndex = 0;
    private bool fisheyeEnabled = false;
    private Vector2 rotation = Vector2.zero;
    private Vector3 _cameraOffset;

    // Start is called before the first frame update
    void Start() {
        // initialization:
        Cursor.lockState = CursorLockMode.Locked;
        InitCameras();
    }

    // Update is called once per frame
    void Update() {
        if (cameras.Length > 0) {
            Camera currCam = cameras[currCamIndex];
            
            if (fisheyeEnabled) {
                // Camera view rotate around parent center
                Quaternion camXAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotateSpeed, Vector3.up);
                Quaternion camYAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * rotateSpeed, Vector3.right);
                _cameraOffset = camXAngle * camYAngle * _cameraOffset;
                Vector3 newCamPos = currCam.transform.parent.position + _cameraOffset;
                currCam.transform.position = Vector3.Slerp(currCam.transform.position, newCamPos, 0.3f);
                currCam.transform.LookAt(currCam.transform.parent.position);
            } else {
                // Camera view rotate around self center
                rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
                currCam.transform.eulerAngles = rotation * rotateSpeed / 10;
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                // switch to the next camera
                currCamIndex++;
                if (currCamIndex >= cameras.Length)
                    currCamIndex = 0;
                cameras[currCamIndex - 1].gameObject.SetActive(false);
                currCam = cameras[currCamIndex];
                currCam.gameObject.SetActive(true);
                fisheyeEnabled = currCam.transform.position != currCam.transform.parent.position;
                Debug.Log("Camera (" + currCam.name + ") enabled");
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                EnableFakeFisheye(currCam);
            }
        }
    }

    public void InitCameras() {
        cameras = Camera.allCameras;
        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false); // Disable all cameras
        }
    }

    /// <summary>
    /// Faking Fisheye effect by moving the perspective camera outside the cubemap instance
    /// </summary>
    /// <param name="camera">The camera in the cubemap instance</param>
    private void EnableFakeFisheye(Camera camera) {
        if (fisheyeEnabled) {
            fisheyeEnabled = false;
            camera.transform.position += camera.transform.forward * 1;
        } else {
            fisheyeEnabled = true;
            camera.transform.position -= camera.transform.forward * 1;
            camera.orthographicSize = 0.5f;
            camera.farClipPlane = 2f;
            _cameraOffset = camera.transform.position - camera.transform.parent.position;
        }
    }
}
