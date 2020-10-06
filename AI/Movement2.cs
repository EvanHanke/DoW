using UnityEngine;

[System.Serializable]
public class Movement2{

	//user defined instance specifics
	public AudioClip mySFX;

	public float duration;
	public string textBubble;
	int saycount = 0;

	public string animationName;
	public GameObject spawnPrefab;

	public enum MoveClass {Walk, Hop, Fly};
	public enum MoveType {Wait, Wander, Chase, Flee, Patrol, Hover};

	public MoveClass myMoveClass;
	public MoveType myMoveType;

	public float hover_height=1;
	public SavedPath myPath;


	//main methods
	//SET conforms entities to the correct state
	public void Set(Entity e){
		e.SetAnim(animationName);
		Vector3 dir = e.move_vector;

		switch(myMoveType){
		case MoveType.Wander:
			dir = GetRandomLoc();
			break;
		case MoveType.Chase:
			if(e.target == null) break;
			//Debug.Log("chase " + e.target.position);
			dir = (e.target.position - e.transform.position);
			//Debug.Log("dir " + dir);
			break;
		case MoveType.Flee:
			if(e.target == null) break;
			dir = (e.target.position - e.transform.position) * -1f;
			break;
		case MoveType.Patrol:

			break;
		case MoveType.Hover:
			if(e.target == null) break;
			//Debug.Log("chase " + e.target.position);
			dir = (e.target.position - (e.transform.position) + (Vector3.up * hover_height));
			break;
		}

		if(myMoveClass == MoveClass.Hop){
			if(CheckGrounded.Check(e.transform.position, e.my_BC)){
				Vector3 hop_force = ((Vector3.up));
				e.my_RB.AddForce((dir+hop_force)*5f* e.my_RB.mass, ForceMode.Impulse);
			}
		}

		dir.Normalize();
		e.move_vector = dir;
		Vector3 normal = CheckGrounded.Check1(e.transform.position, e.my_BC);
		if (normal != Vector3.zero){
			Vector3 temp = Vector3.Cross(normal, e.move_vector);
			e.move_vector = Vector3.Cross(temp, normal);
		}

		if(spawnPrefab != null){
			SpawnPrefab(spawnPrefab, e);
		}
		if(mySFX != null){
			if(e.GetComponent<AudioSource>() == null)
			LocalSfx.PlayFx(e.transform, mySFX, true, 0.5f);
			else{
				e.GetComponent<AudioSource>().clip = mySFX;
				e.GetComponent<AudioSource>().Play();
			}
		}
	}
		
	public void Move(Entity e){
		if(e.my_RB.isKinematic) return;
		if(myMoveType == MoveType.Patrol){
			if(myPath.points == 0) return;
			EntityPath ep = e.GetComponent<EntityPath>();
			if(ep==null){
				ep = e.gameObject.AddComponent<EntityPath>();
				ep.Set(myPath);
			}
			if(Vector3.Distance( e.transform.position , ep.path[ep.curr]) < 0.5f) ep.Advance();
			Vector3 dir = ep.path[ep.curr] - e.transform.position;
			e.move_vector = dir.normalized;

			Vector3 new_pos = (e.transform.position + (e.move_vector * Time.deltaTime * e.stats.GetModStat("speed") *e.statSheet.speed));
			if(CheckBlocked(new_pos, e).collider != null){
				ep.Advance();
			}
		}

		Vector3 destinationVector = Vector3.zero;
		if(myMoveType == MoveType.Wait){
			//do nothing case
			return;
		}
		else {


			Vector3 new_pos = (e.transform.position + (e.move_vector * MoveAmt(e) ));	//check if position free
			RaycastHit hit = CheckBlocked(new_pos, e);
			bool blocked = (hit.collider != null);
			bool grounded = ((CheckGrounded.Check(new_pos, e.my_BC) &
				CheckGrounded.Check(e.transform.position, e.my_BC)) 
				|| myMoveClass == MoveClass.Fly || e.target.position.y < e.transform.position.y);
			bool move = (!blocked && grounded);


			if(move){
				e.my_RB.MovePosition(new_pos);

				//Debug.Log("moving");
				return;
			}


			else if (blocked){
				e.move_vector = Vector3.Reflect(hit.normal, e.move_vector);
			}

		}

	}

	float MoveAmt(Entity e){
		return Time.deltaTime * e.stats.GetModStat("speed") * e.statSheet.speed;
	}

	RaycastHit CheckBlocked(Vector3 new_pos, Entity e){
		RaycastHit hit;
		if (Physics.BoxCast(e.transform.position, e.my_BC.size, new_pos-e.transform.position, 
			out hit, Quaternion.identity, MoveAmt(e))){
			if(hit.collider!=e.my_BC && hit.transform != e.target){
				return hit;
			}
		}
		return new RaycastHit();
	}

	//helper methods
	Vector3 GetRandomLoc(){
		float randX, randZ;
		randX = (Random.value-0.5f)*2f;
		randZ = (Random.value-0.5f)*2f;
		return new Vector3(randX, 0f, randZ);
	}


	public void SpawnPrefab(GameObject spawn, Entity e){
		if (spawn != null){
			GameObject go = GameObject.Instantiate(spawn);
			Attack ak = go.AddComponent<Attack>();
			go.transform.position = e.transform.position;
			ak.c = e.stats;
			ak.direction = e.move_vector;
			ak.target = e.target;
			go.transform.SetParent(e.transform.parent, true);
		}
	}
}

[System.Serializable]
public class SavedPath{
	public int points;
	public float radius;
	public Vector3 start_dir;
}
