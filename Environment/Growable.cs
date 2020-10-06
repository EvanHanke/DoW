using UnityEngine;

[CreateAssetMenu]
//Item class for all plant items
public class Growable : ConsumableItem {
	public enum G  {budding, grown, clipped, withered};
	public int bounty;
	public int state;
	public int harvest_xp;
	public GrowState[] life_cycle;
	public Sprite clippedSprite;

	public override bool OnUse(){
		Vector3 playerpos = PlayerController.me.transform.position + new Vector3(0f, 0.09f, 0f);

		//if the player is not grounded, nothing happens
		if (!Physics.Raycast(playerpos, Vector3.down, 0.1f)) return false;

		//if a plant is already here
		if (!Environment.me.PosFree(playerpos)){
			PushMessage.Push("Something else is already planted here...");
			return false;
		}

		if (droppablePrefab != null){
			//create object and place in the scene
			GameObject go = GameObject.Instantiate(droppablePrefab, playerpos, Quaternion.identity, Zone.currentSubZone.addedPrefabs.transform);
			go.GetComponent<StateSaver>().my_time = TimeTracker.me.day;
			go.GetComponent<Flower>().g = this;
			go.GetComponent<Flower>().SetState(state);

			//make player crouch for half a second
			PlayerController.me.Crouch(0.5f);
		}
		AudioLoader.PlaySound("dirt2", 1f, true, 0.4f);
		return true;
	}

	public string GetNameFromState(int state){
		return life_cycle[state].name;
	}
	public Sprite GetSpriteFromState(int state){
		if (state >= 0)
		return life_cycle[state].worldSprite;
		else
			return clippedSprite;
	}
}

[System.Serializable]
public class GrowState{
	public Sprite worldSprite;

	public string name; //i.e budding, grown, withered
	public Growable.G myState;
}
