using UnityEngine;
using System.Collections.Generic;

public class SongPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the audio source component
    public float startDelay = 1f; // Delay before starting the song (adjust as needed)
    public CubeSpawner cubeSpawner; // Reference to the CubeSpawner script
    private List<Note> notes; // List to store the notes obtained from the BeatmapReader script
    private float currentTimestamp = 0f;
    private int noteIndex = 0;

    private void Start()
    {
        // Obtain the notes from the BeatmapReader script
        BeatmapReader beatmapReader = GetComponent<BeatmapReader>();
        notes = new List<Note>(beatmapReader.ExtractNotes());

        // Start playing the song after the start delay
        Invoke("StartSong", startDelay);
    }

    private void StartSong()
    {
        audioSource.Play();
    }

    private void Update()
    {
        currentTimestamp = audioSource.time;

        // Spawn cubes for notes that should appear at the current timestamp
        while (noteIndex < notes.Count && notes[noteIndex].time <= currentTimestamp)
        {
            Note note = notes[noteIndex];
            cubeSpawner.SpawnCubes(notes, currentTimestamp); // Pass the list of notes and the current timestamp
            noteIndex++;
        }
    }
}
