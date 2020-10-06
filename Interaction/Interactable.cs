using UnityEngine;

public class Interactable : MonoBehaviour {
	
	public string myname;
	public bool autoTrigger = false;


	InteractionScript myScript;
	public AltInteractionScript xScript; //if the player uses their x or c item on this
	public AltInteractionScript cScript;

	AltInteractionScript[] allAltScripts;

	Interactor player;

	Transform ground; //enables objects to move with a moving ground
	Vector3 groundPos;
	Vector3 prevGroundPos;

	public string labelDesc{
		get {
			return myScript.LabelDesc();
		}
	}


	void Awake(){
		player = GameObject.Find("Player").GetComponent<Interactor>();
		allAltScripts = GetComponents<AltInteractionScript>();

		Ray r = new Ray(transform.position, Vector3.down);
		RaycastHit hit;
		if (Physics.Raycast(r, out hit, 0.1f)){
			ground = hit.transform;
			groundPos = prevGroundPos = ground.position;
		}
	}

	public void SetScript(InteractionScript iscript){
		myScript = iscript;


	}

	void Start(){
		if(autoTrigger) {
			Invoke("OnInteract", 1.1f);
		}
	}

	public void OnInteract(){
		myScript.OnInteract();
	}
	public void OnApproach(){
		//myScript.OnApproach();
	}
	public void OnLeave(){
		//myScript.OnLeave();
	}

	//check if equipped items have any special interactions with this object
	public void CheckAltScipts(){
		allAltScripts = GetComponents<AltInteractionScript>();
		xScript = cScript = null;
		foreach(AltInteractionScript alt in allAltScripts){
			if (alt.IsValid() == 'x'){
				//Debug.Log("Alt scripts X" + allAltScripts.Length);
				xScript = alt;
			}
			if (alt.IsValid() == 'c'){
				cScript = alt;
			}
		}
	}

	//check if nearest is valid, not obscured by a collider
	bool IsValid(Transform t){
		RaycastHit o;
		Ray toTarget = new Ray(transform.position + new Vector3(0f, 0.1f, 0f), t.transform.position - transform.position + new Vector3(0f, 0.5f, 0f));
		if (Physics.Raycast(toTarget, out o, player.maxDistance)){
			if (o.collider.transform != t.transform){
				return false;
			}
			return true;
		}
		return true;
	}

	void Update(){
				//check distance from player
		float distance = Vector3.Distance(transform.position, player.transform.position);
		if (distance < player.distanceToNearest){
			if (IsValid(player.transform)){
				CheckAltScipts();
				player.SetNearest(this,  distance);
			}
		}
		else if (distance > player.maxDistance){
			player.RemoveNearest(this);
		}

		//check ground movement
		if (ground != null){
			prevGroundPos = groundPos;
			groundPos = ground.position;
			Vector3 deltaGround = (groundPos - prevGroundPos);
			if (!deltaGround.Equals(Vector3.zero)){
					transform.position += deltaGround;
				//Debug.Log(deltaGround);
				}
			}
	}

	void OnDestroy(){
		player.RemoveNearest(this);
	}

}
