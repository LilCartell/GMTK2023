using UnityEngine;

public class TileUI : MonoBehaviour
{
    public GameObject Pushy;
    public GameObject Flotty;
    public GameObject Floor;
    public GameObject Box;
    public GameObject Door;
    public GameObject Wall;

    private Tile _tile;

    public void LoadWithTile(Tile tile)
    {
        _tile = tile;
        Reload();
    }

    public void Reload()
    {
        Pushy.SetActive(false);
        Flotty.SetActive(false);
        Floor.SetActive(false);
        Box.SetActive(false);
        Door.SetActive(false);
        Wall.SetActive(false);

        switch(_tile.Type)
        {
            case TileType.DOOR:
                Floor.SetActive(true);
                Door.SetActive(true);
                break;

            case TileType.FLOOR:
                Floor.SetActive(true);
                break;

            case TileType.HOLE: //Background is already a hole
                break;

            case TileType.WALL:
                Floor.SetActive(true);
                Wall.SetActive(true);
                break;
        }

        switch(_tile.CurrentElementPresent)
        {
            case TileElement.BOX:
                Box.SetActive(true);
                break;

            case TileElement.FLOTTY:
                Flotty.SetActive(true);
                break;

            case TileElement.PUSHY:
                Pushy.SetActive(true);
                break;
        }
    }
}