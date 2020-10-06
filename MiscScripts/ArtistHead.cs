using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArtistHead : MonoBehaviour {

	public Quest q_ref;

	public int total_seen;
	[SerializeField]
	public List<string> npcs;

	List<NPC2> active;
	string[] possible_reactions = {
		"Wow! What a striking performance!",
		"What is that giant head doing? I'm so confused",
		"Did an artist really seal themselves in that marble bust? That's amazing!",
		"Why would anyone willingly cripple themselves like that? It makes no sense.",
		"These artists are so pretentious. Leave me alone!",
		"Wow, what a sacrifice for art. I'm absolutely stunned.",
		"I honestly don't care much for this 'art'. It means nothing to me.",
		"This artist is so egocentric. I don't care.",
		"This reminds of a piece I did once. Ah, those were the days",
		"No, I don't care. Thanks.",
		"Another artist stroking their ego and begging for attention? Please."
	};
	string[] already_seen = {
		"Ah, so they're back. Great",
		"When will this artist leave? I've seen this already!",
		"Ah, this piece never gets old. Thanks.",
		"I've said it before and I'll say it again, I don't care!",
		"Won't these annoying artists get real jobs?",
		"I love this piece. It never gets old.",
		"Is this artist back again? Tell them to leave!",
		"Ya need more attention, huh?",
		"I was an artist once too, you know.",
		"I still don't care.",
		"I dated an artist once. Never again!"
	};

	void Start () {
		ArtistHead[] others = Zone.currentZone.GetComponentsInChildren<ArtistHead>();
		foreach(ArtistHead head in others){
			if(head != this){
				GameObject.Destroy(head.gameObject);
			}
		}

		//load total audience members
		string loaded = GlobalStater.me.GetState("Audience");
		npcs = new List<string>();
		if(loaded != null) npcs.AddRange( JsonUtility.FromJson<EZWrapper>(loaded).a);

		Debug.Log(GlobalStater.me.GetState("Audience#"));
		total_seen = npcs.Count;

		DetectNPCS();
	}

	void Save(){
		Debug.Log(npcs.Count);
		EZWrapper e = new EZWrapper();
		e.a = npcs.ToArray();
		Debug.Log(JsonUtility.ToJson(e));
		GlobalStater.me.SetState("Audience", JsonUtility.ToJson(e));
		if(npcs.Count > 14 && q_ref.currentStage == 0){
			QuestTracker.AdvanceQuest(q_ref, 1);
		}
	}

	void DetectNPCS(){
		active = new List<NPC2>();
		NPC2[] all = Zone.currentZone.GetComponentsInChildren<NPC2>();
		NPC2 me = GetComponent<NPC2>();
		foreach(NPC2 n in all){
			if(n != me)
			if(Vector3.Distance(n.transform.position, transform.position) < 15f){
				ShortMessage sm = n.gameObject.AddComponent<ShortMessage>();
				n.GetComponent<Interactable>().SetScript(sm);
				sm.label = "Gauge reaction";
				if(!npcs.Contains(n.npcName)){
					sm.message = GetRandom(possible_reactions, n.npcName);
					npcs.Add(n.npcName);
					total_seen++;
				}
				else {
					sm.message = GetRandom(already_seen, n.npcName);
				}
				FloatingTextSpawner.SpawnText("!", Color.yellow, n.transform);
				active.Add(n);
			}
		}
		Save();
	}

	void OnDestroy(){
		foreach(NPC2 n in active){
			if(n != null)
			n.GetComponent<Interactable>().SetScript(n);
		}
	}

	string GetRandom(string[] ops, string seed){
		int x = SumChars(seed);
		int s = ops.Length;
		Random.InitState(x);
		int r = (int) (Random.value * (float) s) ;
		return ops[r];
	}

	int SumChars(string s){
		int a = 0;
		char[] c = s.ToCharArray();
		foreach(char cc in c){
			a += (int) cc;
		}
		return a;
	}

}
[System.Serializable]
public class EZWrapper{
	public string[] a;
}