using UnityEngine;
using System.Collections.Generic;

public abstract class CardSource : MonoBehaviour {
	public abstract List<CardBase> GetCards();
}
