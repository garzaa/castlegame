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
			gameTile.ReplaceSelf(decayTo);
		}
	}
}
