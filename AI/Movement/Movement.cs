using UnityEngine;

[System.Serializable]
public class Movement{

	public AudioClip mySFX;
	public string name; //name of the class
	public float limit;
	public float timer;
	public string[] saysomething;
	int saycount = 0;

	public string animationName;
	public GameObject spawnPrefab;

	//returns a movement behvaior based on the name
	public MovementBehavior CreateBehavior(Rigidbody rb, MovementMachine myMover){
		name = name.ToLower();
		switch (name){
		case "wander":
			return new Wander(rb, myMover, timer);
		case "randomhop":
			return new RandomHop(rb, myMover, (int) limit);
		case "chase":
			return new Chase(rb, myMover, timer);
		default:
			return new Wait(rb, myMover, timer);
		}
		return null;
	}

	public string Say(){
		if (saysomething.Length == 0) return "";
		string s = saysomething[saycount];
		saycount = (saycount + 1 < saysomething.Length)? saycount + 1 : 0;
		return s;
	}
}
