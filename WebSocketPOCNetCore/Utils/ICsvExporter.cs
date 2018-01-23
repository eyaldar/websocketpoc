using System;
using System.Collections.Generic;

namespace WebSocketsPOC
{
    public interface ICsvExporter
    {
        void Export<TDataType>(string filePath, string[] columnNames, TDataType[][] data);
    }
}