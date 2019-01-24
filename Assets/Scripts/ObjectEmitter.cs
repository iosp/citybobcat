using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZObjectPools;
public class ObjectEmitter : MonoBehaviour {
	public EZObjectPool pool;
public GameObject ObjectToSpawn;
	// Use this for initialization
	void Start () {
		pool=FindObjectOfType<EZObjectPool>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Insert)) Emit();
	}
	public void Emit(){
		pool.TryGetNextObject(transform.position,Quaternion.identity);
	}
}
