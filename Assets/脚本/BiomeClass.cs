using System.Collections;
using UnityEngine;
[System.Serializable]
  public class BiomeClass
    {
      public string Name;
   public  Color biomeCol;
    public TileAtlas tileAtlas;

    [Header("Noise Settings")]
    public float caveFreq ;
    public float terrainFreq ;
    public Texture2D caveNoiseTexture;

    [Header("Generation Settings")]
    public float surfaceValue;
    public int soilDepth;
    public int deepDepth;
    public int superDeepDepth;
    public float heightMultiplier;
    public int heightAddition;

    [Header("Tree Settings")]
    public int maxTreeHeight = 12;
    private int t;
    public int minTreeHeight = 4;
    private int treeHeight;

    [Header("Addons")]
    public int grassChances = 1;
    public int specialGrassChances = 1;

    [Header("Ore Settings")]
    public OreClass[] ores;

}

