using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour {

	public static MainMenuUI me;
	public GameObject TitleCard;
	public GameObject MenuCard;
	public GameObject LoadSaveCard;
	public GameObject NewSaveCard;
	public GameObject openCard;
	public GameObject Selector;
	public GameObject credits;
	RectTransform Background;

	GameObject[] xfader;
	bool paused = false;
	bool openback = false;

	int target = 0;
	int current_card = 0;
	int current_selection = 0;


	void Start () {
		Background = transform.GetChild(0).GetComponent<RectTransform>();
		Background.localScale = new Vector3(1f, 0f, 1f);
		xfader = new GameObject[2];
	}


	//main update
	void Update () {
		if(paused) return;

		if(MyInput.GetState("Z") == 'p' || MyInput.GetState("ENTER") == 'p'){
			AudioLoader.PlayMenuSelect();
			if(current_card == 0){
				openback = true;
				xfader[0] = TitleCard;
				xfader[1] = MenuCard;
				target = 1;
				StartCoroutine("AnimBack");
				StartCoroutine("CrossFade", xfader);
			}
			if(current_card == 1){
				if(current_selection == 0){
					target = 2;
					xfader[0] = MenuCard;
					xfader[1] = LoadSaveCard;
					StartCoroutine("CrossFade", xfader);
				}
				else if (current_selection == 1){
					target = 3;
					xfader[0] = MenuCard;
					xfader[1] = NewSaveCard;
					StartCoroutine("CrossFade", xfader);
				}
				else if (current_selection == 2){
					target = 3;
					xfader[0] = MenuCard;
					xfader[1] = credits;
					StartCoroutine("CrossFade", xfader);
				}
			}
		}

		if(MyInput.GetState("SHIFT") == 'p'){
			AudioLoader.PlayMenuCancel();
			if(current_card == 1){
				openback = false;
				xfader[1] = TitleCard;
				xfader[0] = MenuCard;
				target = 0;
				StartCoroutine("AnimBack");
				StartCoroutine("CrossFade", xfader);
			}
			else{
				xfader[0] = openCard;
				xfader[1] = MenuCard;
				target = 1;
				StartCoroutine("CrossFade", xfader);
			}
		}

		//move main selector
		if((current_card == 1) && MyInput.GetState("UP", true) == 'p'){
			current_selection = (current_selection - 1 >= 0)? current_selection - 1 : 2;
			UpdateSelector();
		}
		else if((current_card == 1) && MyInput.GetState("DOWN", true) == 'p'){
			current_selection = (current_selection + 1 <= 2)? current_selection + 1 : 0;
			UpdateSelector();
		}

	}
		
	//helper functions
	void UpdateSelector(){
		RectTransform rt = Selector.GetComponent<RectTransform>();
			rt.anchoredPosition = 
				new Vector2(rt.anchoredPosition.x, 20f - (current_selection * 35f));
		AudioLoader.PlayMenuBlip();
	}


	IEnumerator AnimBack(){
		if(openback){
			while(Background.localScale.y < 1f){
				Background.localScale += Vector3.up*Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			Background.localScale = Vector3.one;
		}
		else{
			while(Background.localScale.y > 0f){
				Background.localScale -= Vector3.up*Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			Background.localScale.Scale(new Vector3(1f, 0f, 1f));
		}
	}

	IEnumerator CrossFade(GameObject[] objs){
		paused = true;

		CanvasGroup c_a = objs[0].GetComponent<CanvasGroup>();
		CanvasGroup c_b = objs[1].GetComponent<CanvasGroup>();
		c_b.gameObject.SetActive(true);
		c_b.alpha = 0f;

		float timer = 0f;
		while(timer < 1f){
			c_a.alpha -= Time.deltaTime;
			c_b.alpha += Time.deltaTime;
			timer += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		c_a.alpha = 0f;
		c_b.alpha = 1f;
		c_a.gameObject.SetActive(false);
		current_card = target;
		openCard = c_b.gameObject;

		paused = false;
	}

}
