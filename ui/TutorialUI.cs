using UnityEngine;
using UnityEngine.UI;


public class TutorialUI : MonoBehaviour {

	public GameObject text;
	public GameObject sel;
	public GameObject option; 

	Text atext;
	RectTransform asel;

	UIList list;

	string[] options = {"Ok, got it.", "Disable tutorial"};

	public delegate void Fin();
	public Fin onEnd;

	public void Awake(){
		atext = text.GetComponent<Text>();
		asel = sel.GetComponent<RectTransform>();
		list = new UIList();
		Init();
	}

	void Init(){
		list.Init(2, option, 80f, asel, options);
	}

	public void Set(string t){
		atext.text = t;
	}

	void Update(){
		if(MyInput.GetState("UP",true) == 'p'){
			list.Dinc();
		}
		if(MyInput.GetState("DOWN",true) == 'p'){
			list.Inc();
		}
		if(MyInput.GetState("SHIFT",true) == 'p'){
			GameObject.Destroy(gameObject);
		}
		if(MyInput.GetState("Z",true) == 'p'){
			GameObject.Destroy(gameObject);
			if(list.GetSelected() == 1){
				GlobalStater.me.SetState("Tutorial", "0");
			}
		}

	}

	void OnDestroy(){
		onEnd();
	}


}
