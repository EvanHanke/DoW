using UnityEngine;
using System.Collections;

public class BladeEfx : MonoBehaviour {

	//Fall down 1 unit, then blink and dissapear
	bool fall = false;
	bool blink = false;
	SpriteRenderer spr;
	Vector3 scale;
	public int blinks = 10;

	void Awake () {
		spr = GetComponentInChildren<SpriteRenderer>();
		transform.position += Vector3.up; //assuming you spawn at ground level
		scale = transform.localScale;
		transform.localScale = new Vector3(scale.x, 0f, scale.z);
		StartCoroutine("Fall");
	}

	void Update(){
		if (fall && !blink){
			StartCoroutine("Blink");
			blink = true;
		}
	}

	IEnumerator Fall() {
		for (int i = 0; i < 9; i++){
			transform.position -= new Vector3(0f, 0.1f, 0f);
			transform.localScale += new Vector3(0f, scale.y/10f, 0f);
			yield return new WaitForSeconds(0.01f);
		}
		fall = true;
	}

	IEnumerator Blink(){
		for(int i = 0; i < blinks; i++){
			spr.enabled = !spr.enabled;
			yield return new WaitForSeconds(0.1f);
		}
		GameObject.Destroy(this.gameObject);
	}
}
