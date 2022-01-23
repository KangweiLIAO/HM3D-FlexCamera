using UnityEngine;

public class CameraController : MonoBehaviour {
    public Camera[] cameras;
    private int currCamIndex = 0;
    // Start is called before the first frame update
    void Start() {
        Debug.Log("Camera Count: " + Camera.allCamerasCount);
        cameras = Camera.allCameras;
        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false);
        }
        if (cameras.Length > 0) {
            cameras[0].gameObject.SetActive(true);
            Debug.Log("Camera with name: " + cameras[0].name + ", is now enabled");
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            currCamIndex++;
            Debug.Log("C button has been pressed. Switching to the next camera");
            if (currCamIndex < cameras.Length) {
                cameras[currCamIndex - 1].gameObject.SetActive(false);
                cameras[currCamIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + cameras[currCamIndex].name + ", is now enabled");
            } else {
                cameras[currCamIndex - 1].gameObject.SetActive(false);
                currCamIndex = 0;
                cameras[currCamIndex].gameObject.SetActive(true);
                Debug.Log("Camera with name: " + cameras[currCamIndex].name + ", is now enabled");
            }
        }
    }
}
