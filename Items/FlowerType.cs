using UnityEngine;

[CreateAssetMenu]
public class FlowerType : ScriptableObject {

	public FlowerItem grownItem, witheredItem, clippedItem;
	//public PlantableItem seedItem;
	public Item shears;
	public int bounty; //number of times this object can be sheared
	public int harvest_xp = 1;
}
