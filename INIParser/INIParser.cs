using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class IniParser
{
    // Внутреннее хранилище: секция -> (имя -> значение)
    private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

    // Метод для загрузки файла
    public void Load(string filename)
    {
        try
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string currentSection = null;
                string line;
                int lineNumber = 0;

                bool firstNonEmptyLineIsSection = false; // флаг для проверки начала файла

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    line = line.Trim();

                    // Игнорируем пустые строки и комментарии
                    if (string.IsNullOrEmpty(line) || line.StartsWith(";") || line.StartsWith("#"))
                        continue;

                    // Проверка на секцию [SECTION]
                    var sectionMatch = Regex.Match(line, @"^$$(.+?)$$$");
                    if (sectionMatch.Success)
                    {
                        currentSection = sectionMatch.Groups[1].Value.Trim();
                        if (!data.ContainsKey(currentSection))
                        {
                            data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        }
                        // Первая непустая строка - секция
                        if (!firstNonEmptyLineIsSection)
                            firstNonEmptyLineIsSection = true;

                        continue;
                    }


                    // Проверка на пару key=value
                    var paramMatch = Regex.Match(line, @"^(\w+)\s*=\s*(.+)$");
                    if (paramMatch.Success && currentSection != null)
                    {
                        string key = paramMatch.Groups[1].Value.Trim();
                        string value = paramMatch.Groups[2].Value.Trim();

                        data[currentSection][key] = value;
                    }
                }

                if (data.Count == 0)
                {
                    Console.WriteLine("Ошибка: в файле отсутствуют секции или он пустой.");
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Файл {filename} не найден.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
        }
    }

    public string Get(string section, string key, string defaultValue = null)
    {
        if (data.TryGetValue(section, out var sectionDict))
        {
            if (sectionDict.TryGetValue(key, out var value))
            {
                return value;
            }
        }
        return defaultValue;
    }

    public int GetInt(string section, string key)
    {
        string valueStr = Get(section, key);
        if (valueStr == null)
            throw new Exception("Неверный тип параметра");
        if (int.TryParse(valueStr, out int result))
            return result;
        else
            throw new Exception("Неверный тип параметра");
    }

    public double GetDouble(string section, string key)
    {
        string valueStr = Get(section, key);
        if (valueStr == null)
            throw new Exception("Неверный тип параметра");
        if (double.TryParse(valueStr, out double result))
            return result;
        else
            throw new Exception("Неверный тип параметра");
    }

    public void PrintAll()
    {
        foreach (var section in data)
        {
            Console.WriteLine($"[{section.Key}]");
            foreach (var kvp in section.Value)
            {
                Console.WriteLine($"{kvp.Key}={kvp.Value}");
            }
            Console.WriteLine();
        }
    }
}
