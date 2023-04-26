using System;

[Serializable]
public class InputEntry
{
    public int Index;
    public string Title;
    public string ImageFilePath;
    public string RecordingFileName;
    public float RecordingClipLength;
    public string Notes;
    public bool Favorited;


    public InputEntry(int index, string title, string imageFilePath, string recordingFileName, float recordingClipLength, string notes, bool favorited)
    {
        Index = index;
        Title = title;
        ImageFilePath = imageFilePath;
        RecordingFileName = recordingFileName;
        RecordingClipLength = recordingClipLength;
        Notes = notes;
        Favorited = favorited;
    }

    //public InputEntry(int index, string title, string imageFilePath, string notes)
    //{
    //    Index = index;
    //    Title = title;
    //    ImageFilePath = imageFilePath;
    //    RecordingFilePath = "";
    //    Notes = notes;
    //}
}


