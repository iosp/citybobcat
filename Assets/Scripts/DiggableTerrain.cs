using UnityEngine;
using System.Collections;
using System;
public class DiggableTerrain : MonoBehaviour
{

    public Dozer Digger;
    public bool TestWithMouse = false,Blur=false;
    public Terrain myTerrain;
    public int SmoothArea, DigArea = 2;
    private int xResolution;
    private int zResolution;
    private float[,] heights;
    private float[,] heightMapBackup;
    public int DeformationTextureNum = 1;
    protected int alphaMapWidth, heightmapResolution;
    protected int alphaMapHeight;
    protected int numOfAlphaLayers;
    private float[,,] alphaMapBackup;

    public float Stiffness = 2000;
    public float MaxSandReturn = 0;
    public float SandMaterialSpawnRatio = 1;
    public float terrainHeight;
    public float SoilCompressionRatio = 1f;

    public GameObject Sand;

    public float LooseSandToSpawn = 0;
    public float MaterialMissing = 0;
    Vector3 TrrSize;
    public float LooseSand = 0;
    float lastInteractionTime=0;
    void Awake()
    {
        xResolution = myTerrain.terrainData.heightmapResolution;
        zResolution = myTerrain.terrainData.heightmapResolution;
        alphaMapWidth = myTerrain.terrainData.alphamapWidth;
        alphaMapHeight = myTerrain.terrainData.alphamapHeight;
        numOfAlphaLayers = myTerrain.terrainData.alphamapLayers;

        heightmapResolution = myTerrain.terrainData.heightmapResolution;
        TrrSize = myTerrain.terrainData.size;
        terrainHeight = TrrSize.y;

        MaxSandReturn /= terrainHeight;

        heights = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
        heightMapBackup = myTerrain.terrainData.GetHeights(0, 0, xResolution, zResolution);
        alphaMapBackup = myTerrain.terrainData.GetAlphamaps(0, 0, alphaMapWidth, alphaMapHeight);
        InvokeRepeating("UpdateTerrain", 1, 0.5f);

    }
    void OnApplicationQuit()
    {
        myTerrain.terrainData.SetHeights(0, 0, heightMapBackup);
        myTerrain.terrainData.SetAlphamaps(0, 0, alphaMapBackup);
    }

