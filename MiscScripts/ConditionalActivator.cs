using UnityEngine;

public class ConditionalActivator : MonoBehaviour {

	public CheckCondition my_cond;
	public bool flip_logic;

	public void Start(){
		if (my_cond.Check() == flip_logic){
			gameObject.SetActive(false);
			NPC2 n = GetComponent<NPC2>();
			if(n != null){
				n.rot_sensitive = false;
			}
		}
	}
}
