using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TileManager>();
            }
            return instance;
        }
    }
    private static TileManager instance;
    [SerializeField] private GameObject[] tilePrefab;
    [SerializeField] private GameObject currentTile;
    [SerializeField] private int preCreateTiles = 10;

    public Stack<GameObject> LeftTiles
    {
        get { return leftTiles; }
        set { leftTiles = value; }
    }
    private Stack<GameObject> leftTiles = new Stack<GameObject>();
    public Stack<GameObject> TopTiles
    {
        get { return topTiles; }
        set { topTiles = value; }
    }
    private Stack<GameObject> topTiles = new Stack<GameObject>();

    private void Start()
    {
        CreateTiles(preCreateTiles * 2);
        for (int i = 0; i < preCreateTiles; i++)
        {
            SpawnTile();
        }
    }

    public void CreateTiles(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            leftTiles.Push(Instantiate(tilePrefab[0], this.transform));
            topTiles.Push(Instantiate(tilePrefab[1], this.transform));
            leftTiles.Peek().name = "LeftTile";
            leftTiles.Peek().SetActive(false);
            topTiles.Peek().name = "TopTile";
            topTiles.Peek().SetActive(false);
        }
    }

    public void SpawnTile()
    {
        int randomIndex = Random.Range(0, 2);
        if (randomIndex == 0)
        {
            GameObject tmp = leftTiles.Pop();
            tmp.SetActive(true);
            tmp.transform.position = currentTile.transform.GetChild(0).transform.GetChild(randomIndex).position;
            currentTile = tmp;
        }
        else if (randomIndex == 1)
        {
            GameObject tmp = topTiles.Pop();
            tmp.SetActive(true);
            tmp.transform.position = currentTile.transform.GetChild(0).transform.GetChild(randomIndex).position;
            currentTile = tmp;
        }
        if (Random.Range(0, AppData.gemSpawnRate) == 0)
        {
            currentTile.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}