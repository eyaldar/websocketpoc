using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using CsvHelper;

namespace WebSocketsPOC 
{
    public class CsvHelperCsvExporter : ICsvExporter
    {
        public void Export(string savePath, IList<string> columnNames, TimeSpan[][] data)
        {
            var records = new List<dynamic>();
            for(int i = 0; i < data[0].Length; i++)
            {
                dynamic obj = new ExpandoObject();
                var dObj = obj as IDictionary<string, object>;
                for(int j = 0; j < columnNames.Count; j++)
                {
                    dObj[columnNames[j]] = data[j][i];
                }

                records.Add(obj); 
            }


            using (var txtWriter = new StreamWriter(savePath))
            {
                var csv = new CsvWriter(txtWriter);
                csv.WriteRecords(records);
            }
        }
    }
}