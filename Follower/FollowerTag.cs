using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerTag : InteractionScript {

	public Follower my_follower;
	public GameObject follower_UI;
	public bool auto_follow = false;

	public override string LabelDesc()
	{
		return my_follower.my_name;
	}

	public override void OnInteract(){
		GlobalStateMachine.GPause();
		GameObject.Instantiate(follower_UI, GameObject.Find("UI").transform).GetComponent<FollowerUI>().Set(my_follower);
	}

	void Start(){
		if(my_follower.a.world_ref == null){
			my_follower.a.stats = GetComponent<Character>();
			my_follower.a.world_ref = gameObject;
			if(my_follower.a.stat_save != null){
				my_follower.a.stats.Load(my_follower.a.stat_save);
			}
			if(auto_follow){
				my_follower.active = true;
				transform.parent = PlayerStats.me.transform.parent;
				QuestTracker.CheckAll(my_follower);
			}
		}
		else{
			gameObject.SetActive(false);
		}

	}

	void Update(){
		Transform player = PlayerStats.me.transform;
		if(Vector3.Distance(player.position, transform.position) > 20f)
			FollowerTracker.me.TeleportFollower(my_follower);
	}
}
