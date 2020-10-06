using UnityEngine;

public class PathFollower : MonoBehaviour {

	public bool loop = true;
	public float speed;
	public Vector3[] path;
	int curr = 0;

	// Use this for initialization
	void Start () {
		curr = 0;
		for(int i = 0 ; i < path.Length; i++){
			path[i] = path[i] + transform.position;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GlobalStateMachine.currentState == GlobalStateMachine.States.paused) return;
		if (transform.position != path[curr]){
			transform.position = Vector3.MoveTowards(transform.position, path[curr], Time.deltaTime * speed);
		}
		else{
			curr = (curr + 1 < path.Length)? curr + 1 : (loop)? 0 : curr;
		}
		
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
