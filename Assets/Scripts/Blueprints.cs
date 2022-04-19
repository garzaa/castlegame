using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Blueprints : ButtonSource {
	#pragma warning disable 0649
	[SerializeField] BlueprintAction buttonTemplate;
	[SerializeField] List<GameTile> tiles;
	[SerializeField] bool keepBlueprintsUnlocked = true;
	#pragma warning restore 0649

	HashSet<GameTile> lastSourced = new HashSet<GameTile>();
	HashSet<GameTile> permanentlyUnlocked = new HashSet<GameTile>();

	TileTracker tileTracker;

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
	}

	public List<GameTile> GetTiles() {
		return tiles;
	}

	override public List<ActionButton> GetButtons() {
		lastSourced.Clear();
		if (tileTracker == null) tileTracker = GameObject.FindObjectOfType<TileTracker>();
		List<ActionButton> buttons = new List<ActionButton>();
		// for each blueprint
		// if it's got a blueprint unlock, and it's satisfied, then make a new button, initialize it, and add it to the bar
		foreach (GameTile tile in tiles) {
			if (tile == null) continue;
			BlueprintUnlock unlock = tile.GetComponent<BlueprintUnlock>();
			if ((unlock && unlock.Unlocked(tileTracker)) || (unlock && keepBlueprintsUnlocked && permanentlyUnlocked.Contains(tile))) {
				buttons.Add(SpawnButton(tile));
				lastSourced.Add(tile);
				AddPermanentUnlock(tile);
			} else if (!unlock) {
				buttons.Add(SpawnButton(tile));
				lastSourced.Add(tile);
				AddPermanentUnlock(tile);
			}
		}
		return buttons;	
	}

	override public List<ActionButton> GetMidRoundButtons() {
		List<ActionButton> newButtons = new List<ActionButton>();
		foreach (GameTile tile in tiles) {
			if (tile == null) continue;
			BlueprintUnlock unlock = tile.GetComponent<BlueprintUnlock>();
			if (unlock && !lastSourced.Contains(tile) && unlock.Unlocked(tileTracker)) {
				newButtons.Add(SpawnButton(tile));
				lastSourced.Add(tile);
			}
		}
		return newButtons;
	}
	
	void AddPermanentUnlock(GameTile tile) {
		permanentlyUnlocked.Add(tile);
	}

	BlueprintAction SpawnButton(GameTile tile) {
		BlueprintAction b = Instantiate(buttonTemplate);
		b.Initialize(tile);
		return b;
	}
}
