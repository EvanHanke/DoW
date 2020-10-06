using UnityEngine;

[CreateAssetMenu]
public class CastableItem : UsableItem {

	//an item which spawns a prefab in front of the player

	public GameObject castPrefab;
	public GameObject upgradedPrefab;
	public bool upgraded = false;
	public bool charged = false;
	public int max_charges = 10;
	public int charges;
	public bool disabled = false;
	[TextArea(0, 3)]
	public string upgrade_desc;
	public int upgrade_cost;

	public void Recharge(){
		charges = max_charges;
	}

	public override bool OnUse(){
		if((!charged || charges > 0) && !disabled && PlayerController.me.riding == false){
			//create the prefab
			GameObject c = (!upgraded)? castPrefab : upgradedPrefab;
			GameObject go = GameObject.Instantiate(c, PlayerStats.me.transform.parent);
			go.transform.position += PlayerStats.me.transform.position+PlayerController.me.GetDirection();
			Attack a = go.AddComponent<Attack>();
			a.c = PlayerStats.myStats;
			a.direction = PlayerController.me.GetDirection();

			if(charged){
				charges--;
				EZStatInfo.UpdateStats();
			}

			return true;
		}
		else return false;
	}

	public string Desc(){
		if(upgraded)
			return description + "\n\nUpgrade:\n" + TextColorer.ToColor(upgrade_desc, Color.green);
		else return description;
	}
}
