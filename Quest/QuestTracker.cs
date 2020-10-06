using UnityEngine;
using System.Collections.Generic;

public static class QuestTracker{

	public static List<Quest> myQuests = new List<Quest>();

	public static void Init(){
		myQuests.Clear();
	}

	public static bool HasQuest(Quest q){
		if (myQuests.Contains(q)) return true;
		else return false;
	}

	public static void AddQuest(Quest q){
		if(myQuests.Contains(q)) return;
		myQuests.Add(q);
		q.UpdateQuest();
	}

	public static Quest[] GetQuests(){
		return myQuests.ToArray();
	}

	public static bool IsStageComplete(Quest q, int stage){
		if (myQuests.Contains(q)){
			return q.stages[stage].completed;
		}
		else return false;
	}
		

	public static void AdvanceQuest(Quest q, int stage){
		if(stage >= 0){
			while(q.currentStage < stage){
				q.currentStage++;
				if (q.currentStage > q.stages.Length-1){
					q.currentStage = q.stages.Length;
					q.completed = true;
					break;
				}
				else {
					q.UpdateQuest();
				}
			}
		}
		else if (stage < 0){
			q.failed = true;
			q.currentStage = -1;
		}
	}

	public static void CheckAll(ScriptableObject o){
		foreach(Quest q in myQuests){
			q.Check(o);
		}
	}

	public static void UpdateQuests(){
		foreach(Quest q in myQuests){
			q.UpdateQuest();
		}
	}
}

[System.Serializable]
public class QuestChecker {

	public Quest which;
	public int stage;
	//public string newDialogue;

}