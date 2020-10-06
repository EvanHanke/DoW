using UnityEngine;

[CreateAssetMenu]
public class GlyphItem : UsableItem {

	public enum Glyph {health, mana, power, magic, armor, magic_def, omega};
	public Glyph myGlyph;

	public override bool OnUse(){
		AudioLoader.PlaySound("LevelUp3");
		PlayerController.me.Display(image, 1f);
		//omega increases all stats
		if(myGlyph == Glyph.omega){
			foreach(string stat1 in Character.statNames){
				PlayerStats.myStats.SetStat(stat1, 
					PlayerStats.myStats.GetStat(stat1) + 1);
			}


			EZStatInfo.UpdateStats();
			return true;
		}

		//others increase one stat
		string stat = "maxhp";
		string substat = "null"; //boost current hp and mp
		int amount = 1;
		switch(myGlyph){
		case Glyph.health: 
			stat = "maxhp";
			substat = "hp";
			break;
		case Glyph.mana: stat = "maxmp";
			substat = "mp";
			break;
		case Glyph.power: stat = "power";
			amount = 2;
			break;
		case Glyph.magic: stat = "magic_pow";
			amount = 2;
			break;
		case Glyph.armor: stat = "armor";
			amount = 3;
			break;
		case Glyph.magic_def: stat = "magic_def";
			amount = 3;
			break;
		}
		FloatingTextSpawner.SpawnText("Stat Increased!", Color.yellow);
		PlayerStats.myStats.SetStat(stat, 
			PlayerStats.myStats.GetStat(stat) + amount);
		if(substat != "null")
		PlayerStats.myStats.SetStat(substat, 
			PlayerStats.myStats.GetStat(substat) + amount);
		EZStatInfo.UpdateStats();
		return true;
	}
}
