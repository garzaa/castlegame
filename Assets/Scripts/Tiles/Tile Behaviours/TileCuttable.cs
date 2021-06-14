using UnityEngine;

public class TileCuttable : MonoBehaviour, IStat {
	public ScriptableTile cutTo;

	public string Stat() {
		return "Cuttable to " + cutTo.tileObject.name;
	}
}
