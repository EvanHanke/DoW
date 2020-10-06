using UnityEngine;

public class InstantEffect : MonoBehaviour {

	public TextReplace t_r;
	public Effect[] efx;

	public void Awake(){
		
	}

	public void Start(){
		foreach(Effect e in efx){
			e.effectName = t_r.check(e.effectName);
			e.effectDescription = t_r.check(e.effectDescription);
		}

		foreach(Effect e in efx){
			PlayerStats.myStats.AddEffect(e);
		}
		GameObject.Destroy(gameObject);
	}
}
