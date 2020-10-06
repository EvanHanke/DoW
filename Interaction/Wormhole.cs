using UnityEngine;

public class Wormhole : InteractionScript {

	StateSaver ss;
	public WormHoleSave my_save;

	void Start(){
		base.Awake();
		my_save = Wormholer.me.GetFromLoc(transform.position);
		if(my_save == null){
			my_save = Wormholer.me.AddNew(transform);
		}
	}

	public override string LabelDesc(){
		return "Use Wormhole";
	}

	public override void OnInteract(){
		GameObject g = UIController.me.CreateWHMenu();

		g.GetComponent<WormholeUI>().wh = this;
	}
}
