using UnityEngine;
[System.Serializable]
public class Modifier {

	int stacks;

	public string statName;
	public float modVal;
	public int flatBonus;

	public Modifier(string n, float f){
		statName = n;
		modVal = f;
		stacks = 1;
		flatBonus = 0;
	}
	public Modifier(string n, float f, int b){
		statName = n;
		modVal = f;
		stacks = 1;
		flatBonus = b;
	}

	public Modifier(string name, float mod, int bonus, int stacks){
		statName = name;
		modVal = mod;
		this.stacks = stacks;
		flatBonus = bonus;
	}

	public void AddStack() {
		stacks++;
	}
	public int GetStacks(){
		return stacks;
	}

	public string ToRichString(){
		Color c = Color.green;
		string b = TextColorer.StatName(statName) + " : ";
		if(modVal > 0 && modVal != 1f) {
			b+= (modVal*100) + "%";
			if(modVal < 1f) c = Color.red;
			if(flatBonus != 0)b+= ", ";
		}

		if(flatBonus!=0){
			if(flatBonus>0){
				b+="+"+flatBonus;
			} else{
				c = Color.red;
				b+="-"+Mathf.Abs(flatBonus);
			}
		}
		return TextColorer.ToColor(b, c);
	}

}
