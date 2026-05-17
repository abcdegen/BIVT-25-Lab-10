namespace Lab10;

public interface IFileManager
{
    string FolderPath { get;}
    string FileName { get;}
    string FileExtension { get;}
    string FullPath { get; }

    void SelectFolder(string smth);
    void ChangeFileName(string smth);
    void ChangeFileFormat(string smth);
}