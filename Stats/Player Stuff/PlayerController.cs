using UnityEngine;

public class PlayerController : MonoBehaviour {

	//delegates
	public delegate void PlayerDel();
	public PlayerDel onReset;

	//important referenecs
	public static PlayerController me;
	Inventory myInv;
	GlobalStateMachine GSM;
	PlayerMovement playerMovement;
	Interactor interactor;
	Rigidbody myRB;
	Vector3 storedVelocity;
	public GameObject menuPrefab;
	GameObject realMenu;
	GearSpriter headGear;
	GearSpriter bodyGear;

	//animations/sprites
	public Sprite displaySprite;
	public Sprite crouchSprite;
	SpriteRenderer aboveSprite;
	public SpriteRenderer playerSprite;

	//timer for holding item above head
	float displayTimer;
	public bool display = false; //display is used to pause player movement without pausing the game
	bool crouch = false; 

	//timer for blinking player sprite
	float blinkTimer;
	float nextBlink;
	bool blink = false;
	float blinkSpeed = 0.1f;

	//timer for using an item
	MyAnimation useAnims;
	float useTimer = 0f;

	//flags
	public bool riding = false;
	public Vehicle ride;

	//where to reset position to
	public Vector3 resetPos = Vector3.zero;

	float basespeed;

	public Item soul;
	MyAnimation cryAnim;

	//grab references
	void Awake() {
		me = this;

		playerMovement = GetComponent<PlayerMovement>();
		playerSprite = GetComponentInChildren<SpriteRenderer>();
		aboveSprite = GetComponentsInChildren<SpriteRenderer>()[1];
		aboveSprite.enabled = false;

		interactor = GetComponent<Interactor>();
		myRB = GetComponent<Rigidbody>();
		GSM = GetComponentInParent<GlobalStateMachine>();
		myInv = GetComponent<Inventory>();

		headGear = GetComponentsInChildren<GearSpriter>()[0];
		bodyGear = GetComponentsInChildren<GearSpriter>()[1];

		MyAnimation[] anims = GetComponentsInChildren<MyAnimation>();
		useAnims = anims[3];
		cryAnim = anims[4];

		QuestTracker.Init();
	}

	void Start(){
		//myInv.AddItem(soul, 1, false);
		EZStatInfo.UpdateStats();
		basespeed = playerMovement.baseSpeed;
	}

	public void PlayCry(){
		cryAnim.PlayAnim();
	}
	public void StopCry(){
		cryAnim.StopAnim();
	}

	public void Ride(Vehicle v){
		if (v == null){
			riding = false;
			playerSprite.enabled = true;
			playerMovement.myRB = myRB;
			myRB.isKinematic = false;
			GetComponent<BoxCollider>().enabled = true;
			resetPos = transform.position;
			headGear.Toggle(true);
			bodyGear.Toggle(true);
			playerMovement.baseSpeed = basespeed;
		}
		else {
			riding = true;
			transform.position = v.transform.position + v.playerOffset;
			playerMovement.myRB = v.myRB;

			myRB.isKinematic = true;
			GetComponent<BoxCollider>().enabled = false;
			if(v.altSprite != null){
				playerSprite.enabled = false;
				headGear.Toggle(false);
				bodyGear.Toggle(false);
			}
			playerMovement.baseSpeed = v.speed;

		}
		ride = v;
		playerMovement.UpdateStats();
		playerMovement.ride = v;

		//Debug.Log(v.name);
	}

	//Get a locational position in the direction the player is facing
	public Vector3 GetDirection(){
		int dir = (int) playerMovement.facing;
		Vector3 offset = Vector3.zero;
		switch(dir){
		case 0: offset = Vector3.back * 0.75f; break;
		case 1: offset = Vector3.right; break;
		case 2: offset = Vector3.forward * 0.75f; break;
		case 3: offset = Vector3.left; break;
		}
		return offset;
	}

	//trigger "use" sprite and disable action for duration
	public void Use(float time){
		int dir = (int) playerMovement.facing;
		switch(dir){
		case 0: useAnims.SetAnim(0); break;
		case 1:
		case 3: useAnims.SetAnim(1); break;
		case 2: useAnims.SetAnim(2); break;
		}
		useTimer = time;
		PauseMovementAnim();
		playerMovement.input_paused = true;
	}


	//add an item and hold it over your head
	public void AcquireItem(Item itm, int amt, int state, string sound){
		AudioLoader.PlaySound(sound, .8f, true, 0.6f);
		string desc = "";
		myInv.AddItem(itm, amt, state);
		Display(itm.GetSpriteFromState(state), .5f);
		if (itm is Book) desc = "book, ";
		if (amt == 1)
			PushMessage.Push("You found a " + desc +itm.name);
		else
			PushMessage.Push("You found " + desc +itm.name + "(" + amt +")");

		if(itm is CastableItem){
			CastableItem c = (CastableItem) itm;
			if(c.charged) c.charges = c.max_charges;
		}
	}
	public void AcquireItem(Item itm){
		AcquireItem(itm, 1);
	}
	public void AcquireItem(Item itm, int amt){
		AcquireItem(itm, amt, 0);
	}
	public void AcquireItem(Item itm, int amt, int state){
		AcquireItem(itm, amt, 0, "M3");
	}

	//reset's the players position to zone's entry point
	public static void ResetPos(){
		me.transform.position = me.resetPos + Vector3.up;
		me.myRB.velocity.Set(0f, 0f, 0f);
		PlayerMovement.me.yvel = 0f;
		PlayerMovement.me.prevyvel = 0f;
		Blink(1f); //blink for 1 seconds
		if(me.onReset!=null){
			me.onReset();
		}
		RaycastHit hit;
		if(Physics.BoxCast(me.transform.position, PlayerMovement.me.myBC.bounds.extents * 2f, Vector3.up, out hit, Quaternion.identity, 2f)){
			me.transform.position = new Vector3(me.transform.position.x, 
				hit.collider.bounds.center.y + hit.collider.bounds.extents.y, 
				me.transform.position.y);
		}
		Interactor.me.RefreshLabels();
	}

