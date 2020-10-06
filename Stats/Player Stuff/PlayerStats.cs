using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//fun stuff
public class PlayerStats : MonoBehaviour {

	public static PlayerStats me;
	public static Character myStats;

	//for tracking nearest interactable
	Interactor interactor;

	//track xp/level
	public int xp, maxxp, level, skillPoints, karma;

	static EquipSlot xEquip;
	static EquipSlot cEquip;

	public static EquipSlot getXEquip() { return xEquip; }
	public static EquipSlot getCEquip() { return cEquip; }

	public static Inventory playerInv;

	public static WearableItem headEquip;
	public static WearableItem bodyEquip;

	void Awake(){
		xEquip = new EquipSlot();
		cEquip = new EquipSlot();
		if(MyInput.me!= null){
			xEquip.my_button = MyInput.me.GetButton("X");
			cEquip.my_button = MyInput.me.GetButton("C");
		}
		playerInv = GetComponent<Inventory>();
		interactor = GetComponent<Interactor>();

		xp = 0;
		maxxp = 5;
		level = 1;
		skillPoints = 3;
		karma = 10;

		me = this;

		myStats = GetComponent<Character>();
		myStats.myController = this;
	}

	void Start(){
		xEquip.my_button = MyInput.me.GetButton("X");
		cEquip.my_button = MyInput.me.GetButton("C");
	}

	public static void AddXp(int amount){
		me.LocalAddXp(amount);
	}

	void LocalAddXp(int amount){
		/*
		xp += amount;
		FloatingTextSpawner.SpawnText(amount.ToString() + "xp", Color.white, transform);

		while (xp >= maxxp){
			xp = xp - maxxp;
			LevelUp();
		}

		EZStatInfo.UpdateStats();
		*/
	}

	void LevelUp(){
		FloatingTextSpawner.SpawnText("Level Up!", Color.yellow, transform, "levelup3");
		level++;
		skillPoints += 3;
		maxxp = CalcMaxXp();
	}

	int CalcMaxXp(){
		return (int)(5f * Mathf.Pow(2f, (float) level));
	}

	public void UpdateUI(){
		EZStatInfo.UpdateStats();
	}

	public void OnDamage(Damager d){
		CameraFollower.me.Shake(0.2f, 0.3f);
		float r_variation = (Random.value / 10f) + 1.4f;
		AudioLoader.PlaySound("hurt1", r_variation, true, 0.3f);
		if (myStats.GetStat("hp") == 0){
			OnDeath();
		}
		UpdateUI();
	}

	void OnDeath(){
		GlobalStateMachine.GPause();
		GetComponent<Rigidbody>().Sleep();
		foreach(MyAnimation a in PlayerController.me.GetComponentsInChildren<MyAnimation>()) a.StopAnim();
		PlayerController.me.PlayCry();
		PlayerController.Blink(1f);
		Zone.currentZone.gameObject.SetActive(false);
		UIController.me.CreateDeathPanel();
	}

	public static void WearItem(WearableItem i){
		bool a = true;
		if(i.mySlot == WearableItem.Slot.Head){
			if (headEquip != null){
				headEquip.equipped = "";
				foreach(Effect e in headEquip.effects) myStats.RemoveEffect(e);
			}
			if (headEquip != i){
				headEquip = i;
				i.equipped = "[Head]";
				foreach(Effect e in headEquip.effects) myStats.AddEffect(e);
			}
			else{
				headEquip = null;
				a = false;
			}
			Debug.Log("wear item head");
		}
		else{
			Debug.Log("wear item");
			if (bodyEquip != null){
				bodyEquip.equipped = "";
				foreach(Effect e in bodyEquip.effects) myStats.RemoveEffect(e);
			}
			if (bodyEquip != i){
				bodyEquip = i;
				i.equipped = "[Body]";
				foreach(Effect e in bodyEquip.effects) myStats.AddEffect(e);
			}
			else{

				bodyEquip = null;
				a = false;
			}
		}
		if(a){
			AudioLoader.PlaySound("puton", 1f, true, 0.5f, false);
		}
		else{
			AudioLoader.PlaySound("takeoff", 1f, true, 0.5f, false);
		}
		EZStatInfo.UpdateStats();
	}


