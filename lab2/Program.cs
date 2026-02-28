using System;
using System.IO;

namespace lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] filesInfo = dirInfo.GetFiles("*.txt");

            //Проверки
            if (filesInfo.Length == 0)
            {
                Console.WriteLine("В каталоге приложения не обнаружено текстовых файлов!");
                return;
            }


            while(true)
            {
                Console.WriteLine("В каталоге приложения обнаружены текстовые файлы:");
                for (int i = 0; i < filesInfo.Length;  i++)
                {
                    Console.WriteLine(i+1, filesInfo[i].Name);
                }

            }
            
        }
    }
}
