using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldPointCanvas : MonoBehaviour {

    new Camera camera;
    public Vector2 position;
    public GameObject optionalAnchor;
    RectTransform r;

    void Start() {
        camera = Camera.main;
        r = GetComponent<RectTransform>();
    }

    void LateUpdate() {
        if (optionalAnchor) position = optionalAnchor.transform.position;
        r.position = camera.WorldToScreenPoint(position);
    }
}
