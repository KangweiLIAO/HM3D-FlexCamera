using UnityEngine;
using UnityEditor;

/// <summary>
/// Bind to a cube/sphere that cubemap can apply on
/// </summary>
public class CubemapController : MonoBehaviour {
    public Camera targetCam;
    public Shader cubemapShader;
    public Material cubemapMaterial;

    [HideInInspector]
    public int cubemapIndex;

    // Start is called before the first frame update
    void Awake() {
        cubemapMaterial = new Material(cubemapShader);
    }
    
    /// <summary>
    /// Captures a cubemap texture base on the target camera
    /// </summary>
    /// <returns>captured render texture</returns>
    public RenderTexture CaptureCubemapTexture() {
        RenderTexture cubemap = new RenderTexture(4096, 4096, 32);
        if (targetCam) {
            cubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            targetCam.RenderToCubemap(cubemap);
            // save texture to disk
            AssetDatabase.CreateAsset(cubemap, "Assets/Textures/cubemap_" + cubemapIndex + ".asset");
            cubemapMaterial.SetTexture("_CubeMap", cubemap); // update shader property "_CubeMap"
            if (cubemapMaterial) {
                // save material to disk
                AssetDatabase.CreateAsset(cubemapMaterial, "Assets/Materials/material_" + cubemapIndex + ".asset");
            }
        } else {
            Debug.LogWarning("Target Camera not assigned, " + gameObject.name + "returning default render texture.");
        }
        return cubemap;
    }

    /// <summary>
    /// Captures a panorama texture base on the target camera
    /// </summary>
    public void CapturePanoramaPicture() {
        RenderTexture cubemapRT = CaptureCubemapTexture(); // to store cubemap texture
        RenderTexture equirectRT = new RenderTexture(4096, 2048, 16); // to store img render texture
        if (equirectRT.Create()) {
            cubemapRT.ConvertToEquirect(equirectRT);
            cubemapMaterial.SetTexture("_CubeMap", cubemapRT);
            Texture2D t2d = new Texture2D(equirectRT.width, equirectRT.height); // to store img texture
            RenderTexture.active = equirectRT;
            // store equirectRT image in a Texture2D:
            t2d.ReadPixels(new Rect(0, 0, equirectRT.width, equirectRT.height), 0, 0);
            RenderTexture.active = null;
            byte[] bytes = t2d.EncodeToJPG();
            string path = Application.dataPath + "/Resources/Panorama/" + gameObject.name + ".jpg"; // save as panorama jpg file
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Cubemap successfully captured");
        }
    }
}
