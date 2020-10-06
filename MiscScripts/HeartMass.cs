using UnityEngine;

public class HeartMass : MonoBehaviour {

	public float delay = 2f;

	public void Update(){
		if(delay > 0f){
			delay -= Time.deltaTime;
			if(delay<=0f){
				Rigidbody rb = GetComponent<Rigidbody>();
				if (PlayerStats.me.karma >= 0){
					rb.mass = 0.5f;
				}
				else rb.mass = 2f;
			}
		}
	}

}
