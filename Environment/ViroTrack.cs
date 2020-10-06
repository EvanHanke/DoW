using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViroTrack : MonoBehaviour {

	Dictionary<int, ViroIndex> zoneIndexes;
	public static ViroTrack me;

	void Awake(){
		zoneIndexes = new Dictionary<int, ViroIndex>();
		me = this;
	}

	public void LogCurrent(){
		//Debug.Log("Logging Z environment");
		Zone z = Zone.currentZone;
		Zone sz = Zone.currentSubZone;
		ViroIndex v_i = null;
		if (zoneIndexes.ContainsKey(z.myID)) v_i = zoneIndexes[z.myID];
		else{
			v_i = new ViroIndex(z);
			zoneIndexes.Add(z.myID, v_i);
		}
		v_i.Log(sz);
		//Debug.Log("Logged Environment " + zoneIndexes[z.myID].Count() + " number flowers logged health = " + TotalzHealth(Zone.currentZone));
	}

	public int GetAmtInZone(string s, int id){
		if (zoneIndexes.ContainsKey(id)) return zoneIndexes[id].Count(s);
		else return 0;
	}

	public int TotalzHealth(Zone z){
		if (zoneIndexes.ContainsKey(z.myID)) return zoneIndexes[z.myID].TotalzHealth();
		else return 0;
	}
}


//Tracks flowers within a zone
public class ViroIndex{
	Dictionary<int, int> subHealth;
	Dictionary<int, Dictionary<string, ViroEntry> > entries; //each sub dictionary is a subzone

	public ViroIndex(Zone entry){
		entries = new Dictionary< int, Dictionary<string, ViroEntry >>();
		subHealth = new Dictionary<int, int>();
	}

	//logs current active subzone
	public void Log(Zone z){
		Flower[] fl = z.GetComponentsInChildren<Flower>();

		if (!entries.ContainsKey(z.myID)){
			entries[z.myID] = new Dictionary<string, ViroEntry>();
		}
		entries[z.myID].Clear();
		foreach(Flower f in fl){
			if (!entries[z.myID].ContainsKey(f.g.name)){
				entries[z.myID].Add(f.g.name, new ViroEntry(f));
			}
			else{
				entries[z.myID][f.g.name].amt++;
			}
		}
		//Debug.Log(entries.ToString());
		subHealth[z.myID] = GrowArea.me.totalHealth;
	}

	public int TotalzHealth(){
		int c = 0;
		foreach(int i in subHealth.Values){
			c += i;
		}
		return c;
	}

	//counts the total number of flowers "s" in a zone
	public int Count(string s){
		int a = 0;
		foreach(Dictionary<string, ViroEntry> dd in entries.Values){
			if (dd.ContainsKey(s)){
				a += dd[s].amt;
			}
		}
		return a;
	}
	public int Count(){
		int a = 0;
		foreach(Dictionary<string, ViroEntry> dd in entries.Values){
			a += dd.Keys.Count;
		}
		return a;
	}
}

//track a flower state
public class ViroEntry{

	GrowState gs;
	public int amt;

	public ViroEntry(Flower f){
		gs = f.g.life_cycle[f.g.state];
		amt = 1;
	}
}
