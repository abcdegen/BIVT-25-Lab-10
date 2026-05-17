namespace Lab10.Purple;

public abstract class PurpleFileManager<T> : MyFileManager, ISerializer<T> where T : Lab9.Purple.Purple
{
    public PurpleFileManager(string name) : base(name)
    { }

    public PurpleFileManager(string name, string folderName, string fileName, string fileExtension = "txt") : base(name, folderName, fileName, fileExtension)
    { }

    public override void EditFile(string content)
    {
        if (string.IsNullOrWhiteSpace(FullPath))
            return;

        base.EditFile(content);
    }

    public override void ChangeFileExtension(string newExtension)
    {
        if (string.IsNullOrWhiteSpace(FullPath) || string.IsNullOrWhiteSpace(newExtension))
            return;

        base.ChangeFileExtension(newExtension);
    }

    public abstract void Serialize(T obj);

    public abstract T Deserialize();
}