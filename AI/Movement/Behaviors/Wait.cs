using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : MovementBehavior {

	//used to either stand still and not do anything or optionally spawn a prefab

	float waittime;
	float timer;

	public Wait(Rigidbody rb, MovementMachine mm, float time) : base (rb, mm){
		waittime = time;
	}
	public override void Start(){

		timer = waittime;
	}
	public override bool Move(){
		myRB.velocity = new Vector3(0f, myRB.velocity.y, 0f);
		timer -= Time.deltaTime;

		if (timer <= 0f)
			return true;
		else
			return false;
	}
}
