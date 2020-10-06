using UnityEngine;
using System.Collections.Generic;

public class GrowArea : MonoBehaviour {
	public static GrowArea me;
	public static float Distance_Constant = 6f;
	public static List<GrowNode> allNodes;
	public List<LocalGrowArea> allGrowAreas;
	public List<GameObject> allSlimes;
	public int totalHealth;

	public void Awake(){
		me = this;
		allNodes = new List<GrowNode>();
		allGrowAreas = new List<LocalGrowArea>();
		allSlimes = new List<GameObject>();
	}
	public void UpdateGrowArea(Flower[] nodes){
		//Generate map of flower nodes from Environment 
		allNodes.Clear();
		allGrowAreas.Clear();
		totalHealth = 0;
		foreach(GameObject s in allSlimes){
			GameObject.Destroy(s);
		}
		allSlimes.Clear();
		foreach(Flower f in nodes){
			allNodes.Add(new GrowNode(f));
		}
		//Generate map of neighbors
		foreach(GrowNode f in allNodes){
			f.CalcNeighbors(allNodes);
		}
		//Generate map of local zones based on grouped flowers
		SummoningToken[] sts = Zone.currentZone.GetComponentsInChildren<SummoningToken>();
		foreach(GrowNode f in allNodes){
			LocalGrowArea lga = f.CalcLocalZone();
			if (!allGrowAreas.Contains(lga)) allGrowAreas.Add(lga);
		}
		//Generate health factors for each area, check ViroTransforms
		ViroTransform[] v_tt = Zone.currentZone.GetComponentsInChildren<ViroTransform>();
		foreach(LocalGrowArea ga in allGrowAreas){
			ga.CalcHealth();
			ga.AddMiscTokens(sts);
			PlantEffector.me.CheckEfx(ga);
			totalHealth += ga.area_health;

			foreach(ViroTransform v1 in v_tt) v1.Check(ga);
		}
		//update global health factors
		foreach(Trash t in Zone.currentZone.GetComponentsInChildren<Trash>()){
			totalHealth--;
		}
		foreach(Rot r in Zone.currentZone.GetComponentsInChildren<Rot>()){
			totalHealth-=10;
		}
		//VisitorSpawner.me.CheckVisitors(allGrowAreas.ToArray());
	}
	//Updates zone stat info without passing time
	public void CheckGrowArea(){
		//Generate health factors for each area
		foreach(LocalGrowArea ga in allGrowAreas){
			ga.CalcHealth();
			totalHealth += ga.area_health;
		}
	}

	public void OnDrawGizmos(){
		/*
		if (allNodes != null){
			foreach(GrowNode f in allNodes){
				foreach(GrowNode n in f.neighbors){
					Gizmos.DrawLine(n.loc + Vector3.up, f.loc+Vector3.up);
				}
			}
		}
		*/
		if(allGrowAreas != null){
			foreach(LocalGrowArea lga in allGrowAreas){
				Gizmos.color = lga.c;
				Gizmos.DrawWireCube(lga.bounds.center, lga.bounds.size);
				Gizmos.DrawCube(lga.center, Vector3.one);


				foreach(GrowNode f in lga.myNodes)
					foreach(GrowNode n in f.neighbors){
						Gizmos.DrawLine(n.loc + Vector3.up, f.loc+Vector3.up);
					}
			}
		}
	}
}

public class LocalGrowArea{
	public List<GrowNode> myNodes;
	public List<SummoningToken> misc_tokens;
	public GrowNode root;
	public int area_health;
	public Vector3 center;
	public Bounds bounds;
	public Color c;

	public LocalGrowArea (GrowNode g){
		root = g;
		area_health = 0;
		myNodes = new List<GrowNode>();
		misc_tokens = new List<SummoningToken>();
		bounds = new Bounds();
		bounds.size = Vector3.one;
		c = Color.HSVToRGB(Random.value, 0.6f, 0.6f);
	}

	public void AddMiscTokens(SummoningToken[] sts){
		misc_tokens.Clear();
		foreach(SummoningToken st in sts){
			if (CheckContained(st.transform.position)){
				misc_tokens.Add(st);
			}
		}
	}

