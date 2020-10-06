using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public enum Directions {front, right, back, left};
	public Directions facing = Directions.front;
	public Directions stored_facing = Directions.front;

	public Rigidbody myRB;
	public BoxCollider myBC;

	public Collider ground; //collider u are standing on
	Vector3 groundPos, prevGroundPos;
	Vector3 ridePos, prevRidePos;
	Vector3 deltaGround;
	Vector3 normal;

	MyAnimation[] walkingAnims; //all walking animations
	public MyAnimation walkingAnim; //current walking animation
	MyAnimation prevWalkingAnim; //previous frame's walking animation
	public bool walking = false;
	public float baseSpeed = 4;
	public float speed = 4;

	public static PlayerMovement me;
	public bool jumping = false;

	float saveTimer = 1f;

	public Vehicle ride; //if you are riding something (like a boat);
	bool riding {
		get{ return !(ride == null); }
	}

	public bool input_paused = false;

	//jump
	float baseJumpPower = 90f;
	float jumpPower = 90f;
	int jumps = 0;
	float jump_delay = 0; //disables jumping for a few frames after jumping
	public int maxJumps = 1;
	int bonusJumps = 0;
	bool falling = false;
	Vector3 prev_pos;
	int landed_frames = 0;
	//bool canJump = true;

	public float yvel;
	public float prevyvel;

	//move
	float h, prevh;
	float v, prevv;

	float footstepTimer;


	Character playerChar;
	PlayerController playerController;

	ConstantForce gravity;
	Vector3 gravityConstant = new Vector3(0.0f, -200f, 0.0f);

	public void UpdateStats(){
		speed = baseSpeed * playerChar.GetModStat("speed") * ((playerChar.GetModStat("gravity")+1f) / 2f);
		jumpPower = baseJumpPower * playerChar.GetModStat("jump");
		bonusJumps = playerChar.GetStatBonus("extra_jumps");
		gravity.force = gravityConstant * playerChar.GetModStat("gravity");
	}

	//anim 0 = front, 1 = back, 2 = side
	public MyAnimation AnimFromDir(Directions dir){
		
		walkingAnim.myRenderer.flipX = (dir == Directions.left);

		if (dir == Directions.left || dir == Directions.right)
			return walkingAnims[2];
		else if (dir == Directions.front)
			return walkingAnims[0];
		else 
			return walkingAnims[1];
	}

	//deactivate the from animation and activate the to animation
	void ChangeAnim(MyAnimation from, MyAnimation to){
		bool playing = from.playing;
		from.StopAnim();
		to.SetAnim();
		if (playing) to.PlayAnim();
	}

	void SetJumpAnim(int frame){
		SpriteRenderer s_r = GetComponentInChildren<SpriteRenderer>();
		walkingAnim.StopAnim();
		s_r.sprite = walkingAnim.frames[frame];
	}

	void Awake(){
		me = this;
		myRB = GetComponent<Rigidbody>();
		walkingAnims = GetComponentsInChildren<MyAnimation>();
		walkingAnim = walkingAnims[0];
		playerChar = GetComponent<Character>();
		playerController = GetComponent<PlayerController>();
		facing = Directions.front;
		Physics.gravity *= 2;
		myBC = GetComponent<BoxCollider>();
		footstepTimer = 0f;

		gravity = gameObject.AddComponent<ConstantForce>();
		gravity.force = gravityConstant;
	}

	void OnCollisionEnter(Collision c){
		if(jumping || falling)
			Land();
		
		if (c.collider.gameObject.tag == "Respawn"){
			PlayerController.ResetPos();
			return;
		}
		
	}

	void OnCollisionStay(Collision c){
		if((falling || jumping) && jump_delay <= 0f) 
			Land();
	}

	void OnCollisionLeave(Collision c){
		if (ground == c.collider){
			ground = null;
			Debug.Log("ground is null");
		}
	}

	void PlayFootstep(){
		AudioLoader.PlaySound("simplehit", 1.2f, true, 0.65f);
	}

	//check inputs in update
	void Update(){

		if (GlobalStateMachine.currentState == GlobalStateMachine.States.paused) return;
		if (playerController.display){
			h = v = 0f;
			return;
		}

		//read movement inputs
		prevh = h;
		prevv = v;
		if(input_paused == false){
			if(MyInput.GetState("LEFT") == 'h') h = -1;
			else if (MyInput.GetState("RIGHT") == 'h') h = 1;
			else h= 0;
			if(MyInput.GetState("UP")=='h') v = 1;
			else if(MyInput.GetState("DOWN") == 'h') v = -1;
			else v = 0;
		}
		else{
			h = v = 0f;
		}


		if (riding) return; //following code doesn't apply to vehicles


		//if jump key pressed
		if(jump_delay > 0)jump_delay -= Time.deltaTime;
		if (MyInput.GetState("SPACE", true) == 'p'){
			Jump();
		}

		//read inputs and update facing direction and animation
		prevWalkingAnim = walkingAnim;

		if(input_paused == false && jumping == false && playerController.display == false){
			if (h != 0f && (prevh != h || v == 0f)) facing = (h < 0)? Directions.left : Directions.right;
			if (v != 0f && (prevv != v || h == 0f)) facing = (v < 0)? Directions.front : Directions.back;

			if (!jumping){
				walkingAnim = AnimFromDir(facing);
				if (walkingAnim != prevWalkingAnim){
					ChangeAnim(prevWalkingAnim, walkingAnim);
					footstepTimer = 0f;
				}
			}
		}

	}

	public void Jump(){
		if (jumps < (maxJumps+bonusJumps)){
			myRB.velocity = new Vector3(myRB.velocity.x, 0, myRB.velocity.z);

			Vector3 jump = new Vector3(0, jumpPower, 0);
			Vector3 force = Vector3.zero;
			if(ground != null){
				force = (deltaGround*4f)/Time.deltaTime;
				ground = null;
			}

			myRB.AddForce(jump + force, ForceMode.Impulse);
			jumps++;
			jumping = true;
			SetJumpAnim(1);
			footstepTimer = 0f;
			AudioLoader.PlaySound("jumpy1", .8f + (float) jumps / 10f, true, 0.2f);
			jump_delay = .2f;
		}
	}

	void Land(){
		RaycastHit[] hits;
		hits = Physics.BoxCastAll (myBC.bounds.center,
			Vector3.Scale(myBC.bounds.extents, new Vector3(0.8f, 1f, 0.8f)), 
				Vector3.down, Quaternion.identity,0.1f);
		//Debug.Log("landed");
		//landing
		if (hits.Length > 0){

			foreach(RaycastHit hit in hits){
				if(!hit.collider.isTrigger && hit.collider != myBC ){
					normal = hit.normal.normalized;

					//Debug.Log(c.collider.gameObject.name);
					//Debug.Log(hit.collider.gameObject.name);
					//Debug.Log(hit.collider.gameObject.name);
					if(ground != hit.collider){
						ground = hit.collider;
						groundPos = ground.transform.position;
						AudioLoader.PlaySound("digital_thud", 1f, false, 0.5f);
						//prevGroundPos = groundPos;
					}
					//Debug.Log("ground is " + ground.name);
					if(playerController.display) return;
					jumps = 0;
					SetJumpAnim(0);
					if(jumping){
						if (MyInput.GetState("SPACE") == 'h' && jump_delay <= 0){
							Jump();
							SetJumpAnim(1);
						}
						else{
							
							jumping = false;

						}
					}
					Invoke("CheckSafe", 0.1f);

					break;
				}
			}
		}
		else if (ground != null){
			ground = null;
		}
	}

	void CheckGround(){
		RaycastHit hit;

		if(Physics.Raycast(transform.position+Vector3.up,Vector3.down, out hit, 1.1f)){
			if(ground != hit.collider){
				ground = hit.collider;
				groundPos = ground.transform.position;
				AudioLoader.PlaySound("digital_thud", 1f, false, 0.5f);
				//prevGroundPos = groundPos;
			}
		}
	}

	void OnUnpause(){
		CheckGround();
	}

	void CheckSafe(){
		if(ground != null)
		if(Physics.Raycast(transform.position, Vector3.down, 0.1f)
			&& deltaGround == Vector3.zero
			&& ground.GetComponent<Projectile>() == null){

			PlayerController.me.resetPos = prev_pos;
		}
	}

	//update physics in fixedupdate
	void FixedUpdate () {

		if (GlobalStateMachine.currentState == GlobalStateMachine.States.paused) return;

		//if (playerController.display) return;



		/*
		if(yvel ==0 && prevyvel ==0 && jumps > 0){
			landed_frames ++;
			if(landed_frames > 10){
				Land();
				landed_frames = 0;
			}
		}*/



		//compare vertical velocity with the frame before, relative to ground movement

		//check falling
		if(!jumping && myRB.velocity.y < 0f){
			//Debug.Log("falling ?");
			if(!CheckGrounded.Check(transform.position, myBC)){
				jumping = true;
				jumps++;
				SetJumpAnim(1);
			}
		}

		if (riding){
			prevRidePos = ridePos;
			ridePos = ride.transform.position;
			Vector3 diff = ridePos - prevRidePos;
			if (diff.magnitude < 1f){
				transform.Translate(ridePos - prevRidePos);
			}
		}



		//check to see if ground is moving
		if (ground != null){
			CheckGround();
			prevGroundPos = groundPos;
			groundPos = ground.transform.position;
			deltaGround = groundPos - prevGroundPos;

			if(!riding)
				CameraFollower.me.transform.position += deltaGround;



			//Debug.Log("move ground yes");
		} 


		//Move the player based on movement inputs
		myRB.velocity = new Vector3(0f, myRB.velocity.y, 0f);
		float distance = Time.deltaTime * speed;
		Vector3 movement = new Vector3(h*distance, 0f, v*distance);
		Vector3 newPosition;

		prev_pos = transform.position;
		if (!riding){
			newPosition = transform.position + movement + deltaGround;
			if (Vector3.Magnitude(movement) > 0f && ground != null && !jumping){
				footstepTimer -= Time.deltaTime;
				if(footstepTimer <= 0f){
					PlayFootstep();
					footstepTimer = .6f;
				}
			}
			myRB.MovePosition(newPosition);
		}
		else{
			newPosition = ridePos + movement;
			myRB.MovePosition(newPosition);
		}



		//update the animation & sound
		if(input_paused || playerController.display) return;


		if (Vector3.Distance(Vector3.zero, movement) > 0f && walking == false && !riding && !jumping){
			walkingAnim.PlayAnim();
			walking = true;
			footstepTimer = 0f;
		}

		else if (Vector3.Distance(Vector3.zero, movement) < 0.01f && walking == true && !riding && !jumping){
			walkingAnim.StopAnimOnNextExit();
			walking = false;
		}

		else if (walking == true && jumping == false && !walkingAnim.playing){
			walkingAnim.playing = true;
		}

		prevyvel = yvel;
		yvel = (newPosition.y-prev_pos.y) / Time.fixedDeltaTime;


	}
	void OnDrawGizmos(){
		//if (pos != null){
		//	Gizmos.DrawLine(pos, pos - new Vector3(0f, 0.6f, 0f));
		//}
	}
}
	
