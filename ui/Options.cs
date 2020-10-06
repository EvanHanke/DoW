using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {
	GameObject label;
	GameObject selector;
	GameObject rebind;

	UIList currentList;
	int timer = 0;
	int state = 0;

	string[] labels;
	string[] others = {"SFX:", "Music:"};

	bool await_bind = false;

	void Awake(){
		label = GameObject.Find("Label");
		selector = GameObject.Find("Selector1");
		rebind = GameObject.Find("Rebind");
		rebind.SetActive(false);
	}

	void Start(){
		Init();
	}

	void Init(){
		labels = Button_labels();
		for(int i = 0; i < labels.Length; i++){
			Button b = MyInput.me.btns[i];
			labels[i] = string.Format("{0, -10}: {1, 10} ", b.formal_name, b.key.ToString());
		}
		currentList = new UIList();
		currentList.Init(7, label, 60f, selector.GetComponent<RectTransform>(), labels);
	}

	string[] Button_labels(){
		labels = new string[MyInput.me.btns.Length];
		for(int i = 0; i < labels.Length; i++){
			Button b = MyInput.me.btns[i];
			labels[i] = string.Format("{0, -10}: {1, 10} ", b.formal_name, b.key.ToString());
		}
		return labels;
	}

	string[] AltLabels(){
		others[0] = string.Format("{0, -10}: {1, 10} ", "Mute SFX", AudioLoader.muted);
		others[1] = string.Format("{0, -10}: {1, 10} ", "Mute Music", MusicLoader.me.muted);
		return others;
	}

	void Update(){
		if(await_bind == false && timer == 0){
			if (MyInput.GetState("DOWN") == 'p'){
				currentList.Inc();
			}
			else if (MyInput.GetState("UP") == 'p'){
				currentList.Dinc();
			}

			if (MyInput.GetState("RIGHT", true) == 'p' || 
				MyInput.GetState("LEFT", true) == 'p'){
				ToggleMenu();
				AudioLoader.PlayMenuSelect();
			}

			if (MyInput.GetState("Z", true) == 'r'){
				if(state == 0){
					await_bind = true;
					rebind.SetActive(true);
					AudioLoader.PlayMenuSelect();
				}
				else{
					ToggleAudio();
					AudioLoader.PlayMenuSelect();
				}
			}
			if (MyInput.GetState("SHIFT", true) == 'r'){
				GameObject.Destroy(gameObject);
			}
		}
		else if (timer > 0) timer--;
			
	}

	void OnGUI(){
		if(await_bind){
			Event e = Event.current;
			if(e != null && e.isKey || e.shift){
				KeyCode new_key = e.keyCode;

				if(e.shift) new_key = KeyCode.LeftShift;

				Button b = MyInput.me.btns[currentList.GetSelected()];

				foreach(Button ob in MyInput.me.btns){
					if(ob.key == new_key){
						ob.key = b.key;
					}
				}

				b.key = new_key;
				currentList.RefreshList(Button_labels());
				rebind.SetActive(false);
				await_bind = false;
				MyInput.Clear();
				timer = 10;
				AudioLoader.PlayMenuSelect();
			}
		}
	}

	void ToggleAudio(){
		if(currentList.GetSelected() ==0){
			AudioLoader.me.Toggle();
		}
		else{
			MusicLoader.me.Toggle();
		}
		currentList.RefreshList(AltLabels());
	}

	void ToggleMenu(){
		if(state == 0){
			currentList.SetLabels(AltLabels(), true);
			state = 1;
		}
		else{
			currentList.SetLabels(labels, true);
			state = 0;
		}
	}

}
