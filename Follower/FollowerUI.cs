using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowerUI : MonoBehaviour {

	Follower myFollower;
	TextSelectorUI options;
	Text botText;
	bool enabled = true;
	bool giving = false;
	InvEntry[] items;
	Inventory inv;

	string[] optionsText = { "Use this item!", "Your new nickname is...", "Go home.", "<Close menu>"};
	string[] optionsBText = {"Come with me!", "<Close menu>"};

	void Awake(){
		botText = GetComponentInChildren<Text>();
	}

	public void Set(Follower f){
		myFollower = f;
		if(f.active == true)
			options = UIController.me.CreateTextSelector(optionsText);
		else
			options = UIController.me.CreateTextSelector(optionsBText);

		SetText();
	}

	void SetText(){
		botText.text = string.Format("{0}\nHP: {1}/{2}", myFollower.my_name, myFollower.a.stats.GetStat("hp"), myFollower.a.stats.GetStat("maxhp"));
	}

	void Update(){
		if(!enabled) return;
		if(MyInput.GetState("Z", true) == 'p'){
			AudioLoader.PlayMenuSelect();
			if(!myFollower.active){
				switch(options.Selected()){
				case 0: 
					Recruit();
					break;
				case 1:
					Exit();
					break;
				}
			}
			else if(!giving){
				switch(options.Selected()){
				case 0: 
					Give();
					break;
				case 1: 
					Name();
					break;
				case 2:
					Dismiss();
					break;
				case 3:
					Exit();
					break;
				}
			}
			else{
				if(options.Selected() > 0){
					StatModItem s = (StatModItem) items[options.Selected()-1].itm;
					inv.RemoveItem(s, 1);
					foreach(Effect e in s.effects)
						myFollower.a.stats.AddEffect(e);
				}
				ExitGiveMenu();
			}
		}
		if(MyInput.GetState("SHIFT", true) == 'p'){
			if(!giving) Exit();
			else ExitGiveMenu();
		}
	}

	void Give(){
		inv = GameObject.Find("Player").GetComponent<Inventory>();
		items = inv.GetItems<StatModItem>();
		string[] new_labels = new string[items.Length+1];
		new_labels[0] = "<Return>";
		for(int i = 0; i < new_labels.Length-1; i++){
			new_labels[i+1] = items[i].itm.name;
		}
		GameObject.Destroy(options.gameObject);
		options = UIController.me.CreateTextSelector(new_labels);
		giving = true;
	}

	void Name(){
		enabled = false;
		UserInputter uIn = UIController.me.UserInput();
		uIn.onReturn = SetName;
		uIn.onExit = ReEnable;
	}

	void ReEnable(){
		enabled = true;
	}

	public void SetName(string n){
		myFollower.my_name = n;
		SetText();
	}

	void Recruit(){
		myFollower.active = true;
		myFollower.a.world_ref.transform.parent = PlayerStats.me.transform.parent;
		Exit();
	}

	void Dismiss(){
		FollowerTracker.me.DismissFollower(myFollower);
		Exit();
	}

	void ExitGiveMenu(){
		GameObject.Destroy(options.gameObject);
		options = UIController.me.CreateTextSelector(optionsText);
		giving = false;
	}

	void Exit(){
		GlobalStateMachine.GUnpause();
		GameObject.Destroy(gameObject);
		GameObject.Destroy(options.gameObject);
	}
}
