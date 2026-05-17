using System.Text.Json;          
using System.Text.Encodings.Web;  
using System.Xml.Serialization;   

namespace Lab10.Purple;

public class DTOPurple
{
    public string TypeName { get; set; } = string.Empty;
    public string Input { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}

public class PurpleXmlFileManager<T> : PurpleFileManager<T> where T : Lab9.Purple.Purple
{
    public PurpleXmlFileManager(string name) : base(name)
    { }

    public PurpleXmlFileManager(string name, string folderName, string fileName, string fileExtension = "xml") : base(name, folderName, fileName, fileExtension)
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
        T obj = Deserialize();
        ChangeFileFormat("xml");
        if (obj != null)
            Serialize(obj);
    }

    public override void Serialize(T obj)
    {
        if (obj == null || string.IsNullOrWhiteSpace(FullPath)) return;

        var options = new JsonSerializerOptions 
        { 
            IncludeFields = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        string jsonData = JsonSerializer.Serialize(obj, obj.GetType(), options);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonData, options) 
                   ?? new Dictionary<string, object>();

        if (obj is Lab9.Purple.Task4 t4 && t4.Codes != null)
            dict["_codes"] = t4.Codes;

        DTOPurple dto = new DTOPurple
        {
            TypeName = obj.GetType().Name,
            Input = obj.Input,
            JsonData = JsonSerializer.Serialize(dict, options) 
        };

        if (!string.IsNullOrWhiteSpace(FolderPath) && !Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DTOPurple));
        using (StreamWriter sw = new StreamWriter(FullPath))
            xmlSerializer.Serialize(sw, dto);
    }

    public override T Deserialize()
    {
        if (!File.Exists(FullPath)) 
            return null;

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(DTOPurple));
        DTOPurple dto;
        using (StreamReader sr = new StreamReader(FullPath))
        {
            dto = (DTOPurple)xmlSerializer.Deserialize(sr);
        }

        if (dto == null || string.IsNullOrWhiteSpace(dto.TypeName) || string.IsNullOrWhiteSpace(dto.JsonData))
            return null;

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(dto.JsonData);

        var codesList = new List<(string, char)>();
        string codesKey = dict != null && dict.ContainsKey("_codes") ? "_codes" : "Codes";

        if (dict != null && dict.ContainsKey(codesKey))
        {
            foreach (JsonElement element in dict[codesKey].EnumerateArray())
            {
                string pair = element.TryGetProperty("pair", out var p) ? p.GetString() : element.GetProperty("Item1").GetString();

                JsonElement cEl = element.TryGetProperty("code", out var c) ? c : element.GetProperty("Item2");

                char code = cEl.ValueKind == JsonValueKind.Number ? (char)cEl.GetInt32() : cEl.GetString()[0];

                codesList.Add((pair, code));
            }
        }

        Lab9.Purple.Purple task = dto.TypeName switch
        {
            "Task2" => new Lab9.Purple.Task2(dto.Input),
            "Task3" => new Lab9.Purple.Task3(dto.Input),
            "Task4" => new Lab9.Purple.Task4(dto.Input, codesList.ToArray()),
            _ => new Lab9.Purple.Task1(dto.Input)
        };

        task.Review();
        
        return task as T;
    }
}
