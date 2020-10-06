using UnityEngine;

public class Vehicle : InteractionScript {

	public string verb; //ride, sail, whatever
	public string myname; //boat, car (?)
	bool use = false;
	public Vector3 playerOffset; //where to place the player when they exit
	public Sprite altSprite; //sprite to display when in use
	public string zone, subzone;
	public bool gravity = false;
	public float speed;

	public StateSaver ss;

	[HideInInspector]
	public Rigidbody myRB;

	public override string LabelDesc(){
		return verb + " " + myname;
	}

	void Awake(){
		ss = gameObject.AddComponent<StateSaver>();
		ss.onLoad = Load;
		base.Awake();


	}

	void Start(){
		if(PlayerMovement.me.ride != null){
			if ((PlayerMovement.me.ride.name == name && PlayerMovement.me.ride != this) 
				|| PlayerMovement.me.ride == null) gameObject.SetActive(false);
		}

		Vehicle[] v = Zone.currentZone.GetComponentsInChildren<Vehicle>();
		foreach(Vehicle vv in v){
			if(vv.myname.Contains(myname)){
				float d_a = Vector3.Distance(transform.position, PlayerController.me.transform.position);
				float d_b = Vector3.Distance(vv.transform.position, PlayerController.me.transform.position);
				if(d_a > d_b){
					gameObject.SetActive(false);
					return;
				}
			}
		}
	}

	public void Load(){
		if(ss.Get("x") != null){
			float x = float.Parse( ss.Get("x"));
			float y = float.Parse( ss.Get("y"));
			float z = float.Parse( ss.Get("z"));
			transform.position = new Vector3(x, y, z);
		}
	}

	public override void OnInteract(){
		if(PlayerMovement.me.ground != GetComponent<Collider>()) return;

		if(altSprite != null){
			Sprite s = GetComponentInChildren<SpriteRenderer>().sprite;
			GetComponentInChildren<SpriteRenderer>().sprite = altSprite;
			altSprite = s;
		}

		if (!use){
			use = true;
			myRB = gameObject.AddComponent<Rigidbody>();
			myRB.useGravity = gravity;
			myRB.constraints = RigidbodyConstraints.FreezeRotation;
			PlayerController.me.Ride(this);
			transform.parent = GameObject.Find("Player").transform.parent;
			MusicLoader.me.ride_speed = 0.2f;
		}
		else{
			MusicLoader.me.ride_speed = 0f;
			PlayerController.me.Ride(null);
			use = false;
			Destroy(myRB);
			transform.parent = Zone.currentSubZone.transform;
			ss.Save("x", transform.position.x.ToString());
			ss.Save("y", transform.position.y.ToString());
			ss.Save("z", transform.position.z.ToString());
		}
	}
}
