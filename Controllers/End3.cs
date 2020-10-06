using UnityEngine;

public class End3 : MonoBehaviour {

	void Awake(){
		GameObject player = GameObject.Find("Player");
		player.transform.parent = GameObject.Find("TheGlobe").transform;
		GameObject.Destroy(GameObject.Find("ENDING"));
	
		player.GetComponent<PlayerMovement>().enabled = true;
		player.GetComponent<Interactor>().nearest = null;
		player.GetComponent<Rigidbody>().useGravity = true;
		player.GetComponent<PlayerController>().enabled = true;
		player.GetComponent<PlayerStats>().enabled = true;
		UIController.me.RemoveLabel();
		player.GetComponent<Interactor>().enabled = true;                           
		foreach(SpriteRenderer s in player.GetComponentsInChildren<SpriteRenderer>(true)){
			s.enabled = true;
		}
	}
}
