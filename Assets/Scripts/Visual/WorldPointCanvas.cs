using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldPointCanvas : MonoBehaviour {

    new Camera camera;
    public Vector2 position;
    RectTransform r;

    void Start() {
        camera = Camera.main;
        r = GetComponent<RectTransform>();
    }

    void LateUpdate() {
        r.position = camera.WorldToScreenPoint(position);
    }
}
