using UnityEngine;

public class BurstAttack : MonoBehaviour {

	public GameObject prefab;
	public float speed;
	public float distance;
	public int number;
	public float delay;
	public float y_height = 0f;
	public bool add_projectile = true;

	Vector3 target;
	float timer;

	Attack at;

	public void Start(){
		
		timer = 0f;
	}

	public void Update(){
		if(timer <= 0f && GlobalStateMachine.paused == false){
			at = GetComponent<Attack>();

			if(at != null){
				if(at.c != null)
					transform.position = at.c.transform.position;
				else{
					GameObject.Destroy(gameObject);
				}
			}

			timer = delay;
			target = PlayerStats.me.transform.position;
			GameObject a = GameObject.Instantiate(prefab, Zone.currentSubZone.transform);
			Vector3 dir = target - transform.position;
			if(at.c == PlayerStats.myStats){
				dir = PlayerController.me.GetDirection();
			}
			a.transform.position = transform.position+dir.normalized + new Vector3(0f, y_height, 0f);

			if(add_projectile)
				a.AddComponent<SimpleProjectile>().Set(dir, speed, distance);
			
			number--;


			Attack a2 = a.AddComponent<Attack>();
			a2.c = at.c;
			a2.direction = at.direction;
			a2.target = at.target;

			if(number == 0) GameObject.Destroy(gameObject);
		}
		else if (GlobalStateMachine.paused == false){
			timer -= Time.deltaTime;
		}
	}
}
