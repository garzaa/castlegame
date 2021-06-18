using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CommandInput : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] RectTransform textOutputParent;
	[SerializeField] InputField input;
	[SerializeField] GameObject textOutput;
	[SerializeField] List<SceneReference> levels;
	[SerializeField] List<BuildCommand> buildCommands;
	#pragma warning restore 0649

	Dictionary<string, ScriptableTile> buildTiles;
	WinCondition[] winConditions;

	bool gameOver = false;
	bool wonLevel = false;

	int actions = 0;
	const int actionsPerTick = 3;

	int daysWithoutActions;
	int totalDays;

	public static CommandInput c;
	TileTracker tileTracker;
	TilemapVisuals tilemapVisuals;

	bool sleeping = false;
	int daysSlept = 0;

	void Awake() {
		ClearConsole();
		c = this;
	}

	void Start() {
		tileTracker = GameObject.FindObjectOfType<TileTracker>();
		tilemapVisuals = GameObject.FindObjectOfType<TilemapVisuals>();

		buildTiles = new Dictionary<string, ScriptableTile>();
		foreach (BuildCommand bc in buildCommands) {
			buildTiles[bc.name] = bc.tile;
		}
		buildCommands.Clear();

		int levelNumber = 0;
		for (int i=0; i<levels.Count; i++) {
			if (levels[i].ScenePath.Equals(SceneManager.GetActiveScene().path)) {
				levelNumber = i;
				break;
			}
		}
		Log($"Level {levelNumber+1}: {SceneManager.GetActiveScene().name}");

		foreach (SceneFlavorText t in GameObject.FindObjectsOfType<SceneFlavorText>()) {
			Log(t.text);
		}

		winConditions = GameObject.FindObjectsOfType<WinCondition>();
		Log("Win condition"+(winConditions.Length>1 ? "s" : "")+": ");
		foreach (WinCondition c in winConditions) {
			Log(c.GetDescription());
		}

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
		string command = input.text.Trim();
		Log("<color='#c7cfdd'>"+command+"</color>");
		ClearInput();
		SelectInput();
		ParseCommand(command);
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

	void ParseCommand(string originalCommand) {
		string command = originalCommand.ToLower();

		string[] args = command.Split(' ');
		if (args[0] == "reload") {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			return;
		}

		else if (args[0] == "levels") {
			foreach (SceneReference level in levels) {
				// levels/level.unity
				string[] path = level.ScenePath.Split('/');
				Log(path[path.Length-1].Split('.')[0]);
			}
			return;
		}

		else if (args[0] == "load") {
			bool valid = false;
			string scenePath = "";
			foreach (SceneReference level in levels) {
				scenePath = level.ScenePath;
				if (scenePath.ToLower().Contains(args[1])) {
					valid = true;
					break;
				}
			}
			if (!valid) {
				Log(args[1]+" not a valid level, use >levels for a full list");
				return;
			}
			SceneManager.LoadScene(scenePath);
			return;
		}

		else if (args[0] == "stat") {
			string coords = args[1];

			GameTile t = tileTracker.GetTileNoRedirect(tileTracker.StrToPos(coords));
			IStat[] s = t.GetComponents<IStat>();
			for (int i=0; i<s.Length; i++) {
				Log(s[i].Stat());
			}
			return;
		}

		else if (args[0] == "help") {
			Log("");
			Log("All structures decay.");
			Log("Some repair others.");
			Log("");
			Log("COMMANDS:");
			Log("<color='#c7cfdd'>reload</color>: reload the current level");
			Log("<color='#c7cfdd'>levels</color>: show a list of playable levels");
			Log("<color='#c7cfdd'>load [level]</color>: load the specified level");
			Log("<color='#c7cfdd'>stat [tile]</color>: display status of a tile");
			Log("<color='#c7cfdd'>clear</color>: clear console");
			Log("<color='#c7cfdd'>sleep </color>: end turn and advance time");
			Log("<color='#c7cfdd'>sleep [days]</color>: sleep for [days]");
			Log("<color='#c7cfdd'>cut [tile]</color>: cut a tile");
			Log("<color='#c7cfdd'>resources</color>: show player resources");
			Log("<color='#c7cfdd'>fix</color>: repair a structure");
			Log("<color='#c7cfdd'>blueprints</color>: show available blueprints");
			Log("<color='#c7cfdd'>build [blueprint] [tile]</color>: build blueprint on tile");
			return;
		}

		else if (args[0] == "clear") {
			ClearConsole();
			return;
		}

		else if (args[0] == "wake") {
			if (sleeping) WakeUp();
			else Log("Not currently sleeping");
			return;
		}

		else if (args[0] == "resources") {
			Log(PlayerResources.Stat());
			return;
		}

		else if (args[0] == "blueprints") {
			foreach (string blueprint in buildTiles.Keys) {
				Log(ToSentence(blueprint));
				GameTile tileObject = buildTiles[blueprint].tileObject.GetComponent<GameTile>();
				Log(tileObject.description);
				foreach (TileRequiredResource r in tileObject.GetComponents<TileRequiredResource>()) {
					Log(r);
				}
				Log("");
			}
			return;
		}


		// non-action commands have ended, everything below this takes an action

		if (sleeping) {
			Log("Can't act while sleeping, abort with wake");
		}

		if (gameOver) {
			Log("Your Keep has been reclaimed by the Forest.");
			Log("Reload or choose a new level.");
			return;
		}

		else if (args[0] == "sleep") {
			int time = 1;
			if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) {
				time = int.Parse(args[1]);
			}
			sleeping = true;
			StartCoroutine(SlowTick(time));
			return;
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

		else if (args[0] == "fix") {
			actions++;
			tileTracker.RepairTile(tileTracker.StrToPos(args[1]));
		}

		else if (args[0] == "build") {
			if (Build(args[1], tileTracker.StrToPos(args[2]))) {
				actions++;
			}
		}

		else {
			Log("'"+originalCommand+"' bad command");
		}

		if (gameOver) return;

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

	void Tick() {
		if (actions == 0) {
			daysWithoutActions++;
		} else {
			daysWithoutActions = 0;
		}
		totalDays++;
		actions = 0;
		tileTracker.Tick();
		CheckWinConditions();
	}

	IEnumerator SlowTick(int time) {
		if (!sleeping) yield break;
		Tick();
		if (time > 1) yield return new WaitForSeconds(0.5f);
		daysSlept += 1;
		if (daysSlept == 1) {
			Log("1 day passed");
		} else {
			Log($"{daysSlept} days passed");
		}
		time--;
		if (time > 0) {
			StartCoroutine(SlowTick(time));
		} else {
			WakeUp();
		}
	}

	void WakeUp() {
		Log($"Woke after {daysSlept} days");
		daysSlept = 0;
		sleeping = false;
		actions = 0;
	}

	public void OnKeepDestroyed() {
		gameOver = true;
		Log("Your Keep has been taken by the Forest.");
		Log("<color='#c42430'>GAME OVER</color>");
	}

	void CheckWinConditions() {
		if (wonLevel || gameOver) return;
		WinCondition won = null;
		foreach (WinCondition c in winConditions) {
			if (c.Satisfied(tileTracker)) {
				won = c;
				break;
			}
		}
		if (won) {
			wonLevel = true;
			Log($"<color='#00cdf9'>{SceneManager.GetActiveScene().name} won after {totalDays} days!</color>");
			Log($"<color='#00cdf9'>Board condition satisfied:</color>");
			Log($"<color='#00cdf9'>{won.GetDescription()}</color>");
			Log("You can keep playing or load the next level.");
		}
	}

	public static int GetDaysWithoutActions() {
		return c.daysWithoutActions;
	}

	string ToSentence(string s) {
		return s.ToString().Substring(0, 1).ToUpper() + s.ToString().Substring(1);
	}
}

[System.Serializable]
public class BuildCommand {
	public string name;
	public ScriptableTile tile;
}
