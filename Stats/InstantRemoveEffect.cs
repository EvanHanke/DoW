using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRemoveEffect : MonoBehaviour {

	public string e_name;

	void Start(){
		Character c = PlayerStats.myStats;
		Effect x = null;
		foreach(Effect e in c.GetAllEffects()){
			if(e.effectName == e_name){
				x = e;
				break;
			}
		}
		if(x != null){
			c.RemoveEffect(x);
		}

		GameObject.Destroy(gameObject); 
	}
}
