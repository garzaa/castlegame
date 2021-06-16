using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour, IStat {
	[SerializeField]
	protected int decayThreshold = 7;
	protected int originalDecayThreshold { get; private set; }

	[SerializeField]
	ScriptableTile decayTo;

	[SerializeField]
	[Tooltip("Amount of days before decay is compounded by neighbors")]
	protected int gracePeriod = 0;

	TileAge tileAge;

	override protected void Awake() {
		base.Awake();
		tileAge = GetComponent<TileAge>();
		originalDecayThreshold = decayThreshold;
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
		return decay;
	}

	virtual public string Stat() {
		return $"Decay {GetDecay()}/{decayThreshold} to {decayTo.tileObject.name}";
	}

	public int GetDecayThreshold() {
		return decayThreshold;
	}
}
