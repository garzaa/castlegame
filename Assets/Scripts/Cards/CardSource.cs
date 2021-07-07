using UnityEngine;
using System.Collections.Generic;

public abstract class CardSource : MonoBehaviour {
	public int priority;
	public abstract List<CardBase> GetCards();
	public virtual List<CardBase> GetMidRoundCards() {
		return new List<CardBase>();
	}
}
