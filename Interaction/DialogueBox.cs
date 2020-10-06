using UnityEngine;

public class DialogueBox{
	public bool freeze = false;
	public bool auto = false;
	TextSelectorUI textSelector;
	UIController ui;
	TextBox textBox;
	Line[] tree;
	Line line;
	Line prevLine;

	//pass dialogue tree into the box on creation
	public DialogueBox(Line[] tree, UIController uic){
		this.tree = tree;
		line = tree[0];
		ui = uic;
	}

	public void Launch(){
		if (line == null){
			Debug.Log("Trying to launch a dialogue box with no lines");
			return;
		}
		textBox = ui.CreateMsgBox();
		textBox.Print(line);
		prevLine = line;
	}

	public void StepBack(){
		if (textSelector != null)
			GameObject.Destroy(textSelector.gameObject);
		
		line = prevLine;
		textBox.Print(line);
	}

	public bool Loop(){ //returns true on completion
		if(!freeze);
		if (MyInput.GetState("Z", true) == 'p' || auto == true){
			auto = false;
			//if there is currently a pending question
			if (textSelector != null){
				AudioLoader.PlayMenuSelect();
				line.branches[textSelector.Selected()].choiceEvent.Trigger();
				GameObject.Destroy(textSelector.gameObject);
				int s = line.branches[textSelector.Selected()].pointer;
				FloatingTextSpawner.SpawnText(line.branches[textSelector.Selected()].text);

				if (s < tree.Length && s > 0){
					line = tree[s];
					textBox.Print(line);
				}
				else{
					GameObject.Destroy(textBox.gameObject);
					return true;
				}
			}

			//if all the line is printed
			else if (textBox.isDone()){
				AudioLoader.PlayMenuSelect();
				//if there are no more lines to print, close the textbox and exit loop, or go to next line
				if (line.branches.Length == 0){
					return Destroy();
				}
				else if (line.branches[0].text == ""){
					line = tree[line.branches[0].pointer];
					textBox.Print(line);
				}
				else{
					string[] options = new string[line.branches.Length];
					for(int i = 0; i < line.branches.Length; i++)
						options[i] = line.branches[i].text;
					textSelector = ui.CreateTextSelector(options);
				}
			}

			//finish the line
			else{
				textBox.QuickComplete();
			}
		}

		return false;
	}

	public bool Destroy(){
		AudioLoader.voice_mod = 1f;
		textBox.Destroy();
		Interactor.me.RefreshLabels();
		return true;
	}
}
