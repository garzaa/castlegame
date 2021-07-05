using UnityEngine;

public class TileAge : TileBehaviour, IConsoleStat {
	int age = 0;

	public void Clockwork() {
		age += 1;
	}

	public int GetAge() {
		return age;
	}

	public void Repair() {
		gameTile.GetTracker().SendBoardChanged();
		gameTile.visuals.ShowRepairEffect(gameTile);
		age = 0;
	}

	public string Stat() {
		return "Age "+GetAge();
	}
}
