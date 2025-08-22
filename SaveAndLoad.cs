using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class SaveAndLoad <T>
    {
        public static void SaveData(List<T> list, string fileName)
        {
            string jsonSave = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, jsonSave);
        }

        public static List<T> LoadData (string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            string json = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }
    }
}
