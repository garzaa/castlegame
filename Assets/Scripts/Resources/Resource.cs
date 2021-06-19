using UnityEngine;

[CreateAssetMenu(menuName="Data Type/Resource")]
public class Resource : ScriptableObject {
	[TextArea]
	public string description;
}
