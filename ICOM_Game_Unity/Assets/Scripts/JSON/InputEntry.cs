using System;

[Serializable]
public class InputEntry
{
    public int Index;
    public string Title;
    public string ImageFilePath;
    public string RecordingFilePath;
    public string Notes;


    //public InputEntry(int index, string title, string imageFilePath, string recordingFilePath, string notes)
    //{
    //    Index = index;
    //    Title = title;
    //    ImageFilePath = imageFilePath;
    //    RecordingFilePath = recordingFilePath;
    //    Notes = notes;
    //}

    public InputEntry(int index, string title, string imageFilePath, string notes)
    {
        Index = index;
        Title = title;
        ImageFilePath = imageFilePath;
        RecordingFilePath = "";
        Notes = notes;
    }
}


