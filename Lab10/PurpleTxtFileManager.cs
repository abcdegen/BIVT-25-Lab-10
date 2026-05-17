using System.Text.Json;

namespace Lab10.Purple;

public class PurpleTxtFileManager<T> : PurpleFileManager<T> where T : Lab9.Purple.Purple
{
    public PurpleTxtFileManager(string name) : base(name)
    { }

    public PurpleTxtFileManager(string name, string folderName, string fileName, string fileExtension = "txt") : base(name, folderName, fileName, fileExtension)
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
        base.ChangeFileExtension("txt");
    }

    public override void Serialize(T obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(FullPath)) return;

        var options = new JsonSerializerOptions { IncludeFields = true };

        string jsonData = JsonSerializer.Serialize(obj, obj.GetType(), options);
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonData, options) ?? new Dictionary<string, JsonElement>();

        var txtLines = new List<string>
        {
            $"Type:{obj.GetType().Name}",
            $"Input:{obj.Input}"
        };

        foreach (var kvp in dict)
        {
            if (kvp.Key == "Type" || kvp.Key == "Input" || kvp.Key == "Codes" || kvp.Key == "_codes") 
                continue;

            txtLines.Add($"{kvp.Key}:{kvp.Value.GetRawText()}");
        }

        if (obj is Lab9.Purple.Task4 t4 && t4.Codes != null)
        {
            string codesJson = JsonSerializer.Serialize(t4.Codes, options);
            txtLines.Add($"_codes:{codesJson}");
        }

        if (!string.IsNullOrWhiteSpace(FolderPath) && !Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }

        File.WriteAllLines(FullPath, txtLines);
    }

    public override T Deserialize()
    {
        if (!File.Exists(FullPath)) return null;

        string[] lines = File.ReadAllLines(FullPath);
        var fieldsDict = new Dictionary<string, string>();
        string typeName = "";
        string inputValue = "";

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            int sep = line.IndexOf(':');
            if (sep == -1) continue;

            string key = line.Substring(0, sep).Trim();
            string val = line.Substring(sep + 1).Trim();

            if (key == "Type") typeName = val;
            else if (key == "Input") inputValue = val;
            else fieldsDict[key] = val; 
        }

        var codesList = new List<(string, char)>();
        string codesKey = fieldsDict.ContainsKey("_codes") ? "_codes" : "Codes";

        if (fieldsDict.ContainsKey(codesKey))
        {
            using JsonDocument doc = JsonDocument.Parse(fieldsDict[codesKey]);
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                string pair = element.TryGetProperty("pair", out var p) ? p.GetString() : element.GetProperty("Item1").GetString();

                JsonElement cEl = element.TryGetProperty("code", out var c) ? c : element.GetProperty("Item2");

                char code = cEl.ValueKind == JsonValueKind.Number ? (char)cEl.GetInt32() : cEl.GetString()[0];

                codesList.Add((pair, code));
            }
        }

        Lab9.Purple.Purple task = typeName switch
        {
            "Task2" => new Lab9.Purple.Task2(inputValue),
            "Task3" => new Lab9.Purple.Task3(inputValue),
            "Task4" => new Lab9.Purple.Task4(inputValue, codesList.ToArray()),
            _ => new Lab9.Purple.Task1(inputValue) 
        };

        task.Review();
        
        return task as T;
    }
}