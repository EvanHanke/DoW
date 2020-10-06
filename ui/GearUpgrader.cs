using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GearUpgrader : MonoBehaviour {
	int state = 0; //select armor, select upgrade, confirm

	public UpgradeSlot[] upgrades;
	UpgradeSlot unlocked;
	RectTransform selector;
	GameObject label;
	UIList list;
	Text desc, effectText;
	Image sprite;

	InvEntry[] gear;

	WearableItem s_gear;
	UpgradeSlot s_upgrade;

	string[] confirm = {"Buy upgrade", "Cancel"};

	void Start(){
		DialogueBox2.active.Destroy();
		transform.SetParent(GameObject.Find("UI").transform, false);
		Invoke("Init", 0.1f);
	}

	void Init(){
		GlobalStateMachine.GPause();
		gear = PlayerStats.playerInv.GetItems<WearableItem>();
		selector = GameObject.Find("Selector1").GetComponent<RectTransform>();
		label = GameObject.Find("list");
		desc = GameObject.Find("Info").GetComponent<Text>();
		sprite= GameObject.Find("GearSprite").GetComponent<Image>();
		effectText = GameObject.Find("EffectText").GetComponent<Text>();
		s_upgrade = null;
		list = new UIList();
		list.Init(4, label, 70f, selector, SetGearLabels());
		RefreshInfo();
	}

	string[] SetGearLabels(){
		string[] labels = new string[gear.Length];
		int i = 0;
		foreach(InvEntry w in gear){
			labels[i] = w.name;
			i++;
		}
		return labels;
	}

	string[] SetUpgradeLabels(){
		List<UpgradeSlot> us = new List<UpgradeSlot>();
		foreach(UpgradeSlot u in upgrades){
			if(u.unlocked){
				us.Add(u);
			}
		}


		string[] labels = new string[us.Count];
		int i = 0;
		foreach(UpgradeSlot u in us){
			labels[i] = TextColorer.ToColor( u.upgrade.name, u.color);
			i++;
		}
		return labels;
	}

	void SetLabels(string[] s){
		list.RefreshList(s);
	}

	void RefreshInfo(){
		list.RefreshList();
		string x = "";
		if(state == 0){
			s_gear = (WearableItem) gear[list.GetSelected()].itm;
		}
		else if(state ==1){
			s_upgrade = upgrades[list.GetSelected()];
			x = TextColorer.ToColor(s_upgrade.upgrade.name, Color.cyan) + ": " + s_upgrade.upgrade.my_modifier.ToRichString();
		}
		effectText.text = s_gear.effects[0].ToRichString() + "\n" + x;

		sprite.sprite = s_gear.image;
		string a = "Upgrade "  + s_gear.name;
		if(s_upgrade != null){
			a += " with " + TextColorer.ToColor(s_upgrade.upgrade.name + " augmentation", s_upgrade.color);
			Color c = (s_upgrade.CanAfford())? Color.green : Color.red;
			a+= "\n" + TextColorer.ToColor(string.Format("Cost: (x{1}) {0}", s_upgrade.item.name, s_upgrade.amount), c);
		}
		desc.text = a;
	}

	void Update(){
		if(MyInput.GetState("UP", true) == 'p'){
			AudioLoader.PlayMenuBlip();
			list.Dinc();
			RefreshInfo();
		}
		if(MyInput.GetState("DOWN", true) == 'p'){
			AudioLoader.PlayMenuBlip();
			list.Inc();
			RefreshInfo();
		}
		if(MyInput.GetState("Z", true) == 'p'){
			if(state == 0){
				state = 1;
				list.SetLabels(SetUpgradeLabels(), true);
				AudioLoader.PlayMenuSelect();
			}
			else if (state == 1){
				if(s_upgrade.CanAfford() && s_upgrade.unlocked){
					state = 2;
					list.SetLabels(confirm, true);
					AudioLoader.PlayMenuSelect();
				}
				else{
					AudioLoader.PlayMenuCancel();
				}
			}
			else if (state == 2){
				if(list.GetSelected() ==0){
					s_gear.effects[0].upgrades.Add(s_upgrade.upgrade);
					AudioLoader.PlaySound("levelup2");
					PushMessage.Push("Gear upgraded!");
					state = 2;
					list.SetLabels(confirm, true);
				}
				else{
					AudioLoader.PlayMenuCancel();
				}
				state = 1;
				list.SetLabels(SetUpgradeLabels(), true);

			}
			RefreshInfo();
		}
		if(MyInput.GetState("SHIFT") == 'p'){
			AudioLoader.PlayMenuCancel();
			if(state == 0){
				End();
			}
			else if(state == 1){
				list.SetLabels(SetGearLabels(), true);
				state = 0;
			}
			else if(state == 2){
				list.SetLabels(SetUpgradeLabels(), true);
				state = 1;
			}
			RefreshInfo();
		}
	}

	void End(){
		GameObject.Destroy(gameObject);
		GlobalStateMachine.GUnpause();
	}

}

[System.Serializable]
public class UpgradeSlot{
	public Upgrade upgrade;
	public Color color;
	public CheckCondition condition;
	public Item item;
	public int amount;
	public bool unlocked{
		get{
			//return true;
		return condition.Check();
		}
	}

	public bool CanAfford(){
		//return true;
		return (PlayerStats.playerInv.QuantityOf(item) >= amount);
	}

	public void Buy(){
		PlayerStats.playerInv.RemoveItem(item, amount);
	}
}