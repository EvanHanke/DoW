using UnityEngine;
using System.Collections.Generic;
//base class for buffs/debuffs/modifiers

[System.Serializable]
public class Effect {

	//instant effect
	public int healAmt;
	public int restoreMPAmt;

	//durational effect
	public int days = 0; //if the effect lasts a number of days, this takes precendace
	public float duration = 5; //seconds
	public float timeLeft = 5;
	public int stacks;
	public int maxStacks = 3; //default
	public string effectName;

	[TextArea (0,3)]
	public string effectDescription;
	public bool isStackable = true;
	public GameObject particlesPrefab;
	public Modifier[] myModifiers;
	public List<Upgrade> upgrades;

	public float GetTimeLeft() { return timeLeft;}

	public string ToRichString(){
		string a ="";
		if(healAmt > 0)a += TextColorer.ToColor("Heal +" + healAmt +"\n", Color.red);
		if(restoreMPAmt > 0)a += TextColorer.ToColor("Restore Mana +" + restoreMPAmt+"\n", Color.cyan);
		foreach(Modifier m in myModifiers){
			a += m.ToRichString()+"\n";
		}
		if(upgrades!=null)
		foreach(Upgrade u in upgrades){
			a+= TextColorer.ToColor(u.name, u.color) + " " + u.my_modifier.ToRichString()+"\n";
		}
		return a;
	}

	public void ResetTimer(){
		timeLeft = duration;
		Debug.Log(effectName + "time started: " + timeLeft +"s");
	}

	//returns true if it is time for the effect to expire
	public bool UpdateTime(float timeElapsed){

		if(GlobalStateMachine.currentState != GlobalStateMachine.States.paused)
		if(timeLeft > 0)
			timeLeft -= timeElapsed;



		if(duration == 0) return false; //0 duration = permanent
		return (timeLeft > 0)? false : true;
	}

	public Effect(Modifier[] mds){
		myModifiers = mds;
	}

	public void AddStack(){
		stacks++;
	}
	public void ResetStack(){
		stacks = 1;
	}
	public int GetStacks(){return stacks;}
}
