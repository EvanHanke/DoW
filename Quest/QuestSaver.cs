using UnityEngine;

[System.Serializable]
public class QuestSaver{

	public string quest_name;
	public int quest_stage;
	public bool completed, failed;
	public int[] objectives;

	public QuestSaver (string n , int i, bool c, bool f, int[] a){
		quest_name = n;
		quest_stage = i;
		completed = c;
		objectives = a;
		failed = f;
	}

	public static QuestSaver[] Save(){
		Quest[] qs = QuestTracker.GetQuests();
		QuestSaver[] qss = new QuestSaver[qs.Length];
		for(int i = 0; i < qs.Length; i++){
			int[] a = new int[0];
			if(!qs[i].completed && !qs[i].failed){
				 a = new int[qs[i].stages[qs[i].currentStage].goals.Length];
				for(int j = 0; j < a.Length; j++){
					a[j] = qs[i].stages[qs[i].currentStage].goals[j].currAmt;
				}
			}
			qss[i] = new QuestSaver(qs[i].name, qs[i].currentStage, qs[i].completed, qs[i].failed, a);
		}
		return qss;
	}

	public static void Load(QuestSaver[] qss){
		Quest[] qqq = Resources.LoadAll<Quest>("Quests/");
		QuestTracker.myQuests.Clear();
		foreach(QuestSaver q in qss){
			
			Quest qq = null;
			foreach(Quest qa in qqq){
				if(qa.name == q.quest_name){
					QuestTracker.AddQuest(qa);
					qa.currentStage = q.quest_stage;
					qa.completed = q.completed;
					qa.failed = q.failed;

					for(int j = 0; j < q.objectives.Length; j++){
						if(qa.stages[qa.currentStage].goals.Length < j)
							qa.stages[qa.currentStage].goals[j].currAmt = q.objectives[j];
					}
					break;
				}
			}
		}
	}

}
