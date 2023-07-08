using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int LinesCount;
    public int ColumnsCount;
    public List<TextAsset> LevelFiles;

    public List<TileDefinition> GetLevelTiles(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < LevelFiles.Count)
            return GetLevelTiles(LevelFiles[levelIndex]);
        return null;
    }

    private List<TileDefinition> GetLevelTiles(TextAsset levelFile)
    {
        var errorStrings = new List<string>();
        var warningStrings = new List<string>();
        var levelTiles = new List<TileDefinition>();
        var lines = levelFile.text.Split("\n").Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
        
        if(lines.Count != LinesCount)
        {
            errorStrings.Add("Expecting " + LinesCount.ToString() + " lines, got " + lines.Count.ToString());
        }

        for(int i = 0; i < lines.Count; ++i)
        {
            var trimmedLine = lines[i].Replace(" ", "");
            trimmedLine = trimmedLine.Replace("\r", "");
            if (trimmedLine.Length != ColumnsCount)
            {
                errorStrings.Add("At line " + (i + 1).ToString() + " : expecting " + ColumnsCount.ToString() + " symbols, got " + trimmedLine.Length);
            }
            for(int j = 0; j < trimmedLine.Length; ++j)
            {
                var currentTileElement = TileElement.NONE;
                var currentTileType = TileType.HOLE;

                switch(trimmedLine[j])
                {
                    case '-':
                        currentTileType = TileType.FLOOR;
                        break;

                    case 'O':
                        break;

                    case 'X':
                        currentTileType = TileType.DOOR;
                        break;

                    case '+':
                        currentTileType = TileType.DOOR;
                        currentTileElement = TileElement.BOX;
                        break;

                    case 'P':
                        currentTileType = TileType.FLOOR;
                        currentTileElement = TileElement.PUSHY;
                        break;

                    case 'F':
                        currentTileType = TileType.FLOOR;
                        currentTileElement = TileElement.FLOTTY;
                        break;

                    case 'V':
                        currentTileType = TileType.HOLE;
                        currentTileElement = TileElement.FLOTTY;
                        break;

                    case '|':
                        currentTileType = TileType.WALL;
                        break;

                    case 'B':
                        currentTileType = TileType.FLOOR;
                        currentTileElement = TileElement.BOX;
                        break;

                    default:
                        errorStrings.Add("Line " + (i + 1).ToString() + " symbol " + (j + 1).ToString() + " : unexpected symbol");
                        break;
                }

                levelTiles.Add(new TileDefinition(j, i, currentTileType, currentTileElement));
            }
        }

        int numberOfPushys = levelTiles.Count(tile => tile.StartingElement == TileElement.PUSHY);
        int numberOfFlottys = levelTiles.Count(tile => tile.StartingElement == TileElement.FLOTTY);

        int numberOfDoors = levelTiles.Count(tile => tile.Type == TileType.DOOR);
        if (numberOfDoors < 2)
            errorStrings.Add("Not enough doors, you only have " + numberOfDoors.ToString());
        else if (numberOfDoors > 2)
            warningStrings.Add("Might be too much doors, you have " + numberOfDoors.ToString() + ". It's possible but you might want to double-check that.");
        if (numberOfPushys != 1)
            errorStrings.Add("Expecting 1 Pushy, got " + numberOfPushys.ToString());
        if (numberOfFlottys != 1)
            errorStrings.Add("Expecting 1 Flotty, got " + numberOfFlottys.ToString());


        if (errorStrings.Count == 0)
            Debug.Log("Level " + levelFile.name + " loaded successfully");
        else
        {
            foreach(var error in errorStrings)
            {
                Debug.LogError("Error on level " + levelFile.name + " : " + error);
            }
        }

        foreach (var warning in warningStrings)
        {
            Debug.LogWarning("Warning on level " + levelFile.name + " : " + warning);
        }

        return levelTiles;
    }

    public void TestAllLevels()
    {
        foreach(var levelFile in LevelFiles)
        {
            var levelTiles = GetLevelTiles(levelFile);
        }
    }
}