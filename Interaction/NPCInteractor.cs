using UnityEngine;

public class NPCInteractor : InteractionScript {

	public float voiceMod = 1f;
	public string npcName;
	Dialogue[] myDialogues;
	int currentDialogue = 0;
	public int timesInteracted = 0;
	public QuestChecker[] myQuests;
	public string timeForwardDialog, timeBackDialog;
	float timer;
	public Color bubbleColor = Color.white;
	public string[] bubbles;
	int curr;

	public void AdvanceTime(){
		if (timeForwardDialog != null) ChangeDialogue(timeForwardDialog);
	}
	public void RecallTime(){
		if (timeBackDialog != null) ChangeDialogue(timeBackDialog);
	}

	void Awake() {
		myDialogues = GetComponents<Dialogue>();
		if (isPrimary){
			GetComponent<Interactable>().SetScript(this);
		}
		curr = 0;
		timer= 0f;
	}

	public override string LabelDesc(){
		if (timesInteracted == 0) return "???";
		else return npcName;
	} 

	public void ChangeDialogue(int which){
		currentDialogue = which;
	}

	public void ChangeDialogue(string s){
		for(int i = 0; i < myDialogues.Length; i++){
			if (myDialogues[i].dName == s){
				ChangeDialogue(i);
			}
		}
	}
	
	public override void OnInteract(){
		timesInteracted++;
		AudioLoader.voice_mod = voiceMod;
		myDialogues[currentDialogue].OnInteract();
	}

	public byte[] Save(){
		byte[] data = {(byte) currentDialogue, (byte) timesInteracted};
		return data;
	}

	public void Load(byte[] data){
		currentDialogue = (int) data[0];
		timesInteracted = (int) data[1];
		//Debug.Log("NPC LOADED" + currentDialogue + " " + timesInteracted);
	}

	public void Update(){
		if(GlobalStateMachine.paused == false && bubbles.Length > 0){
			timer += Time.deltaTime;
			if (timer > 2f){
				timer = 0f;
				FloatingTextSpawner.SpawnText(bubbles[curr], bubbleColor, transform);
				curr = (curr < bubbles.Length-1)? curr + 1 : 0 ;
			}
		}
	}
}


