using UnityEngine;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour {
	[SerializeField] Text scrollback;
	[SerializeField] InputField input;
	[SerializeField] TileTracker tileTracker;
	[SerializeField] TilemapVisuals tilemapVisuals;

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
			int idx = tilemapVisuals.GetLetters().ToLower().IndexOf(coords[0]);
			int x = int.Parse(idx.ToString());
			int y = int.Parse(coords[1].ToString());

			GameTile t = tileTracker.GetTile(x, y);
			Log(t.name + $" at {coords.ToUpper()}");
			Log("object hash: " + tileTracker.GetTile(x, y).GetHashCode());
		}

		else if (args[0] == "clear") {
			ClearConsole();
		} 
	}
}
