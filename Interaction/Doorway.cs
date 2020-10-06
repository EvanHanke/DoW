using UnityEngine;

public class Doorway : MonoBehaviour {

	public string required_vehicle;
	public string targetZoneName;
	public string subZoneName;
	public int targetPoint;

	public Item key;
	public int num_keys = 1;

	StateSaver saver;

	void Awake(){
		saver = gameObject.AddComponent<StateSaver>();
		saver.onLoad = Load;
	}

	void Load(){
		if(saver.Get("opened") == "1") key = null;
	}

	void OnCollisionEnter(Collision collision){
		
		if (collision.gameObject.tag == "Player"){
			//if locked
			if (key != null){
				if (PlayerStats.playerInv.RemoveItem(key, num_keys)){
					key = null;
					PushMessage.Push("Door unlocked");
					saver.Save("opened", "1");
				}
				else{
					PushMessage.Push("This door is locked with a " + key.name);
					return;
				}
			}
			if(required_vehicle.Length >0){
				if(PlayerMovement.me.ride != null){
					if(PlayerMovement.me.ride.name != required_vehicle){
						return;
					}
				}
				else return;
			}
			AudioLoader.PlaySound("shuffle1", 1f, true);
			GlobalStateMachine.GPause();
			CameraEfx.FadeInOutB(1f, ChangeZone, GlobalStateMachine.GUnpause);

		}
	}

	public void ChangeZone(){
		Zone.ChangeZone(targetZoneName, subZoneName, targetPoint);
	}
}
