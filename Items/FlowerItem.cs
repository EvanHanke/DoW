using UnityEngine;

public class FlowerItem : MiscItem {

	public int state;

	public override bool OnUse(){
		Vector3 playerpos = PlayerController.me.transform.position + new Vector3(0f, 0.09f, 0f);

		//if the player is not grounded, nothing happens
		if (!Physics.Raycast(playerpos, Vector3.down, 0.1f)) return false;

		if (droppablePrefab != null){
			//create object and place in the scene
			GameObject go = GameObject.Instantiate(droppablePrefab, playerpos, Quaternion.identity, Zone.currentSubZone.addedPrefabs.transform);
			go.GetComponent<StateSaver>().my_time = TimeTracker.me.day;
			go.GetComponent<Flower>().SetState(state);

			//make player crouch for half a second
			PlayerController.me.Crouch(0.5f);
		}
		return true;
	}
}
