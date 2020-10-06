using UnityEngine;

public class UsableItem : Item {
	public Item addOnUseItem;
	public int addAmt;
	public float useDuration;
	public virtual bool OnUse(){return true;}
	public string soundFileName;
}
