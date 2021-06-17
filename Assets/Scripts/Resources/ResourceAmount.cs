[System.Serializable]
public class ResourceAmount {
	public int amount;
	public Resource resource;

	public override string ToString() {
		return amount + " " + resource.name;
	}
}
