using UnityEngine;

[CreateAssetMenu(menuName="Data Type/Resource")]
public class Resource : ScriptableObject {
	[TextArea]
	public string description;
	public Sprite icon;
	public Sprite detailedIcon;
}
