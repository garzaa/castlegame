using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour, IStat, ICardStat {
	#pragma warning disable 0649
	[SerializeField]
	protected int decayThreshold = 7;
	protected int originalDecayThreshold { get; private set; }

	[SerializeField]
	ScriptableTile decayTo;

	[SerializeField]
	[Tooltip("Amount of days before decay is compounded by neighbors")]
	protected int gracePeriod = 0;
	#pragma warning restore 0649

	TileAge tileAge;
	protected bool inGame = false;

	override protected void Awake() {
		base.Awake();
		inGame = true;
		tileAge = GetComponent<TileAge>();
		originalDecayThreshold = decayThreshold;
	}

	public ScriptableTile GetDecayTo() {
		return decayTo;
	}

	public void Clockwork() {
		// if it was already removed
		if (gameTile == null) return;
		// call the inherited version
		int decay = GetDecay();
		if (decay > decayThreshold) {
			gameTile.visuals.ShowDecayEffect(gameTile);
			gameTile.QueueForReplace(decayTo);
		} else if (decay == decayThreshold && !(this is NeighborDecay && decay==0)) {
			CommandInput.Log(gameTile + " is about to decay to "+decayTo.tileObject.name);
		}
	}

	virtual public int GetDecay() {
		int decay = tileAge.GetAge()-gracePeriod;
		return decay;
	}

	virtual public string Stat() {
		if (!gameTile) return $"Decays to <color='#94fdff'>{decayTo.tileObject.name}</color> in <color='#94fdff'>{decayThreshold}</color>.";

		int daysUntilDecay = Mathf.Max(decayThreshold-GetDecay(), 0);
		return $"Decay in <color='#94fdff'>{daysUntilDecay}</color> to <color='#94fdff'>{decayTo.tileObject.name}</color>.";
	}

	public int GetDecayThreshold() {
		return decayThreshold;
	}
}
