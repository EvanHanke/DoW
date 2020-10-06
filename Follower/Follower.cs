using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Follower : ScriptableObject {

	public string my_name;
	public GameObject prefab;
	public int sleep_timer;
	public bool active;
	public Follower2 a;
}

public class Follower2{
	public GameObject world_ref;
	public Character stats;
	public CharacterSave stat_save;
}
