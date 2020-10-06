using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class DebugItem{
	public Item i;
	public int amt;
}

public class Inventory : MonoBehaviour {

	public DebugItem[] debugItems;
	
	public int money = 100;
	List<InvEntry> myItems = new List<InvEntry>();
	Dictionary<string, InvEntry> itemsByName = new Dictionary<string, InvEntry>();

	public Sprite defaultSprite;
	PlayerStats player;

	void Start(){
		foreach(DebugItem i in debugItems){
			AddItem(i.i, i.amt);
		}
	}


	public InvEntry[] GetAllItems(){
		return myItems.ToArray();
	}

	public InvEntry[] GetItems<T>(){
		List<InvEntry> filtered = new List<InvEntry>();
		foreach(InvEntry inv in myItems){
			if (inv.itm is T){
				filtered.Add(inv);
			}
		}
		return filtered.ToArray();
	}

	public Dictionary<string, int> GetItemDic(){
		Dictionary<string, int> itms = new Dictionary<string, int>();
		foreach(InvEntry iv in myItems){
			itms.Add(iv.name, iv.amt);
		}
		return itms;
	}

	public void LoadFromInvSave(InventorySave IS){
		foreach(InventorySave invs in IS.allItems){
			Item i = Itemer.me.GetItem(invs.itemName);
			AddItem(i, invs.amount, invs.state);
			if(i is CastableItem){
				CastableItem c = (CastableItem) i;
				if(c.charged){
					c.charges = invs.charge;
				}
			}
		}
	}

	public int QuantityOf(Item itm){
		//Debug.Log("Quant of " + itm.name);
		if (itm == null) return 0;
		else return QuantityOf(itm.name);
	}

	public int QuantityOf(string n){
		if (itemsByName.ContainsKey(n)){
			return itemsByName[n].amt;
		}
		else return 0;
	}

	public bool RemoveItem(Item item, int quantity){
		return RemoveItem(item, quantity, 0);
	}

	public bool RemoveItem(Item item, int quantity, int state){
		string s_name = item.GetNameFromState(state);
		if (!itemsByName.ContainsKey(s_name)){
			Debug.Log("Trying to remove an item which does not exist!");
			return false;
		}
		else if (itemsByName[s_name].amt - quantity < 0){
			Debug.Log("Trying to remove more items then you have!");
			return false;
		}
		itemsByName[s_name].amt -= quantity;

		//if you have no items left
		if (itemsByName[s_name].amt == 0){
			myItems.Remove(itemsByName[s_name]);
			itemsByName.Remove(s_name);
			PlayerStats.RefreshEquips();
		}
		return true; //return true if item removed successfully
	}

	public void AddItem(Item itm, int amount){
		AddItem(itm, amount, 0);
	}

	public void AddItem(Item itm, int amount, int state){

		string s_name = itm.GetNameFromState(state);
		//if item already exists in inventory, increase its quantity
		if (itemsByName.ContainsKey(s_name)){
			itemsByName[s_name].amt += amount;
		}

		//else create a new entry for the item in inventory
		else{
			Debug.Log("item added");
			InvEntry iv = new InvEntry(itm, s_name, amount, state);
			myItems.Add(iv);
			itemsByName.Add(s_name, iv);

			//if you have a free equip slot, auto-equip
			if (PlayerStats.getXEquip().onUse == null && itm is UsableItem){
				PlayerStats.SetEquip(PlayerStats.getXEquip(), (UsableItem) itm);
			}
			else if (PlayerStats.getCEquip().onUse == null && itm is UsableItem){
				PlayerStats.SetEquip(PlayerStats.getCEquip(), (UsableItem) itm);
			}
		}
		EZStatInfo.me.UpdateEquips();

		if(!(itm is Growable))
			QuestTracker.CheckAll(itm);
	}

	public void AdvanceTime(){
		foreach(InvEntry i_e in myItems){
			if(i_e.itm is CastableItem){
				CastableItem u =(CastableItem) i_e.itm;
				u.Recharge();
			}
		}
	}
}

public class InvEntry{
	public Item itm;
	public string name;
	public int amt;
	public int state;

	public InvEntry(Item i, string n, int a, int s){
		itm = i;
		name = n;
		amt = a;
		state = s;
	}
}