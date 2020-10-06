using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue2{
	public string dName;
	public CheckCondition condition;
	public CheckCondition[] subConditions;

	public string menu_question;

	public DialogueEvent[] allDevents;
	public Line2[] allLines;
	public TextReplace textReplace;

	public string[] bubbles;


	public void ResetLines(){
		foreach(Line2 l in allLines){
			l.shown = false;
			l.CheckBranches();
		}
	}
	public void CheckBranches(){
		foreach(Line2 l in allLines){
			l.CheckBranches();
		}
	}

	public void TriggerEvent(string n){
		if(allDevents == null) return;
		foreach(DialogueEvent e in allDevents){
			if (e.inspectorName == n){
				e.Trigger();
			}
		}
	}
	public void TriggerLateEvent(string n){
		if(allDevents == null) return;
		foreach(DialogueEvent e in allDevents){
			if (e.inspectorName == n){
				e.LateTrigger();
			}
		}
	}

}

[System.Serializable]
public class TextReplace{
	public string key;
	public enum Replace {e_level, stat, spell_name};
	public string stat;
	public Replace with;

	public string check(string s){
		if (!string.IsNullOrEmpty(key) && s.Contains(key)){
			return s.Replace(key, Calculate());
		}
		return s;
	}
		

	string Calculate(){
		string r = "";
		switch(with){
		case Replace.e_level: 
			Environment.me.UpdateEnvironment(false);
			r = Environment.me.GetCurrentHealth().ToString();
			break;
		case Replace.spell_name:
			Spell[] ss = SpellChecker.GetLearnedSpells();
			r = "Unkown Spell";
			foreach(Spell s in ss){
				if(s.name == stat) r = s.key;
			}
			break;
		}
		return r;
	}
}

[System.Serializable]
public class Line2{
	[TextArea(0,4)]
	public string[] message;
	public Branch2[] branches;
	[HideInInspector]
	public Dialogue2 myDiag;
	//[HideInInspector]
	public bool shown = false;
	public Vector3 cameraTilt, cameraOffset;

	public string dEvent;

	public void CheckBranches(){
		if(branches == null) return;
		foreach(Branch2 b in branches){
			b.CheckAlts();
		}
	}

	public Line2(string s){
		message = new string[1];
		message[0] = s;
	}
}

[System.Serializable]
public class Branch2{
	public string text;
	public int pointer;
	public AltBranch2 alternate;

	public string d_pointer; //dialogue pointer for auto_starting a new dialogue
	[HideInInspector]
	public string d_group; //for updating the npc menu

	public void CheckAlts(){
		AltBranch2 b = alternate;
		if (b != null && b.condition.Check()){
			text = b.text;
			pointer = b.pointer;
			d_pointer = b.d_pointer;
		}
	}
}
[System.Serializable]
public class AltBranch2{
	public CheckCondition condition;
	public string text;
	public int pointer;
	public string d_pointer;
}