using System.Collections.Generic;
using System.Linq;
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
    private List<Tile> _modifiedTilesThisFrame = new List<Tile>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        LoadNextLevel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCharacters();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartLastLevel();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ExecuteMovement(Directions.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ExecuteMovement(Directions.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ExecuteMovement(Directions.UP);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ExecuteMovement(Directions.DOWN);
        }

        RefreshModifiedTiles();
        TriggerGameOverIfNeeded();
        TriggerSuccessIfNeeded();
    }

    private void ExecuteMovement(Directions direction)
    {
        var flottyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.FLOTTY);
        var pushyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.PUSHY);

        var nextFlottyTile = GetTileAfterMovement(flottyTile, direction);
        if(nextFlottyTile != null)
        {
            if(nextFlottyTile.CurrentElementPresent != TileElement.BOX
                && nextFlottyTile.Type != TileType.WALL) //Flotty can go anywhere except tiles with box or walls
            {
                flottyTile.SetNewElement(TileElement.NONE);
                nextFlottyTile.SetNewElement(TileElement.FLOTTY);
                //Otherwise nothing happens so we only need to refresh tiles in this case
                _modifiedTilesThisFrame.Add(flottyTile);
                _modifiedTilesThisFrame.Add(nextFlottyTile);
            }
        }

        var nextPushyTile = GetTileAfterMovement(pushyTile, direction);
        if(nextPushyTile != null)
        {
            if(nextPushyTile.Type != TileType.WALL)
            {
                bool blockedByBox = false;
                if(nextPushyTile.CurrentElementPresent == TileElement.BOX) //Pushy dies on holes but he CAN travel there
                //Pushy is only blocked by boxes if they can't be pushed, i.e if they're next to a wall or another box
                {
                    var nextBoxTile = GetTileAfterMovement(nextPushyTile, direction);
                    if(nextBoxTile != null && nextBoxTile.Type != TileType.WALL
                        && nextBoxTile.CurrentElementPresent != TileElement.BOX
                        && nextBoxTile.CurrentElementPresent != TileElement.FLOTTY)
                    {
                        if(nextBoxTile.Type != TileType.HOLE) //Boxes disappear in holes so hole tiles remain unaffected
                        {
                            nextBoxTile.SetNewElement(TileElement.BOX);
                            _modifiedTilesThisFrame.Add(nextBoxTile);
                        }
                    }
                    else //If movement direction is taken by a wall or box or the tile doesn't exist, we're blocked and nothing happens
                    {
                        blockedByBox = true;
                    }
                }
                if(!blockedByBox)
                {
                    pushyTile.SetNewElement(TileElement.NONE);
                    nextPushyTile.SetNewElement(TileElement.PUSHY);
                    _modifiedTilesThisFrame.Add(pushyTile);
                    _modifiedTilesThisFrame.Add(nextPushyTile);
                }
            }
        }
    }

    private Tile GetTileAfterMovement(Tile tile, Directions movementDirection)
    {
        int desiredX = tile.X;
        int desiredY = tile.Y;

        switch(movementDirection)
        {
            case Directions.UP:
                --desiredY;
                break;

            case Directions.DOWN:
                ++desiredY;
                break;

            case Directions.LEFT:
                --desiredX;
                break;

            case Directions.RIGHT:
                ++desiredX;
                break;
        }

        return _tiles.FirstOrDefault(testedTile => testedTile.X == desiredX && testedTile.Y == desiredY);
    }

    private void SwitchCharacters()
    {
        var flottyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.FLOTTY);
        var pushyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.PUSHY);

        flottyTile.SetNewElement(TileElement.PUSHY);
        pushyTile.SetNewElement(TileElement.FLOTTY);

        _modifiedTilesThisFrame.Add(flottyTile);
        _modifiedTilesThisFrame.Add(pushyTile);
    }

    private void RefreshModifiedTiles()
    {
        var tileUIs = TilesGrid.GetComponentsInChildren<TileUI>();
        foreach (var tile in _modifiedTilesThisFrame)
        {
            tileUIs.First(tileUI => tileUI.LinkedTile == tile).Reload();
        }
        _modifiedTilesThisFrame.Clear();
    }

    private void TriggerGameOverIfNeeded()
    {
        if(_tiles.Exists(tile => tile.Type == TileType.HOLE && tile.CurrentElementPresent == TileElement.PUSHY))
        {
            GameOver();
        }
    }

    private void TriggerSuccessIfNeeded()
    {
        if (_tiles.Exists(tile => tile.Type == TileType.DOOR && tile.CurrentElementPresent == TileElement.PUSHY)
            && _tiles.Exists(tile => tile.Type == TileType.DOOR && tile.CurrentElementPresent == TileElement.FLOTTY))
        {
            LevelSuccess();
        }
    }

    private void GameOver()
    {
        RestartLastLevel();
    }

    private void LevelSuccess()
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
