using System.Collections.Generic;

using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{


    [Header("Tile Atlas")]

    public float seed;
    public BiomeClass[] biomes;

    [Header("Biomes")]
    public Texture2D biomeMap;
    public float biomeFequency;
    public Gradient biomeGradient;

    [Header("World Settings")]
    
     public int worldSize;
   
    public int chunkSize = 10;
    private GameObject[] worldChunks;
    private bool generateCaves = true;

    [Header("Tree Settings")]
    public int maxTreeHeight = 12;
    private int t;
    public int minTreeHeight = 4;
    private int treeHeight;
    [Header("Addons")]
    public int grassChances = 1;
    public int specialGrassChances = 1;


    [Header("Noise Settings")]
    //public Texture2D caveNoiseTexture;
 

    public List<Vector2> worldTiles = new List<Vector2>();
    private BiomeClass curBiome;

    private void Start()
    {
       
        seed = Random.Range(-10000, 10000);
      
    
        DrawTextures();
        GenerateChunks();
        GenerateTerrain();

    }
    public void DrawTextures()
    { 
        biomeMap = new Texture2D(worldSize, worldSize);
      
        DrawBiomeTexture();
     
        for (int i = 0; i < biomes.Length; i++)
        {    
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);
            for (int o = 0; o< biomes[i].ores.Length; o++)
            { 
                biomes[i].ores[o].spreadTexture = new Texture2D(worldSize, worldSize);
            }
              GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);
            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                GenerateNoiseTexture(biomes[i].ores[o].rarity, biomes[i].ores[o].size, biomes[i].ores[o].spreadTexture);
            }

        }

    }
    public void DrawBiomeTexture()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float v = Mathf.PerlinNoise((x + seed) * biomeFequency, (y+seed) * biomeFequency);
                Color col = biomeGradient.Evaluate(v);
                biomeMap.SetPixel(x, y, col);
              
            }
        }
        biomeMap.Apply();

    }
    public void GenerateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];

        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }

    }
   public BiomeClass GetCurrentBiome(int x, int y)
{
    if (x < 0 || x >= worldSize || y < 0 || y >= worldSize)
    {
        return biomes[0]; 
    }

    Color currentColor = biomeMap.GetPixel(x, y);
    

    for (int i = 0; i < biomes.Length; i++)
    {
        float tolerance = 0.01f; 
        if (Mathf.Abs(biomes[i].biomeCol.r - currentColor.r) < tolerance &&
            Mathf.Abs(biomes[i].biomeCol.g - currentColor.g) < tolerance &&
            Mathf.Abs(biomes[i].biomeCol.b - currentColor.b) < tolerance)
        {
          
            return biomes[i];
        }
    }

 
    return biomes[0];
}


        public void GenerateTerrain()
        { 
            Sprite[] tileSprites;

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, 0); float height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + curBiome.heightAddition;
                if (y >= height)
                    break;
                else
                {
                    if (y < height - curBiome.soilDepth && y >= height - curBiome.deepDepth)
                    {
                        tileSprites = curBiome.tileAtlas.stone.tileSprites;

                        if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.coal.tileSprites;

                        if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.copper.tileSprites;
                        if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.iron.tileSprites;
                        if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.silver.tileSprites;


                    }
                    else if (y < height - curBiome.deepDepth && y >= height - curBiome.superDeepDepth)
                    {
                        tileSprites = curBiome.tileAtlas.stone.tileSprites;
                        if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.coal.tileSprites;
                        if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.copper.tileSprites;
                        if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.iron.tileSprites;
                        if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.silver.tileSprites;
                        if (curBiome.ores[4].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.gold.tileSprites;
                        if (curBiome.ores[5].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.diamond.tileSprites;


                    }
                    else if (y < height - curBiome.superDeepDepth)
                    {
                        tileSprites = curBiome.tileAtlas.deepStone.tileSprites;
                        if (curBiome.ores[6].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.ruby.tileSprites;
                        if (curBiome.ores[7].spreadTexture.GetPixel(x, y).r > 0.5f)
                            tileSprites = curBiome.tileAtlas.superRuby.tileSprites;


                    }
                    else if (y < height - 2 && y >= height - curBiome.soilDepth)
                    {
                        tileSprites = curBiome.tileAtlas.dirt.tileSprites;
                        generateCaves = false;
                    }
                    else if (y < height - 1 && y >= height - curBiome.soilDepth)
                    {
                        tileSprites = curBiome.tileAtlas.dirt1.tileSprites;

                        generateCaves = false;
                    }
                    else
                    {
                        tileSprites = curBiome.tileAtlas.surface.tileSprites;
                        generateCaves = false;


                    }

                    if (generateCaves)
                    {
                        if (curBiome.caveNoiseTexture.GetPixel(x, y).r > curBiome.surfaceValue)
                        {
                            PlaceTiles(tileSprites, x, y);
                        }
                    }
                    else
                    {
                        PlaceTiles(tileSprites, x, y);

                    }
                    if (y >= height - 1)
                    {
                        treeHeight = Random.Range(minTreeHeight, maxTreeHeight);
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            GeneratePlants(x, y);

                        }


                    }
                    generateCaves = true;

                }
            }
        }
        }

        private void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
        {
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    float v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);
                    if (v > limit)
                        noiseTexture.SetPixel(x, y, Color.white);
                    else
                        noiseTexture.SetPixel(x, y, Color.black);
                }
            }
            noiseTexture.Apply();
        }
        private void PlaceTiles(Sprite[] tileSprites, int x, int y)
        {
        if (tileSprites == null || tileSprites.Length == 0)
        {
            Debug.LogWarning($"尝试放置空的tiles在位置({x}, {y})");
            return; 
        }
        if (worldTiles.Contains(new Vector2(x, y)))
            {
                return;
            }

            GameObject newTile = new GameObject();
            float chunkCoord = (Mathf.RoundToInt(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;
            newTile.transform.parent = worldChunks[(int)chunkCoord].transform;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
            newTile.AddComponent<SpriteRenderer>();
            int spriteIndex = Random.Range(0, tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];
            newTile.name = tileSprites[0].name;
            worldTiles.Add(new Vector2(x, y));
        }

        private void GeneratePlants(int x, int y)          
                      
        {
            t = Random.Range(0, 10);
            if (t == 1)
            {
            for (int i = 0; i <= treeHeight; i++)
                {
                    PlaceTiles(curBiome.tileAtlas.log.tileSprites, x, y + i + 1);
                }


                int leafLayerCount = Mathf.Clamp(treeHeight / 2, 1, 4);
                for (int i = 0; i < leafLayerCount; i++)
                {
                    int leafOffset = leafLayerCount - i;
                
                    for (int j = -leafOffset; j <= leafOffset; j++)
                    {
                        PlaceTiles(curBiome.tileAtlas.leaf.tileSprites, x + j, y + treeHeight + 1 + i);
                        if (i == leafLayerCount - 1)
                        {
                            PlaceTiles(curBiome.tileAtlas.leaf.tileSprites, x, y + treeHeight + 2 + i);
                        }
                    }
                }
            }
            else
            {
                int i = Random.Range(0, 3);
                if (i == grassChances)

                    PlaceTiles(curBiome.tileAtlas.tallGrass.tileSprites, x, y + 1); 

            }
        }
} 

