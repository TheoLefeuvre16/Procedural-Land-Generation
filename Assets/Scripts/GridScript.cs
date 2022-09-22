using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class GridScript : MonoBehaviour
{
    public GameObject boxCollider;
    public GameObject[] treePrefabs;
    public Material terrainMaterial;
    public Material edgeMaterial;
    public float waterLevel = .4f;
    public float scale = .1f;
    public float treeNoiseScale = .05f;
    public float treeDensity = .5f;
    public float riverNoiseScale = .06f;
    public int rivers = 5;
    public int size = 100;

    Cell[,] grid;
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GameObject[] env = GameObject.FindGameObjectsWithTag("Environment");
            foreach (GameObject e in env)
                GameObject.Destroy(e);
            GenerateAll();

        }
    }
    void GenerateAll()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                bool isWater = noiseValue < waterLevel;
                Cell cell = new Cell(isWater);
                grid[x, y] = cell;
            }
        }

        GenerateRivers(grid);
        DrawTexture(grid);
        GenerateTrees(grid);
    }

    void Start()
    {
        GenerateAll();
    }
    
    void GenerateRivers(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * riverNoiseScale + xOffset, y * riverNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        GridGraph gg = AstarData.active.graphs[0] as GridGraph;
        gg.center = new Vector3(size / 2f - .5f, 0, size / 2f - .5f);
        gg.SetDimensions(size, size, 1);
        AstarData.active.Scan(gg);
        AstarData.active.AddWorkItem(new AstarWorkItem(ctx => {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    GraphNode node = gg.GetNode(x, y);
                    node.Walkable = noiseMap[x, y] > .4f;
                }
            }
        }));
        AstarData.active.FlushGraphUpdates();

        int k = 0;
        for (int i = 0; i < rivers; i++)
        {
            GraphNode start = gg.nodes[Random.Range(16, size - 16)];
            GraphNode end = gg.nodes[Random.Range(size * (size - 1) + 16, size * size - 16)];
            ABPath path = ABPath.Construct((Vector3)start.position, (Vector3)end.position, (Path result) => {
                for (int j = 0; j < result.path.Count; j++)
                {
                    GraphNode node = result.path[j];
                    int x = Mathf.RoundToInt(((Vector3)node.position).x);
                    int y = Mathf.RoundToInt(((Vector3)node.position).z);
                    grid[x, y].isWater = true;
                }
                k++;
            });
            AstarPath.StartPath(path);
            AstarPath.BlockUntilCalculated(path);
        }
    }


    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (cell.isWater)
                    colorMap[y * size + x] = Color.blue;
                else
                {
                    colorMap[y * size + x] = new Color(153 / 255f, 191 / 255f, 115 / 255f);
                    GameObject newCollider = Instantiate(boxCollider, transform);
                    newCollider.transform.position = new Vector3(x, -0.5f, y);
                    newCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
                    newCollider.transform.localScale = Vector3.one;

                }
            }
        }
    }

    void GenerateTrees(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                if (!cell.isWater)
                {
                    float v = Random.Range(0f, treeDensity);
                    if (noiseMap[x, y] < v)
                    {
                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);
                        tree.transform.position = new Vector3(x, 0, y);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                    }
                }
            }
        }
    }

}
