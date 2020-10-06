using UnityEngine;

public class InteractionScript : MonoBehaviour {
	public virtual void OnInteract(){}
	public virtual void OnApproach(){}
	public virtual void OnLeave(){}
	public virtual string LabelDesc(){return name;}

	protected UIController ui;
	protected PlayerController player;

	//if this is the primary interaction script of the game object
	public bool isPrimary = true;

	public void Awake(){
		ui = GameObject.Find("UI").GetComponent<UIController>();
		player= GameObject.Find("Player").GetComponent<PlayerController>();

		if (isPrimary && GetComponent<Interactable>() != null){
			GetComponent<Interactable>().SetScript(this);
		}
	}
}
