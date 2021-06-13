using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	[SerializeField] CommandInput console;
	[SerializeField] Canvas doubleScaleCanvas;
	[SerializeField] GameObject legendTemplate;

	public const string letters = "abcdefghijklmnopqrstuvwxyz";

	Tilemap tilemap;
	Vector3Int origin;

	void Awake() {
		console.Log("tightening tilemap bounds");
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
	}

	void Start() {
		console.Log("initializing legend");
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

	public string GetLetters() {
		return letters;
	}

	void AddLetterLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.right*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = letters[idx].ToString().ToUpper();
	}

	void AddNumberLegend(int idx) {
		GameObject g = Instantiate(legendTemplate, Vector3.zero, Quaternion.identity, doubleScaleCanvas.transform);
		g.GetComponent<WorldPointCanvas>().position = tilemap.CellToWorld(origin + Vector3Int.up*(idx+1)) + Vector3.down*tilemap.cellSize.y/2f;
		g.GetComponent<Text>().text = idx.ToString();
	}
}