	public Vector3 GetRandomPosInBounds(){
		bool grounded = false;
		Vector3 pos = center;
		int failsafe = 0;
		Random.InitState(TimeTracker.me.day);

		while(!grounded && failsafe < 10){
			float randX = Random.value;
			float randZ = Random.value;
			randX = (randX-0.5f)*(bounds.size.x);
			randZ = (randZ-0.5f)*bounds.size.z;
			pos = new Vector3(center.x + randX, center.y, center.z+randZ);
			Ray r= new Ray(pos+Vector3.up, Vector3.down);
			failsafe++;
			if(CheckGrounded.Check(pos)) grounded = true;
		}
		if(grounded) return pos;
		else return center;
	}

	public void CheckBounds(Vector3 pos){
		if(!bounds.Contains(pos)) bounds.Encapsulate(pos);
		//center = bounds.center;
	}
	public bool CheckContained(Vector3 pos){
		return bounds.Contains(pos);
	}

	public static int GetGrowMod(Growable.G gs){
		switch(gs){
		case Growable.G.budding: return 0;break;
		case Growable.G.grown: return 1; break;
		case Growable.G.withered: return -1; break;
		}
		return 0;
	}

	public void CalcHealth(){
		Debug.Log("CALCHEALTH");
		Vector3 runningAvg = Vector3.zero;
		foreach(GrowNode gn in myNodes){
			int health_factor = 0;
			runningAvg += gn.myFlwr.transform.position;
			health_factor = GetGrowMod(gn.myFlwr.GetState());
			area_health += health_factor;
		}
		Debug.Log("Grow Area Health: " + area_health);
		if(area_health >= 0){
			foreach(GrowNode gn in myNodes){
				Environment.me.AddGrowth(gn.loc);
			}
		}
		center = runningAvg / (float)myNodes.Count;
		bounds = new Bounds(center, Vector3.one);
		foreach(GrowNode gn in myNodes){
			CheckBounds(gn.loc);
		}
		/*
		if(area_health < -2){
			int num_slimes = Mathf.Abs(area_health) / 3;
			for(int i = 0; i < num_slimes; i++){
				GameObject slime = Prefabber.me.GetFromName("Slime");
				GameObject g = GameObject.Instantiate(slime, Zone.currentSubZone.addedPrefabs.transform);
				GrowArea.me.allSlimes.Add(g);
				g.transform.position = GetRandomPosInBounds();
			}
		}
		if(area_health < -5){
			int num_flies = Mathf.Abs(area_health) / 5;
			for(int i = 0; i < num_flies; i++){
				GameObject slime = Prefabber.me.GetFromName("FlyGuy");
				GameObject g = GameObject.Instantiate(slime, Zone.currentSubZone.addedPrefabs.transform);
				GrowArea.me.allSlimes.Add(g);
				g.transform.position = GetRandomPosInBounds();
			}
		}
		*/
	}
}

public class GrowNode{
	public Flower myFlwr;
	public LocalGrowArea myArea;
	public List<GrowNode> neighbors;
	public Vector3 loc;
	bool isDecay;

	//get neighbors based on distance
	public void CalcNeighbors(List<GrowNode> list){
		foreach(GrowNode other in list){
			if (Vector3.Distance(other.loc, loc) < GrowArea.Distance_Constant){
				neighbors.Add(other);
			}
		}
	}

	//traverse neighbors tree and set all neighbors to this zone
	public LocalGrowArea CalcLocalZone(){
		if (myArea == null){
			LocalGrowArea new_area = new LocalGrowArea(this);
			SetLocalZone(new_area);
		}
		return myArea;
	}
	public void SetLocalZone(LocalGrowArea g){
		if (myArea == null){
			myArea = g;
			myFlwr.myGrowArea = g;
			g.myNodes.Add(this);
			foreach(GrowNode gn in neighbors){
				gn.SetLocalZone(g);
			}
		}
		else return;
	}

	//node based on flower health and pos
	public GrowNode(Flower f){
		myFlwr= f;
		f.myNode = this;
		loc = f.transform.position;
		isDecay = (f.g.life_cycle[f.state].myState == Growable.G.withered)? true : false;
		neighbors = new List<GrowNode>();
	}
}
