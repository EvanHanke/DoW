using UnityEngine;
using UnityEngine.UI;

public class NewDreamUI : MonoBehaviour {

	Text name_text;
	string prefix = "<I>TITLE: </I>    ";
	string currText = "";

	public GameObject diff_sel;

	void Start(){
		name_text = GetComponentsInChildren<Text>()[0];
	}

	void Update(){
		
		foreach (char c in Input.inputString){
			if (c == '\b') // has backspace/delete been pressed?
			{
				if (currText.Length > 0)
				{
					currText = currText.Substring(0, currText.Length - 1);
				}
			}
			else if ((c == '\n') || (c == '\r') && currText.Length > 0) // enter/return
			{
				this.enabled = false;
				CameraEfx.FadeInOut(3f, StartNewGame);
				AudioLoader.PlayMenuSelect();
			}
			else if (currText.Length < 16 && char.IsLetterOrDigit(c))
			{
				currText += char.ToUpper(c);
				AudioLoader.PlayMenuBlip();
			}
		}

		name_text.text = prefix + currText + "_";
	}

	void StartNewGame(){
		
		SaveFiler.activeSave = SaveFiler.CreateNewSave(currText);
		diff_sel.SetActive(true);
		transform.parent.parent.gameObject.SetActive(false);
	}
}
