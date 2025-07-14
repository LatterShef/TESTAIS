using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;


namespace ParserINI
{
    public class IniParser
    {
        // Внутреннее хранилище: секция -> (имя -> значение)
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
        private string allowedCharacters = "0123456789abcdefghijklmnopqrstuvwxyz_";

        // Метод для загрузки файла
        public void Load(string filenameIn)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filenameIn))
                {
                    data.Clear();
                    string currentSection = null;
                    string line;
                    int lineNumber = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim().Split(';')[0];
                        // Игнорируем пустые строки и комментарии
                        if (line == "") continue;

                        // Проверка на секцию [SECTION]
                        string[] sectionStart = line.Split('[');
                        if (sectionStart.Length == 2 && sectionStart[0] == "" && sectionStart[1] != "")
                        {
                            string[] sectionEnd = sectionStart[1].Split(']');
                            if (sectionEnd.Length == 2 && sectionEnd[1] == "" && sectionEnd[0] != "")
                            {
                                //Проверка на правильность названия секции
                                if (!StringIsCorrect(sectionEnd[0])) continue;
                                currentSection = sectionEnd[0];
                                if (!data.ContainsKey(currentSection))
                                {
                                    data[currentSection] = new Dictionary<string, string>();
                                }
                                //Игнорируем повторяющиеся секции
                                else currentSection = null;
                                continue;
                            }
                        }
                        //Игнорируем параметры вне секций
                        if (currentSection == null) continue;

                        //Проверка на пару key=value
                        string[] param = line.Split('=');
                        if (param.Length == 2 && param[0] != "" && param[1] != "")
                        {
                            //Проверки на правильность их записи
                            string key = param[0];
                            if (!StringIsCorrect(key)) continue;
                            string value = param[1];
                            if (!StringIsCorrect(value.Replace(".", "").Replace("/", ""))) continue;
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
                Console.WriteLine($"Файл {filenameIn} не найден.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        //Получение значения параметра в виде строки
        public string GetString(string section, string key)
        {
            if (data.TryGetValue(section, out var sectionDict))
            {
                if (sectionDict.TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new Exception("Параметр отсутвует");
            }
            throw new Exception("Секция отсутствует");
        }


        //Проверка на допустимость строки
        private bool StringIsCorrect(string valueStr)
        {
            return valueStr.ToLower().Except(allowedCharacters).Count() == 0;
        }

        //Получение значения параметра в виде целого числа, если это возможно
        public int GetInt(string section, string key)
        {
            string valueStr = GetString(section, key);
            if (int.TryParse(valueStr, out int result))
                return result;
            else
                throw new Exception("Неверный тип параметра");
        }

        //Получение значения параметра в виде целого числа, если это возможно
        public double GetDouble(string section, string key)
        {
            string valueStr = GetString(section, key);
            if (double.TryParse(valueStr, out double result))
                return result;
            else
                throw new Exception("Неверный тип параметра");
        }

        //Запись полученных данных в файл
        public void WriteInFile(string filenameOut)
        {
            if (data.Count() == 0)
            {
                Console.WriteLine("Данные отстутствуют");
                return;
            }
            try
            {
                using (StreamWriter writer = new StreamWriter(filenameOut))
                {
                    foreach (var section in data)
                    {
                        writer.WriteLine($"[{section.Key}]");
                        foreach (var kvp in section.Value)
                        {
                            writer.WriteLine($"{kvp.Key}={kvp.Value}");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Ошибка при записи в файл");
            }
        }


        //Вывод всех данных
        public void PrintAll()
        {
            if (data.Count() == 0)
            {
                Console.WriteLine("Данные отстутствуют");
                return;
            }
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
}
