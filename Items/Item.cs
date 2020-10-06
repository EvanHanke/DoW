using UnityEngine.UI;
using UnityEngine;

//generic Item class used for inheritance
[System.Serializable]
public class Item: ScriptableObject{

	[TextArea (0, 4)]
	public string description;
	public Sprite image;
	public int value;
	public bool consumable;

	public GameObject droppablePrefab;

	[HideInInspector]
	public string equipped= "";

	public void Awake(){
		equipped = "";
	}

	public bool Drop(){
		Vector3 playerpos = PlayerController.me.transform.position + new Vector3(0f, 0.09f, 0f);
		AudioLoader.PlaySound("simpleblip1", 1f, true, 0.4f);
		//if the player is not grounded, nothing happens
		if (!Physics.Raycast(playerpos, Vector3.down, 0.1f)) return false;

		if (droppablePrefab != null){
			//create object and place in the scene
			GameObject.Instantiate(droppablePrefab, playerpos, Quaternion.identity, Zone.currentSubZone.addedPrefabs.transform);
			//make player crouch for half a second
			PlayerController.me.Crouch(0.5f);
			return true;
		}
		return false;
	}

	public string GetNameFromState(int state){
		return name;
	}
	public Sprite GetSpriteFromState(int state){
		return image;
	}

	public string Desc(){
		return description;
	}

	public static string GetFullDesc(Item itm){
		string s = TextColorer.ToColor(itm.name, Color.yellow) +"\n"+ itm.Desc();

		if(itm is StatModItem){
			StatModItem smi = (StatModItem) itm;
			if(smi.effects.Length>0)
				s += "\n"+smi.effects[0].ToRichString();
		}
		else if (itm is CastableItem){
			CastableItem ci = (CastableItem) itm;
			s = ci.Desc();
		}
		else if(itm is WearableItem){
			WearableItem w = (WearableItem) itm;
			if(w.effects.Length>0)
				s += "\n"+w.effects[0].ToRichString();
		}

		return s;
	}
}
