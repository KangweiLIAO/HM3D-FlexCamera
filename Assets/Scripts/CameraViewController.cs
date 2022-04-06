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
    void FixedUpdate() {
        if (cameras.Length > 0) {
            Camera currCam = cameras[currCamIndex];
            
            if (fisheyeEnabled) {
                Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotateSpeed, Vector3.up);
                _cameraOffset = camTurnAngle * _cameraOffset;
                Vector3 newCamPos = currCam.transform.parent.position + _cameraOffset;
                currCam.transform.position = Vector3.Slerp(currCam.transform.position, newCamPos, 0.3f);
                currCam.transform.LookAt(currCam.transform.parent.position);
            } else {
                rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
                currCam.transform.eulerAngles = rotation * rotateSpeed / 10;
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                // switch to the next camera
                currCamIndex++;
                Debug.Log(currCamIndex);
                if (currCamIndex >= cameras.Length)
                    currCamIndex = 0;
                cameras[currCamIndex - 1].gameObject.SetActive(false);
                currCam = cameras[currCamIndex];
                currCam.gameObject.SetActive(true);
                fisheyeEnabled = currCam.orthographic;
                Debug.Log("Camera (" + currCam.name + ") enabled");
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                SwitchFisheye(currCam);
            }
        }
    }

    private void SwitchFisheye(Camera camera) {
        if (fisheyeEnabled) {
            fisheyeEnabled = false;
            camera.orthographic = false;
            camera.transform.position += camera.transform.forward * 1;
        } else {
            fisheyeEnabled = true;
            camera.orthographic = true;
            camera.transform.position -= camera.transform.forward * 1;
            camera.orthographicSize = 0.5f;
            camera.farClipPlane = 2f;
            _cameraOffset = camera.transform.position - camera.transform.parent.position;
        }
    }

    public void InitCameras() {
        cameras = Camera.allCameras;
        for (int i = 1; i < cameras.Length; i++) {
            // disable all cameras
            cameras[i].gameObject.SetActive(false);
        }
    }
}
