using UnityEngine;
using UnityEngine.UI;


public class DifficultySelection : MonoBehaviour {

	Text description;
	GameObject selector;
	GameObject option_A;

	GameObject diff, controls;
	GameObject X, Y;
	int curr;

	public GameObject next_slide;

	UIList options;

	string[] names = {"Explorer", "Gamer"};
	string[] descs = {"Creatures will not attack unless attacked.", "Creatures will attack when you approach."};

	public void Start(){
		diff = GameObject.Find("A");
		controls = GameObject.Find("B");
		X = GameObject.Find("X");
		Y = GameObject.Find("Y");
		curr = 0;
		Y.SetActive(false);
		diff.SetActive(false);
	}

	void InitMenu(){
		description= GameObject.Find("Description").GetComponent<Text>();
		selector = GameObject.Find("Selector");
		option_A = GameObject.Find("Option A");

		options = new UIList();
		options.Init(2, option_A, 80f, selector.GetComponent<RectTransform>(), names);
	}

	public void Update(){
		if(curr == 1){
			if(MyInput.GetState("DOWN", true) == 'p'){
				options.Inc();
				UpdateDesc(options.GetSelected());
			}
			if(MyInput.GetState("UP", true) == 'p'){
				options.Dinc();
				UpdateDesc(options.GetSelected());
			}
		}
		if(MyInput.GetState("Z", true) == 'p' || MyInput.GetState("ENTER", true) == 'p'){
			AudioLoader.PlaySound("M3M7", 1.3f, false, 0.6f);
			if(curr ==1 ){
				CameraEfx.me.FadeInOut1(3f, Change);
			}
			else{
				diff.SetActive(true);
				controls.SetActive(false);
				InitMenu();
				curr = 1;
			}
		}
	}

	public void UpdateDesc(int i){
		description.text = descs[i];
		X.SetActive(i == 0);
		Y.SetActive(i != 0);
	}

	public void Change(){
		gameObject.SetActive(false);
		next_slide.SetActive(true);
	}
}
