using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitorSpawner : MonoBehaviour {

	public static VisitorSpawner me;
	public Visitor[] visitors;
	[HideInInspector]
	public int spawnedVisitors;
	public int num_visitors;

	public void Awake(){
		me = this;
		//spawnedVisitors = new List<Visitor>();
	}

	//Check if a local grow area contains the necessary resources
	public bool CheckRequires(LocalGrowArea lga, PrefabCheck check){
		int c = 0;
		foreach (GrowNode g in lga.myNodes){
			if(g.myFlwr.name.Contains(check.name)){
				c++;
				Debug.Log("zone contains " + g.myFlwr.name);
			}
		}
		foreach(SummoningToken t in lga.misc_tokens){
			if(t.name.Contains(t.name)){
				c++;
			}
		}
		check.totalAmt=c;
		if(c >= check.amount) return true;
		else return false;
	}
	//check if the requirements are exceeded to support duplication
	public bool CheckExtraRequires(Visitor v, int iter){
		if(v.requirements!=null)
			foreach(PrefabCheck pc in v.requirements){
				if (pc.totalAmt - (v.ratio*iter*pc.amount) <= pc.amount){
					return false;
					break;
				}
			}
		return true;
	}

	//Main check / spawn
	public void CheckVisitors(LocalGrowArea[] lgas){
		//checks what's going on with each visitor
		num_visitors = 0;

		Rot[] r = Zone.currentZone.GetComponentsInChildren<Rot>();

		foreach(Visitor v in visitors){
			v.RemoveAll();
			for(int ii = 0 ; ii < lgas.Length; ii++){
				LocalGrowArea lga = lgas[ii];

				//
				//check if area health requirements are met
				int numToSpawn = 0;

				bool health_req = false;
				//negative health scenerio
				if(((v.healthReq < 0) && (v.healthReq >= lga.area_health)) && (lga.area_health < 0)){ health_req = true ; }
				//postive/neutral health scenerio
				else if (v.healthReq > 0 && lga.area_health > 0 && v.healthReq <= lga.area_health) { health_req = true ; }
				else if (v.healthReq == 0) health_req = true;


				if(health_req){
					//Debug.Log("visitor " + v.name + " health req met");
					bool item_req = true;

					//check if item requirements met
					foreach(PrefabCheck p_c in v.requirements){
						int c = 0;
						foreach(GrowNode f in lga.myNodes){
							//Debug.Log("" + f.myFlwr.name + " containing " + p_c.name + c);
							if(f.myFlwr.name.Contains(p_c.name)) c++;
						}
						foreach(SummoningToken g in lga.misc_tokens){
							if(g.gameObject.name.Contains(p_c.name)) c++;
						}
						if(c < p_c.amount) item_req = false;
					}
					//
					if (item_req){
						//Debug.Log("visitor " + v.name + " item req met");
						//
						//requirements met

						numToSpawn = 1;

						if(!v.unique){
							int difference = Mathf.Abs(lga.area_health) - Mathf.Abs(v.healthReq);
							while(numToSpawn*v.ratio < difference){
								numToSpawn++;
								if(numToSpawn >= v.max) break;
							}
						}


						//Debug.Log("visitor " + v.name + " num to spawn = " + numToSpawn);
						for(int i = 0; i < numToSpawn; i++){
							SpawnVisitor(v, lga);
							spawnedVisitors++;
						}
					}
				}
			}
		}

	}

	public void SpawnVisitor(Visitor v, LocalGrowArea lga){
		num_visitors++;
		Debug.Log("spawning " + v.prefab.name);
		Vector3 w = lga.GetRandomPosInBounds();
		v.Spawn(w);
	}

	public void RemoveVisitor(Visitor v){
		v.RemoveAll();
	}
}
[System.Serializable]
public class Visitor{
	public string name;
	public GameObject prefab;
	public int healthReq;
	[HideInInspector]
	public List<GameObject> world_reference;
	public PrefabCheck[] requirements;
	//optional params
	public bool unique;
	public int max;
	public int ratio;

	public GameObject Spawn(Vector3 pos){
		if(world_reference == null) world_reference = new List<GameObject>();

		GameObject newV = GameObject.Instantiate(prefab);
		newV.transform.parent = Zone.currentSubZone.transform;
		newV.transform.position = pos;
		world_reference.Add(newV);

		return newV;
	}

	public void Remove(){
		if(world_reference.Count > 0){
			GameObject g = world_reference[0];
			world_reference.Remove(g);
			GameObject.Destroy(g);
		}
	}

	public void RemoveAll(){
		for(int i = 0; i < world_reference.Count; i++){
			Remove();
		}
	}
}

[System.Serializable]
//Can be a growable or a summoning token
public class PrefabCheck{
	public int amount;
	public string name;
	[HideInInspector]
	public int totalAmt;
}