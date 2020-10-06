using UnityEngine;
using UnityEngine.UI;


public class GolemInfo : MonoBehaviour {


	Golem myGolem;
	TextSelectorUI options;
	Text botText;

	string[] optionsA = { "<Name Golem>" , "Bye"};
	string[] optionsB = { "Follow me", "Build something here", "Remove Something", "Bye"};

	bool named;


	public void Set(Golem which){
		myGolem = which;

		if (myGolem.golemName.Length == 0){
			options = UIController.me.CreateTextSelector(optionsA);
			named = false;
		}else{
			options = UIController.me.CreateTextSelector(optionsB);
			named = true;
		}

		botText = GetComponentInChildren<Text>();
		RefreshText();
	}

	public void RefreshText(){
		string s;
		optionsB[0] = (myGolem.currState == Golem.State.Follow)? "Stay Here" : "Follow me";
		if (named){
			s = "Golem " + myGolem.golemName;
			options.Set(optionsB);
		}
		else{
			s = "Unnamed Golem ";
			options.Set(optionsA);
		}
		s += "\nlevel"  +myGolem.level.ToString();
		botText.text = s;
	}

	public void Update(){
		if (MyInput.GetState("Z", true) == 'p'){
			if (named){
				switch(options.Selected()){
				case 0: SetFollow() ; break;
				case 1: Build(); break;
				case 2: GameObject.Destroy(gameObject); break;
				}
			}
			else{
				switch(options.Selected()){
				case 0: NameGolem(); enabled = false; break;
				case 1: GameObject.Destroy(gameObject); break;
 				}
			}
		}
	}

	void SetFollow(){
		if(myGolem.currState == Golem.State.Follow)myGolem.SetState(Golem.State.Idle);
		else myGolem.SetState(Golem.State.Follow);
		GameObject.Destroy(gameObject);
	}
	public void ReEnable(){
		if (this == null) return;
		options.enabled = true;
		enabled = true;
	}

	public void NameGolem(){
		UserInputter uIn = UIController.me.UserInput();
		uIn.onReturn = SetName;
		uIn.onExit = ReEnable;
	}

	public void SetName(string n){
		FloatingTextSpawner.SpawnText("..." + n, Color.green, myGolem.transform);
		myGolem.golemName = n;
		named = true;
		enabled = true;
		options.enabled = false;
		RefreshText();
	}

	public void Build(){
		enabled = false;
		options.enabled = false;
		UserInputter uIn = UIController.me.UserInput();
		uIn.onReturn = BuildObj;
		uIn.onExit = ReEnable;
		botText.text = "Build what?";
	}

	public void BuildObj(string what){
		GameObject g = RecipeLoader.me.GetRecipeObj(what);
		if (g != null){
			GameObject n = GameObject.Instantiate(Prefabber.me.buildParticles, Zone.currentSubZone.addedPrefabs.transform);
			n.transform.position = myGolem.transform.position + Vector3.back;
			n.GetComponent<ObjectBuilder>().Set(g, 10f, 3f);
			myGolem.ob = n.GetComponent<ObjectBuilder>();
			myGolem.SetState(Golem.State.Build);
		}
		GameObject.Destroy(gameObject);
	}

	public void OnDestroy(){
		GameObject.Destroy(options.gameObject);
		GlobalStateMachine.GUnpause();
	}
}
