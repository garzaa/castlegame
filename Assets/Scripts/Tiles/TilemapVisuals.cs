using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] CommandInput console;
	[SerializeField] Canvas doubleScaleCanvas;
	[SerializeField] GameObject legendTemplate;
	#pragma warning restore 0649

	Tilemap tilemap;
	Vector3Int origin;

	void Awake() {
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
	}

	void Start() {
		origin = tilemap.cellBounds.min;
		// CommandInput.Log("initializing legend");
		// CommandInput.Log("computed origin: "+ origin.ToString());
		// CommandInput.Log("board size: "+tilemap.cellBounds.size);

		for (int i=0; i<tilemap.cellBounds.size.x; i++) {
			AddLetterLegend(i);
		}

		for (int i=0; i<tilemap.cellBounds.size.y; i++) {
			AddNumberLegend(i);
		}

	}

	void AddLetterLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.right*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = TileTracker.letters[idx].ToString().ToUpper();
	}

	void AddNumberLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.up*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = (idx+1).ToString();
	}
}
