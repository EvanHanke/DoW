using UnityEngine;

[System.Serializable]
public class DialogueEvent{
	public string inspectorName;
	//All possibilities
	public ChangeZone changeZone;
	public string newDialogue; //must have npc interactor attached to work
	public Quest quest;
	public int quest_stage;
	public Item removeItem;
	public int removeAmt = 1;
	public Item giveItem;
	public int giveAmt = 1;
	public Spell registerSpell;
	public Trade trade;
	public GameObject PlayCutscene;
	public bool turn_aggro = false;
	public GameObject spawn_prefab;
	public int add_karma;
	public SetFlag set_flag;
	public SetDayCounter day_timer;
	public NPC2 myNPC;
	public string register_look;
	//
	[HideInInspector]
	public Dialogue myDialogue; //set my the dialogue scripty obj
	[HideInInspector]
	public Dialogue2 myDialogue2;

	ItemBuyer iBuy;

	public void Trigger(){

		if(registerSpell != null){
			UserInputter i = UIController.me.UserInput();
			i.registerSpell = registerSpell;
			i.onCancel = OnCancel;
			i.onRegister = OnRegister;
			myNPC.enabled = false;
			myNPC.dBox2.freeze = true;
			Debug.Log("LEARN SPELL");
			GlobalStateMachine.GPause();
		}

		if(trade.sell != null){
			Debug.Log("Trade EVEnt");
			GameObject go = GameObject.Instantiate(UIController.me.buySellItemPrefab, UIController.me.transform, false);
			ItemBuyer buyer = go.AddComponent<ItemBuyer>();
			buyer.Set(go, trade.sell, trade.buy, trade.sellAmount, trade.buyAmount);
			buyer.onCancel = OnCancel2;
			buyer.onComplete = OnCancel2;
			iBuy = buyer;
			myNPC.enabled = false;
			myNPC.dBox2.freeze = true;
		}
		if(PlayCutscene != null){
			myNPC.enabled = false;
			myNPC.dBox2.freeze = true;
			CameraEfx.FadeOut(3f, ChangeScene);
		}

		if(add_karma != 0){
			PlayerStats.me.karma+= add_karma;
		}
	}
	public void LateTrigger(){
		if (quest != null){
			quest.AddQuest();
			QuestTracker.AdvanceQuest(quest, quest_stage);
		}
		if (newDialogue.Length > 0){
			myNPC.ChangeDialogue(newDialogue);

		}
		if(removeItem != null){
			PlayerStats.playerInv.RemoveItem(removeItem, removeAmt);
		}
		if(giveItem != null){
			if(giveAmt == 0) giveAmt = 1;
			PlayerStats.playerInv.AddItem(giveItem, giveAmt);
			PlayerController.me.Display(giveItem.image, 1.5f);
			AudioLoader.PlaySound("M3", .9f, true, 0.6f);
			PushMessage.Push("You received " + giveItem.name);
		}
		if(changeZone != null && changeZone.zone.Length > 0){
			CameraEfx.FadeInOutB(1f, ChangeZone, GlobalStateMachine.GUnpause);
			myNPC.dBox2.Destroy();
		}
		if(turn_aggro){
			Entity e = myNPC.GetComponent<Entity>();
			if(e != null){
				e.BecomeAware();
			}
		}
		if(spawn_prefab != null){
			GameObject.Instantiate(spawn_prefab, Zone.currentZone.addedPrefabs.transform);
		}
		if(!string.IsNullOrEmpty(set_flag.key)){
			GlobalStater.me.SetState(set_flag.key, set_flag.value);
		}
		if(!string.IsNullOrEmpty(day_timer.key)){
			TimeTracker.AddCounter(day_timer.key, day_timer.days);
		}
		if(!string.IsNullOrEmpty(register_look)){
			ScreenShotter.me.RegisterLook(register_look);
		}
	}

	public void ChangeScene(){
		GameObject.Find("TheGlobe").SetActive(false);
		GameObject gg = GameObject.Instantiate(PlayCutscene);
		gg.transform.SetParent(GameObject.Find("Game").transform);
		Backstory b = gg.GetComponentInChildren<Backstory>(true);
		b.gameObject.SetActive(true);
		b.OnComplete = ResumeFade;
	}

	public void ChangeZone(){
		GlobalStateMachine.GPause();
		Zone.ChangeZone(changeZone.zone, changeZone.subzone, changeZone.target);
	}

	public void OnRegister(){
		Resume();
		myNPC.dBox2.AdvanceMessage();
		TextBox.freeze = false;
	}
	public void OnCancel(){
		Resume();
		TextBox.freeze = false;
		Debug.Log("SPELL CANCEL");
		myNPC.dBox2.StepBack();
	}
	public void OnCancel2(){
		
		ResumeDiag();
		TextBox.freeze = false;
		Debug.Log("ITEM CANCEL");
		myNPC.dBox2.AdvanceMessage();
	}
	public void ResumeDiag(){
		if(iBuy != null)
			GameObject.Destroy(iBuy.gameObject);
		
		//myDialogue.enabled = true;
		Resume();
	}
	void Resume(){
		myNPC.enabled = true;
		myNPC.dBox2.freeze = false;
	}
	void ResumeFade(){
		Resume();
		CameraEfx.me.fadeAmt = 1f;
		CameraEfx.me.isAnimating = true;
		CameraEfx.me.fadeToBlack = false;
		CameraEfx.me.fadeTime = 1f;
		CameraEfx.me.fadeInOut = true;
	}
}

[System.Serializable]
public class RemoveItem{
	public Item i;
	public int amt;
}
[System.Serializable]
public class ChangeZone{
	public string zone, subzone;
	public int target;
}
[System.Serializable]
public class Trade{
	public Item sell;
	public int sellAmount;
	public Item buy;
	public int buyAmount;
}
[System.Serializable]
public class SetFlag{
	public string key, value;
}
[System.Serializable]
public class SetDayCounter{
	public string key;
	public int days;
}