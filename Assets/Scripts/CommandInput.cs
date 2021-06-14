using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CommandInput : MonoBehaviour {
	[SerializeField] Text scrollback;
	[SerializeField] InputField input;
	[SerializeField] TileTracker tileTracker;
	[SerializeField] TilemapVisuals tilemapVisuals;

	public List<BuildCommand> buildCommands;
	Dictionary<string, ScriptableTile> buildTiles;

	public ScriptableTile clearedTile;
	public ScriptableTile gardenTile;

	int actions = 0;
	const int actionsPerTick = 3;

	public static CommandInput c;

	readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	void Awake() {
		ClearConsole();
		c = this;
	}

	void Start() {
		buildTiles = new Dictionary<string, ScriptableTile>();
		foreach (BuildCommand bc in buildCommands) {
			buildTiles[bc.name] = bc.tile;
		}
		buildCommands.Clear();

		ClearInput();
		SelectInput();
	}

	void ClearConsole() {
		scrollback.text = "";
	}

	public void OnCommandSubmit() {
		if (string.IsNullOrWhiteSpace(input.text)) {
			input.text = "";
			return;
		}
		string command = input.text;
		scrollback.text += $"\n<color=#C7CFDD>{command}</color>";
		ClearInput();
		SelectInput();
		ParseCommand(command.ToLower());
	}

	void ClearInput() {
		input.text = "";
	}

	public void SelectInput() {
		input.Select();
		input.ActivateInputField();
	}

	public static void Log(Object text) {
		Log(text.ToString());
	}

	public static void Log(string s) {
		c.scrollback.text += "\n"+s;
	}

	void ParseCommand(string command) {
		string[] args = command.Split(' ');
		if (args[0] == "stat") {
			string coords = args[1];

			GameTile t = tileTracker.GetTile(tileTracker.StrToPos(coords));
			IStat[] s = t.GetComponents<IStat>();
			for (int i=0; i<s.Length; i++) {
				Log(s[i].Stat());
			}
		}

		else if (args[0] == "clear") {
			ClearConsole();
		}

		else if (args[0] == "tick" || args[0] == "tock") {
			int time = 1;
			if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) {
				time = int.Parse(args[1]);
			}
			Tick(time);
		}

		else if (args[0] == "slowtick") {
			StartCoroutine(SlowTick(int.Parse(args[1])));
		}

		else if (args[0] == "cut") {
			if (tileTracker.GetTile(tileTracker.StrToPos(args[1])).GetTile().name == clearedTile.name) {
				Log(args[1].ToUpper() + " already cleared");
			} else {
				actions++;
				tileTracker.ReplaceTile(tileTracker.StrToPos(args[1]), clearedTile);
			}
		}

		else if (args[0] == "fix") {
			actions++;
			tileTracker.RepairTile(tileTracker.StrToPos(args[1]));
		}

		else if (args[0] == "build") {
			Build(args[1], tileTracker.StrToPos(args[2]));
		}

		if (actions >= actionsPerTick) {
			Log("Sunset");
			Tick();
			Log("Sunrise\n"+ actionsPerTick + " actions remaining today");
		} else {
			int remaining = actionsPerTick - actions;
			if (remaining > 1) {
				Log(remaining + " actions remaining today");
			} else {
				Log(remaining + " action remaining today");
			}
		}
	}

	void Build(string name, Vector3Int pos) {
		if (!buildTiles.ContainsKey(name)) {
			Log("Unknown build command "+name);
			return;
		}
		actions++;
		tileTracker.ReplaceTile(pos, buildTiles[name]);
	}

	void Tick(int time=1) {
		actions = 0;
		for (int t=0; t<time; t++) {
			GameTile[] tiles = tileTracker.GetTiles<GameTile>();
			for (int i=0; i<tiles.Length; i++) {
				tiles[i].Clockwork();
			}
		}
		tileTracker.FinishTick();
	}

	IEnumerator SlowTick(int time) {
		yield return new WaitForSeconds(0.5f);
		Tick(1);
		Log("1 day done");
		time--;
		if (time > 0) StartCoroutine(SlowTick(time));
	}
}

[System.Serializable]
public class BuildCommand {
	public string name;
	public ScriptableTile tile;
}
