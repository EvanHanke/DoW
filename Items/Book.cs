using UnityEngine;

[CreateAssetMenu]
public class Book : UsableItem {

	[TextArea(3,20)]
	public string[] pages;
	public Sprite[] pictures;

	public override bool OnUse(){
		BookUI.ShowBook(this);
		Debug.Log("book read");
		return true;
	}

}
