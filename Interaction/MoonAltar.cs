using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonAltar : InteractionScript{

	public int myWarpPoint;

	public override void OnInteract(){
		SpawnPoint.SetRespawn(transform.position+Vector3.back);
		PlayerStats.myStats.RestoreAll();

		SaveFiler.SaveGame();
		PushMessage.Push("Progress Saved!");


		AudioLoader.PlaySound("dim_echo", 1f, true, 0.2f);
	}

	public override string LabelDesc(){
		return "Moon Altar";
	} 
}
