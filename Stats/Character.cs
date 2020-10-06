using System.Collections.Generic;
using UnityEngine;

//holds statistical information for a character
public class Character : MonoBehaviour {

	SpriteRenderer mySprite;
	[HideInInspector]
	public PlayerStats myController;
	[HideInInspector]
	public Entity myEntity;
	public CharacterSheet statSheet;
	Vector3 baseScale;
	public Character target, attacker;
	bool inited;


	/*
	 * tracks modifiers, effects, and stats
	 * also related particles
	 */

	//stats
	Dictionary<string, int> stats = new Dictionary<string, int>();
	public static string[] statNames = {"hp", "maxhp", "mp", "maxmp", "level", "magic_pow", "magic_def",
								"power", "armor"};

	public Dictionary<string, int> CopyOfStats(){
		Dictionary <string, int> new_dic = new Dictionary<string, int>();
		foreach(string key in stats.Keys){
			new_dic.Add(key, stats[key]);
		}
		return new_dic;
	}

	public int GetStatRaw(string name){
		if (stats.ContainsKey(name)){
			return stats[name];
		}
		return -1;
	}

	public int GetStat(string name){
		if (stats.ContainsKey(name)){
			float mod = GetModStat(name);
			return (int)((float)(stats[name]+GetStatBonus(name)) * mod);
		}
		return -1;
	}

	public void SetStat(string name, int amt){
		if (stats.ContainsKey(name)) stats[name] = amt;
	}

	float hitFlashTimer; //tracks invincibility frames after taking a hit

	//load from file
	public void Load(CharacterSave save){
		if(save!= null && save.stat_names.Length > 0){
			List<string> keys = new List<string> (stats.Keys);
			foreach(string stat in keys){
				stats[stat] = save.GetStat(stat);
			}
			foreach(EffectSave es in save.efs){
				AddEffect(es.Get(), false);
			}
		}
		else{
			InitializeStats();
		}
	}

	//particles, effects, modifiers
	//
	//
	Dictionary<Effect, GameObject> particlesDict = new Dictionary<Effect, GameObject>();

	List<Effect> effects = new List<Effect>();
	Dictionary<string, Effect> effectDict = new Dictionary<string, Effect>(); //all effects by name

	List<Modifier> modifiers = new List<Modifier>(); //all modifiers
	Dictionary<string, List<Modifier>> effectMods = new Dictionary<string, List<Modifier>>(); //all modifiers by Effect
	Dictionary<string, List<Modifier>> modIndex = new Dictionary<string, List<Modifier>>(); //all modifiers by stat

	List<Effect> efxToClear = new List<Effect>();
	List<Modifier> modsToClear = new List<Modifier>();
	//
	//
	//

	void Awake(){
		inited = false;
		myEntity = GetComponent<Entity>();
		if(myEntity != null)
			statSheet = myEntity.statSheet;
		
		InitializeStats();

		baseScale= transform.localScale;
		mySprite = GetComponentInChildren<SpriteRenderer>();

		/*
		particlesDict.Clear();
		effects.Clear();
		effectDict.Clear();
		modifiers.Clear();
		effectMods.Clear();
		modIndex.Clear();
		*/
	}

	void Update(){

		//Track Effect Timers
		efxToClear.Clear();
		for(int i = 0; i < effects.Count; i++){
			if (effects[i].UpdateTime(Time.deltaTime)){
				efxToClear.Add(effects[i]);
				Debug.Log("efx clear");
			}
		}
		int count = efxToClear.Count;
		for(int j = 0; j < count ; j++){
			RemoveEffect(efxToClear[j]);
		}
		//track combat timers
		TrackHitFlash();

	}