	//cause the player's sprite to blink
	public static void Blink(float time){
		me.blink = true;
		me.blinkTimer = Time.time + time;
		me.nextBlink = Time.time+me.blinkSpeed;
	}

	//hold an item above your head
	public void Display(Sprite itm, float duration){
		Display(itm, duration, Vector3.one);
	}

	//hold an item above your head
	public void Display(Sprite itm, float duration, Vector3 scale){
		display = true;
		displayTimer = Time.time + duration;
		PauseMovementAnim();
		playerMovement.walkingAnim.SetAnim(0);
		playerSprite.sprite = displaySprite;
		aboveSprite.enabled = true;
		aboveSprite.sprite = itm;
		aboveSprite.transform.localScale = scale;
		playerMovement.stored_facing = playerMovement.facing;
		playerMovement.facing = PlayerMovement.Directions.front;
		//headGear.Toggle(false);
	}

	//stop holding an item above your head
	void UnDisplay(){
		Debug.Log("Undisplay");
		display = false;
		ResumeMovementAnim();
		aboveSprite.enabled = false;
		aboveSprite.transform.localScale = Vector3.one;
		playerMovement.facing = playerMovement.stored_facing;
		if(GSM.IsPaused()==false) playerMovement.input_paused = false;
		//headGear.Toggle(true);
	}

	//change sprite to crouch and stop moving for a second
	public void Crouch(float duration){
		display = crouch = true;
		displayTimer = Time.time + duration;
		PauseMovementAnim();
		playerSprite.sprite = crouchSprite;
		headGear.Toggle(false);
		playerMovement.stored_facing = playerMovement.facing;
		playerMovement.facing = PlayerMovement.Directions.front;
	}

	void Uncrouch(){
		display = crouch = false;
		ResumeMovementAnim();
		headGear.Toggle(true);
		playerMovement.facing = playerMovement.stored_facing;
	}

	//interrupts the normal walk cycle so that crouch/display sprites can be shown 
	void PauseMovementAnim(){
		playerMovement.walkingAnim.StopAnim();
	}
	//return sprite control to the normal walk cycle
	void ResumeMovementAnim(){
		Debug.Log("RESUMEMOVEMENTANIUM");
		playerMovement.walkingAnim.SetAnim();
	}


	//pause
	void OnPause () {
		playerMovement.input_paused = true;
		interactor.enabled = false;
		storedVelocity = myRB.velocity;
		myRB.velocity = Vector3.zero;
		myRB.isKinematic = true;
	}

	//unpause
	void OnUnpause(){
		interactor.enabled = true;
		myRB.velocity = storedVelocity;


		if(!display)
		playerMovement.input_paused = false;

		if(!riding)
		myRB.isKinematic = false;
	}

	//update cycle
	void Update(){
		if (display){
			if(Time.time > displayTimer){
				if (crouch)Uncrouch();
				UnDisplay();
			}
		}

		if (blink){
			if (Time.time > blinkTimer){
				playerSprite.color = new Color(1f, 1f, 1f, 1f);
				blink = false;
			}
			else if (Time.time > nextBlink){
				//toggle the color aplha
				playerSprite.color = (playerSprite.color.a == 1f)? new Color(1f, 1f, 1f, 0f) : new Color(1f, 1f, 1f, 1f);
				nextBlink = Time.time+me.blinkSpeed;
			}
		}

		//if not paused
		if (!GlobalStateMachine.paused){

			if (useTimer > 0f){
				useTimer -= Time.deltaTime;
				if (useTimer <= 0f){
					Debug.Log("usetimer");
					ResumeMovementAnim();
					playerMovement.input_paused = false;
				}
			}
			else if (!playerMovement.input_paused){
				//use X item

				if (MyInput.GetState("X", true) == 'p'){
					PlayerStats.UseEquip(PlayerStats.getXEquip());
				}

				//use C item
				else if (MyInput.GetState("C", true) == 'p'){
					PlayerStats.UseEquip(PlayerStats.getCEquip());
				}
				

				//If the menu key is pressed
				if (MyInput.GetState("SHIFT", true) == 'p'){
					if (realMenu == null){ //create menu if it isn't open
						AudioLoader.PlayMenuSelect();
						realMenu = GameObject.Instantiate(menuPrefab);
						realMenu.transform.SetParent(GameObject.Find("UI").transform, false);
						realMenu.transform.SetAsFirstSibling();
						GSM.Pause();
					}
				}
				else if( MyInput.GetState("T", true) == 'p'){
					Shortcut(0);
				}
				else if( MyInput.GetState("BAG", true) == 'p'){
					Shortcut(1);	
				}
				else if( MyInput.GetState("SLF", true) == 'p'){
					Shortcut(2);	
				}
				else if( MyInput.GetState("QST", true) == 'p'){
					Shortcut(3);	
				}
				else if( MyInput.GetState("SPLS", true) == 'p'){
					Shortcut(4);	
				}
				else if( MyInput.GetState("CONFIG", true) == 'p'){
					Shortcut(5);	
				}
			}
		}
	}

	void Shortcut(int i){
		realMenu = GameObject.Instantiate(menuPrefab);
		realMenu.transform.SetParent(GameObject.Find("UI").transform, false);
		realMenu.transform.SetAsFirstSibling();
		realMenu.GetComponent<MenuController>().Shortcut(i);
		GSM.Pause();
	}
}
