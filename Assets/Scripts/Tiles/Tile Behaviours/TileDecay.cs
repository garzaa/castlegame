using UnityEngine;

[RequireComponent(typeof(TileAge))]
public class TileDecay : TileBehaviour, IStat {
	[SerializeField]
	int decayThreshold = 7;
	public ScriptableTile decayTo;

	[SerializeField]
	protected int multiplier = 1;
	[SerializeField]
	protected int gracePeriod = 3;

	TileAge tileAge;

	override protected void Start() {
		base.Start();
		tileAge = GetComponent<TileAge>();
	}

	virtual public void Clockwork() {
		if (GetDecay() > decayThreshold) {
			gameTile.ReplaceSelf(decayTo);
		}
	}

	public int GetDecay() {
		int decay = tileAge.GetAge()-gracePeriod;
		if (decay > 0) decay *= multiplier;
		return decay;
	}

	virtual public string Stat() {
		return $"decay {GetDecay()}/{decayThreshold} to {decayTo.tileObject.name}";
	}
}
