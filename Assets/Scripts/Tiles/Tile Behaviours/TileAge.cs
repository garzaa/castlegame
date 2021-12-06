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
		gameTile.visuals.ShowRepairEffect(gameTile);
		age = 0;
		gameTile.GetTracker().SendBoardChanged();
	}

	public string Stat() {
		return "Age "+GetAge();
	}
}
