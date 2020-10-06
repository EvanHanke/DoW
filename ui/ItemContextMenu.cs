using UnityEngine;
using UnityEngine.UI;

public class ItemContextMenu : MonoBehaviour {

	//menu for choosing what to do with an item

	Item item; //item to be utilized
	InventoryController ic;

	Text[] options;
	Image myPanel;
	RawImage selector;

	int selected = 0;

	void Awake () {
		ic = GetComponentInParent<InventoryController>();
		options = GetComponentsInChildren<Text>();
		myPanel = GetComponent<Image>();
		selector = GetComponentInChildren<RawImage>();
		foreach(Text t in options){
			t.enabled = false;
		}
		myPanel.enabled = false;
		selector.enabled = false;

	}

	//Use to enable the menu
	public void SetItem(Item itm){
		item = itm;
		selector.enabled = true;
		foreach(Text t in options){
			t.enabled = true;
		}
		myPanel.enabled = true;
		selected = 0;
		ic.scrollEnabled = false;
		UpdateSelectorPos();
	}

	void Update(){
		

		if(MyInput.GetState("DOWN") == 'p'){
			selected = (selected < options.Length-1)? selected+1 : 0;
			UpdateSelectorPos();
		}
		else if(MyInput.GetState("UP") == 'p'){
			selected = (selected > 0)? selected-1 : options.Length-1;
			UpdateSelectorPos();
		}

		else if (MyInput.GetState("Z", true) == 'p'){
			switch(selected){
			case 0: 
				Use();
				break;
			case 1: 
				Cancel();
				break;
			}
		}

		else if (MyInput.GetState("SHIFT") == 'p'){
			Cancel();
		}
	}

	void Use(){
		ic.UseItem(item);
		Cancel();
	}
		

	void Cancel(){
		//return to empty/ not being shown
		item = null;
		foreach(Text t in options){
			t.enabled = false;
		}
		myPanel.enabled = false;
		selector.enabled = false;
		ic.scrollEnabled = true;
	}

	void UpdateSelectorPos(){
		selector.GetComponent<RectTransform>().anchoredPosition =
			new Vector2(-70, SelectorYPos());
	}

	float SelectorYPos(){
		return (-90f-70f*selected);
	}

}
