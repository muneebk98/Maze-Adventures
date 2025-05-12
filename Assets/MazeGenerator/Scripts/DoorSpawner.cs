using UnityEngine;

[RequireComponent(typeof(MazeSpawner))]
public class DoorSpawner : MonoBehaviour
{
    [Tooltip("The door prefab to spawn")]
    public GameObject doorPrefab;

    [Tooltip("How high above the spawn cell to start the raycast")]
    public float raycastStartHeight = 50f;

    void Start()
    {
        // 1. Grab maze parameters
        var sp = GetComponent<MazeSpawner>();
        float gap = sp.AddGaps ? 0.2f : 0f;
        float cw = sp.CellWidth + gap;
        float ch = sp.CellHeight + gap;
        int rows = sp.Rows;
        int cols = sp.Columns;

        // 2. Compute X,Z of the far corner cell center
        float x = (cols - 1) * cw + cw / 2f;
        float z = (rows - 1) * ch + ch / 2f;

        // 3. Figure out door half-height from its prefab scale
        float doorHalf = doorPrefab.transform.localScale.y / 2f;

        // 4. Raycast down from high above to find the floor surface
        Vector3 origin = new Vector3(x, raycastStartHeight, z);
        float finalY;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastStartHeight * 2f))
        {
            // place door so its bottom just touches the floor
            finalY = hit.point.y + doorHalf;
        }
        else
        {
            // fallback: assume Floor prefab is 1 unit thick (cube), so its top sits at y=+0.5
            finalY = 0.5f + doorHalf;
        }

        // 5. Spawn the door, rotated so it faces back into the maze
        Vector3 spawnPos = new Vector3(x, finalY, z);
        Instantiate(
            doorPrefab,
            spawnPos,
            Quaternion.Euler(0f, 180f, 0f),
            transform
        );
    }
}
