namespace Lab10;

public interface IFileLifeController
{
    void CreateFile();
    void DeleteFile();
    void EditFile(string smth);
    void ChangeFileExtension(string smth);
}