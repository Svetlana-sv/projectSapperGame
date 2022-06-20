using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SapperGame.Interface
{
    public interface IFileService
    {
        void CreateFile(string text);
        void WriteFile(string text);
        ObservableCollection<string> ReadFile();
    }
}
