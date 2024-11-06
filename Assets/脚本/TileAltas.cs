using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Altas ")]
public class TileAtlas : ScriptableObject
{
    [Header("Ores")]
    public TileClass stone;
    public TileClass deepStone;
    public TileClass dirt;
    public TileClass surface;
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond; 
    public TileClass ruby;
    public TileClass superRuby;
    public TileClass silver;  
    public TileClass copper;
    public TileClass dirt1;
    [Header("Environment")]
    public TileClass log;
    public TileClass leaf;
    public TileClass tallGrass;
    public TileClass water;
    public TileClass lava;
   

}