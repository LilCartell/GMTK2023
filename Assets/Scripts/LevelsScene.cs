using System.Collections.Generic;
using UnityEngine;

public class LevelsScene : MonoBehaviour
{
    public static LevelsScene Instance { get { return _instance; } }
    private static LevelsScene _instance;

    public LevelManager LevelManager;
    public GameObject TilesGrid;

    private int _currentLevelIndex = -1;
    private List<TileDefinition> _lastLevelLoaded;
    private List<Tile> _tiles = new List<Tile>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LoadNextLevel();
    }

    public void LoadNextLevelDebug()
    {
        _lastLevelLoaded = LevelManager.GetLevelTiles(++_currentLevelIndex);
        if (_lastLevelLoaded == null)
        {
            _currentLevelIndex = 0;
            _lastLevelLoaded = LevelManager.GetLevelTiles(_currentLevelIndex);
        }
        LoadLevel(_lastLevelLoaded);
    }

    public void LoadNextLevel()
    {
        _lastLevelLoaded = LevelManager.GetLevelTiles(++_currentLevelIndex);
        if(_lastLevelLoaded != null)
        {
            LoadLevel(_lastLevelLoaded);
        }
        else
        {
            //TODO End game
        }
    }

    public void RestartLastLevel()
    {
        LoadLevel(_lastLevelLoaded);
    }

    private void LoadLevel(List<TileDefinition> levelDefinition)
    {
        _tiles = new List<Tile>();
        foreach(var tileDefinition in levelDefinition)
        {
            _tiles.Add(new Tile(tileDefinition));
        }
        int tilesIndex = 0;
        foreach(var tileUI in TilesGrid.GetComponentsInChildren<TileUI>())
        {
            tileUI.LoadWithTile(_tiles[tilesIndex++]);
        }
    }
}
