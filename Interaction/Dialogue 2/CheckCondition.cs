using UnityEngine;

[System.Serializable]
public class CheckCondition{

	public enum ConditionType {none, has_item, stat_level, quest_stage, rot_amount, worm_amount, 
		viro_health, knows_spell, has_effect, karma, global_flag, day_counter, always_yes, outfit_matches};
	public ConditionType myCondition;

	public string target_name;
	public int condition_threshold;

	public bool reverse = false;

	public bool Check(){
		if(!reverse) return Check1();
		else return !(Check1());
	}

	public bool Check1(){
		if(myCondition == ConditionType.none) return false;
		else if(myCondition == ConditionType.always_yes) return true;

		switch(myCondition){
		case ConditionType.has_item:
			return CheckItem();
		case ConditionType.quest_stage:
			return CheckQuest();
		case ConditionType.rot_amount:
			return CheckRot();
		case ConditionType.stat_level:
			return CheckStat();
		case ConditionType.worm_amount:
			return CheckWorm();
		case ConditionType.viro_health:
			return CheckViro();
		case ConditionType.knows_spell:
			return CheckSpell();
		case ConditionType.has_effect:
			return HasEffect();
		case ConditionType.karma:
			return CheckKarma();
		case ConditionType.day_counter:
			return DayCounter();
		case ConditionType.global_flag:
			return GlobalFlag();
		case ConditionType.outfit_matches:
			return CheckOutfit();
		}
		return false;
	}

	public bool CheckOutfit(){
		WearableItem a = PlayerStats.headEquip;
		WearableItem b = PlayerStats.bodyEquip;
		if(a != null && b != null){
			bool aa = false;
			bool bb = false;
			foreach(Modifier e in a.effects[0].myModifiers){
				if(e.statName == target_name && (e.modVal == 0 || e.modVal >= 1f) && e.flatBonus >= 0) aa = true;
			}
			foreach(Modifier e in b.effects[0].myModifiers){
				if(e.statName == target_name && (e.modVal == 0 || e.modVal >= 1f) && e.flatBonus >= 0) bb = true;
			}
			return (aa && bb);
		}
		return false;
	}

	public bool CheckItem(){
		Debug.Log("checking item");
		int a = PlayerStats.playerInv.QuantityOf(target_name);
		if(a >= condition_threshold) return true;
		else return false;
	}

	public bool CheckQuest(){
		Quest[] qs = QuestTracker.GetQuests();
		foreach(Quest q in qs){
			if(q.name == target_name){
				if((q.currentStage >= condition_threshold && condition_threshold >= 0 )|| 
					(condition_threshold >= q.stages.Length-1 && q.completed)) return true;
				else if (q.failed && condition_threshold == -1) return true;
				else return false;
			}
		}
		return false;
	}

	public bool CheckStat(){
		int s = PlayerStats.myStats.GetStat(target_name);
		if(s >= condition_threshold) return true;
		else return false;
	}

	public bool CheckRot(){
		int r = Zone.currentZone.GetComponentsInChildren<Rot>().Length;
		if(r <= condition_threshold) return true;
		else return false;
	}
	public bool CheckWorm(){
		return (WormTracker.me.worms.Count >= condition_threshold);

	}
	public bool CheckViro(){
		int e = Environment.me.GetCurrentHealth();
		return ((Mathf.Abs(e) >= Mathf.Abs(condition_threshold)) && (e*condition_threshold >= 0));
	}
	public bool CheckSpell(){
		foreach(Spell s in SpellChecker.GetLearnedSpells()){
			if(s.name == target_name) return true;
		}
		return false;
	}
	public bool HasEffect(){
		foreach(Effect e in PlayerStats.myStats.GetAllEffects()){
			if(e.effectName == target_name){
				return true;
			}
		}
		return false;
	}
	public bool CheckKarma(){
		return (PlayerStats.me.karma >= condition_threshold);
	}

	public bool GlobalFlag(){
		int t = -99;
		if (int.TryParse( GlobalStater.me.GetState(target_name) , out t))
			return ( t == condition_threshold );
		else return false;
	}

	public bool DayCounter(){
		return (TimeTracker.GetCounter(target_name) <= condition_threshold);
	}
}
