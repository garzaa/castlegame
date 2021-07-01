using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Collections.Generic;

public class TilemapVisuals : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] GameObject legendTemplate;
	[SerializeField] Tile highlightTile;
	[SerializeField] Tilemap highlightTilemapTemplate;
	[SerializeField] Tile homeTile;
	[SerializeField] Tile clickedTile;
	[SerializeField] Canvas doubleScaleCanvas; // this gets overridden with the initialized version of itself...is that ok??
	[SerializeField] TileInfo infoBubbleTemplate;
	#pragma warning restore 0649

	CommandInput console;
	Tilemap tilemap;
	Vector3Int origin;
	Tilemap highlightTilemap;
	Tilemap iconTilemap;
	Tilemap clickedTilemap;
	Vector3 mouseWorldPos;
	Vector3Int gridMousePos;
	TileTracker tracker;

	GameObject currentInfoBubble;
	bool showingTileVisuals;
	Vector3Int currentSelectedGridPosition;
	Vector3Int targetedTile;

	void Awake() {
		tilemap = GetComponent<Tilemap>();
		tilemap.CompressBounds();
	}

	void Start() {
		CreateHighlightTilemap();
		CreateIconTilemap();
		CreateSelectedTilemap();
		CreateDoubleScaleCanvas();
		console = GameObject.FindObjectOfType<CommandInput>();
		tracker = GameObject.FindObjectOfType<TileTracker>();
		origin = tilemap.cellBounds.min;

		for (int i=0; i<tilemap.cellBounds.size.x; i++) {
			AddLetterLegend(i);
		}

		for (int i=0; i<tilemap.cellBounds.size.y; i++) {
			AddNumberLegend(i);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			HideInfoBubble();
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

	void CreateDoubleScaleCanvas() {
		doubleScaleCanvas = Instantiate(doubleScaleCanvas, this.transform);
	}
	
	void CreateHighlightTilemap() {
		highlightTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder = tilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateIconTilemap() {
		iconTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		iconTilemap.GetComponent<TilemapRenderer>().sortingOrder = highlightTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	void CreateSelectedTilemap() {
		clickedTilemap = Instantiate(highlightTilemapTemplate, transform.parent);
		clickedTilemap.GetComponent<TilemapRenderer>().sortingOrder = iconTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
	}

	public void HighlightTile(Vector3Int gridPos) {
		if (!tilemap.cellBounds.Contains(gridPos)) {
			return;
		}
		highlightTilemap.SetTile(gridPos, highlightTile);
	}

	public void HighlightTiles(IEnumerable<GameTile> tiles) {
		foreach (GameTile tile in tiles) {
			highlightTilemap.SetTile(tile.gridPosition, highlightTile);
		}
	}

	void OnMouseOver() {
		mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		gridMousePos = highlightTilemap.WorldToCell(mouseWorldPos);
		gridMousePos.z = 0;
		if (gridMousePos == targetedTile) return;
		targetedTile = gridMousePos;
		highlightTilemap.ClearAllTiles();
		HighlightTile(gridMousePos);
	}

	void OnMouseDown() {
		OnTileClick(gridMousePos);
		// GameObject.FindObjectOfType<Card>().Initialize(gameTile);
	}

	// TODO: don't play a sound on info refresh
	void OnTileClick(Vector3Int gridPos, bool silent=false) {
		GameTile gameTile = tracker.GetTileNoRedirect(tracker.CellToBoard(gridPos));
		if (gameTile == null) return;
		currentSelectedGridPosition = gridPos;
		if (!silent) gameTile.PlayQuerySound();
		DisplayTileVisuals(gameTile);
		ShowInfoBubble(gameTile);
	}

	void ShowInfoBubble(GameTile tile) {
		if (currentInfoBubble) {
			GameObject.Destroy(currentInfoBubble);
		}

		TileInfo tileInfo = Instantiate(infoBubbleTemplate, doubleScaleCanvas.transform);
		currentInfoBubble = tileInfo.gameObject;
		tileInfo.Initialize(tile);
	}

	void HideInfoBubble() {
		GameObject.Destroy(currentInfoBubble);
		clickedTilemap.ClearAllTiles();
	}

	void ShowTileIcon(TileHighlight highlight) {
		foreach (Vector3Int gridPos in highlight.targets) {
			iconTilemap.SetTile(gridPos, highlight.tile);
		}
	}

	public void DisplayTileVisuals(GameTile tile) {
		clickedTilemap.ClearAllTiles();
		iconTilemap.ClearAllTiles();
		HighlightTile(tile.gridPosition);
		clickedTilemap.SetTile(tile.gridPosition, clickedTile);
		foreach (ITileHighlighter h in tile.GetComponents<ITileHighlighter>()) {
			ShowTileIcon(h.GetHighlight());
		}
	}

	public void OnGameBoardChanged() {
		// update the tile bubble in-place
		if (currentInfoBubble != null) {
			OnTileClick(currentSelectedGridPosition, silent:true);
		}
	}
}
