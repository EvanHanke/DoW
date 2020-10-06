using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC_menu{
	public bool enabled = false;
	[TextArea(3,3)]
	public string[] intro_msg;
	public NPC_DTree[] groups;

	[HideInInspector]
	public int current_active;
	[HideInInspector]
	public NPC2 myNPC;

	public Dialogue2 GenerateDialogue(){
		Dialogue2 d = new Dialogue2();
		d.allLines = new Line2[1];
		d.allLines[0] = new Line2("");

		if(intro_msg != null)
			d.allLines[0].message = intro_msg;
		else{
			string[] d_s = {"?"};
			d.allLines[0].message = d_s;
		}

		d.allLines[0].branches = new Branch2[groups.Length];
		for(int i = 0; i < groups.Length; i++){
			Dialogue2 b_d = myNPC.GetDialogue( groups[i].GetCurrent() );
			d.allLines[0].branches[i] = new Branch2();
			d.allLines[0].branches[i].text = b_d.menu_question;
			d.allLines[0].branches[i].d_pointer = b_d.dName;
		}

		return d;
	}
}

[System.Serializable]
public class NPC_DTree{
	public string group_name;

	public string default_option;
	public D_Option[] alternate_options;

	public string GetCurrent(){
		string r_d = default_option;
		foreach(D_Option d in alternate_options){
			if (d.condition.Check()){
				r_d = d.dialogue;
			}
		}
		return r_d;
	}
}

[System.Serializable]
public class D_Option{
	public CheckCondition condition;
	public string dialogue;
}