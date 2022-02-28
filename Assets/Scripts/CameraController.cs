using UnityEngine;

public class CameraController : MonoBehaviour {
    public Camera[] cameras;
    public float rotateSpeed = 2f;

    private int currCamIndex = 0;
    private Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start() {
        // initialization:
        Cursor.lockState = CursorLockMode.Locked;
        cameras = Camera.allCameras;
        Debug.Log("Camera Count: " + Camera.allCamerasCount);

        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].gameObject.SetActive(false);
        }
        if (cameras.Length > 0) {
            cameras[0].gameObject.SetActive(true);
            Debug.Log("Camera(" + cameras[0].name + ") enabled");
        }
    }

    // Update is called once per frame
    void Update() {
        rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        cameras[currCamIndex].transform.eulerAngles = rotation * rotateSpeed;

        if (Input.GetKeyDown(KeyCode.C)) {
            currCamIndex++;
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
