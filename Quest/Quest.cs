using UnityEngine;

[CreateAssetMenu]
public class Quest : ScriptableObject{
	//Quests have a static name and description
	//but stage-dependant goals and detailed description
	[TextArea (0, 4)]
	public string description;
	public int currentStage = 0;
	public QuestStage[] stages;

	public bool completed = false;
	public bool failed = false;

	[TextArea (0, 3)]
	public string completedInfo, failedInfo;

	public void AddQuest(){
		if(!QuestTracker.HasQuest(this)){
			QuestTracker.AddQuest(this);
			PushMessage.Push("New Quest !");
			currentStage = 0;
			completed = false;
			failed = false;

			foreach (QuestStage s in stages){
				s.Init();
			}

			Debug.Log("quest added");
		}
		//Debug.Log("curernt stage" + currentStage);
	}

	public bool isStageCompelete(){
		return stages[currentStage].completed;
	}

	public bool UpdateQuest(){
		if(failed || completed) return false;
		if( stages[currentStage].Update()){
			QuestTracker.AdvanceQuest(this, currentStage+1);
		}
		return false;
	}

	public void Check(ScriptableObject f){
		if(!completed && !failed){
			stages[currentStage].Check(f);
			if (UpdateQuest()){
				PushMessage.Push("Quest Updated");
				QuestTracker.AdvanceQuest(this, currentStage+1);
			}
		}
	}
}

[System.Serializable]
public class QuestStage{
	[TextArea (0, 4)]
	public string description;
	public QuestGoal[] goals;
	public bool completed = false;
	public int xp = 0;

	public void Init(){
		completed = false;
		foreach (QuestGoal g in goals){
			g.Init();
		}
		Update();

	}

	public bool Update(){
		bool c = true;
		foreach (QuestGoal g in goals){
			g.Refresh();
			if (g.completed == false) c = false;
		}
		if(goals.Length ==0) c = false;
		return c;
	}

	public bool Check(ScriptableObject f){
		foreach(QuestGoal g in goals){
			g.Check(f);
		}
		return Update();
	}
}

//Each 
[System.Serializable]
public class QuestGoal{
	public ScriptableObject goal;
	public bool completed = false;
	public string zoneFilter;

	string targetName;
	public int targetAmt;
	public int currAmt;

	public void Init(){
		currAmt = 0;
		completed = false;
	}

	public void Check(ScriptableObject f){
		
		if (f == goal){
			currAmt++;
		}
		if(f is CharacterSheet && currAmt >= targetAmt) completed = true;

		Refresh();

	}

	public void Refresh(){
		if(!string.IsNullOrEmpty(zoneFilter)){
			if(Zone.currentSubZone.displayName != zoneFilter) return;
		}
		if (goal is Growable){
			currAmt = 0;
			Flower[] fs = Zone.currentSubZone.addedPrefabs.GetComponentsInChildren<Flower>();
			foreach(Flower f in fs){
				if(f.g == goal && f.IsGrown()){
					currAmt++;
				}
			}
			if(currAmt >= targetAmt) completed = true;
		}
		else if(goal is Item){
			currAmt = PlayerStats.playerInv.QuantityOf(goal.name);
		}
		else if (goal is Spell){
			if (goal.name == "Summon Portal"){
				if(string.IsNullOrEmpty(zoneFilter)){
					currAmt = Wormholer.me.wormholes.Count;
				}
				else{
					int a = 0;
					foreach(WormHoleSave whs in Wormholer.me.wormholes){
						if(zoneFilter == whs.richname){
							a++;
						}
					}
					currAmt = a;
				}
				if(currAmt >= targetAmt) completed = true;
			}
		}

	}

	public override string ToString(){
		if(goal != null){
			string prefix = "-";
			string infix = "";
			string postfix = "";
			if(goal is CharacterSheet) infix = "Kill ";
			else if(goal is Growable) infix = "Grow ";

			targetName = "";

			if (goal is Spell && goal.name == "Summon Portal") infix = "Open Wormhole ";
			else targetName = goal.name;

			string amt = (targetAmt > 0)? currAmt.ToString() + "/" + targetAmt.ToString() : " ";
			if(!string.IsNullOrEmpty(zoneFilter)) postfix = " in " + zoneFilter;
			return prefix + infix + amt + " " + targetName + postfix;
		}
		return "";
	}
}