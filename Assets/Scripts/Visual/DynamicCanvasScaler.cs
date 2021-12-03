using UnityEngine;

public class DynamicCanvasScaler : MonoBehaviour {
    static int pixelScale = 1;
    Canvas targetCanvas;

    public float multiplier = 1;
    public int minimumScale = 1;

    void Start() {
        targetCanvas = GetComponent<Canvas>();
    }

    void LateUpdate() {
        pixelScale = ComputePixelScale();
        targetCanvas.scaleFactor = Mathf.Max(1, pixelScale * multiplier);
    }

    int ComputePixelScale() {
        return Mathf.Max(minimumScale, Mathf.CeilToInt((float)Screen.height / 720f));
    }

    public static int GetPixelScale() {
        return pixelScale;
    }
}
