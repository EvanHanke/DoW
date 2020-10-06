using UnityEngine;

public class Chase : MovementBehavior {

	//Chase moves towards the player for a duration

	float movetime;
	float timer;
	Vector3 direction;

	public Chase(Rigidbody rb, MovementMachine mm, float timer) : base(rb, mm){
		movetime = timer;
	}

	public override void Start(){
		Debug.Log("starting");
		direction = PlayerController.me.transform.position - myRB.transform.position;
		direction.Normalize();
		timer= movetime;
		//myRB.velocity = new Vector3(direction.x, myRB.velocity.y, direction.y);
	}

	public override bool Move(){
		timer -= Time.deltaTime;
		Vector3 pos = myRB.transform.position+(direction*Time.deltaTime*myMover.speed);
		if(IsValidPos(pos))
			myRB.MovePosition(pos);
		return (timer <= 0);
	}
}
