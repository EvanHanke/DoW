using UnityEngine;

public class ShortMessage : InteractionScript {

	[TextArea(0,2)]
	public string message;

	public string label = "Check";

	public override string LabelDesc ()
	{
		return label;
	}

	public override void OnInteract ()
	{
		UIController.me.PrintMsg(message);
	}
}
