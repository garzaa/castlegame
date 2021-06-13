using UnityEngine;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour {
	[SerializeField] Text scrollback;
	[SerializeField] InputField input;
	[SerializeField] TileTracker tileTracker;
	[SerializeField] TilemapVisuals tilemapVisuals;

	public ScriptableTile clearedTile;

	readonly string[] nullAges = new string[] {"eternal", "immeasurable", "unfathomable"};

	void Awake() {
		ClearConsole();
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
		ParseCommand(command.ToLower());
		SelectInput();
	}

	void SelectInput() {
		input.text = "";
		input.Select();
		input.ActivateInputField();
	}

	public void Log(Object text) {
		Log(text.ToString());
	}

	public void Log(string s) {
		scrollback.text += "\n"+s;
	}

	void ParseCommand(string command) {
		string[] args = command.Split(' ');
		if (args[0] == "stat") {
			string coords = args[1];
			int idx = TileTracker.letters.IndexOf(coords[0]);
			int x = int.Parse(idx.ToString());
			int y = int.Parse(coords[1].ToString());

			GameTile t = tileTracker.GetTile(x, y);
			Log(t.name + $" at {coords.ToUpper()}");
			if (t.GetComponent<TileAge>()) {
				Log($"age: {t.GetComponent<TileAge>().GetAge()}");
			} else {
				Log($"age: {nullAges[Random.Range(0, nullAges.Length)]}");
			}
			Log("object hash: " + tileTracker.GetTile(x, y).GetHashCode());
		}

		else if (args[0] == "clear") {
			ClearConsole();
		}

		else if (args[0] == "tick" || args[0] == "tock") {
			int time = 1;
			if (args.Length > 1) {
				time = int.Parse(args[1]);
			}
			GameTile[] tiles = tileTracker.GetTiles<GameTile>();
			for (int t=0; t<time; t++) {
				for (int i=0; i<tiles.Length; i++) {
					tiles[i].Clockwork();
				}
			}
		}

		else if (args[0] == "cut") {
			tileTracker.ReplaceTile(tileTracker.StrToPos(args[1]), clearedTile);
		}
	}
}