	public static void SetEquip(EquipSlot e, UsableItem itm){
		if(e.itm != null) e.itm.equipped = "";
		if(e.itm == itm){
			AudioLoader.PlaySound("takeoff", 1f, true, 0.5f, false);
			ClearEquip(e);
			return;
		}


		e.itm = itm;
		e.myName = itm.name;
		e.mySprite = itm.image;
		e.onUse = new EquipSlot.OnUseDelegate(itm.OnUse);
		e.whatami = itm;


		//cannot equip the same item twice
		if (e.myName.Equals(xEquip.myName) && e.myName.Equals(cEquip.myName)){
			Debug.Log("c same as x");
			EquipSlot otherSlot = (Object.ReferenceEquals(e, xEquip))? cEquip : xEquip;
			ClearEquip(otherSlot);
		}
		AudioLoader.PlaySound("puton", 1f, true, 0.5f, false);
		itm.equipped = "["+e.my_button.key.ToString()+"]";
		EZStatInfo.me.UpdateEquips();
	}

	public static void ClearEquip(EquipSlot e){
		if(e.itm != null) e.itm.equipped = "";
		e.mySprite = null;
		e.myName = null;
		e.onUse = null;
		e.whatami = null;
		e.itm = null;
		EZStatInfo.me.UpdateEquips();
	}

	public static void UseEquip(EquipSlot e){
		
		if (e.onUse != null){
			//if use is successful
			if (e.onUse()){
				UseItem(e.itm);
			}

		}
	}

	public static void UseItem(Item u){
		//if x is consumable
		if (u.consumable){
			playerInv.RemoveItem(u, 1);
			EZStatInfo.me.UpdateEquips();
		}
		UsableItem u_i;
		if (u is UsableItem){
			u_i = (UsableItem) u;

			if(u is StatModItem || u is CastableItem || u is Book)
				PlayerController.me.Use(u_i.useDuration);

			if (u_i.addOnUseItem != null){
				playerInv.AddItem(u_i.addOnUseItem, u_i.addAmt);
			}

			if(u_i.soundFileName.Length > 0){
				AudioLoader.PlaySound(u_i.soundFileName, 1f, true, 0.5f);
			}
		}
	}

	public static void RefreshEquips(){
		Item itm = (Item) xEquip.whatami;
		if (itm != null && playerInv.QuantityOf(itm) == 0){
			ClearEquip(xEquip);
		}
		itm = (Item) cEquip.whatami;
		if (itm != null && playerInv.QuantityOf(itm) == 0){
			ClearEquip(cEquip);
		}

		if (me.interactor.GetNearest() != null){
			//update labels in case new item has a new interaction with nearest interactable
			me.interactor.GetNearest().CheckAltScipts();
		}

		if(headEquip != null){
			if(playerInv.QuantityOf(headEquip) == 0) WearItem(headEquip);
		}
		if(bodyEquip != null){
			if(playerInv.QuantityOf(bodyEquip) == 0) WearItem(bodyEquip);
		}
	}

	public static Transform GetNearestTarget(float range){
		Character[] ts = Zone.currentZone.GetComponentsInChildren<Character>();
		Transform closest = null;
		float d = range;
		foreach(Character cc in ts){
			float ad = Vector3.Distance(cc.transform.position, me.transform.position);
			if(ad < d && cc != myStats){
				closest = cc.transform;
				d = ad;
			}
		}
		return closest;
	}

}

public class EquipSlot{
	public Sprite mySprite;
	public string myName;
	public delegate bool OnUseDelegate();
	public OnUseDelegate onUse;
	public Object whatami;
	public Item itm;
	public Button my_button;
}

