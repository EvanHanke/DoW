using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskTerminal : InteractionScript {

	public string title;
	public int pillar;

	public override string LabelDesc(){
		return title;
	}

	public override void OnInteract(){

		if (DiskController.me.Move(pillar)){
			FloatingTextSpawner.SpawnText("Processing...", Color.yellow, transform);
		} 
		else{
			FloatingTextSpawner.SpawnText("Error! No space!", Color.red, transform);
		}
	}
}
