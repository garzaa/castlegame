[System.Serializable]
public class TileRedirect {
	public GameTile origin { get; private set; }
	public GameTile target { get; private set; }

	public TileRedirect(GameTile o, GameTile t) {
		this.origin = o;
		this.target = t;
	}
}
