using UnityEngine;

public class BobUpDown : MonoBehaviour {

	bool bobbing = true;
	Vector3 ogPos;
	public float distance = 0.1f;
	public float phase = 0;
	float myTime;

	void Awake(){
		ogPos = transform.localPosition;
	}

	void FixedUpdate () {
		if (bobbing){
			myTime +=Time.deltaTime;
			float a = myTime + phase;
			transform.localPosition = ogPos + new Vector3(0, (Mathf.Sin(a) * distance) - (distance/2f), 0);
		}

	}

	void OnPause(){
		bobbing = false;
	}
	void OnUnpause(){
		bobbing = true;
	}
}
