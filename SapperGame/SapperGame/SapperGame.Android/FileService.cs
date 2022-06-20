
using SapperGame.Droid;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SapperGame.Interface;
using Android.App;
using Android.Content.Res;
using System.Collections.ObjectModel;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace SapperGame.Droid
{
    public class FileService : IFileService
    {
        public string GetRootPath()
        {
            return Application.Context.GetExternalFilesDir(null).ToString();
        }

        public void CreateFile(string text)
        {
            var fileName = "Rating.txt";

            var destination = Path.Combine(GetRootPath(), fileName);

            
           

            if (File.Exists(destination))
            {
                WriteFile(text);
            }
            else
            {
                File.WriteAllText(destination, text + "\n", Encoding.UTF8);
                //File.WriteAllText(destination, fileName);
            }
        }

       
        public void WriteFile(string text)
        {

            var fileName = "Rating.txt";

            var destination = Path.Combine(GetRootPath(), fileName);

            //using (StreamReader sr = new StreamReader(destination))
            //{
            //    content = sr.ReadToEnd();
            //}
            using (StreamWriter sw = File.AppendText(destination))
            {
                sw.WriteLine(text + "\n");
           
            }
            

        }
        public ObservableCollection<string> ReadFile()
        {

            ObservableCollection<string> content = new ObservableCollection<string>();
            var fileName = "Rating.txt";

            var destination = Path.Combine(GetRootPath(), fileName);


            if (File.Exists(destination))
            {
                using (StreamReader sr = new StreamReader(destination))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line != "")
                        {
                            content.Add(line);
                        }

                    }

                }
            }
            else
            {
                content.Add("Нет данных");
            }
            

            return content;

        }




    }
}