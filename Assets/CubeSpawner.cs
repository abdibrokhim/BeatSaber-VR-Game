using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // The cube prefab to spawn
    public Transform spawnOriginRight; // Transform of the spawn origin for the right side
    public Transform spawnOriginLeft; // Transform of the spawn origin for the left side

    public void SpawnCubes(List<Note> notes, float currentTimestamp)
    {
        foreach (Note note in notes)
        {
            if (note.time <= currentTimestamp)
            {
                GameObject cube = Instantiate(cubePrefab, GetSpawnPosition(note), Quaternion.identity);
                // Configure the cube with note properties (lineIndex, lineLayer, type, cutDirection) if needed
            }
        }
    }

    private Vector3 GetSpawnPosition(Note note)
    {
        Transform spawnOrigin = note.lineIndex == 0 ? spawnOriginLeft : spawnOriginRight;
        Vector3 position = spawnOrigin.position;
        position.y += note.lineLayer;
        return position;
    }
}
