using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PushMessage : MonoBehaviour {

	static PushMessage pusher; //allows push message to be referenced without GetComponent in other classes
	Text myText;

	RectTransform rect;
	bool shown = false; //shown flag
	bool ff = true; //finished flag
	float showTime = 1f; //seconds
	float timer;

	public void Awake () {
		pusher = this;
		myText = GetComponentInChildren<Text>();
		rect = GetComponent<RectTransform>();
		Push("Ass");
		Debug.Log(myText.text);
		timer = 0f;
	}
		
	void Update () {

		//remove label when its time is out
		if(myText.text != ""){
			if(rect.anchoredPosition.y != -25f && timer > 0f){
				Move(-1f);
			}
			else if (rect.anchoredPosition.y != 25f && timer <= 0f){
				Move(1f);
			}
			else if(rect.anchoredPosition.y == 25f) myText.text = "";
			else if (timer > 0f){
				timer -= Time.deltaTime;
			}
		}

	}

	void Move(float dir){
		Vector2 newPos = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y+Time.deltaTime*dir*50f);

		if(Mathf.Abs( rect.anchoredPosition.y ) > 25f){
			newPos = new Vector2(rect.anchoredPosition.x, 25f*dir);
		}

		rect.anchoredPosition = newPos;
	}

	public void Push1(string msg){
		myText.text = msg;
		shown = false;
		ff = true;
		timer = showTime;
	}

	public static void Push(string msg){
		if (pusher!=null)
		pusher.Push1(msg);
	}
}
