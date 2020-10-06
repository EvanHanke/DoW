using UnityEngine;

public class KillOnAnimEnd : MonoBehaviour {

	MyAnimation myAnim;


	void Awake () {
		myAnim = GetComponentInChildren<MyAnimation>();
	}

	void Update () {
		if (myAnim.playing == false){
			GameObject.Destroy(this.gameObject);
		}
	}
}
