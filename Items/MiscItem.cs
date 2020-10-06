using UnityEngine;

[CreateAssetMenu]

public class MiscItem : ConsumableItem {
	public override bool OnUse(){

		return Drop();

	}
}
