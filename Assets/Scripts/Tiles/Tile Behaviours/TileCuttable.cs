using UnityEngine;

public class TileCuttable : MonoBehaviour, IStat {
	public ScriptableTile cutTo;

	public string Stat() {
		return "Cuttable to <color='#94fdff'>" + cutTo.tileObject.name+"</color>";
	}
}
