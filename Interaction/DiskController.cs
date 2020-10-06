using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskController : MonoBehaviour {

	public static DiskController me;
	bool solved = false;
	GameObject froggyhat;

	StateSaver ss;

	Disk[] discs;
	Pillar[] ps;

	public AudioClip win;
	public AudioClip move;

	public void Awake(){
		me = this;
		ss = gameObject.AddComponent<StateSaver>();
		discs = new Disk[4];
		for(int i = 0; i < 4; i++){
			discs[i] = new Disk();
			discs[i].size = i;
			discs[i].g = transform.GetChild(i).gameObject;
		}
		ps = new Pillar[3];
		for(int i = 0; i < 3; i++){
			ps[i] = new Pillar();
			ps[i].pos = new Vector3((i-1f)*8f, 2f, 14f);
		}

		froggyhat = GameObject.Find("froggyhat");
		froggyhat.SetActive(false);

		ss.onLoad = Load;
		ss.onInit = Load;
	}

	void Load(){
		solved = (ss.Get("solved") == "1");
		int a = 0;
		if(solved){
			froggyhat.SetActive(true);
			a = 2;
		}
		for(int i = 0; i < 4; i++){
			ps[a].Push(discs[3-i]);
		}
	}

	public bool Move(int a){
		if(solved) return false;
		AudioLoader.PlaySound(move.name, 1f, true, 0.6f);
		if(ps[a].ds.Count == 0) return false;
		Disk d = ps[a].ds[ps[a].ds.Count-1];
		for(int i = 0; i < 2; i++){
			if(ps[GetNextPillar(a+i)].Push(d)){
				ps[a].ds.Remove(d);

				if(ps[2].ds.Count == 4){
					solved = true;
					ss.Save("solved", "1");
					GlobalStater.me.SetState("HatMachine", "1");
					froggyhat.SetActive(true);
					Invoke("PlaySound", 1f);
				}

				return true;
			}
		}


		return false;
	}
	int GetNextPillar(int a){
		int b = a;
		b++;
		if(b >= ps.Length)
			b -= ps.Length;
		return b;
	}

	public void PlaySound(){
		AudioLoader.PlaySound(win.name, 1f, true, 0.6f);
	}
}

class Disk{
	public int size;
	public GameObject g;
}

class Pillar{
	public List<Disk> ds;
	public Vector3 pos;

	public Pillar(){
		ds = new List<Disk>();
	}

	public bool Push(Disk a){
		foreach(Disk d in ds){
			if(d.size < a.size) return false;	
		}

		ds.Add(a);
		a.g.transform.position = pos + Vector3.up * ds.Count;
		return true;
	}
}
