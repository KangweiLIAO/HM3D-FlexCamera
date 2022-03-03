using UnityEngine;

public class CameraViewController : MonoBehaviour {

    [Range(1, 100)]
    public float rotateSpeed = 50f;
    [HideInInspector]
    public Camera[] cameras;

    private int currCamIndex = 0;
    private Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start() {
        // initialization:
        Cursor.lockState = CursorLockMode.Locked;
        cameras = Camera.allCameras;
        for (int i = 1; i < cameras.Length; i++) {
            // disable all cameras
            cameras[i].gameObject.SetActive(false);
        }
        if (cameras.Length > 0) {
            // enable first camera
            cameras[0].gameObject.SetActive(true);
            Debug.Log("Camera (" + cameras[0].name + ") enabled");
        }
    }

    // Update is called once per frame
    void Update() {
        rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        cameras[currCamIndex].transform.eulerAngles = rotation * rotateSpeed / 10;

        if (Input.GetKeyDown(KeyCode.C)) {
            // switch to next camera
            currCamIndex++;
            if (currCamIndex >= cameras.Length)
                currCamIndex = 0;
            cameras[currCamIndex - 1].gameObject.SetActive(false);
            cameras[currCamIndex].gameObject.SetActive(true);
            Debug.Log("Camera (" + cameras[currCamIndex].name + ") enabled");
        }
    }
}
