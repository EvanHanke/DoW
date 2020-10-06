using UnityEngine;
using System.Collections;

public class SlimeBall : MonoBehaviour {

	/*
	 * slimeball floats upwards then shoots at player
	 */ 

	float riseTimer = 0f;
	float shootTimer = 0f;
	MyAnimation myAnim;
	Vector3 target;
	public float riseTime = 1.5f;
	public float speed = 2f;
	public float lifetime = 2f;

	public AudioClip myA_C;
	public AudioClip popA_C;


	void Awake () {
		myAnim = GetComponentInChildren<MyAnimation>();
		LocalSfx.PlayFx(transform, myA_C, true, 0.05f);
		Invoke("Init", 0.1f);
	}

	void Init(){
		target = GetComponent<Attack>().target.position + new Vector3(0f, 0.5f, -0.1f);
	}

	void Update(){
		if (GlobalStateMachine.paused) return;

		if (riseTimer < riseTime){
			transform.position += new Vector3(0f, 1f, 0f) * Time.deltaTime;
			riseTimer += Time.deltaTime;
		}
		else if(shootTimer < lifetime){
			transform.position = 
				Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
			shootTimer += Time.deltaTime;
		}
		else if (!myAnim.IsDone() && myAnim.noloop){
			if (!myAnim.playing){
				LocalSfx.PlayFx(transform, popA_C, false, 0.4f);
				myAnim.PlayAnim();
			}
		}
		else{
			GameObject.Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider c){
		if (c.transform == GetComponent<Attack>().target){
			riseTimer = shootTimer = 2f;
		}
	}

	public void AdvanceTime(){
		GameObject.Destroy(gameObject);
	}
}
