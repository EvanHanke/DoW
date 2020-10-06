using UnityEngine;

public class Bell : InteractionScript {

	public override string LabelDesc(){
		return "Ring Bell";
	}

	public override void OnInteract(){
		float r = (Random.value-0.5f)/10f;
		AudioLoader.PlaySound("Gong", .3f+r, true);
		CameraEfx.FadeInOut(1f, Advance);
		GlobalStateMachine.GPause();
	}

	public void Advance(){
		TimeTracker.AdvanceTime();
		GlobalStateMachine.GUnpause();

	}
}
