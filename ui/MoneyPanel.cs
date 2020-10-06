using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : MonoBehaviour {

	Text myText;

	void Awake(){
		myText = GetComponentInChildren<Text>();
		UpdatePanel();
	}

	void Update () {
		
	}

	public void UpdatePanel(){
		myText.text = "Money: " + GameObject.Find("Player").GetComponent<Inventory>().money + "$";
	}
}
