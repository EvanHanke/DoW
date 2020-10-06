[System.Serializable]
public class Wormy{
	public string name;
	public string zone;
	public int level;
	public int hunger;
	public int age;
	public bool laid_egg;

	public Wormy(){
		age = 1;
		level = 1;
		hunger = 1;
		name = "Unnamed Worm";
		laid_egg = false;
	}

}
