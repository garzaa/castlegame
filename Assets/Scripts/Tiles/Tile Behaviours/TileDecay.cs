using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour, IStat {
	[SerializeField]
	int decayThreshold = 7;
	public ScriptableTile decayTo;

	[SerializeField]
	int baseMultiplier = 1;
	[SerializeField]
	protected int gracePeriod = 3;

	TileAge tileAge;

	override protected void Start() {
		base.Start();
		tileAge = GetComponent<TileAge>();
	}

	virtual public void Clockwork() {
		// call the inherited version
		if (GetDecay() > decayThreshold) {
			gameTile.QueueForReplace(decayTo);
		}
	}

	virtual public int GetDecay() {
		Debug.Log("root getting decay");
		int decay = tileAge.GetAge()-gracePeriod;
		if (decay > 0) decay *= baseMultiplier;
		return decay;
	}

	virtual public string Stat() {
		return $"Decay {GetDecay()}/{decayThreshold} to {decayTo.tileObject.name}";
	}
}
