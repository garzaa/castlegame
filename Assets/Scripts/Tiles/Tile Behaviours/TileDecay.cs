using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour, IStat {
	[SerializeField]
	int decayThreshold = 7;

	[SerializeField]
	ScriptableTile decayTo;

	[SerializeField]
	int baseMultiplier = 1;

	[SerializeField]
	[Tooltip("Amount of days before decay is compounded.")]
	protected int gracePeriod = 0;

	TileAge tileAge;

	override protected void Awake() {
		base.Awake();
		tileAge = GetComponent<TileAge>();
	}

	public void Clockwork() {
		// if it was already removed
		if (gameTile == null) return;
		// call the inherited version
		if (GetDecay() > decayThreshold) {
			gameTile.QueueForReplace(decayTo);
		}
	}

	virtual public int GetDecay() {
		int decay = tileAge.GetAge()-gracePeriod;
		if (decay > 0) decay *= baseMultiplier;
		return decay;
	}

	virtual public string Stat() {
		return $"Decay {GetDecay()}/{decayThreshold} to {decayTo.tileObject.name}";
	}

	public int GetDecayThreshold() {
		return decayThreshold;
	}
}
