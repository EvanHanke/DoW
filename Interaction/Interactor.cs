using UnityEngine;

public class Interactor : MonoBehaviour {

	public static Interactor me;
	public GameObject haloPrefab;
	GameObject halo;

	UIController ui;

	Interactable prevNearest;
	public Interactable nearest;
	Interactable[] all;

	public float distanceToNearest;
	public float maxDistance;


	//timer which limits the rate at which interactables are refreshed to minimize unnecesary raycasts


	void Awake(){
		me = this;
		ui = GameObject.Find("UI").GetComponent<UIController>();
		//updateTimer = Time.time;
		distanceToNearest = maxDistance = 1.5f;
	}

	void Update () {
		
		//check if the nearest interable has changed since last frame
		if (prevNearest != nearest){
			UpdateHalo();
			RefreshLabels();
		}
		prevNearest = nearest;

		if(nearest != null) distanceToNearest = Vector3.Distance(transform.position, nearest.transform.position);

		//When player hits Z
		if (nearest != null && MyInput.GetState("Z", true) == 'p' && CameraEfx.me.isAnimating == false){
			Debug.Log("Z hit interactor " + nearest.name);
			nearest.Invoke("OnInteract", 0f);
		}

	}

	public Interactable GetNearest(){
		return nearest;
	}


	public void SetNearest(Interactable i, float d){
		//if(PlayerController.me.riding) return;
		nearest = i;
		distanceToNearest = d;
		nearest.OnApproach(); //call interactable's interaction script on approach
	}

	public void RemoveNearest(Interactable i){
		if (nearest == i){
			nearest = null;
			distanceToNearest = maxDistance;
			UpdateHalo();
			RefreshLabels();
		}
	}

	void UpdateHalo(){
		//if halo should evaporate, 
		if (nearest == null && halo != null){
			GameObject.Destroy(halo);
			return;
		}
		else if (nearest != null && halo == null){ 
			//create a halo if it isn't existing
			halo = GameObject.Instantiate<GameObject>(haloPrefab);
			halo.transform.SetParent(nearest.transform, false);
			halo.transform.SetParent(nearest.transform, false);
		}
		else if (halo == null && nearest == null) return;
		else if (halo.transform.parent != nearest.transform){
			//move halo to target
			halo.transform.SetParent(nearest.transform, false);
		}
	}

	public void RefreshLabels(){
		if (nearest == null){
			ui.RemoveLabel();
			ui.RemoveAltLabel();
			return;
		}
		string alt = "";
		if (nearest.xScript != null) alt = "X: " + nearest.xScript.Label();
		if (nearest.cScript != null) alt = "C: " + nearest.cScript.Label();
		string s = "Z: " + nearest.labelDesc;
		if(alt!="")
			ui.CreateAltLabel(alt);
		ui.CreateLabel("Z: " + nearest.labelDesc);

	}
}
