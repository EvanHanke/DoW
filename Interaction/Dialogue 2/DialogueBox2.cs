using UnityEngine;

public class DialogueBox2{
	public static DialogueBox2 active;
	public bool freeze = false;
	public bool auto = false;
	Dialogue2 myDialogue;
	TextSelectorUI textSelector;
	UIController ui;
	TextBox textBox;
	Line2[] tree;
	Line2 line;
	Line2 prevLine;
	int msg;
	NPC2 my_NPC;
	bool l_m = false;
	//pass dialogue tree into the box on creation
	public DialogueBox2(Dialogue2 dialogue, UIController uic){
		SetMyD(dialogue);
		ui = uic;
		CameraFollower.me.SetLine(line);
		Debug.Log(uic.name);
	}

	public void SetMyD(Dialogue2 dialogue){
		myDialogue = dialogue;
		tree = dialogue.allLines;
		line = tree[0];

		active = this;

		myDialogue.CheckBranches();
	}

	public void Launch(NPC2 npc, bool low_msg){
		l_m = low_msg;
		Debug.Log(l_m);
		Launch(npc);

	}

	public void Launch(NPC2 npc){
		Launch();
		my_NPC = npc;
	}

	public void Launch(){
		if (line == null){
			Debug.Log("Trying to launch a dialogue box with no lines");
			return;
		}
		if(ui == null) Debug.Log("ui not instantiaed");
		Debug.Log(l_m);
		textBox = ui.CreateMsgBox(l_m);
		Show(0);
		prevLine = line;
		myDialogue.ResetLines();
		msg = 0; //current message of the line
	}

	public void StepBack(){
		if (textSelector != null)
			GameObject.Destroy(textSelector.gameObject);

		line.shown = false;
		line = prevLine;
		line.shown = false;
		Show(0);
	}

	public bool Loop(){ //returns true on completion
		if(freeze) return false;

		//trigger dialogue event on line start
		if (!line.shown){
			myDialogue.TriggerEvent(line.dEvent);
			line.shown = true;
		}

		if (MyInput.GetState("Z", true) == 'p' || auto == true){
			auto = false;
			//if there is currently a pending question
			if (textSelector != null){
				//play sound, destroy menu
				AudioLoader.PlayMenuSelect();
				GameObject.Destroy(textSelector.gameObject);

				//grab nextline pointer
				int p = textSelector.Selected();

				//Change Dialogue
				if(line.branches[p].d_pointer.Length > 0){
					Dialogue2 new_d = my_NPC.GetDialogue(line.branches[p].d_pointer);
					if(new_d != null){
						//print the message
						SetMyD(new_d);
						new_d.ResetLines();
						Show(0);
						prevLine = line;
						msg = 0;
					}
				}

				//Change Line
				else{
					int s = line.branches[textSelector.Selected()].pointer;
					FloatingTextSpawner.SpawnText(line.branches[textSelector.Selected()].text);

					if (s < tree.Length && s > 0){
						line = tree[s];
						msg = 0;
						Show(0);
						CameraFollower.me.SetLine(line);
					}
					else{
						GameObject.Destroy(textBox.gameObject);
						return true;
					}
				}
			}

			else if (textBox.isDone()){

				return AdvanceMessage();
			}

			//finish the line
			else{
				textBox.QuickComplete();
			}
		}

		return false;
	}

	void Show(int index){
		string s = line.message[index];
		if(myDialogue.textReplace != null && myDialogue.textReplace.key.Length > 1){
			s = myDialogue.textReplace.check(s);
		}
		textBox.Print(s);
	}

	public bool AdvanceMessage(){
		//if all the lines is printed

		AudioLoader.PlayMenuSelect();

		//check if there is another msg in the line
		if (msg < line.message.Length-1){
			msg++;
			Show(msg);
			return false;
		}

		myDialogue.TriggerLateEvent(line.dEvent);

		//if there are no more lines to print, close the textbox and exit loop, or go to next line
		if (line.branches == null || line.branches.Length == 0 && msg == line.message.Length-1){
			return Destroy();
		}
		else{
			string[] options = new string[line.branches.Length];
			for(int i = 0; i < line.branches.Length; i++)
				options[i] = line.branches[i].text;
			if(options[0].Length > 0){
				textSelector = ui.CreateTextSelector(options);

				if(my_NPC.low_msgbox){
					textSelector.GetComponent<RectTransform>().anchoredPosition += Vector2.up*600f;
				}
			}
			else{
				//auto advance to next message
				if(line.branches[0].pointer > 0){
					line = tree[line.branches[0].pointer];
					msg = 0;
					Show(0);
				}
				else{
					return Destroy();
				}
			}
		}

		return false;
	}

	public bool Destroy(){
		CameraFollower.me.SetLine(null);
		AudioLoader.voice_mod = 1f;
		textBox.Destroy();
		Interactor.me.RefreshLabels();
		my_NPC.RefreshBubbles();
		return true;
	}
}