	/*
	 * Get stat from enum
	 */
	public int GetStatPow(Damager.Element ele){
		switch(ele){
		case Damager.Element.magic: return GetStat("magic_pow");
		case Damager.Element.phys: return GetStat("power");
		}
		return 0;
	}
	public int GetStatDef(Damager.Element ele){
		switch(ele){
		case Damager.Element.magic: return GetStat("magic_def");
		case Damager.Element.phys: return GetStat("armor");
		}
		return 0;
	}
	/*
	 * Damage code
	 * 
	 */

	public void RestoreAll(){
		stats["hp"] = GetStat("maxhp");
		stats["mp"] = GetStat("maxmp");
		PlayerStats.me.UpdateUI();
		//FloatingTextSpawner.SpawnText("Health and Mana restored", Color.yellow, transform);
	}

	public void Heal(int amt){
		
		if (GetStat("hp")+amt > GetStat("maxhp")) amt = GetStat("maxhp") - stats["hp"];
		stats["hp"] += amt;
		if(amt > 0)
		FloatingTextSpawner.SpawnText("Heal! +" + amt, Color.red, transform);
	}

	public void RestoreMP(int amt){
		if (stats["mp"]+amt > GetStat("maxmp")) amt =  GetStat("maxmp") - stats["mp"];
		stats["mp"] += amt;
		if(amt > 0)
		FloatingTextSpawner.SpawnText("Mana! +" + amt, Color.cyan, transform);
	}

	public void HitDamage(Damager d, Damager.Element ele){
		foreach(Effect e in d.effects){
			AddEffect(e);
		}
		if (d.damage != 0 && !CheckReflect(d) && hitFlashTimer < Time.time){
			bool instakill = false;
			if(d.caster != null){
				d.caster.target = this;
				attacker = d.caster;

				foreach(Effect e in d.caster.GetAllEffects()){
					if(e.effectName == "Executioner"){
						instakill = true;
						d.caster.RemoveEffect(e);
						break;
					}
				}
			}
			int dmg;
			dmg = CalcDamage(d);
			Debug.Log("damage = " + dmg);
			stats["hp"] -= dmg;
			hitFlashTimer = Time.time + 0.5f;
			Color cdc= (d.damageType == Damager.Element.phys)? Color.red : Color.cyan;
			FloatingTextSpawner.SpawnText(" -" + dmg.ToString(), cdc, this.transform);
			Knockback(d);

				if (stats["hp"] < 0 || instakill) stats["hp"] = 0;
				if (myController!= null) myController.OnDamage(d);
				else if (myEntity!= null) myEntity.OnDamage(d);
				if(statSheet.OnDamageSound != null && dmg > 0) LocalSfx.PlayFx(transform, statSheet.OnDamageSound, false, 0.6f);
	
		}
		CombatInfo.me.Check();
	}

	public bool CheckReflect(Damager d){
		
		int shield = 0;
		switch(d.damageType){
		case Damager.Element.magic: 
			shield = GetModifierValue("magic_reflect");
			break;
		case Damager.Element.phys:
			shield = GetModifierValue("power_reflect");
			break;
		}

		int threshold = d.damage;
		if(d.caster != null){
			threshold  = d.caster.GetStatPow(d.damageType) + d.damage;
		}
		if(d.caster != this && threshold <= shield){
			Reflect(d);
			return true;
		}
		else return false;
	}
	public void Reflect(Damager d){
		if(d.caster == null) return;
		d.immune.Clear();
		Character prev_caster = d.caster;
		d.caster = this;
		foreach(Component c in d.transform.GetComponents<Component>()){
			if ( !(c is Damager) && !(c is Transform) && !(c is Collider)){
				GameObject.Destroy(c);
			}
		}
		d.gameObject.AddComponent<SimpleProjectile>().Set(prev_caster.transform.position - d.transform.position, 
			8f, 3f);
	}

