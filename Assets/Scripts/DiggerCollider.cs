using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DiggerCollider : MonoBehaviour
{
    [CanEditMultipleObjects]
	
    public float Spring=1000,Damper=100,DigScale=1,force;
    Rigidbody rb;
	Transform MyTransform;
    DiggableTerrain diggableTerrain;
    ConfigurableJoint spring;
	Vector3 origpos,disp;
    // Use this for initialization
    void Start()
    {
		origpos=transform.localPosition;
        spring = GetComponent<ConfigurableJoint>();
        diggableTerrain = FindObjectOfType<DiggableTerrain>();
        rb = GetComponent<Rigidbody>();
        MyTransform = transform; 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
		var temp=spring.xDrive;
		temp.positionSpring=Spring;
		temp.positionDamper=Damper;
		spring.xDrive=temp;
		spring.zDrive=temp;
		temp.positionSpring=Spring/1000;
		temp.positionDamper=Damper/1000;
		spring.yDrive=temp;
		disp=transform.localPosition-origpos;
		force=disp.magnitude;
    }
	private void OnCollisionStay(Collision other) {
           
		   if(other.gameObject.CompareTag("Diggable Terrain"))  diggableTerrain.MaterialMissing -= diggableTerrain.raiselowerTerrainArea(MyTransform.position+MyTransform.forward*0.2f, 1, 1, diggableTerrain.SmoothArea * 2, -force*DigScale) * diggableTerrain.terrainHeight;
		
	}
}
