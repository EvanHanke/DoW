using UnityEngine;

public class Rot : MonoBehaviour {

	public bool fading = false;
	SpriteRenderer sr;
	float fade_time = 3f;

	void Awake(){
		sr = GetComponentInChildren<SpriteRenderer>();
	}

	void Start(){
		RaycastHit[] hits = Physics.SphereCastAll(transform.position, 15f, Vector3.zero);
		if(hits != null){
			foreach(RaycastHit hit in hits){
				NPC2 n = hit.collider.GetComponent<NPC2>();
				if(n != null){
					n.gameObject.SetActive(false);
				}
			}
		}
	}

	void Update(){
		if(fading){
			fade_time -= Time.deltaTime;
			transform.localScale -= Vector3.one*(Time.deltaTime/3f);
			float a = sr.color.a - (Time.deltaTime/3f);
			if(fade_time < 0f){
				GameObject.Destroy(gameObject);
				PlayerStats.me.karma+=10;
			}
		}
	}
}
