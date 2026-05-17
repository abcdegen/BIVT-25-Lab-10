using System.Text.Json;         

namespace Lab10.Purple;

public class PurpleJsonFileManager<T> : PurpleFileManager<T> where T : Lab9.Purple.Purple
{
    public PurpleJsonFileManager(string name) : base(name)
    { }

    public PurpleJsonFileManager(string name, string folderName, string fileName, string fileExtension = "json") : base(name, folderName, fileName, fileExtension)
    { }

    public override void EditFile(string content)
    {
        T obj = Deserialize();
        if (obj != null)
        {
            obj.ChangeText(content);
            Serialize(obj);
        }
    }

    public override void ChangeFileExtension(string newExtension)
    {
        base.ChangeFileExtension("json");
    }

    public override void Serialize(T obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(FullPath)) return;

        var options = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };

        string jsonData = JsonSerializer.Serialize(obj, obj.GetType(), options);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData, options) ?? new Dictionary<string, object>();
        
        if (obj is Lab9.Purple.Task4 t4 && t4.Codes != null)
        {
            dict["_codes"] = t4.Codes;
        }

        dict["ObjectType"] = obj.GetType().Name;
        dict["Input"] = obj.Input;

        if (!string.IsNullOrWhiteSpace(FolderPath) && !Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
        File.WriteAllText(FullPath, JsonSerializer.Serialize(dict, options));
    }

    public override T Deserialize()
    {
        if (!File.Exists(FullPath)) return null;

        string jsonText = File.ReadAllText(FullPath);

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonText);

        if (dict == null || !dict.ContainsKey("ObjectType") || !dict.ContainsKey("Input")) return null;

        string typeName = dict["ObjectType"].GetString();
        string inputValue = dict["Input"].GetString();

        var codes = new List<(string, char)>();
        string codesKey = dict.ContainsKey("_codes") ? "_codes" : "Codes";

        if (dict.ContainsKey(codesKey))
        {
            foreach (var element in dict[codesKey].EnumerateArray())
            {
                string pair = "";
                if (element.TryGetProperty("pair", out var p)) pair = p.GetString();
                else if (element.TryGetProperty("Item1", out var i1)) pair = i1.GetString();

                char code = ' ';
                if (element.TryGetProperty("code", out var cNode))
                {
                    code = cNode.ValueKind == JsonValueKind.Number ? (char)cNode.GetInt32() : cNode.GetString()[0];
                }
                else if (element.TryGetProperty("Item2", out var i2Node))
                {
                    code = i2Node.ValueKind == JsonValueKind.Number ? (char)i2Node.GetInt32() : i2Node.GetString()[0];
                }

                codes.Add((pair, code));
            }
        }

        Lab9.Purple.Purple task = typeName switch
        {
            "Task2" => new Lab9.Purple.Task2(inputValue),
            "Task3" => new Lab9.Purple.Task3(inputValue),
            "Task4" => new Lab9.Purple.Task4(inputValue, codes.ToArray()),
            _ => new Lab9.Purple.Task1(inputValue) 
        };

        task.Review();

        return task as T;
    }
}