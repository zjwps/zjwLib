using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileBaseGame
{
    public class Tile
    {
        public bool Walkable = false;
        public int Frame = 2;
    }
    public class Game
    {
        public int TileW = 30;
        public int TileH = 30;
        public int[,] MapData = new int[,] {
            { 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1},
            };
        public Transform clip;
        public Dictionary<string, Transform> ViewMap;

        public Game()
        {
            BuildMap(MapData);
        }
        public void BuildMap(int[,] map)
        {
            clip = new GameObject().transform;
            ViewMap = new Dictionary<string, Transform>();
            //多少行
            var mapHeight = map.GetLength(0);
            // 多少列
            var mapWidth = map.GetLength(1);
            Debug.Log("mapWidth:"+ mapWidth+" "+ "mapHeight:" + mapHeight);
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    // var name = "";

                }
            }
        }
    }
}
