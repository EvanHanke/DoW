using UnityEngine;

public class ClockTicker : MonoBehaviour {

	public float tick;
	float nextTick;

	void Awake(){
		nextTick = Time.time + tick;
	}

	void Update () {
		if(Time.time > nextTick){
			transform.Rotate(0f, 0f, -6f);
			nextTick = Time.time+tick;
		}
	}
}
