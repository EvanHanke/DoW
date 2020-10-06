using UnityEngine;

public class Golem : InteractionScript {

	public enum State {Idle, Follow, Build};
	public State currState;
	public string golemName;
	public int level;
	public GameObject golemUI;
	public ObjectBuilder ob;
	Rigidbody myRB;
	MovementAnim myMoveAnim;

	void Start(){
		myRB = GetComponent<Rigidbody>();
		myMoveAnim = GetComponentInChildren<MovementAnim>();
		myMoveAnim.walking = false;
	}

	public override string LabelDesc(){
		return (golemName.Length > 0)? golemName : "Golem";
	}

	public override void OnInteract ()
	{
		GlobalStateMachine.GPause();
		GameObject.Instantiate(golemUI, GameObject.Find("UI").transform).GetComponent<GolemInfo>().Set(this);
	}

	public void SetState(State s){
		currState = s;
		if (s == State.Follow){
			myMoveAnim.walking = true;
		}
		else if (s == State.Build){
			myMoveAnim.walking = false;
			myMoveAnim.SetFromName("Build");
		}
		else{
			myMoveAnim.SetFromName("Idle");
		}
	}

	public void FixedUpdate(){
		if (GlobalStateMachine.paused == false){
			switch(currState){
			case State.Idle: Idle(); break;
			case State.Build: Build(); break;
			case State.Follow: Follow(); break;
			}
		}
	}

	public void Build(){
		if (ob == null){
			SetState(State.Idle);
			return;
		}
	}

	public void Idle(){
		myMoveAnim.walking = false;
	}

	public void Follow(){
		Vector3 playerpos = player.transform.position;
		if(Vector3.Distance(playerpos, transform.position) > 2f){
			Vector3 direction = Vector3.Normalize(playerpos - transform.position)*Time.deltaTime;
			myRB.MovePosition(transform.position + direction);
		}
	}


}
