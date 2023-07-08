using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsScene : MonoBehaviour
{
    public static LevelsScene Instance { get { return _instance; } }
    private static LevelsScene _instance;

    public LevelManager LevelManager;
    public GameObject TilesGrid;

    public GameObject ArrowsControls;
    public GameObject ResetControls;
    public GameObject SwapControls;
    public int LevelToDisplaySwap = 4;
    public int LevelToHideTuto = 6;

    #region Animation timing
    public float PushyFallingTime = 0.7f;
    public float RockFallingTime = 0.5f;
    public float SwappingSmokeTime = 0.8f;
    
    #endregion

    #region Debug
    public GameObject DebugButton;
    #endregion

    private int _currentLevelIndex = -1;
    private List<TileDefinition> _lastLevelLoaded;
    private List<Tile> _tiles = new List<Tile>();
    private List<Tile> _modifiedTilesThisFrame = new List<Tile>();
    private int _animationsPlaying = 0;

    private void Awake()
    {
        _instance = this;
        if (SwapControls != null)
            SwapControls.SetActive(false);
        if (ArrowsControls != null)
            ArrowsControls.SetActive(true);
        if (ResetControls != null)
            ArrowsControls.SetActive(true);
    }

    private void Start()
    {
        SoundManager.Instance.PlayMusic(SoundManager.Instance.LevelsMusic);

        LoadNextLevel();

        if(!Application.isEditor)
        {
            DebugButton.SetActive(false);
        }
    }

    private void Update()
    {
        if (_animationsPlaying > 0)
            return;

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
            if(nextFlottyTile.CurrentElementPresent != TileElement.BOX && nextFlottyTile.CurrentElementPresent != TileElement.PUSHY
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
            if(nextPushyTile.Type != TileType.WALL && nextPushyTile.CurrentElementPresent != TileElement.FLOTTY)
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
                            SoundManager.Instance.PlaySound(SoundManager.Instance.RockPushSound);
                            nextBoxTile.SetNewElement(TileElement.BOX);
                            _modifiedTilesThisFrame.Add(nextBoxTile);
                        }
                        else
                        {
                            PlayRockFall(nextBoxTile);
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
        StartCoroutine(SwitchCharactersCoroutine());
    }


    private IEnumerator SwitchCharactersCoroutine()
    {
        ++_animationsPlaying;

        SoundManager.Instance.PlaySound(SoundManager.Instance.SwapSound);
        var flottyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.FLOTTY);
        var pushyTile = _tiles.First(tile => tile.CurrentElementPresent == TileElement.PUSHY);

        flottyTile.SetNewElement(TileElement.PUSHY);
        pushyTile.SetNewElement(TileElement.FLOTTY);

        var flottyTileUI = GetUITile(flottyTile);
        var pushyTileUI = GetUITile(pushyTile);

        flottyTileUI.Flotty.SetActive(false);
        flottyTileUI.Swapping.SetActive(true);
        pushyTileUI.Pushy.SetActive(false);
        pushyTileUI.Swapping.SetActive(true);

        yield return new WaitForSeconds(SwappingSmokeTime);

        --_animationsPlaying;
        flottyTileUI.Reload();
        pushyTileUI.Reload();
    }

    private void PlayPushyFall(Tile tile)
    {
        StartCoroutine(PlayPushyFallCoroutine(tile));
    }

    private IEnumerator PlayPushyFallCoroutine(Tile tile)
    {
        ++_animationsPlaying;
        SoundManager.Instance.PlaySound(SoundManager.Instance.PushyFallingSound);
        var pushyUITile = GetUITile(tile);
        pushyUITile.Pushy.SetActive(false);
        pushyUITile.PushyFalling.SetActive(true);
        yield return new WaitForSeconds(PushyFallingTime);
        pushyUITile.PushyFalling.SetActive(false);
        --_animationsPlaying;
        RestartLastLevel();
    }

    private void PlayRockFall(Tile tile)
    {
        StartCoroutine(PlayRockFallCoroutine(tile));
    }

    private IEnumerator PlayRockFallCoroutine(Tile tile)
    {
        ++_animationsPlaying;
        SoundManager.Instance.PlaySound(SoundManager.Instance.RockFallingSound);
        var rockFallUITile = GetUITile(tile);
        rockFallUITile.RockFalling.SetActive(true);
        yield return new WaitForSeconds(RockFallingTime);
        rockFallUITile.RockFalling.SetActive(false);
        --_animationsPlaying;
    }

    private void RefreshModifiedTiles()
    {
        foreach (var tile in _modifiedTilesThisFrame)
        {
            RefreshTile(tile);           
        }
        _modifiedTilesThisFrame.Clear();
    }

    private void RefreshTile(Tile tile)
    {
        GetUITile(tile).Reload();
    }

    private TileUI GetUITile(Tile tile)
    {
        var tileUIs = TilesGrid.GetComponentsInChildren<TileUI>();
        return tileUIs.First(tileUI => tileUI.LinkedTile == tile);
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
        PlayPushyFall(_tiles.First(tile => tile.CurrentElementPresent == TileElement.PUSHY));
    }

    private void LevelSuccess()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ReachedDoorSound);
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
            SceneManager.LoadScene("EndScene");
        }

        if (_currentLevelIndex + 1 == LevelToDisplaySwap && SwapControls != null)
            SwapControls.SetActive(true);

        if(_currentLevelIndex +1 == LevelToHideTuto)
        {
            if (SwapControls != null)
                SwapControls.SetActive(false);
            if (ArrowsControls != null)
                ArrowsControls.SetActive(false);
            if (ResetControls != null)
                ResetControls.SetActive(false);
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
