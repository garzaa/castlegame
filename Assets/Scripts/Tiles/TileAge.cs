using UnityEngine;

public class TileAge : TileBehaviour {
	int age = 0;

	public void Clockwork() {
		age += 1;
	}

	public int GetAge() {
		return age;
	}
}
