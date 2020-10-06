using UnityEngine;

public class CloudFader : MonoBehaviour {

	Projectile myProj;
	SpriteRenderer mySpr;
	ParticleSystem myPM;

	void Awake(){
		myProj = GetComponent<Projectile>();
		mySpr = GetComponentInChildren<SpriteRenderer>();
		myPM = GetComponentInChildren<ParticleSystem>();
	}

	void Update(){
		float vis = 1f - (myProj.life / myProj.lifetime);

		mySpr.color = new Color(mySpr.color.r, mySpr.color.g, mySpr.color.b, vis);
		ParticleSystem.MainModule module = myPM.main;
		module.startColor = new Color(myPM.main.startColor.color.r, 
			myPM.main.startColor.color.g, 
			myPM.main.startColor.color.b, vis);
	}
}
