using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JelliCatch : MonoBehaviour {

	public string entityName;
	public Item addItem;
	public AudioFile on_catch;

	SpriteRenderer[] sprs;
	bool show = true;
	float duration = 1f;
	float blinkTimer = 0.2f;
	bool captured = false;

	void Awake(){
		sprs = GetComponentsInChildren<SpriteRenderer>();
	}

	void Update(){
		if(duration  > 0f && !GlobalStateMachine.paused){
			duration -= Time.deltaTime;
			blinkTimer -= Time.deltaTime;
			if(blinkTimer <= 0f){
				blinkTimer = 0.2f;
				Toggle();
			}
		}
		else if (duration <= 0f){
			GameObject.Destroy(gameObject);
		}
	}

	void Toggle(){
		show = !show;
		foreach(SpriteRenderer sr in sprs){
			sr.enabled = show;
		}
	}

	void OnTriggerEnter(Collider c){
		Entity e = c.GetComponent<Entity>();
		if(e!=null && captured == false){
			if(e.statSheet.name == entityName){
				transform.position = e.transform.position;
				e.transform.SetParent(transform);
				e.enabled = false;
				AudioLoader.me.PlayFile(on_catch);
				captured = true;
				duration = 2f;
			}
		}
	}

	void OnDestroy(){
		if(captured){
			PushMessage.Push("You caught a " + entityName);
			PlayerController.me.AcquireItem(addItem);
		}
	}

}
