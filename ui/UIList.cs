using UnityEngine;
using UnityEngine.UI;

//flexible class for creating a navigatable list out of premade elements
public class UIList : MonoBehaviour {

	int selected, size, max, totalSize; //currently selected list element and size of the labels array and size of all elements

	int startingOffset; //for scrolling

	GameObject firstElement; //First list element *must have TextComponent in child*
	public GameObject[] allElements; //stores all elements

	GameObject moreAbove, moreBelow;

	RectTransform selector; //the selector visual
	Vector2 selectorRoot; //original transform of the selector
	float spacing; //spacing between each list element

	public string[] labels;

	public int GetSelected(){
		return selected;
	}

	public void ResetSelector(){

		selector.anchoredPosition = selectorRoot;
	}

	public void Init(int mxSz, GameObject first, float spcing, RectTransform slctr, string[] lls){

		//init needed variables
		startingOffset = 0;
		selected = 0;
		size = max = mxSz;
		firstElement = first;
		spacing = spcing;
		selector = slctr;
		selectorRoot = selector.anchoredPosition;



		SetLabels(lls);
		RefreshList();
	}

	public void CheckAboveBelow(GameObject above, GameObject below){
		if(startingOffset > 0 && size > 0) above.SetActive(true);
		else above.SetActive(false);

		if(startingOffset + size < totalSize) below.SetActive(true);
		else below.SetActive(false);
	}


	void SetLabels(string[] lls){
		SetLabels(lls, true);
	}
	//pass string array to labels array
	public void SetLabels(string[] lls, bool reset){
		if(reset){
			selected = 0;
			startingOffset = 0;
		}
		labels = lls;
		totalSize = labels.Length;
		firstElement.SetActive(true);
		if(lls.Length < size)
			size = lls.Length;
		else if (lls.Length > size)
			size = max;

		if (allElements != null){
			foreach(GameObject go in allElements){
				if(go != firstElement)
				GameObject.Destroy(go);
			}
			allElements = null;
		}
	
		if(totalSize > 0){
			allElements = new GameObject[((size > 0)? size : 1)];
			//init array and elements in the correct locations
			allElements[0] = firstElement;
			//only elements >0 need to be instantiated
			for(int i = 1; i < size; i++){
				Vector2 v = new Vector2(0f, spacing*i);
				allElements[i] = GameObject.Instantiate(firstElement);
				allElements[i].transform.SetParent(firstElement.transform.parent, false);
				RectTransform r = allElements[i].GetComponent<RectTransform>();
				r.anchoredPosition -= v;
			}
		}
		else{
			firstElement.SetActive(false);
		}

		RefreshList();
	}

	//update the UIList with new data
	public void RefreshList(string[] newList){
		SetLabels(newList, false);
		RefreshList();
	}

	//Eachs array element is text displayed on the label
	public void RefreshList(){

		startingOffset = (selected+1) - size;
		if (startingOffset < 0) startingOffset = 0;

		for(int i = 0; i < size; i++){
			//visible range = root + remaining)
			//root = selected
			//remaining = totalSize - (selected+1)
			if (i + startingOffset < totalSize){
				allElements[i].SetActive(true);
				allElements[i].GetComponentInChildren<Text>().text = labels[i+startingOffset];
			}
			else if (i < size){
				allElements[i].SetActive(false);
			}
		}

		RefreshSelector();
	}

	public void RefreshSelector(){
		//enable or disable the selector depending on if the list is populated or not
		selector.gameObject.SetActive(firstElement.activeSelf);
		if (firstElement.activeSelf)
			selector.anchoredPosition = selectorRoot - new Vector2(0, (selected-startingOffset) * spacing);
	}

	//increment // deincrement selector
	public void Inc(){
		AudioLoader.PlayMenuBlip();
		//if selector is not yet at the visual bottom of the list
		if (selected < totalSize-1){
			selected++;
			Debug.Log("Move past bottom");
		}
		//if selector is at the real bottom of the list, return to top
		else{
			selected = 0;
			Debug.Log("Jump t0 top");
		}

		RefreshList();
	}


	public void Dinc(){
		AudioLoader.PlayMenuBlip();
		//if selector is not yet at the visual top of the list
		if (selected-startingOffset > 0){
			selected--;
		}
		//if selector is at the real top of the list, jump to bottom
		else{
			selected = labels.Length-1;
		}
		RefreshList();
	}
}
