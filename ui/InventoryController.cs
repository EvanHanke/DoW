using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class InventoryController : MonoBehaviour {

	public static InventoryController me = null;

	UIList myList;

	Inventory inv;
	public GameObject itemTag; //first item in the list (for iterating)
	List<Transform> itemList; //list of all item tags currently being displayed
	Image itemImg;
	Sprite emptyItemSprite;
	Text itemDesc;
	Text valueLabel;
	Text sortLabel;
	int sort = 0;
	RectTransform selector; //the selector visual
	int selected = 0;
	public bool scrollEnabled = true;
	InvEntry[] items;
	ItemContextMenu cm;
	GameObject above, below;


	void Awake () {
		if(!gameObject.activeSelf) return;

		//Grab player's inventory
		inv = GameObject.Find("Player").GetComponent<Inventory>();


		//grab subcomponents
		myList = GetComponentInChildren<UIList>();
		selector = GameObject.Find("Selector1").GetComponent<RectTransform>();;
		itemImg = GameObject.Find("ItemImage").GetComponent<Image>();
		emptyItemSprite = itemImg.sprite;
		itemDesc = GameObject.Find("ItemDesc").GetComponent<Text>();;
		cm = GetComponentInChildren<ItemContextMenu>(true);
		//valueLabel = transform.GetChild(2).GetChild(2).GetComponent<Text>();
		sortLabel = GameObject.Find("SortLabel").GetComponent<Text>();
		above = GameObject.Find("MoreAbove");
		below = GameObject.Find("MoreBelow");

		//init itemList
		itemList = new List<Transform>();
		//itemList.Add(transform.GetChild(1).GetChild(1));
		items = inv.GetAllItems();
		//populate item list
		myList.Init(6, itemTag, 80, selector,LabelTexts(items));

		RefreshInfo(0);

		//flag as open
		me = this;
	}

	void Init(){
		items = inv.GetAllItems();
		RefreshInfo(0);
		myList.CheckAboveBelow(above, below);
	}
	void Init<T>(){
		myList.ResetSelector();
		items = inv.GetItems<T>();
		myList.Init(6, itemTag, 80, selector,LabelTexts(items));
		RefreshInfo(0);
		myList.CheckAboveBelow(above, below);
	}
	void Init<T, B>(){
		myList.ResetSelector();
		List<InvEntry> itms = new List<InvEntry>();
		items = inv.GetItems<T>();
		itms.AddRange(items);
		items = inv.GetItems<B>();
		itms.AddRange(items);
		items = itms.ToArray();
		myList.Init(6, itemTag, 80, selector,LabelTexts(items));
		RefreshInfo(0);
		myList.CheckAboveBelow(above, below);
	}
	void Sort(){
		switch(sort){
		case 0:
			Init<Item>();
			sortLabel.text = "ALL";
			break;
		case 1:
			Init<StatModItem, GlyphItem>();
			sortLabel.text = "USEFUL";
			break;
		case 2:
			Init<Growable>();
			sortLabel.text = "PLANTS";
			break;
		case 3:
			sortLabel.text = "CLOTHES";
			Init<WearableItem>();
			break;
		case 4:
			sortLabel.text = "TOOLS";
			Init<CastableItem, GardeningShears>();
			break;
		case 5:
			sortLabel.text = "BOOKS";
			Init<Book>();
			break;
		case 6:
			sortLabel.text = "MISC";
			Init<MiscItem>();
			break;
		}
	}

	void Update () {
		if (!scrollEnabled) return;
		if (BookUI.bookUI != null) return;

		if (MyInput.GetState("SHIFT", true) == 'p' || MyInput.GetState("BAG") == 'p'){
			me = null;
			GameObject.Destroy(gameObject);
		}

		//if you have items in your inventory, cycle through them
		else if (items.Length > 0){
			if(MyInput.GetState("DOWN") == 'p'){
				myList.Inc();
				RefreshInfo(myList.GetSelected());
			}
			else if(MyInput.GetState("UP") == 'p'){
				myList.Dinc();
				RefreshInfo(myList.GetSelected());
			}
			//if item selected
			else if (MyInput.GetState("Z", true) == 'p'){
				AudioLoader.PlayMenuBlip();
				cm.gameObject.SetActive(true);
				cm.SetItem(items[myList.GetSelected()].itm);
			}
			else if (MyInput.GetState("X", true) == 'p'){
				AudioLoader.PlayMenuBlip();
				SetX(items[myList.GetSelected()].itm);
			}
			else if (MyInput.GetState("C", true) == 'p'){
				AudioLoader.PlayMenuBlip();
				SetC(items[myList.GetSelected()].itm);
			}
		}

		if(MyInput.GetState("LEFT") == 'p'){
			Debug.Log("ping");
			AudioLoader.PlayMenuBlip();
			sort--;
			if(sort < 0) sort = 6;
			Sort();
		}
		else if(MyInput.GetState("RIGHT") == 'p'){
			sort++;
			AudioLoader.PlayMenuBlip();
			if(sort > 6) sort = 0;
			Sort();
		}
	}

	//using an item from the inventory panel
	public void UseItem(Item itemm){
		if (itemm is UsableItem){
			UsableItem c = (UsableItem) itemm;

			if (c.OnUse()){
				PlayerStats.UseItem(c);
				PlayerStats.RefreshEquips();
				if ((c is CastableItem)){
					
					GameObject.Destroy(MenuController.me.gameObject);
					GameObject.Destroy(gameObject);
					me= null;
				}
				else{
					Sort();
				}
			}
		}
	}

	void SetX(Item item){
		PlayerStats.SetEquip(PlayerStats.getXEquip(), (UsableItem) item);
		Debug.Log("set X");
		RefreshText();
	}

	void SetC(Item item){
		PlayerStats.SetEquip(PlayerStats.getCEquip(), ( UsableItem) item);
		Debug.Log("set C");
		RefreshText();
	}

	string[] LabelTexts(InvEntry[] itm){
		string[] lls = new string[itm.Length];
		for(int i = 0; i < itm.Length;i++){
			string a = LabelText(itm[i]);
			if(itm[i].itm.equipped.Length >1){
				a = TextColorer.ToColor(a, Color.yellow);
				a = itm[i].itm.equipped+" "+a;
			}
			if(itm[i].itm is CastableItem){
				CastableItem c = (CastableItem) itm[i].itm;
				if(c.upgraded)
					a+= TextColorer.ToColor("(+)", Color.yellow);
				if(c.charged)
					a+= TextColorer.ToColor(" ("+c.charges+")", Color.green);
				
			}
			lls[i] = a;
		}
		return lls;
	}
		

	string LabelText(InvEntry itm){
		string append = "";
		int q = inv.QuantityOf(itm.name);
		if (q > 1) append = " (" + inv.QuantityOf(itm.name) + ")";

		return itm.name + append;
	}

	void RefreshText(){
		myList.RefreshList(LabelTexts(items));
	}

	//refreshes the visual and description based on item index
	void RefreshInfo(int a){
		myList.CheckAboveBelow(above, below);
		if (items.Length == 0){
			itemImg.enabled = false;
			itemDesc.enabled = false;
		}
		else{
			Item itm = items[a].itm;

			itemImg.enabled = true;
			itemDesc.enabled = true;

			itemImg.sprite= itm.image;

			string s = Item.GetFullDesc(itm);

			itemDesc.text = s;
		}
	}
}
