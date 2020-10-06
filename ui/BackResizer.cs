using UnityEngine;

public class BackResizer : MonoBehaviour {
	TextMesh text;
	SpriteRenderer back;

	public void Init(){
		text = GetComponent<TextMesh>();
		back = GetComponentInChildren<SpriteRenderer>();

		float size = text.characterSize * text.text.Length*1.5f;
		back.size = new Vector2(size, 3f);
	}
}
