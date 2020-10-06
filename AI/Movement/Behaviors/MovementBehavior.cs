using UnityEngine;

[System.Serializable]
public abstract class MovementBehavior{
	/*
	 * abstract root class for determining AI movement patterns
	 * each child class implements run() differently, which updates position per frame
	 * 
	 */


	protected Rigidbody myRB;
	protected Vector3 myTarget;
	protected MovementMachine myMover;
	protected bool gravity;

	public MovementBehavior(Rigidbody rb, MovementMachine mm){
		myRB = rb;
		myMover = mm;
		gravity = rb.useGravity;
	}

	public abstract void Start();
	public abstract bool Move();

	//helper methods for additional functionality
	public void ChangeAnim(string which){
		myMover.SetAnim(which);
	}

	//helper methods for movement
	public bool IsValidPos(Vector3 pos){
		if(!gravity) return true;
		return (IsGroundedDestination(pos));
	}

	public bool IsGroundedDestination(Vector3 d){
		//tests if there is a solid ground at the destination
		if (Physics.Raycast(d + (Vector3.up), Vector3.down, 2f)){
			return true;
		}
		else return false;
	}

	Vector3 FindFurtherstValidFrom(Vector3 a, Vector3 b){
		float d = Vector3.Distance(a, b);
		//finds the furtherst valid point closest to b
		Vector3 c = b;
		while(!IsGroundedDestination(c) || d <= 0.5f){
			c = Vector3.MoveTowards(a, b, d -= 0.5f);
		}
		return c;
	}

}
