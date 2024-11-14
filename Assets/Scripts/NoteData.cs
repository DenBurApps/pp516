using System;

[Serializable]
public class NoteData
{
    public string Note;
    public string Date;

    public NoteData(string note, string date)
    {
        Note = note;
        Date = date;
    }
}
