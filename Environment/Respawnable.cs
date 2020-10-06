using UnityEngine;

public class Respawnable : MonoBehaviour {
	public GameObject prefab;
	public bool gone = false;

	void Start(){
		if(gone == false){
			GameObject go = GameObject.Instantiate(prefab, transform);
			go.transform.position = transform.position;
		}
	}

}
