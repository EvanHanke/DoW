using UnityEngine;

public class PartialRotator : MonoBehaviour {

	public float xAmt, yAmt, zAmt, phase;
	public float speed = 1f;
	Vector3 starting_rot;
	float timer;

	void Start(){
		starting_rot = transform.localRotation.eulerAngles;
		timer = 0;
	}

	void Update(){
		if (!GlobalStateMachine.paused){
			timer += Time.deltaTime;
			transform.localEulerAngles = starting_rot + new Vector3(FromAmt(xAmt), FromAmt(yAmt), FromAmt(zAmt));
		}
	}

	float FromAmt(float amt){
		return amt*Mathf.Sin(timer * Mathf.PI*speed + (phase*Mathf.PI));
	}
}
