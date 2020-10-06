using UnityEngine;

[CreateAssetMenu]
public class GardeningShears : UsableItem {

	//checks if you are near an interable object with the shearable class attached
	public override bool OnUse(){
		if (Interactor.me.GetNearest() != null){
			Shearable s = Interactor.me.GetNearest().GetComponent<Shearable>();
			if (s != null){
				s.OnInteract();
			}
			return true;
		}
		return false;
	}
}
