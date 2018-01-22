using System;
using System.Collections.Generic;

namespace WebSocketsPOC
{
    public interface ICsvExporter    
    {
        void Export(string filePath, IList<string> columnNames, TimeSpan[][] data);
    }
}