using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dozer : MonoBehaviour
{
    public float sand = 0, MaxSand = 3.5f, EmmitAfterSecs = 0.2f, TimeOfLastContact = 0;
    public int EmmisionRate = 3;
    float startSize, startZ, sandLose = 0.05f;
    public ObjectEmitter[] Emitters;

    int l;
    public GameObject SandHolder;
    // Use this for initialization
    void Start()
    {
        if (FindObjectOfType<DiggableTerrain>())
            sandLose = FindObjectOfType<DiggableTerrain>().SandMaterialSpawnRatio;
        startZ = -0.234f;
        startSize = SandHolder.transform.localScale.y;
        SandHolder.transform.localPosition = new Vector3(0, 0.234f, startZ);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SandHolder.transform.localPosition = new Vector3(0, 0.234f, startZ + sand / 10);
        SandHolder.transform.localScale = new Vector3(3.87f, startSize + sand / 4, 0.11f);
        if (sand > 0 && Time.time > TimeOfLastContact + EmmitAfterSecs) for (int i = 0; i < EmmisionRate; i++)
                if (sand > 0)
                {
                    Emit();
                }
        {

        }
    }
    public void Emit()
    {
        Emitters[Random.Range(0, Emitters.Length)].Emit();
        sand -= sandLose;
        TimeOfLastContact = Time.time + 0.1f / EmmisionRate - EmmitAfterSecs;
    }

}
