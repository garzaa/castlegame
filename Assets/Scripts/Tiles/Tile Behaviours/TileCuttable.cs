using UnityEngine;

public class TileCuttable : TileBehaviour, IStat {
	public ScriptableTile cutTo;

	public virtual string Stat() {
		return "Cuttable to <color='#94fdff'>" + cutTo.tileObject.name+"</color>.";
	}

	public virtual bool Cuttable() {
		return true;
	}
}