	int CalcDamage(Damager d){
		/*
		float caster_stat =(float)(  (d.caster!=null)? d.caster.GetStatPow(d.damageType) : d.damage);
		float incoming = d.damage * (1f + (Mathf.Log(caster_stat, 5f)/3f));
		Debug.Log("incoming " + incoming);
		float armor = (float) GetStatDef(d.damageType);
		Debug.Log("armor red " + (Mathf.Log(armor, 5f)/3f));
		incoming *= (1f - (Mathf.Log(armor, 5f)/3f));
		*/

		/*
		float caster_stat =(float)(  (d.caster!=null)? d.caster.GetStatPow(d.damageType) : d.damage);
		float armor = (float) GetStatDef(d.damageType);
		float damage = (float) d.damage;
		if(armor < 1f) armor = 1f;
		float incoming = damage * (caster_stat/5f) * (5f / armor);
		*/

		float base_damager = d.damage;
		float attack = (float)(  (d.caster!=null)? d.caster.GetStatPow(d.damageType) : d.damage);
		float def = ((float) GetStatDef(d.damageType)) * 0.666f;
		if(def < 1f) def = 1f;
		float mod = attack-def;
		mod /= 5f;

		float incoming = base_damager + mod + ((Random.value - 0.5f) * 2f);


		Debug.Log("redutcion: " + mod);

		if (incoming < 0f) incoming = 0f;
		return Mathf.RoundToInt(incoming);
	}

	//Knockback on a two plane axis doesn't add vertical movement
	void Knockback(Damager d){
		Vector3 dir = transform.position - d.transform.position;
		dir.Normalize();
		Knockback k = gameObject.AddComponent<Knockback>();
		k.Set(dir, d.knockback);
	}

	void TrackHitFlash(){
		
		if (hitFlashTimer > Time.time){
			mySprite.color = Color.Lerp(Color.white, Color.red, (hitFlashTimer - Time.time) / 0.5f);
		}

	}

	/*
	 * Mana
	 */ 

	public void SpendMana(int amt){
		stats["mp"] -= amt;
		EZStatInfo.UpdateStats();
	}

	public bool SpendHP(int amt){
		if(stats["hp"] - amt > 0){
			stats["hp"] -= amt;
			FloatingTextSpawner.SpawnText(" -" + amt, Color.red, this.transform);
			EZStatInfo.UpdateStats();
			return true;
		}
		else return false;
	}

	/*
	 * other stats
	 * 
	 */ 

	public void InitializeStats(){
		if(inited) return;

		inited = true;
		//init all stats to 5 by default
		if (statSheet == null)
		foreach(string st in statNames){
			stats.Add(st, 5); 
		}
		else InitFromCharSheet(statSheet);

		foreach(string sname in stats.Keys){
			modIndex.Add(sname, new List<Modifier>());
		}
	}

	void InitFromCharSheet(CharacterSheet cs){
		cs.Init();
		for(int i = 0; i < 9; i++){
			stats.Add(statNames[i], cs.ValFromNameIndex(i)); 
		}
	}

	void UpdateStats(){
		float scaleMod = GetModifierValue("size");
		if(scaleMod != 0){
			transform.localScale = baseScale * scaleMod;
		}
		else transform.localScale = baseScale;

	}

	/*
	 * Effect/Stat Modifier related code
	 * 
	 */
	public void AddEffect(Effect e){
		AddEffect(e, true);
	}

