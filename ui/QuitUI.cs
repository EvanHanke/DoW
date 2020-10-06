using UnityEngine;

public class QuitUI : MonoBehaviour {

	string[] labels = {"No", "Yes"};
	UIList options;
	RectTransform selector;
	GameObject a;

	bool x = false;

	void Awake(){
		selector = GameObject.Find("Selector2").GetComponent<RectTransform>();
		a = GameObject.Find("QuitOption");
		options = new UIList();
		options.Init(2, a, 40f, selector, labels);
	}

	void Update(){
		if(x) return;

		if (MyInput.GetState("UP", true) == 'p'){
			options.Dinc();
			AudioLoader.PlayMenuBlip();
		}
		if(MyInput.GetState("DOWN", true) == 'p'){
			AudioLoader.PlayMenuBlip();
			options.Inc();
		}



		if (MyInput.GetState("Z", true) == 'r'){
			int selected = options.GetSelected();
			AudioLoader.PlayMenuSelect();
			switch(selected){
			case 0: 
				GameObject.Destroy(gameObject);
				break;
			case 1:
				x = true;
				SaveFiler.SaveGame();
				CameraEfx.FadeOut(3f, Quit);
				break;
			}
		}

		if (MyInput.GetState("SHIFT", true) == 'p'){
			GameObject.Destroy(gameObject);
		}
	}

	void Quit(){
		Application.Quit();
	}

}
