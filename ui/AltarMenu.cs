using UnityEngine;

public class AltarMenu : MonoBehaviour {

	UIList options;
	int selected;
	string[] labels = {"level up", "save", "exit"};
	GameObject label;
	RectTransform selector;

	public GameObject levelUpPrefab;
	GameObject open;
	public int zone_target;

	void Awake () {
		options = GetComponent<UIList>();
		label = transform.GetChild(2).gameObject;
		selector = transform.GetChild(3).GetComponent<RectTransform>();
		options.Init(3, label, 40, selector, labels);
	}
	
	void Update () {
		if (open != null) return;

		if (MyInput.GetState("UP", true) == 'p'){
			options.Dinc();
		}
		if(MyInput.GetState("DOWN", true) == 'p'){
			options.Inc();
		}
		if (MyInput.GetState("SHIFT", true) == 'p'){
			GameObject.Destroy(gameObject);
		}
			
		selected = options.GetSelected();

		if (MyInput.GetState("Z", true) == 'p'){
			switch(selected){
			case 0: open = GameObject.Instantiate(levelUpPrefab, transform.parent); break;
			case 1: Save(); break;
			case 2: GameObject.Destroy(gameObject); break;
			}
		}
	}

	void Save(){
		if(SaveFiler.activeSave != null){
			SaveManager.me.SaveZone(Zone.currentZone);
			SaveFiler.activeSave.saveTarget = zone_target;
			SaveFiler.SaveGame();
			PushMessage.Push("Game Saved");
		}else{
			PushMessage.Push("ERROR: Save Failed!");
			Debug.Log(SaveFiler.save_files.ToString());
		}
	}

	void OnDestroy(){
		GlobalStateMachine.GUnpause();
	}
}
