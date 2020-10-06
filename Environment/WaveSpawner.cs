using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

	public GameObject wavePrefab; //requires a sprite animator to work
	public WaveArea[] areas; //square zones in which waves spawn, assumes 32x32 sprites


	public void Start(){
		foreach(WaveArea w in areas){
			
			int numberOfWaves = (int)(w.extents.x / 2f)+1 ;
			int numberOfRows = (int)(w.extents.z / 2f)+1;

			GameObject rowContainer = new GameObject("WaveRow");
			rowContainer.transform.SetParent(transform);
			rowContainer.transform.position = transform.position;

			for(int i = 0; i < numberOfRows; i++){
				float zOffset = transform.position.z + (w.extents.z/2) - i*2f;
				Vector3 rowCenter = new Vector3(transform.position.x, transform.position.y, zOffset)  + new Vector3(1f, 0f, 1f);;
				SpawnRow(rowCenter, numberOfWaves, rowContainer.transform);
			}

			rowContainer.AddComponent<WaveAnimator>();
		}
	}

	void SpawnRow(Vector3 center, int total, Transform p){
		float xOffset = center.x - total;
		for(int i = 0; i < total; i++){
			Vector3 loc= new Vector3( xOffset + i*2, center.y, center.z);
			GameObject.Instantiate(wavePrefab, loc, Quaternion.identity, p);
		}
	}

	void OnDrawGizmos () {
		if (areas.Length > 0)
		foreach(WaveArea w in areas){
			Gizmos.DrawWireCube(transform.position, w.extents);
		}
	}
}

[System.Serializable]
public class WaveArea{
	public Vector3 extents;
	public WaveArea(Vector3 center, Vector3 extents){
		this.extents = extents;
	}
}
