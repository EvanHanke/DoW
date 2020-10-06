using UnityEngine;
using UnityEngine.UI;

public class UserInputter : MonoBehaviour {

	Text input;
	string currText;
	bool clearFlag = false;
	public Spell registerSpell;
	public delegate void OnEnter();
	public OnEnter onRegister, onCancel, onExit;
	public delegate void ReturnVal(string s);
	public ReturnVal onReturn;

	public void Clear(){
		currText = "";
	}

	void Awake(){
		input = GetComponentsInChildren<Text>()[1];
		currText = "";
		clearFlag = false;
	}

	void OnDestroy(){
		if(onExit != null)onExit();
	}

	void Update(){
		if (!clearFlag){
			clearFlag = true;
			return;
		}

		if (MyInput.GetState("SHIFT") == 'p'){
			if (onCancel!=null){
				onCancel();
				onCancel = null;
			}
			GameObject.Destroy(gameObject);
		}
			
		foreach (char c in Input.inputString){
			if (c == '\b') // has backspace/delete been pressed?
			{
				if (currText.Length != 0)
				{
					currText = currText.Substring(0, currText.Length - 1);
				}
			}
			else if ((c == '\n') || (c == '\r')) // enter/return
			{
				if(onReturn != null){
					onReturn(currText);
					onReturn = null;
					GameObject.Destroy(gameObject);
				}
				else if (registerSpell == null)
					Check(currText);
				else Register(currText);
			}
			else if (char.IsLetterOrDigit(c) || c == ' ' || char.IsPunctuation(c) || char.IsSeparator(c))
			{
				currText += c;
			}
		}

		input.text = currText + "_";
	}

	void Check(string s){
		s = s.ToUpper();
		Color color = Color.white;
		//Recipe.CheckPhrase(currText);       //Check summoning data
		if (SpellChecker.CheckSpell(currText)) color = Color.green;  //Check spell data
		FloatingTextSpawner.SpawnText(currText, color);
		GameObject.Destroy(gameObject);
		MenuController.destroyFlag = true;
	}

	void Register(string a){
		a = a.ToUpper();
		if (SpellChecker.me.NameTaken(a) == false){
			registerSpell.learned = true;
			registerSpell.key = a;
			onRegister();
			onRegister = null;
			registerSpell = null;
			GameObject.Destroy(gameObject);
			FloatingTextSpawner.SpawnText(currText, Color.green);
		}
		else{
			PushMessage.Push("You have already used this name!");
		}
	}
}

