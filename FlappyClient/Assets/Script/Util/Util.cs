

using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Util
{
    private static string recordPath = Application.persistentDataPath;
    
    public static void SaveRecord(Recorder record)
    {
        var path = recordPath + "/record" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss");
        string data = JsonConvert.SerializeObject(record, Formatting.Indented);
        
        File.WriteAllText(path, data);
        if (PlayerPrefs.HasKey("record"))
        {
            path = "," + path;
        }
        PlayerPrefs.SetString("record", PlayerPrefs.GetString("record", "") + path);
    }

    public static void SaveRecord()
    {
        var path = recordPath + "/record" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss");
        string data = JsonConvert.SerializeObject(RecorderManager.Instance.GetListRecord(), Formatting.Indented);
        
        File.WriteAllText(path, data);
        if (PlayerPrefs.HasKey("record"))
        {
            path = "," + path;
        }
        PlayerPrefs.SetString("record", PlayerPrefs.GetString("record", "") + path);
    }

    
    public static Recorder LoadRecord(string path)
    {
        return JsonConvert.DeserializeObject<Recorder>(File.ReadAllText(path));
    }
}