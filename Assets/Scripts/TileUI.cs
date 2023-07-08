using UnityEngine;

public class TileUI : MonoBehaviour
{
    public GameObject Pushy;
    public GameObject Flotty;
    public GameObject Floor;
    public GameObject[] FloorVariations;
    public GameObject Box;
    public GameObject Door;
    public GameObject Wall;
    public GameObject[] WallVariations;

    public Tile LinkedTile { get; private set; }

    public void LoadWithTile(Tile tile)
    {
        LinkedTile = tile;
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

        switch(LinkedTile.Type)
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

        switch(LinkedTile.CurrentElementPresent)
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

        for(int i = 0; i < WallVariations.Length; ++i)
        {
            WallVariations[i].SetActive(i == LinkedTile.Variation);
        }

        for (int i = 0; i < FloorVariations.Length; ++i)
        {
            FloorVariations[i].SetActive(i == LinkedTile.Variation);
        }
    }
}