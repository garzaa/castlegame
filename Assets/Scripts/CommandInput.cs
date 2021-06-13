using UnityEngine;
using UnityEngine.UI;

public class CommandInput : MonoBehaviour {
	public Text scrollback;
	public InputField input;

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
		scrollback.text += "\n"+input.text;
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
}
