using UnityEngine;

public class PatientPathFollower : MonoBehaviour {

	public bool dont_reset = false;
	public float speed;
	public Vector3[] path;
	Vector3 target;

	int curr = 0;
	int startingPoint = 0;
	bool moving = false;
	bool l = false;

	StateSaver saver;
	public AudioClip sfx;
	AudioSource a_s;

	bool e = false;

	void Awake(){
		
		e = false;
	}

	void Start(){
		Setup();
		PlayerController.me.onReset += Reset;
	}

	void Setup () {
		for(int i = 0 ; i < path.Length; i++){
			path[i] = path[i] + transform.position;
		}


		a_s = LocalSfx.PlayFx(transform, sfx, true, 0.2f, 0.6f);
		a_s.Stop();

		if(!dont_reset)
			FindClosest();
		target = transform.position;
	}

	void FindClosest(){
		float d = 100f;
		int nearest = 0;
		for(int i = 0; i < path.Length; i++){
			float a = Vector3.Distance(GameObject.Find("Player").transform.position, path[i]);
			if (a < d) {
				d = a;
				nearest = i;
			}
		}
		startingPoint = nearest;
		curr = nearest;
		transform.position = path[nearest];
	}

	void OnDestroy(){
		PlayerController.me.onReset -= Reset;
	}

	public void Reset(){
		FindClosest();
		moving = false;
		transform.position = path[startingPoint];
		curr = startingPoint;
	}

	public void LoadState(){
		/*
		if (l) return;
		l = true;
		Setup();
		curr = saver.my_state;
		startingPoint = curr;
		transform.position = path[curr];
		Debug.Log("PATH FOLLOW STARTING = " + curr);
		*/
	}

	void Update () {
		if (GlobalStateMachine.currentState == GlobalStateMachine.States.paused) return;

		if(moving == true){
			if (transform.position != target){
				if(a_s.isPlaying == false) a_s.Play();

				transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			}
			else{
				if(a_s.isPlaying == true) a_s.Stop();
				moving = false;
				curr = NextPath(curr);
			}
		}

		if(e && PlayerMovement.me.ground != GetComponent<Collider>()){
			e = false;
		}
		else if (!e && PlayerMovement.me.ground == GetComponent<Collider>()){
			target = path[NextPath(curr)];
			moving = true;
			e = true;
		}

	}

	int NextPath(int c){
		return (c + 1 < path.Length)? c + 1 : 0;
	}


	void OnDrawGizmos(){
		if (path != null){
			for(int i = 0; i < path.Length; i++){
				Gizmos.DrawIcon(path[i]  + transform.position, "pathmarker.png", true);
				//UnityEditor.Handles.Label(path[i], i.ToString());

				Gizmos.color = Color.red;
				if (i+1 < path.Length){
					Gizmos.DrawLine(path[i]  + transform.position , path[i+1] + transform.position);
				}
				else{
					Gizmos.DrawLine(path[i]  + transform.position , path[0] + transform.position);
				}
			}
		}
	}
}
