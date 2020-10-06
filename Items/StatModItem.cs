using UnityEngine;

//Any item you can use to add speed modifying effects
[CreateAssetMenu]
public class StatModItem : ConsumableItem{

	public Effect[] effects;

	public override bool OnUse(){
		Character c = GameObject.Find("Player").GetComponent<Character>();
		foreach(Effect e in effects){
			c.AddEffect(e);
		}
		return true;
	}
}
