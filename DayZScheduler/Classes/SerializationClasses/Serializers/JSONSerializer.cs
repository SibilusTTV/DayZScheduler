﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Text.Json;

namespace DayZScheduler.Classes.SerializationClasses.Serializers
{
    public class JSONSerializer
    {
        public static void SerializeJSONFile<JSONFile>(string path, JSONFile jsonfile)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    string json = JsonSerializer.Serialize(jsonfile, options);
                    writer.Write(json);
                }
            }
            catch (Exception ex)
            {
                Manager.WriteToConsole(ex.ToString());
            }
        }

        // Takes a path and returns the deserialized class
        public static JSONFile DeserializeJSONFile<JSONFile>(string path)
        {
            try
            {
                if (FileSystem.FileExists(path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string json = reader.ReadToEnd();
                        return JsonSerializer.Deserialize<JSONFile>(json);
                    }
                }
                else
                {
                    return default(JSONFile);
                }
            }
            catch (Exception ex)
            {
                Manager.WriteToConsole(ex.ToString());
                return default(JSONFile);
            }
        }
    }
}
