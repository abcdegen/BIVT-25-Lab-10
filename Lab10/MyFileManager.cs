namespace Lab10;

public abstract class MyFileManager : IFileManager, IFileLifeController
{
    public string Name { get; private set; }
    public string FolderPath { get; private set; }
    public string FileName { get; private set; }
    public string FileExtension { get; private set; }
    
    public string FullPath => Path.Combine(FolderPath, FileName) + "." + FileExtension;

    public MyFileManager(string name)
    {
        Name = name;
        FolderPath = string.Empty;
        FileName = string.Empty;
        FileExtension = "txt";
    }

    public MyFileManager(string name, string folderName, string fileName, string fileExtension = "txt")
    {
        Name = name;
        FolderPath = folderName;
        FileName = fileName;
        FileExtension = fileExtension;
    }
    
    public void SelectFolder(string smth)
    {
        FolderPath = smth;
    }

    public void ChangeFileName(string smth)
    {
        FileName = smth;
    }

    public void ChangeFileFormat(string smth)
    {
        FileExtension = smth;
        
        if (!string.IsNullOrWhiteSpace(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }

        File.Create(FullPath).Close();
    }
    
    public void CreateFile()
    {
        if (!string.IsNullOrWhiteSpace(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
        
        File.Create(FullPath).Close();
    }

    public void DeleteFile()
    {
        if (File.Exists(FullPath))
        {
            File.Delete(FullPath);
        }
    }

    public virtual void EditFile(string smth)
    {
        File.WriteAllText(FullPath, smth);
    }

    public virtual void ChangeFileExtension(string smth)
    {
        string oldPath = FullPath;
        FileExtension = smth; 

        if (File.Exists(oldPath))
        {
            File.Move(oldPath, FullPath);
        }
    }
}