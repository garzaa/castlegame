using UnityEngine;
using System.Collections.Generic;

public abstract class ButtonSource : MonoBehaviour {
	public int priority;
	public abstract List<ActionButton> GetButtons();
	public virtual List<ActionButton> GetMidRoundButtons() {
		return new List<ActionButton>();
	}
}