	public void AddEffect(Effect e, bool display){

		if (e.healAmt > 0){
			Heal(e.healAmt);
			e.healAmt = 0;
		}
		if (e.restoreMPAmt > 0){
			RestoreMP(e.restoreMPAmt);
			e.restoreMPAmt = 0;
		}

		if(e.duration > -1f && e.myModifiers.Length > 0){
			//if effect is not currently applied
			if (!effectDict.ContainsKey(e.effectName)){
				
				effects.Add(e);
				effectDict.Add(e.effectName, e);
				if(!effectMods.ContainsKey(e.effectName))
					effectMods.Add(e.effectName, new List<Modifier>());
				e.ResetStack();

				foreach(Modifier m in e.myModifiers){
					AddStatModifier(m, e);
				}
				if(e.upgrades != null)
				foreach(Upgrade u in e.upgrades){
					AddStatModifier(u.my_modifier, e);
				}
				if (e.particlesPrefab != null){
					GameObject go = GameObject.Instantiate(e.particlesPrefab);
					go.transform.SetParent(transform, false);
					particlesDict.Add(e, go);
				}
				if(display)
					FloatingTextSpawner.SpawnText(e.effectName, Color.magenta, this.transform);
			}

			//if effect can be stacked and not max stacked
			else if (e.isStackable && e.GetStacks() < e.maxStacks){
				effectDict[e.effectName].AddStack();
				Debug.Log("stack added");
				foreach(Modifier m in e.myModifiers){
					AddStatModifier(m, e);
				}
			}
			e.ResetTimer();
			Debug.Log("effect added " + e.effectName);
				
		}
		BroadcastMessage("UpdateStats");



		if(stats["hp"] > GetStat("maxhp")) stats["hp"] = GetStat("maxhp");
		if(stats["mp"] > GetStat("maxmp")) stats["mp"] = GetStat("maxmp");

		EZStatInfo.UpdateStats();
	}

	public void RemoveEffect(Effect x){
		Effect e = effectDict[x.effectName];
		//remove particle effects
		if (particlesDict.ContainsKey(e)){
			GameObject p = particlesDict[e];
			particlesDict.Remove(e);
			GameObject.Destroy(p);
		}
		//remove stat modifiers
		modsToClear.Clear();
		foreach(Modifier m in effectMods[e.effectName]){
			modsToClear.Add(m);
		}
		//must iterate twice because can't modify base collection while iterating 
		foreach(Modifier mm in modsToClear){
			RemoveStatModifier(mm, e);
		}
		//remove effect
		effectDict.Remove(e.effectName);
		effects.Remove(e);
		Debug.Log("Effect Removed: " + e.effectName);

		Heal(0);
		RestoreMP(0);

		BroadcastMessage("UpdateStats");
	}

	//note: link all modifiers to an effect so that there are no "loose" stat modifiers
	public void AddStatModifier(Modifier mod, Effect ef){
		modifiers.Add(mod);
		if(!modIndex.ContainsKey(mod.statName)){
			modIndex.Add(mod.statName, new List<Modifier>());
		}
		modIndex[mod.statName].Add(mod);
		effectMods[ef.effectName].Add(mod);
		//Debug.Log(mod.statName + " mod is " + GetModStat(mod.statName));
	}

	public void RemoveStatModifier(Modifier m, Effect e){
		modIndex[m.statName].Remove(m);
		effectMods[e.effectName].Remove(m);
		modifiers.Remove(m);
	}

	//Get Stat Modifier value

	public float GetModStat(string name){
		if(!modIndex.ContainsKey(name)){
			//Debug.Log("adding new modifier" + name);
			modIndex.Add(name, new List<Modifier>());
		}

		float amt = 1;

		foreach(Modifier m in modIndex[name]){
			if(m.modVal > 0)
				amt *= m.modVal;
		}
		return amt;
	}

	//Get Stat Bonus Value

	public int GetStatBonus(string stat){
		int b = 0;
		if (modIndex.ContainsKey(stat))
			foreach(Modifier m in modIndex[stat]){
				b += m.flatBonus;
			}
		//Debug.Log("mod" + mod);
		return b;
	}

	public int GetModifierValue(string mod){
		float b = 0;
		b= (float)GetStatBonus(mod);
		b *= GetModStat(mod);
		return (int)b;
	}

	public Effect[] GetAllEffects(){
		return effects.ToArray();
	}

	void OnDestroy(){
		
		if(attacker!=null)
		if(attacker.target == this)
			attacker.target = null;
		if(target != null){
			if(target.attacker == this) target.attacker = null;
		}

		CombatInfo.me.Check();
	}

}

