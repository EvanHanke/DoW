using UnityEngine;

public class FallAttack : MonoBehaviour {
	float timer;
	public float height = 4f;
	void Start(){
		transform.position = GameObject.Find("Player").transform.position + Vector3.up*height;
	}

	void Update(){
		if(GlobalStateMachine.paused == false){
			transform.position += Vector3.down * Time.deltaTime * 6f;
			timer += Time.deltaTime;
			if(timer > 3.5f) GameObject.Destroy(gameObject);
		}

	}

}
