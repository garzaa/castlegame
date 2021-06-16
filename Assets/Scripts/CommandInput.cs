using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CommandInput : MonoBehaviour {
	[SerializeField] RectTransform textOutputParent;
	[SerializeField] InputField input;
	[SerializeField] TileTracker tileTracker;
	[SerializeField] TilemapVisuals tilemapVisuals;
	[SerializeField] GameObject textOutput;

	public List<BuildCommand> buildCommands;
	Dictionary<string, ScriptableTile> buildTiles;

	int actions = 0;
	const int actionsPerTick = 3;

	public static CommandInput c;


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
		foreach(RectTransform t in textOutputParent.transform) {
			GameObject.Destroy(t.gameObject);
		}
	}

	public void OnCommandSubmit() {
		if (string.IsNullOrWhiteSpace(input.text)) {
			input.text = "";
			return;
		}
		string command = input.text;
		Log(command);
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
		GameObject g = Instantiate(c.textOutput, c.textOutputParent);
		g.GetComponent<Text>().text = s;
	}

	void ParseCommand(string command) {
		string[] args = command.Split(' ');
		if (args[0] == "stat") {
			string coords = args[1];

			GameTile t = tileTracker.GetTileNoRedirect(tileTracker.StrToPos(coords));
			IStat[] s = t.GetComponents<IStat>();
			for (int i=0; i<s.Length; i++) {
				Log(s[i].Stat());
			}
		}

		else if (args[0] == "clear") {
			ClearConsole();
		}

		else if (args[0] == "sleep") {
			int time = 1;
			if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) {
				time = int.Parse(args[1]);
			}
			Tick(time);
		}

		else if (args[0] == "slowsleep") {
			StartCoroutine(SlowTick(int.Parse(args[1])));
		}

		else if (args[0] == "cut") {
			GameTile tile = tileTracker.GetTile(tileTracker.StrToPos(args[1]), null);
			TileCuttable cut = tile.GetComponent<TileCuttable>();
			if (!cut) {
				Log($"{tile.name} at {args[1].ToUpper()} can't be cut");
			} else {
				actions++;
				tileTracker.ReplaceTile(tileTracker.StrToPos(args[1]), cut.cutTo);
			}
		}

		else if (args[0] == "resources") {
			Log(PlayerResources.Stat());
		}

		else if (args[0] == "fix") {
			actions++;
			tileTracker.RepairTile(tileTracker.StrToPos(args[1]));
		}

		else if (args[0] == "build") {
			if (Build(args[1], tileTracker.StrToPos(args[2]))) {
				actions++;
			}
		}

		if (actions >= actionsPerTick) {
			Log("Sunset");
			Tick();
			Log("Sunrise\n"+ actionsPerTick + " actions remaining");
		} else {
			int remaining = actionsPerTick - actions;
			if (remaining > 1) {
				Log(remaining + " actions remaining");
			} else {
				Log(remaining + " action remaining");
			}
		}
	}

	bool Build(string name, Vector3Int pos) {
		if (!buildTiles.ContainsKey(name)) {
			Log("Unknown build command "+name);
			return false;
		}
		return tileTracker.ReplaceTile(pos, buildTiles[name]);
	}

	void Tick(int time=1) {
		actions = 0;
		for (int t=0; t<time; t++) {
			tileTracker.Tick();
		}
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
