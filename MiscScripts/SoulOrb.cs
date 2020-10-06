using UnityEngine;

public class SoulOrb : MonoBehaviour {

	public GameObject soul;

	public void Start(){
		OrbDetector o_d = OrbDetector.me;
		if(o_d != null){
			if (o_d.Check(transform)){
				GameObject.Instantiate(soul, transform.parent).transform.position = transform.position;
				GameObject.Destroy(gameObject);
			}
		}
	}
}
