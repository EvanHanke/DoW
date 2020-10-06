using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC2 : InteractionScript {

	public bool asleep;
	public bool rot_sensitive = false;
	//general
	public float voiceMod = 1f;
	public string npcName;
	public NPC_menu myMenu;
	public Dialogue2[] myDialogues;
	public bool low_msgbox;

	int currentDialogue = 0;
	public int timesInteracted = 0;

	public string initial_dialogue;

	//bubbles
	float timer;
	public Color bubbleColor = Color.white;
	string[] bubbles;
	int curr;
	//

	Sleepable sleepy;
	public void SetSleep(){ 
		ss.Save("s", "1");
		asleep = true; 
	}
	public void SetWake() {
		ss.Save("s", "0");
		asleep = false; 
	}

	//dialogue
	[HideInInspector]
	public DialogueBox2 dBox2;

	//saver
	StateSaver ss;

	void Awake(){
		ss = gameObject.AddComponent<StateSaver>();
		ss.onLoad = Load;

		sleepy = gameObject.AddComponent<Sleepable>();
		sleepy.onSleep += SetSleep;
		sleepy.onWake += SetWake;

		base.Awake();
	}
	public void Load(){
		currentDialogue = ss.my_state;
		timesInteracted = ss.my_time;
		string s = ss.Get("s");
		if(s == "1") sleepy.onSleep();
		else if(s == "0") sleepy.onWake();
	}

	void Start() {
		myMenu.myNPC = this;
		if (isPrimary){
			GetComponent<Interactable>().SetScript(this);
		}
		curr = 0;
		timer= 0f;
		gameObject.AddComponent<Respawner>();


		foreach(Dialogue2 d2 in myDialogues){
			foreach(DialogueEvent d_e in d2.allDevents){
				d_e.myNPC = this;
			}
		}

		if(asleep){
			sleepy.FallAsleep();
		}

		Invoke("RefreshBubbles", 0.1f);
	}

	public void AdvanceTime(){
		Invoke("RefreshBubbles", 0.1f);
	}

	public void RefreshBubbles(){
		curr = 0;
		bubbles = GetCurrDialogue().bubbles;
	}

	public override string LabelDesc(){
		if (timesInteracted == 0) return "???";
		else return npcName;
	} 

	//on interact (launch dialogue)
	public override void OnInteract(){
		if(!asleep){
			GlobalStateMachine.GPause();
			AudioLoader.voice_mod = voiceMod;
			timesInteracted++;
			ss.my_time++;
			//Launch dialogue box
			GetComponentInParent<GlobalStateMachine>().Pause();
			Dialogue2 show_d = null;

			if(myMenu.enabled){
				show_d = myMenu.GenerateDialogue();
			}
			else if(initial_dialogue.Length > 0 && timesInteracted == 1){
				show_d = GetDialogue(initial_dialogue);
			}
			else{
				show_d = GetCurrDialogue();
			}
			dBox2 = new DialogueBox2(show_d, ui);
			dBox2.Launch(this, low_msgbox);

			Interactor.me.RefreshLabels();
		}

		else{
			ui.PrintMsg(npcName + " is deep within a dream.");
		}
	}

	Dialogue2 GetCurrDialogue(){
		Dialogue2 d = myDialogues[0];
		foreach(Dialogue2 d2 in myDialogues){
			if(d2.condition.Check()){
				bool a = true;
				if(d2.subConditions.Length > 0){
					foreach(CheckCondition c in d2.subConditions){
						if (!c.Check()) a = false;
					}
				}
				if(a) d = d2;
			}
		}
		return d;
	}

	void Update(){
		
		//update dialogue
		if (dBox2 != null && dBox2.Loop()){
			GetComponentInParent<GlobalStateMachine>().Unpause();
			dBox2 = null;
		}

		//update bubbles
		if(bubbles != null)
		if(GlobalStateMachine.paused == false && (bubbles.Length > 0 || asleep)){
			timer += Time.deltaTime;
			if (timer > 2f){
				timer = 0f;
				if(!asleep){
					FloatingTextSpawner.SpawnText(bubbles[curr], bubbleColor, transform);
					curr = (curr < bubbles.Length-1)? curr + 1 : 0 ;
				}
				else{
					FloatingTextSpawner.SpawnText("ZZZ...", bubbleColor, transform);

				}
			}
		}
	}

	public Dialogue2 GetDialogue(string n){
		for(int i = 0; i < myDialogues.Length; i++){
			if (myDialogues[i].dName == n){
				return myDialogues[i];
			}
		}
		return null;
	}

	public int GetDialoguePointer(string n){
		for(int i = 0; i < myDialogues.Length; i++){
			if (myDialogues[i].dName == n){
				return i;
			}
		}
		return 0;
	}

	//change dialogue helpers
	public void ChangeDialogue(int which){
		currentDialogue = which;
	}

	public void ChangeDialogue(string s){
		for(int i = 0; i < myDialogues.Length; i++){
			if (myDialogues[i].dName == s){
				ChangeDialogue(i);
			}
		}
	}
}
