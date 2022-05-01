using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter
{
    string filepath = "";
    public string filename = "test.csv";
    public string file = "";

    struct Data
    {
        public TimePair genTime;
        public TimePair meshTime;
    }

    public CSVWriter(string name)
    {
        filename = name;
        filepath = Application.dataPath + "/" + name;
    }

    public void WriteLine(string line, bool encoding = true)
    {
        TextWriter tw = new StreamWriter(filepath, encoding);
        tw.WriteLine(line);
        tw.Close();
    }
}
