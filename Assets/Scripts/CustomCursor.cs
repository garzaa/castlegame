using UnityEngine;

public class CustomCursor : MonoBehaviour {
	public Texture2D cursorTexture;
	
	void Start() {
		if (cursorTexture) Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
	}
}
