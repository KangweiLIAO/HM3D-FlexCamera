using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaCapture : MonoBehaviour {
    public Camera targetCam;
    public RenderTexture cubemap;
    public RenderTexture equirectRT;

    // Start is called before the first frame update
    void Start() {
        targetCam.RenderToCubemap(cubemap);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            // Press Space to update the cubemap
            targetCam.RenderToCubemap(cubemap);
        } else if (Input.GetKeyDown(KeyCode.S)) {
            // Press S to create a panorama jpg file
            cubemap.ConvertToEquirect(equirectRT);
            Texture2D t2d = new Texture2D(equirectRT.width, equirectRT.height);
            RenderTexture.active = equirectRT;
            t2d.ReadPixels(new Rect(0, 0, equirectRT.width, equirectRT.height), 0, 0); // store equirectRT image in a Texture2D
            RenderTexture.active = null;
            byte[] bytes = t2d.EncodeToJPG();
            string path = Application.dataPath + "/PanoramaCapture" + ".jpg"; // save as panorama jpg file
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Cubemap successfully captured");
        }
    }
}
