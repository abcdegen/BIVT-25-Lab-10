namespace Lab10.Purple;
public class Purple<T> where T : Lab9.Purple.Purple
{
    public PurpleFileManager<T> Manager { get; private set; }
    public T[] Tasks { get; private set; }

    private T[] CopyArray(T[] source)
    {
        if (source == null) 
            return Array.Empty<T>();

        return source.ToArray();
    }

    public Purple(T[] tasks)
    {
        Tasks = CopyArray(tasks);
        Manager = null;
    }
    
    public Purple()
    {
        Tasks = Array.Empty<T>();
        Manager = null;
    }

    public Purple(PurpleFileManager<T> manager, T[] tasks = null)
    {
        Manager = manager;
        Tasks = CopyArray(tasks);
    }

    public Purple(T[] tasks, PurpleFileManager<T> manager)
    {
        Tasks = CopyArray(tasks);
        Manager = manager;
    }

    public void Add(T task)
    {
        if (task == null) 
            return;

        T[] newTasks = new T[Tasks.Length + 1];
        Array.Copy(Tasks, newTasks, Tasks.Length);
        newTasks[newTasks.Length - 1] = task;
        
        Tasks = newTasks;
    }

    public void Add(T[] tasks)
    {
        if (tasks == null) 
            return;

        foreach (var task in tasks)
        {
            Add(task); 
        }
    }

    public void Remove(T task)
    {
        if (task == null || Tasks.Length == 0) return;

        int index = Array.IndexOf(Tasks, task);
        if (index < 0) 
            return;

        Tasks = Tasks.Where((val, i) => i != index).ToArray();
    }

    public void Clear()
    {
        Tasks = new T[0];

        if (Manager != null && !string.IsNullOrWhiteSpace(Manager.FolderPath))
        {
            if (Directory.Exists(Manager.FolderPath))
            {
                Directory.Delete(Manager.FolderPath, true); 
            }
        }
    }

    public void SaveTasks()
    {
        if (Manager == null) return;

        for (int i = 0; i < Tasks.Length; i++)
        {
            Manager.ChangeFileName($"task_{i}");
            Manager.Serialize(Tasks[i]);
        }
    }

    public void LoadTasks()
    {
        if (Manager == null) return;

        for (int i = 0; i < Tasks.Length; i++)
        {
            Manager.ChangeFileName($"task_{i}");
            Tasks[i] = Manager.Deserialize();
        }
    }

    public void ChangeManager(PurpleFileManager<T> newManager)
    {
        if (newManager == null) return;

        if (!string.IsNullOrWhiteSpace(newManager.Name))
        {
            if (!Directory.Exists(newManager.Name))
            {
                Directory.CreateDirectory(newManager.Name);
            }
            newManager.SelectFolder(newManager.Name);
        }

        Manager = newManager;
    }
}