using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class ForgeUI : MonoBehaviour {

	GameObject label;
	RectTransform selector;
	Text desc;
	Text upgradeInfo;
	Image sprite;
	List<CastableItem> gear;
	UIList list;

	string[] confirm = {"Buy upgrade", "Cancel"};
	int state = 0;

	void Start(){
		DialogueBox2.active.Destroy();
		transform.SetParent(GameObject.Find("UI").transform, false);
		Invoke("Init", 0.01f);
		state = 0;
	}

	void Init(){
		GlobalStateMachine.GPause();

		//find references
		selector = GameObject.Find("Selector1").GetComponent<RectTransform>();
		label = GameObject.Find("list");
		desc = GameObject.Find("Info").GetComponent<Text>();
		sprite= GameObject.Find("GearSprite").GetComponent<Image>();
		upgradeInfo = GameObject.Find("EffectText").GetComponent<Text>();

		//init list
		gear = new List<CastableItem>();
		InitList();
	}

	void InitList(){
		list = new UIList();
		list.Init(4, label, 70f, selector, GetLabels());
		RefreshInfo();
	}

	bool CanAfford(CastableItem ci){
		return (PlayerStats.playerInv.QuantityOf("Crystal") >= ci.upgrade_cost);
	}

	string[] GetLabels(){
		InvEntry[] inv = PlayerStats.playerInv.GetItems<CastableItem>();
		List<string> labels = new List<string>();
		gear.Clear();

		foreach(InvEntry ie in inv){
			CastableItem ci = (CastableItem) ie.itm;
			if(!ci.upgraded && !ci.consumable){
				string s = ie.name;
				if(CanAfford(ci)) s += TextColorer.ToColor(" (+)", Color.green);
				labels.Add (s);
				gear.Add(ci);
			}
		}
		labels.Add("Exit");
		gear.Add(null);
		return labels.ToArray();
	}

	void RefreshInfo(){
		if(state == 1) return;
		int s = list.GetSelected();
		if(gear[s] != null){
			CastableItem c = gear[s];
			sprite.enabled = true;
			sprite.sprite = c.image;
			upgradeInfo.text = c.description + "\n\n" + TextColorer.ToColor(c.upgrade_desc, Color.green);
			desc.text = TextColorer.ToColor("Upgrade " + c.name, Color.green) + " for " + TextColorer.ToColor(c.upgrade_cost + " crystals.", Color.yellow);
		}
		else{
			sprite.enabled = false;
			upgradeInfo.text = "";
			desc.text = "Exit forge.";
		}
	}

	void BuyUpgrade(){
		int s = list.GetSelected();
		if(s < gear.Count - 1){
			PlayerStats.playerInv.RemoveItem(Itemer.me.GetItem("Crystal"), gear[s].upgrade_cost);
			gear[s].upgraded = true;
			PushMessage.Push("Item Upgraded");
			list.SetLabels(GetLabels(), true);
			EZStatInfo.UpdateStats();
			RefreshInfo();
		}
		else{
			Exit();
		}
	}

	//

	void Update(){
		if(MyInput.GetState("DOWN", true) == 'p'){
			list.Inc();
			RefreshInfo();
		}
		else if (MyInput.GetState("UP", true) == 'p'){
			list.Dinc();
			RefreshInfo();
		}

		if(MyInput.GetState("SHIFT", true) =='p'){
			if(state == 0) Exit();
			else {
				state = 0;
				list.SetLabels(GetLabels(), true);
			}
		}

		if(MyInput.GetState("Z", true) == 'p'){
			if(state == 0){
				if(list.GetSelected() < gear.Count-1){
					if(CanAfford(gear[list.GetSelected()])){
						state = 1;
						list.SetLabels(confirm, true);
					}
				}
				else Exit();
			}
			else if(list.GetSelected() == 0){
				state = 0;
				BuyUpgrade();

			}
			else {
				state = 0;
				list.SetLabels(GetLabels(), true);
			}
		}
	}



	void Exit(){
		GameObject.Destroy(gameObject);
		GlobalStateMachine.GUnpause();
	}
}
