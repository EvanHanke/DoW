using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerTracker : MonoBehaviour {

	public static FollowerTracker me;
	public List<FollowerSave> active;

	Follower[] all;

	public string[] Save(){
		return FollowerSave.Save(all);
	}

	public void Load(string[] file){
		foreach(string s in file){
			FollowerSave f= JsonUtility.FromJson<FollowerSave>(s);
			foreach(Follower ff in all){
				if(f != null && ff.name == f.name){
					SpawnFollower(ff);
					ff.my_name = f.nickname;
					ff.a.stat_save = f.stats;
				}
			}
		}
	}

	void Awake(){
		all = Resources.LoadAll<Follower>("Followers/");
		foreach(Follower f in all){
			f.active = false;
			f.my_name = f.name;
			f.a = new Follower2();
			f.a.stat_save = null;
		}
		me  = this;
	}

	//spawns the follower within a radius
	public void SpawnFollower(Follower f){



		GameObject newf = GameObject.Instantiate(f.prefab, transform.parent);
		f.active = true;
		TeleportFollower(f);
	}

	public void TeleportAll(){
		foreach(Follower f in all){
			if(f.a.world_ref != null && f.active) TeleportFollower(f);
		}
	}

	public void TeleportFollower(Follower f){
		Vector3 loc = new Vector3(Random.value, 0f, Random.value);
		loc -= (Vector3.one * 0.5f);
		loc = Vector3.Normalize(loc)*2f;
		f.a.world_ref.transform.position = transform.position + loc + Vector3.up;
	}

	public void RestoreFollowers(){
		foreach(Follower f in all){
			if(f.a.world_ref == null && f.active) SpawnFollower(f);
		}
	}

	public void DismissFollower(Follower f){
		PushMessage.Push("Your follower returned home.");
		GameObject.Destroy(f.a.world_ref);
		f.active = false;
	}
}

[System.Serializable]
public class FollowerSave{
	public string name;
	public string nickname;
	public CharacterSave stats;

	public static string[] Save(Follower[] all){
		Debug.Log("saving followers");
		string[] active = new string[all.Length];
		for(int i = 0; i < all.Length; i++){
			Follower f = all[i];
			if(f.active){
				FollowerSave f_s = new FollowerSave();
				f_s.name = f.name;
				f_s.nickname = f.my_name;
				f_s.stats = new CharacterSave(f.a.stats);
				active[i] = JsonUtility.ToJson(f_s);
			}
		}
		Debug.Log( active);
		return active;
	}
}