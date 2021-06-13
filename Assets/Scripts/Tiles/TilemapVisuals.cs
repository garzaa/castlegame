using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	[SerializeField] CommandInput console;
	[SerializeField] Canvas doubleScaleCanvas;
	[SerializeField] GameObject legendTemplate;

	const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

	Tilemap tilemap;
	Vector3Int origin;

	void Start() {
		console.Log("initializing legend");
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
		origin = tilemap.cellBounds.min;
		console.Log("computed origin: "+ origin.ToString());
		console.Log("tilemap size: "+tilemap.cellBounds.size);

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
		g.GetComponent<Text>().text = letters[idx].ToString();
	}

	void AddNumberLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.up*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = idx.ToString();
	}
}
