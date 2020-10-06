using UnityEngine;
using UnityEngine.UI;

public class LevelUpMenuUI : MonoBehaviour {

	Text levels;
	Text statPointsTxt;
	Text descText;
	PlayerStats stats;
	RectTransform selector;
	Vector2 basePos;
	int selected;

	int[] currentVals;
	int[] allocated;
	string[] names = {"maxhp", "maxmp", "power", "magic_pow"};
	string [] descs = {"Life increases your total hit points.", "Mana allows you to cast more spells.", 
		"Power increases your strength and damage.", "Magic Power improves your spells"};

	public void Awake(){
		selected = 0;
		allocated = new int[4];
		currentVals = new int[4];
		stats = PlayerStats.me;
		levels = GameObject.Find("Stat Vals").GetComponent<Text>();
		statPointsTxt = GameObject.Find("SkillPoints").GetComponent<Text>();
		descText = GameObject.Find("LDescription").GetComponent<Text>();
		selector = GameObject.Find("LvlUpSelector").GetComponent<RectTransform>();
		basePos = selector.anchoredPosition;
		Init();
	}

	void Init(){
		for(int i = 0; i < 4; i++){
			currentVals[i] = PlayerStats.myStats.GetStat(names[i]);
			allocated[i] = 0;
		}
		Refresh();
	}

	void Refresh(){
		string vals = "";
		for(int i = 0; i < 4; i ++){
			if (allocated[i] > 0) vals += "<";
			vals += (PlayerStats.myStats.GetStat(names[i])+allocated[i]).ToString();
			if (stats.skillPoints > 0) vals += ">";
			vals+="\n";
		}
		levels.text = vals;

		descText.text = descs[selected];
		statPointsTxt.text = "Skill points: " + stats.skillPoints;
	}

	public void Update(){
		if (MyInput.GetState("UP", true) == 'p'){
			selected = (selected > 0)? selected-1 : 6;
			Refresh();
		}
		if (MyInput.GetState("DOWN", true) == 'p'){
			selected = (selected < 4)? selected+1 : 0;
			Refresh();
		}
		if (MyInput.GetState("LEFT", true) == 'p'){
			if (allocated[selected] > 0){
				stats.skillPoints++;
				allocated[selected]--;
				Refresh();
			}
		}
		if (MyInput.GetState("RIGHT", true) == 'p'){
			if (stats.skillPoints > 0){
				stats.skillPoints--;
				allocated[selected]++;
				Refresh();
			}
		}
		//destroy menu and return allocated skill points
		if (MyInput.GetState("SHIFT", true) == 'p'){
			for(int i = 0; i < 4; i++){
				stats.skillPoints += allocated[i];
			}
			GameObject.Destroy(gameObject);
		}

		//commit points
		if (MyInput.GetState("Z", true) == 'p'){
			for(int i = 0; i < 4; i ++){
				PlayerStats.myStats.SetStat(names[i], 
					PlayerStats.myStats.GetStat(names[i]) + allocated[i]);
			}
			PlayerStats.myStats.SetStat("hp", PlayerStats.myStats.GetStat("maxhp"));
			PlayerStats.myStats.SetStat("mp", PlayerStats.myStats.GetStat("maxmp"));
			EZStatInfo.UpdateStats();
			GameObject.Destroy(gameObject);
		}

		selector.anchoredPosition = basePos + new Vector2(0, -51f * selected);
	}

}
