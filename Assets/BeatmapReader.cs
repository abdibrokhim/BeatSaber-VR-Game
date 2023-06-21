using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatmapReader : MonoBehaviour
{
    public TextAsset beatmapFile; // The Beat Saber map file

    public Note[] ExtractNotes()
    {
        List<Note> notes = new List<Note>();
 
        string[] lines = beatmapFile.text.Split('\n');

        foreach (string line in lines)
        {
            if (line.StartsWith("NOTE"))
            {
                string[] noteData = line.Split(',');

                // Extract note properties
                float time = float.Parse(noteData[1]);
                int lineIndex = int.Parse(noteData[2]);
                int lineLayer = int.Parse(noteData[3]);
                NoteType type = (NoteType)int.Parse(noteData[4]);
                NoteCutDirection cutDirection = (NoteCutDirection)int.Parse(noteData[5]);

                // Create a new Note object and add it to the list
                Note note = new Note(time, lineIndex, lineLayer, type, cutDirection);
                notes.Add(note);
            }
        }

        return notes.ToArray();
    }
}

public enum NoteType
{
    LeftNote,
    RightNote
}

public enum NoteCutDirection
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    Any
}

public class Note
{
    public float time;
    public int lineIndex;
    public int lineLayer;
    public NoteType type;
    public NoteCutDirection cutDirection;

    public Note(float time, int lineIndex, int lineLayer, NoteType type, NoteCutDirection cutDirection)
    {
        this.time = time;
        this.lineIndex = lineIndex;
        this.lineLayer = lineLayer;
        this.type = type;
        this.cutDirection = cutDirection;
    }
}
