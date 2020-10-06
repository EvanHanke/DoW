using UnityEngine;
using UnityEngine.UI;

public class Chest : InteractionScript {

	public Item myItem;
	bool opened = false;
	public Sprite openedSprite;
	public int amount = 1;

	StateSaver ss;

	void Awake(){
		ss = gameObject.AddComponent<StateSaver>();
		ss.onLoad = OnLoad;
		base.Awake();
	}

	public void OnLoad(){
		opened = (ss.my_state == 1);
		if(openedSprite != null && opened)
			GetComponentInChildren<SpriteRenderer>().sprite = openedSprite;
	}

	public override string LabelDesc()
	{
		string a = name.Split('(')[0];
		if(a.Contains("MysteriousBox")) a = "Box";
		return (!opened)? "Open " + a : "Empty " + a;
	}

	public override void OnInteract(){
		if (!opened){
			AudioLoader.PlaySound("M3M7", 0.7f, true, 0.2f);
			player.AcquireItem(myItem, amount);
			ui.RemoveLabel();
			if(openedSprite != null)
				GetComponentInChildren<SpriteRenderer>().sprite = openedSprite;
			opened = true;
			ss.my_state = 1;
		}
	}
		

	public override void OnLeave(){
		if (!opened)
		ui.RemoveLabel();
	}
}
