using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour {
	public ScriptableTile decayTo;
	public int decayThreshold;

	TileAge tileAge;

	override protected void Start() {
		base.Start();
		tileAge = GetComponent<TileAge>();
	}

	public void Clockwork() {
		if (tileAge.GetAge() > decayThreshold) {
			CommandInput.Log(gameTile.gameObject.name + " decayed to "+decayTo.tileObject.name+" at age "+tileAge.GetAge());
			gameTile.ReplaceSelf(decayTo);
		}
	}
}
