using UnityEngine;

public class MovementAnim : MonoBehaviour {

	/*
	 * automatically picks an animation by name depending on current move direction
	 * 
	 */ 

	public enum Dir {Up, Right, Down, Left, None};
	Dir currDir;
	Dir prevDir;

	MyAnimation[] anims;
	MyAnimation currAnim;
	SpriteRenderer mySprite;
	Rigidbody myRB;
	Vector3 pos, prevPos;

	public bool walking;

	void Start(){
		pos = prevPos = transform.position;
		currDir = prevDir = Dir.None;
		anims = GetComponentsInChildren<MyAnimation>();
		mySprite = GetComponentInChildren<SpriteRenderer>();
		currAnim = GetFromName("Idle");
		walking = false;
	}


	void FixedUpdate () {
		prevPos = pos;
		pos = transform.position;
		prevDir = currDir;
	
		Vector3 delta = Vector3.Normalize( pos - prevPos);
		float d_x = delta.x;
		float d_z = delta.z;

		if(!walking){
			if  (currDir != Dir.None){
				currDir = Dir.None;
				if(currAnim != GetFromName("Idle")){
					currAnim = GetFromName("Idle");
					currAnim.PlayAnim();
				}
			}
			return;
		}


		if(d_x == 0f && d_z == 0f){
			currAnim.StopAnimOnNextExit();
			//Debug.Log("stopping");
			return; //no change
		}
		else if (!currAnim.playing) currAnim.playing  =true;

		if(Mathf.Abs(d_x) > Mathf.Abs(d_z)){
			if (d_x < 0f && currDir != Dir.Left){
				ChangeDir(Dir.Left);
			}
			else if (d_x > 0f && currDir != Dir.Right){
				ChangeDir(Dir.Right);
			}
		}
		else{
			if (d_z > 0f && currDir != Dir.Up){
				ChangeDir(Dir.Up);
			}
			else if (d_z < 0f && currDir != Dir.Down){
				ChangeDir(Dir.Down);
			}
		}
	}

	void ChangeDir(Dir d){
		Debug.Log("changing anim");
		currAnim.StopAnim();
		currDir = d;
		switch(d){
		case Dir.Down: currAnim = GetFromName("WalkDown"); break;
		case Dir.Left: mySprite.flipX = false;
			currAnim = GetFromName("WalkSide");
			break;
		case Dir.Right: mySprite.flipX = true;
			currAnim = GetFromName("WalkSide");
			break;
		case Dir.Up:
			currAnim = GetFromName("WalkUp"); break;
		}
		currAnim.SetAnim();
		currAnim.PlayAnim();
	}

	MyAnimation GetFromName(string n){
		foreach(MyAnimation a in anims){
			if(a.animName == n) return a;
		}
		return null;
	}

	public void SetFromName(string n){
		currDir = Dir.None;
		MyAnimation a = GetFromName(n);
		if (a != null){
			currAnim.StopAnim();
			currAnim = a;
			currAnim.SetAnim();
			currAnim.PlayAnim();
		}
	}
}