    private void UpdateTerrain()
    {
        if(Blur&&Time.time<lastInteractionTime+2)myTerrain.terrainData.SetHeights(0,0,BlurMat(myTerrain.terrainData.GetHeights(0,0,heightmapResolution,heightmapResolution)));
        myTerrain.ApplyDelayedHeightmapModification();

    }
    void Update()
    {
        // This is just for testing with mouse!
        // Point mouse to the Terrain. Left mouse button
        // raises and right mouse button lowers terrain.
        if (TestWithMouse == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("hit");
                    // area middle point x and z, area width, area height, smoothing distance, area height adjust
                    raiselowerTerrainArea(hit.point, DigArea, DigArea, SmoothArea, MaxSandReturn);
                    // area middle point x and z, area size, texture ID from terrain textures
                    // TextureDeformation(hit.point, 10 * 2f, DeformationTextureNum);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    // area middle point x and z, area width, area height, smoothing distance, area height adjust
                    raiselowerTerrainArea(hit.point, DigArea, DigArea, SmoothArea, -MaxSandReturn);
                    // area middle point x and z, area size, texture ID from terrain textures
                    //  TextureDeformation (hit.point, 10 * 2f, 0);
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    // area middle point x and z, area width, area height, smoothing distance, area height adjust
                    MaterialMissing -= SetTerrainArea(hit.point, DigArea, DigArea, SmoothArea, 0);
                    // area middle point x and z, area size, texture ID from terrain textures
                    //  TextureDeformation (hit.point, 10 * 2f, 0);
                }
            }
        }
    }


    public float raiselowerTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec)
    {
        Vector3 TerrainPos = GetRelativeTerrainPositionFromPos(point, myTerrain, xResolution, zResolution);
        int areax;
        int areaz;
        smooth += 1;
        float amntOfSand = 0f;
        float smoothing;
        int terX = (int)TerrainPos.x;
        int terZ = (int)TerrainPos.z;
        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if (terX < 0) terX = 0;
        if (terX > xResolution - lenx) terX = xResolution - lenx;
        if (terZ < 0) terZ = 0;
        if (terZ > zResolution - lenz) terZ = zResolution - lenz;
        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);
        float y = heights[lenx / 2, lenz / 2];
        float compression = (heightMapBackup[terX + lenx / 2, terZ + lenz / 2] - y) * SoilCompressionRatio;
        if (incdec < 0)
            incdec = Mathf.Clamp(incdec * (-0.0000059057f + Mathf.Pow(0.5f, compression / 0.05757232f)), -0.1f, 0);
        else
            incdec = Mathf.Clamp(incdec * (-0.0000059057f + Mathf.Pow(0.5f, compression / 0.05757232f)), 0, 0.1f);
        for (smoothing = 1; smoothing < smooth + 1; smoothing++)
        {
            float multiplier = (Mathf.Sin(Mathf.PI * smoothing / smooth + 0.5f * Mathf.PI) + 1) / 2;
            for (areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++)
            {
                for (areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++)
                {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution))
                    {
                        amntOfSand += incdec * multiplier;
                        heights[areax, areaz] = Mathf.Clamp(heights[areax, areaz] + incdec * multiplier, 0, 1);
                    }
                }
            }
        }
        StartCoroutine(SandConvert(terrainToGlobal(terX + lenx / 2, terZ + lenx / 2, 0), point, amntOfSand * terrainHeight, 50));
        
        myTerrain.terrainData.SetHeightsDelayLOD(terX, terZ, heights);
        return amntOfSand;
    }
    public float CompressTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float mass)
    {

        float incdec = 0;
        Vector3 TerrainPos = GetRelativeTerrainPositionFromPos(point, myTerrain, xResolution, zResolution);
        int areax;
        int areaz;
        smooth += 1;
        float amntOfSand = 0f;
        float smoothing;
        int terX = (int)TerrainPos.x;
        int terZ = (int)TerrainPos.z;
        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if (terX < 0) terX = 0;
        if (terX > xResolution) terX = xResolution;
        if (terZ < 0) terZ = 0;
        if (terZ > zResolution) terZ = zResolution;
        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);

        for (smoothing = 1; smoothing < smooth + 1; smoothing++)
        {
            float multiplier = (Mathf.Sin(Mathf.PI * smoothing / smooth + 0.5f * Mathf.PI) + 1) / 2;
            for (areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++)
            {
                for (areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++)
                {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution))
                    {
                        float compression = (heightMapBackup[terX + areax, terZ + areaz] - heights[areax, areaz]) * SoilCompressionRatio;
                        incdec = Mathf.Clamp(-(mass - compression * Stiffness) * Time.fixedDeltaTime / (mass * terrainHeight * 20), -0.1f, 0);
                        heights[areax, areaz] = Mathf.Clamp(heights[areax, areaz] + incdec * multiplier, 0, 1);
                    }
                }
            }
        }
        myTerrain.terrainData.SetHeightsDelayLOD(terX, terZ, heights);
        return amntOfSand;
    }
    public float SetTerrainArea(Vector3 point, int lenx, int lenz, int smooth, float incdec)
    {
        Vector3 TerrainPos = GetRelativeTerrainPositionFromPos(point, myTerrain, xResolution, zResolution);
        int areax;
        int areaz;
        smooth += 1;
        float smoothing;
        int terX = (int)TerrainPos.x;
        int terZ = (int)TerrainPos.z;
        float amntOfSand = 0f;
        lenx += smooth;
        lenz += smooth;
        terX -= (lenx / 2);
        terZ -= (lenz / 2);
        if (terX < 0) terX = 0;
        if (terX > xResolution) terX = xResolution;
        if (terZ < 0) terZ = 0;
        if (terZ > zResolution) terZ = zResolution;
        float[,] heights = myTerrain.terrainData.GetHeights(terX, terZ, lenx, lenz);
        float sum = 0, average = 0;
        for (var i = 0; i < lenz; i++)
        {
            for (int j = 0; j < lenx; j++)
            {
                sum += heights[i, j];
            }
        }
        average = sum / heights.Length;
        float y = TerrainPos.y;
        for (smoothing = 1; smoothing < smooth + 1; smoothing++)
        {
            float multiplier = (Mathf.Sin(Mathf.PI * smoothing / smooth + 0.5f * Mathf.PI) + 1) / 2;
            for (areax = (int)(smoothing / 2); areax < lenx - (smoothing / 2); areax++)
            {
                for (areaz = (int)(smoothing / 2); areaz < lenz - (smoothing / 2); areaz++)
                {
                    if ((areax > -1) && (areaz > -1) && (areax < xResolution) && (areaz < zResolution))
                    {
                        incdec = Mathf.Clamp(y - heights[areax, areaz], -1, 0.001f);
                        amntOfSand += incdec * multiplier;
                        heights[areax, areaz] = Mathf.Clamp(heights[areax, areaz] + incdec * multiplier, 0, 1);
                    }
                }
            }
        }
        myTerrain.terrainData.SetHeightsDelayLOD(terX, terZ, heights);
        if(amntOfSand<0)StartCoroutine(SandConvert(terrainToGlobal(terX + lenx / 2, terZ + lenx / 2, average), point, amntOfSand * terrainHeight, 50));
        return amntOfSand;
    }
    private void raiselowerTerrainPoint(Vector3 point, float incdec)
    {
        int terX = (int)((point.x / myTerrain.terrainData.size.x) * xResolution);
        int terZ = (int)((point.z / myTerrain.terrainData.size.z) * zResolution);
        float y = heights[terX, terZ];
        y += incdec;
        float[,] height = new float[1, 1];
        height[0, 0] = Mathf.Clamp(y, 0, 1);
        heights[terX, terZ] = Mathf.Clamp(y, 0, 1);
        myTerrain.terrainData.SetHeightsDelayLOD(terX, terZ, height);
    }

    protected void TextureDeformation(Vector3 pos, float craterSizeInMeters, int textureIDnum)
    {
        Vector3 alphaMapTerrainPos = GetRelativeTerrainPositionFromPos(pos, myTerrain, alphaMapWidth, alphaMapHeight);
        int alphaMapCraterWidth = (int)(craterSizeInMeters * (alphaMapWidth / myTerrain.terrainData.size.x));
        int alphaMapCraterLength = (int)(craterSizeInMeters * (alphaMapHeight / myTerrain.terrainData.size.z));
        int alphaMapStartPosX = (int)(alphaMapTerrainPos.x - (alphaMapCraterWidth / 2));
        int alphaMapStartPosZ = (int)(alphaMapTerrainPos.z - (alphaMapCraterLength / 2));
        float[,,] alphas = myTerrain.terrainData.GetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphaMapCraterWidth, alphaMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;
        for (int i = 0; i < alphaMapCraterLength; i++) //width
        {
            for (int j = 0; j < alphaMapCraterWidth; j++) //height
            {
                circlePosX = (j - (alphaMapCraterWidth / 2)) / (alphaMapWidth / myTerrain.terrainData.size.x);
                circlePosY = (i - (alphaMapCraterLength / 2)) / (alphaMapHeight / myTerrain.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                if (distanceFromCenter < (craterSizeInMeters / 2.0f))
                {
                    for (int layerCount = 0; layerCount < numOfAlphaLayers; layerCount++)
                    {
                        //could add blending here in the future
                        if (layerCount == textureIDnum)
                        {
                            alphas[i, j, layerCount] = 1;
                        }
                        else
                        {
                            alphas[i, j, layerCount] = 0;
                        }
                    }
                }
            }
        }
        myTerrain.terrainData.SetAlphamaps(alphaMapStartPosX, alphaMapStartPosZ, alphas);
    }

    protected Vector3 GetNormalizedPositionRelativeToTerrain(Vector3 pos, Terrain terrain)
    {
        Vector3 tempCoord = (pos - terrain.gameObject.transform.position);
        Vector3 coord;
        coord.x = tempCoord.x / myTerrain.terrainData.size.x;
        coord.y = tempCoord.y / terrainHeight;
        coord.z = tempCoord.z / myTerrain.terrainData.size.z;
        return coord;
    }

    protected Vector3 GetRelativeTerrainPositionFromPos(Vector3 pos, Terrain terrain, int mapWidth, int mapHeight)
    {
        Vector3 coord = GetNormalizedPositionRelativeToTerrain(pos, terrain);
        return new Vector3((coord.x * mapWidth), coord.y, (coord.z * mapHeight));
    }
    protected Vector3 terrainToGlobal(int x, int z, float y)
    {
        Vector3 coord;
        coord.x = x * TrrSize.x / heightmapResolution;
        coord.z = z * TrrSize.z / heightmapResolution;
        coord.y = y * terrainHeight;
        coord += myTerrain.transform.position;
        return coord;
    }
    public void returnSand(Vector3 Point, float amnt)
    {
        MaterialMissing -= raiselowerTerrainArea(Point, 1, 1, SmoothArea * 2, amnt) * terrainHeight;
        lastInteractionTime=Time.time;
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Terrain Compressor"))
        {
            int contactCount = other.contacts.Length;

            foreach (var item in other.contacts)
            {
                MaterialMissing -= CompressTerrainArea(item.point, DigArea, DigArea, SmoothArea, 100) * terrainHeight;

            }
            //  Debug.looseSand(DigPower);
        }

    }
    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.CompareTag("Dozer"))
    //     {
    //         MaterialMissing -= SetTerrainArea(other.transform.position - Vector3.up * 0.05f, DigArea, DigArea, SmoothArea, 0) * terrainHeight;
    //     }
    // }
    IEnumerator SandConvert(Vector3 topPoint, Vector3 botPoint, float amnt, float rate)
    {
        if(amnt>10) Debug.Log("WTF?");
        int sandBunch = 0;
        LooseSandToSpawn = MaterialMissing / SandMaterialSpawnRatio - LooseSand;
        int numberOfSpawns = Mathf.RoundToInt(amnt/SandMaterialSpawnRatio);
        if (LooseSandToSpawn > numberOfSpawns)
            for (int i = -1; i < numberOfSpawns; i++)
            {
                Vector3 point = botPoint + (0.4f + i * 0.2f) * Vector3.up;
                // Instantiate(Sand, point, Quaternion.identity);
                Digger.Emit();
                LooseSand++;
                sandBunch++;
                LooseSandToSpawn = MaterialMissing / SandMaterialSpawnRatio - LooseSand;
                if (sandBunch > 10)
                {
                    sandBunch = 0;
                    yield return new WaitForSeconds(1 / rate);
                }
            }
        lastInteractionTime=Time.time;
            
    }
    float[,] BlurMat(float[,] mat)
    {
        int width = mat.GetLength(0), length = mat.GetLength(1);
        float[,] result = new float[heightmapResolution, heightmapResolution];
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < length - 1; j++)
            {
                float sum = 0;
                for (int f = i - 1; f < i + 2; f++)
                {
                    for (int r = j - 1; r < j + 2; r++)
                    {
                        sum += mat[f, r] / 9f;
                    }
                }
                result[i, j] = sum;
            }
        }
        return result;
    }


}