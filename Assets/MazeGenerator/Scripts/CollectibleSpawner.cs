using UnityEngine;

[RequireComponent(typeof(MazeSpawner))]
public class CollectibleSpawner : MonoBehaviour
{
    public GameObject ballPrefab;    // drag BallPrefab here
    public int ballCount = 10;       // how many orbs you want
    public float heightOffset = 0.5f;// raise them above the floor

    private MazeSpawner maze;

    void Start()
    {
        maze = GetComponent<MazeSpawner>();
        SpawnOrbs();
    }

    void SpawnOrbs()
    {
        int rows = maze.Rows;
        int cols = maze.Columns;
        float cellW = maze.CellWidth;
        float cellH = maze.CellHeight;

        for (int i = 0; i < ballCount; i++)
        {
            int r = Random.Range(0, rows);
            int c = Random.Range(0, cols);
            Vector3 pos = new Vector3(
                r * cellW + cellW / 2f,
                heightOffset,
                c * cellH + cellH / 2f
            );
            Instantiate(ballPrefab, pos, Quaternion.identity);
        }
    }
}
