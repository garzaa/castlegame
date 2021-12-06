using UnityEngine;
using System.Collections.Generic;

public abstract class CardSource : MonoBehaviour {
	public int priority;
	public abstract List<CardBase> GetCards();
}
