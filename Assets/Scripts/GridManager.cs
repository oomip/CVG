// from https://www.youtube.com/watch?v=kkAjpQAM-jE

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _earthTile, _deathTile;

    private Dictionary<Vector2, Tile> _tiles;

    void Start() {
        GenerateGrid();
    }

    void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_earthTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                spawnedTile.Init(x,y);

                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
    public void GenerateDeathTile(Vector2 pos) {
        int x = (int) pos[0];
        int y = (int) pos[1];
        if (GetTileAtPosition(pos) != null) {
            Destroy(GetTileAtPosition(pos));

            var spawnedTile = Instantiate(_deathTile, new Vector3(x,y), Quaternion.identity);
            spawnedTile.name = $"Tile {x} {y}";

            spawnedTile.Init(x,y);

            _tiles[pos] = spawnedTile;
        }
    }
    public void ClearDeathTiles() {
        Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();
        GenerateGrid();
    }
}
