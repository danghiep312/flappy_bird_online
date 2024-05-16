
using System;
using System.Collections.Generic;

public class RecorderManager : Singleton<RecorderManager>
{
    public List<Recorder> list;

    private void Start()
    {
        list = new List<Recorder>();
    }

    public void AddRecord(Recorder rec)
    {
        list.Add(rec);
    }
    
    public void RemoveRecord(Recorder rec)
    {
        list.Remove(rec);
    }
    
    
    public List<Recorder> GetListRecord()
    {
        return list;
    }
}