using UnityEngine;

public class GroundFollower : MonoBehaviour {
	Vector3 pos, prevpos;
	public Transform ground;
	float checkTimer = 0.2f;

	public void Start(){
		Check1();

	}


	public void FixedUpdate(){
		if(ground != null){
			prevpos = pos;
			pos = ground.position;
			Vector3 delta = pos - prevpos;
			transform.position += delta;
		}

		if(checkTimer <= 0f){
			Check1();
			checkTimer = 0.2f;
		}

	}

	public void OnCollisionEnter(Collision c){
		Check(c.collider);
	}

	public void OnCollisionLeave(Collision c){
		if(c.collider.transform == ground){
			ground = null;
		}
	}

	void Check(Collider c){
		
		Debug.Log(name + " checking " + c.name);
		if(c.bounds.center.y+c.bounds.extents.y-0.1f < transform.position.y){
			//if(CheckGrounded.Check(transform.position)){
			Debug.Log(name + " is grounded " + c.name);
			if(ground != c.transform){
				ground = c.transform;
				pos = prevpos = ground.transform.position;
				transform.position = new Vector3(transform.position.x, 
					c.bounds.center.y+c.bounds.extents.y-0.05f, transform.position.z);
			}
				//}
		}
	}


	void Check1(){
		RaycastHit[] hit;
		hit = Physics.RaycastAll(transform.position+Vector3.up*1.5f, Vector3.down, 3f);
		if(hit!=null){
			RaycastHit highest = new RaycastHit();
			foreach(RaycastHit h in hit){
				if(!h.collider.isTrigger)
				if (highest.collider == null || h.point.y > highest.point.y) highest = h;
			}
			if(highest.collider != null){
				Collider c = highest.collider;
				Check(c);
			}
			else ground = null;
		}
	}
}
