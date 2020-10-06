using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighteningSpell : MonoBehaviour {
	public GameObject lightning_prefab;
	public float timer = 3f;
	public int number = 3;
	GameObject[] attacks;

	public AudioFile sound;

	Attack a;

	void Start(){
		attacks = new GameObject[number];
		for(int i = 0; i < number; i++){
			attacks[i] = GameObject.Instantiate(lightning_prefab, Zone.currentZone.transform, false);
			float xOffset = Mathf.Sin(i/(float)number * Mathf.PI*2f)*1.5f;
			float zOffset = Mathf.Cos(i/(float)number * Mathf.PI*2f)*1.5f;
			attacks[i].transform.position = transform.position + new Vector3(xOffset, 0f, zOffset);
		}

		LocalSfx.PlayAudioFile(sound, transform);
	}

	void Update(){
		if(GlobalStateMachine.paused) return;
		if(a == null){
			a = GetComponent<Attack>();
			if(a!=null && a.c != null)
				foreach(GameObject g in attacks){
					g.GetComponent<Damager>().immune.Add(a.c.statSheet.archetype);
					g.GetComponent<Damager>().caster = a.c;
				}
		}

		if(timer > 0f){
			timer -= Time.deltaTime;
			int c = 0;
			foreach(GameObject g in attacks){
				float offset = (float) c / (float) number * Mathf.PI*2f;
				float xOffset = Mathf.Sin((timer/1f * Mathf.PI*2) + offset);
				float zOffset = Mathf.Cos((timer/1f * Mathf.PI*2) + offset);
				Vector3 rot_offset = new Vector3(xOffset, 0f, zOffset) * Time.deltaTime * 5f;
				c++;
				Vector3 movedelta = g.transform.position - transform.position;
				g.transform.position += (movedelta*Time.deltaTime + rot_offset);
			}
		}
		else{
			foreach(GameObject g in attacks){
				GameObject.Destroy(g);
			}
			GameObject.Destroy(gameObject);
		}
	}
}
