using System;
using System.IO;

namespace lab2
{
    
    internal class Program
    {
        const int MAX_N = 20;
        const byte MAX_WEIGHT = 100;
        const byte INF = byte.MaxValue;

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
            int choiceFile;

            while(true)
            {
                Console.Clear();

                Console.WriteLine("В каталоге приложения обнаружены текстовые файлы:");
                for (int i = 0; i < filesInfo.Length;  i++)
                {
                    Console.WriteLine("{0}: {1}", i+1, filesInfo[i].Name);
                }
                Console.Write($"Номер файла с описанием графа (1 .. {filesInfo.Length}): ");

                if (int.TryParse(Console.ReadLine(), out choiceFile) && choiceFile >= 1 && choiceFile <= filesInfo.Length) break;
                else Console.Write("\nНекорректный ввод, нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
            choiceFile--; //Для нас индексация с 0



            //Чтение выбранного файла
            FileInfo mainFile = filesInfo[choiceFile];

            StreamReader streamReader = new StreamReader(mainFile.Name);
            int N; //Количество вершин графа
            byte[,] weightMatrix; //Весовая матрица

            try
            {
                N = int.Parse(streamReader.ReadLine() ?? "0");
                if (N < 1 || N > MAX_N)
                {
                    Error("Недопустимое число вершин!");
                    return;
                }
                weightMatrix = new byte[N,N];

                string[] currentStr;
                for (int i = 0; i < N; i++)
                {
                    currentStr = streamReader.ReadLine().Split(" ");
                    if (currentStr.Length != N)
                    {
                        Error("Некорректный формат данных в файле! Число вершин не соответствует действительности.");
                        return;
                    }

                    for (int j = 0; j < N; j++)
                    {
                        //Недопустимое значение веса ребра
                        if (short.Parse(currentStr[j]) < 0 )
                        {
                            Error("Некорректное значение веса ребра!");
                            return;
                        }

                        byte weight = byte.Parse(currentStr[j]);
                        if (weight > 0 && weight <= MAX_WEIGHT) //Допустимый вес
                        {
                            weightMatrix[i, j] = weight;
                        }
                        else //Отсутствие ребра -> бесконечное расстояние/(A->A, B->B, ... вес == 0)
                        {
                            if (i == j)
                                weightMatrix[i, j] = 0;
                            else
                                weightMatrix[i, j] = INF;
                        }
                    }
                }
                //Что-то осталось в файле?
                if (streamReader.ReadToEnd() != string.Empty)
                {
                    Error("Некорректная запись матрицы смежности!");
                    return;
                }

                streamReader.Close();
            }
            catch (FormatException)
            {
                Error("Парсинг не удался! Некорректный формат данных в файле.");
                return;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("В текущей дериктории не существует файла с таким именем");
                return;
            }
            catch
            {
                Console.WriteLine("Неизвестная ошибка");
                return;
            }

            ////Проверка результатов чтения
            //for (int i = 0; i < N; i++)
            //{
            //    for (int j = 0;j < N; j++)
            //    {
            //        Console.Write(weightMatrix[i,j] + "\t");
            //    }
            //    Console.WriteLine();
            //}



            //Работа с файлом...
            Console.Clear();
            Console.WriteLine($"Операции над графом \"{mainFile.Name}\":");

        }
        static private void Error(string message)
        {
            Console.WriteLine($"Ошибка: {message}");
            Console.Write("Нажмите любую клавишу для продолжения... ");
            Console.ReadKey();
        }
    }
}
