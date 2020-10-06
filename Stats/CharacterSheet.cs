using UnityEngine;

[CreateAssetMenu]
public class CharacterSheet: ScriptableObject{
	
	public AudioClip OnDamageSound;
	public int killxp = 5;
	public GameObject[] dropItems;

	public float speed = 1f;
	public int hp, mp, level, magic_pow, magic_def, power, armor;
	public string archetype = ""; //of the archetypical slime or whatever

	public void DropLoot(Vector3 pos){
		foreach(GameObject loot in dropItems){
			GameObject.Instantiate(loot, Zone.currentSubZone.addedPrefabs.transform).transform.position = pos;
		}
	}

	[HideInInspector]
	public int[] statPointers = new int[9];

	public void Init(){
		statPointers[0] = hp;
		statPointers[1] = hp;
		statPointers[2] = mp;
		statPointers[3] = mp;
		statPointers[4] = level;
		statPointers[5] = magic_pow;
		statPointers[6] = magic_def;
		statPointers[7] = power;
		statPointers[8] = armor;
	}

	//hp and maxhp point to same hp stat
	public int ValFromNameIndex(int i){
		return statPointers[i];
	}
}
