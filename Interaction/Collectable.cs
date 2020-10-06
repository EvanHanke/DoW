using UnityEngine;

public class Collectable : InteractionScript {

	public Item myItem;

	public override string LabelDesc(){
		return "collect " + myItem.name;
	}

	public override void OnInteract(){
		player.AcquireItem(myItem);
		GameObject.Destroy(gameObject);
	}

	public void Start(){
		if(GetComponent<GroundFollower>() != null) gameObject.AddComponent<GroundFollower>();
	}

	public override void OnApproach(){
		//Debug.Log("approached");
	}

	public override void OnLeave(){
		//Debug.Log("left");
	}

}
