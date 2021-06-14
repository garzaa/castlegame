using UnityEngine;

public class TileAge : TileBehaviour, IStat {
	int age = 0;

	public void Clockwork() {
		age += 1;
	}

	public int GetAge() {
		return age;
	}

	public void Repair() {
		age = 0;
	}

	public string Stat() {
		return "Age "+GetAge();
	}
}
