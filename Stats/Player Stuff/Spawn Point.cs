using UnityEngine;

//for reseting the player when they die;
public static class SpawnPoint{

	public static string zone = "Moon Temple";
	public static string subzone = "Dark Passage";
	public static int target = 0;
	public static Vector3 pos = Vector3.zero;

	public static SpawnSave Save(){
		return new SpawnSave(zone, subzone, target, pos);
	}

	public static void Load(SpawnSave file){
		zone = file.s;
		subzone = file.sz;
		target = file.t;
		pos = new Vector3(file.x, file.y, file.z);
	}

	public static void Respawn(){
		if (zone != "")
		Zone.ChangeZone(zone, subzone, target);
	}

	public static void RespawnDead(){
		Zone.ChangeZone("LandOfDead", "Entrance", 0);
	}

	public static void SetRespawn(int t){
		target = t;
		zone = Zone.currentZone.name;
		subzone = Zone.currentSubZone.name;
	}
	public static void SetRespawn(Vector3 v){
		pos = v;
		zone = Zone.currentZone.name;
		subzone = Zone.currentSubZone.name;
	}
	public static void RespawnAtAltar(){
		GlobalStateMachine.GPause();
		CameraEfx.FadeInOutB(0.5f, Change, GlobalStateMachine.GUnpause);
	}
	public static void Change(){
		Zone.ChangeZone(zone, subzone, pos);
	}
}

[System.Serializable]
public class SpawnSave{
	public string s, sz;
	public int t;
	public float x, y, z;
	public SpawnSave(string a, string b, int c, Vector3 d){
		s = a;
		sz = b;
		t = c;
		x = d.x;
		y = d.y;
		z = d.z;
	}
}