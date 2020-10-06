using UnityEngine;
using UnityEngine.UI;

//controls stat information being displayed in the top bar
public class EZStatInfo : MonoBehaviour {

	public static EZStatInfo me;

	Image cImg;
	Image xImg; //item sprite
	Text equipXText; //item name
	Text equipCText;
	Text dayText;

	Text hpText, mpText;
	Text xpText;
	Text levelText;

	Sprite defaultImg;
	Image heartImg;
	Image manaImg;

	string xEquipLabel;
	string cEquipLabel;

	void Awake () {
		me = this;

		heartImg = GameObject.Find("HPImg").GetComponent<Image>();
		manaImg = GameObject.Find("MPImg").GetComponent<Image>();
		equipXText = GameObject.Find("XEquipLabel").GetComponent<Text>();
		equipCText = GameObject.Find("CEquipLabel").GetComponent<Text>();
		cImg = GameObject.Find("CImg").GetComponent<Image>();
		xImg = GameObject.Find("XImg").GetComponent<Image>();

		hpText = GameObject.Find("HPText").GetComponent<Text>();
		mpText = GameObject.Find("MPText").GetComponent<Text>();
		//xpText = GameObject.Find("XPText").GetComponent<Text>();
		//levelText = GameObject.Find("LevelText").GetComponent<Text>();

		//defaultImg = equipCImg.sprite;

		dayText = GameObject.Find("DayText").GetComponent<Text>();
	}

	void Start(){
		UpdateEquips();
		UpdateStats();
	}

	public void AdvanceTime(){
		UpdateStats();
	}
	public void RecallTime(){
		UpdateStats();
	}

	public static void UpdateStats(){
		int hp = PlayerStats.myStats.GetStat("hp");
		int maxhp = PlayerStats.myStats.GetStat("maxhp");
		int mp = PlayerStats.myStats.GetStat("mp");
		int maxmp = PlayerStats.myStats.GetStat("maxmp");

		me.hpText.text = "LIFE: " + hp + "/" + maxhp;
		me.mpText.text = "MANA: " + mp +"/" + maxmp;

		float xHp = (float) hp / (float) maxhp;
		float xMp = (float) mp / (float) maxmp;

		me.heartImg.fillAmount = xHp;
		me.manaImg.fillAmount = xMp;

		//me.xpText.text = "XP:" + PlayerStats.me.xp + "/" + PlayerStats.me.maxxp;
		//me.levelText.text = "LEVEL: " + PlayerStats.me.level;

		/*
		me.heartImg.sprite = me.heartImg.GetComponent<MyAnimation>().frames[
			
			Mathf.RoundToInt(Mathf.Lerp(0f, 5f,  ((float) hp  / (float) maxhp)))];
		
		me.manaImg.sprite = me.manaImg.GetComponent<MyAnimation>().frames[
			Mathf.RoundToInt(Mathf.Lerp(0f, 5f,  ((float) mp  / (float) maxmp)))]; */

		me.dayText.text = TimeTracker.me.today;

		me.UpdateEquips();

	}

	public void UpdateEquips(){
		EquipSlot esx = PlayerStats.getXEquip();
		EquipSlot esc = PlayerStats.getCEquip();
		//equipXImg.sprite = (esx.mySprite == null)? defaultImg : esx.mySprite;
		//equipCImg.sprite = (esc.mySprite == null)? defaultImg : esc.mySprite;
		//xImgBack.enabled = (esx.mySprite == null)? false : true;
		//cImgBack.enabled = (esc.mySprite == null)? false : true;
		xEquipLabel =  MyInput.me.GetButtonKey("X") + ": ";
		cEquipLabel =  MyInput.me.GetButtonKey("C") + ": ";
		if (esx.myName != null){
			int quantX = PlayerStats.playerInv.QuantityOf(esx.itm);
			equipXText.text = (quantX == 1)? xEquipLabel + esx.myName : xEquipLabel + esx.myName + " ("+quantX+")";
			if(esx.itm is CastableItem){
				CastableItem c = (CastableItem) esx.itm;
				if(c.charged)
					equipXText.text += TextColorer.ToColor(" ("+c.charges+")", Color.green);
			}
			xImg.enabled = true;
			xImg.sprite = esx.itm.image;
		}
		else{
			xImg.enabled = false;
			equipXText.text = xEquipLabel + " Empty";
		}
		if (esc.myName != null){
			int quantC = PlayerStats.playerInv.QuantityOf(esc.itm);
			equipCText.text = (quantC == 1)? cEquipLabel + esc.myName : cEquipLabel + esc.myName+ " ("+quantC+")";
		
			if(esc.itm is CastableItem){
				CastableItem c = (CastableItem) esc.itm;
				if(c.charged)
					equipCText.text += TextColorer.ToColor(" ("+c.charges+")", Color.green);
			}
			cImg.enabled = true;
			cImg.sprite = esc.itm.image;
		}
		else{
			cImg.enabled = false;
			equipCText.text = cEquipLabel + " Empty";
		}

	}

}
