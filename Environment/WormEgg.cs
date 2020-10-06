using UnityEngine;

public class WormEgg : MonoBehaviour {

	/*
	 * hatcha the worm 
	 */ 

	public GameObject wormPrefab;
	public GameObject brokenEgg;
	LocalGrowArea myGrowArea;

	public void Check(){
		myGrowArea = null;
		foreach(Flower f in Environment.me.flwrs){
			if (Vector3.Distance(f.transform.position, transform.position) < 2f){
				myGrowArea = f.myGrowArea;
			}
		}

		if(myGrowArea != null){
			if (myGrowArea.myNodes.Count >= 5 && myGrowArea.area_health >= 4){
				Hatch();
			}
		}

	}

	void Hatch(){
		AudioLoader.PlaySound("hatchshort", 0.2f, false, 0.3f);
		PushMessage.Push("A worm has been born");
		GameObject.Instantiate(brokenEgg, transform.parent).transform.position = transform.position;
		WormTracker.me.SpawnNewWorm(transform.position);
		GameObject.Destroy(gameObject);
	}
}
