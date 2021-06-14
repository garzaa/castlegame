using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommandInput : MonoBehaviour {
	[SerializeField] Text scrollback;
	[SerializeField] InputField input;
	[SerializeField] TileTracker tileTracker;
	[SerializeField] TilemapVisuals tilemapVisuals;

	public ScriptableTile clearedTile;
	public ScriptableTile gardenTile;

	public static CommandInput c;

	readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	void Awake() {
		ClearConsole();
		c = this;
	}

	void Start() {
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
		SelectInput();
		ParseCommand(command.ToLower());
	}

	void SelectInput() {
		input.text = "";
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
				Tick(time);
			}
		}

		else if (args[0] == "slowtick") {
			StartCoroutine(SlowTick(int.Parse(args[1])));
		}

		else if (args[0] == "cut") {
			tileTracker.ReplaceTile(tileTracker.StrToPos(args[1]), clearedTile);
		}

		else if (args[0] == "fix") {
			tileTracker.RepairTile(tileTracker.StrToPos(args[1]));
		}

		else if (args[0] == "till") {
			tileTracker.ReplaceTile(tileTracker.StrToPos(args[1]), gardenTile);
		}
	}

	void Tick(int time) {
		for (int t=0; t<time; t++) {
			GameTile[] tiles = tileTracker.GetTiles<GameTile>();
			for (int i=0; i<tiles.Length; i++) {
				tiles[i].Clockwork();
			}
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
