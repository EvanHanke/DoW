using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingStall : InteractionScript {

	public TradeRecipe[] myRecipes; //in ascending priority
	SpriteRenderer SR;
	TradeRecipe activeRecp;

	StateSaver saver;

	void Awake() {
		base.Awake();
		saver = gameObject.AddComponent<StateSaver>();
		saver.onLoad = Load;
	}

	void Start () {
		GameObject s = new GameObject("itemsprite");

		SR = s.AddComponent<SpriteRenderer>();
		s.transform.parent = transform;
		s.transform.position = transform.position + Vector3.up*transform.localScale.y*1.5f + Vector3.back*0.1f;
		//s.AddComponent<BobUpDown>();
		UpdateItem();
	}

	void Load(){
		foreach(TradeRecipe tr in myRecipes){
			if(saver.Get(tr.venderItem.name) == "sold"){
				tr.bought = true;
			}
		}
	}
	void MarkSold(){
		saver.Save(activeRecp.venderItem.name, "sold");
		activeRecp.bought = true;
		UpdateItem();

	}
	//Checks the list of possible vendable items and picks one based on environment health
	void UpdateItem(){
		TradeRecipe my_tr = null;
		foreach(TradeRecipe tr in myRecipes){
			if (!tr.unique || tr.unique && !tr.bought){
				my_tr = tr;
				break;
			}
		}
		if (my_tr != null){
			SR.sprite = my_tr.venderItem.image;
			activeRecp = my_tr;
			SR.transform.position = transform.position + Vector3.up*transform.localScale.y+ Vector3.back*0.1f;
			if(my_tr.venderItem is WearableItem){
				WearableItem w = (WearableItem) my_tr.venderItem;
				if(w.mySlot == WearableItem.Slot.Head){
					SR.transform.position = transform.position + Vector3.up*transform.localScale.y*0.2f + Vector3.back*0.1f;
				}
			}
		}
		else{
			SR.sprite = null;
			activeRecp = null;
		}
	}
		

	public override void OnInteract(){
		if(activeRecp != null){
			GameObject go = GameObject.Instantiate(ui.buySellItemPrefab, ui.transform, false);
			ItemBuyer ib = go.AddComponent<ItemBuyer>();
			ib.Set(go, activeRecp.playerItem, activeRecp.venderItem, activeRecp.playerAmt, activeRecp.venderAmt);
			ib.onComplete = MarkSold;
			ib.onDestroy = GlobalStateMachine.GUnpause;
			ib.unique = activeRecp.unique;
			ui.RemoveLabel();
		}
	}



	public override string LabelDesc(){
		if(activeRecp != null)
			return "Trade " + activeRecp.venderItem.name;
		else return "Empty trading stall"; 
	}
}

[System.Serializable]
public class TradeRecipe{
	public Item venderItem, playerItem;
	public int venderAmt, playerAmt;
	public bool unique= false;
	[HideInInspector]
	public bool bought = false;
}