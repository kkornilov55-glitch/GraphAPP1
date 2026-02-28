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

            //Проверка
            if (filesInfo.Length == 0)
            {
                Console.WriteLine("В каталоге приложения не обнаружено текстовых файлов!");
                return;
            }

            //Выбор файла
            while(true)
            {
                Console.Clear();

                Console.WriteLine("В каталоге приложения обнаружены текстовые файлы:");
                for (int i = 0; i < filesInfo.Length;  i++)
                {
                    Console.WriteLine("{0}: {1}", i+1, filesInfo[i].Name);
                }
                Console.Write($"Номер файла с описанием графа (1 .. {filesInfo.Length}): ");

                if (int.TryParse(Console.ReadLine(), out int choiceFile) && choiceFile >= 1 && choiceFile <= filesInfo.Length) break;
                else Console.Write("\nНекорректный ввод, нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            
            //Работа с  выбранным файлом ...
        }
    }
}
