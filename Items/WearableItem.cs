using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu]
public class WearableItem : UsableItem{
	public enum Slot {Head, Body};
	public Slot mySlot;
	public Sprite front, back, side;
	public float spriteAlpha = 1f;

	public Effect[] effects;

	public Sprite spriteFromDirection(int d){
		if (d == 0) return front;
		if (d == 2) return back;
		else return side;
	}

	public override bool OnUse(){
		PlayerStats.WearItem(this);
		return true;
	}
}
