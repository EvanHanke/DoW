using UnityEngine;
using UnityEngine.UI;

public class SaveFileLister : MonoBehaviour {

	RectTransform selector;
	GameObject first_save;
	UIList myList;
	Text header;
	bool a = true;
	int state = 0;

	void OnEnable(){
		if (myList == null){
			selector = transform.GetChild(0).GetComponent<RectTransform>();
			first_save = transform.GetChild(1).gameObject;
			//initialize list
			myList = gameObject.AddComponent<UIList>();
			string[] labels = new string[SaveFiler.saves.Count];
			for(int i = 0; i < labels.Length; i++){
				labels[i] = SaveNameFromObj(SaveFiler.saves[i]);
			}
			myList.Init(4, first_save, 20, selector, labels);
		}
		else{
			UpdateList();
		}
		a=true;
		header = GameObject.Find("Save Header").GetComponent<Text>();
		state = 0;
	}

	void UpdateList(){
		string[] labels = new string[SaveFiler.saves.Count];
		for(int i = 0; i < labels.Length; i++){
			labels[i] = SaveNameFromObj(SaveFiler.saves[i]);
		}
		myList.RefreshList(labels);
	}

	void Update(){
		
		if (MyInput.GetState("X", true) == 'p'){
			if(state == 0 && (myList.allElements != null)){
				state = 1;
				header.text = "Press again to delete file.";
			}
			else if (state == 1){
				AudioLoader.PlayMenuBlip();
				SaveFiler.DeleteSave(myList.GetSelected());
				state = 2;
				Refresh();
			}
		}
		else if(state == 1 && Input.anyKeyDown && !Input.GetKeyDown(MyInput.me.GetButton("X").key)){
			Refresh();
		}

		if(state == 0){
			if (MyInput.GetState("UP", true) == 'p'){
				myList.Dinc();
				myList.RefreshSelector();
				//AudioLoader.PlayMenuBlip();
			}
			else if (MyInput.GetState("DOWN", true) == 'p'){
				myList.Inc();
				myList.RefreshSelector();
				//AudioLoader.PlayMenuBlip();
			}
			else if (MyInput.GetState("Z", true) == 'p'){
				if (myList.allElements != null)
					CameraEfx.FadeInOut(3f, LoadGame);
			}

		}
	}

	public void Refresh(){
		UpdateList();
		state = 0;
		header.text = "Remember...";
	}

	public void LoadGame(){
		SaveFiler.LoadGame(myList.GetSelected());
	}

	string SaveNameFromObj(Save s){
		return string.Format("{0,5}", s.name);
	}
}
