using System;
using System.IO;

namespace lab2
{
    
    internal class Program
    {
        const int MAX_N = 20;
        const byte MAX_WEIGHT = 100;
        const byte INF = byte.MaxValue;

        static int N; //Количество вершин графа
        static byte[,] weightMatrix; //Весовая матрица

        static void Main(string[] args)
        {
            while (true)
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

                while (true)
                {
                    Console.Clear();

                    Console.WriteLine("В каталоге приложения обнаружены текстовые файлы:");
                    for (int i = 0; i < filesInfo.Length; i++)
                    {
                        Console.WriteLine("{0}: {1}", i + 1, filesInfo[i].Name);
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

                try
                {
                    N = int.Parse(streamReader.ReadLine() ?? "0");
                    if (N < 1 || N > MAX_N)
                    {
                        Error("Недопустимое число вершин!");
                        return;
                    }
                    weightMatrix = new byte[N, N];

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
                            if (short.Parse(currentStr[j]) < 0)
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
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Операции над графом \"{mainFile.Name}\":");
                    Console.WriteLine("1. Вывод матрицы смежности.\n2. Вывод списка рёбер.\n3. Вывод списков смежности.\n4. Определение свойств графа.\n5. Выход из программы.");
                    Console.WriteLine("Введите номер действия (1 .. 5):");

                    if (!byte.TryParse(Console.ReadLine(), out byte choiceOpiration) || choiceOpiration > 5 || choiceOpiration < 1)
                    {
                        Error("Некорректный номер операции");
                        continue;
                    }

                    switch(choiceOpiration)
                    {
                        case 1:
                            PrintAdjacencyMatrix();
                            break;
                        case 2:
                            PrintListEdges();
                            break;
                        case 3:
                            PrintAdjacencyLists();
                            break;
                        case 4:
                            PrintGraphProperties();
                            break;
                        case 5:
                            Console.Write("Выход из программы.\nНажмите любую клавишу для продолжения... ");
                            Console.ReadKey();
                            return;
                        default:
                            Error("-");
                            return;
                    }
                }
            }

        }
        static void PrintAdjacencyMatrix()
        {
            Console.WriteLine("МАТРИЦА СМЕЖНОСТИ");

            char start = 'A';

            for (int i = 0; i < N; i++)
            {
                Console.Write("\t" +(char)(start + i));
            }
            Console.WriteLine();

            for (int i = 0; i < N; i++)
            {
                Console.Write((char)(start + i));

                for (int j = 0; j < N; j++)
                {
                    if (weightMatrix[i, j] == INF || weightMatrix[i, j] == 0)
                        Console.Write("\t" + '-');
                    else
                        Console.Write("\t" + weightMatrix[i,j]);
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
        static void PrintListEdges()
        {
            Console.WriteLine("СПИСОК РЁБЕР");
            char start = 'A';
            Console.WriteLine($"Вершины: {start}-{(char)(start + N-1)}");

            List <int[]> edges = new List <int[]>();

            //int[] mass = {{Первая вершина},{Вторая вершина},{Длинна ребра}}

            for (int i = 0; i < N;i++)
            {
                for (int j = 0;j < N;j++)
                {
                    if (weightMatrix[i,j] == INF || weightMatrix[i, j] == 0)
                        continue;

                    edges.Add(new int[] {start + i, start + j, weightMatrix[i,j]});
                }
            }

            foreach (int[] edge in edges)
            {
                Console.WriteLine($"{(char)edge[0]}->{(char)edge[1]} ({edge[2]})");
            }


            Console.ReadKey();
        }
        static void PrintAdjacencyLists()
        {
            Console.WriteLine("ВЫВОД СПИСКОВ СМЕЖНОСТИ");

            List<int[]>[] AdjacencyLists = new List<int[]> [N];

            for (int i = 0;i < N;i++)
            {
                AdjacencyLists[i] = new List<int[]>();

                for (int j = 0; j < N; j++)
                {
                    if (weightMatrix[i, j] == 0 || weightMatrix[i, j] == INF) continue;

                    AdjacencyLists[i].Add(new int[] { j, weightMatrix[i, j] });
                }
            }

            for (int i = 0; i < N;i++)
            {
                Console.Write(Convert.ToChar(i + 'A') + ": ");

                foreach (int[] edge in AdjacencyLists[i])
                {
                    Console.Write(Convert.ToChar(edge[0] + 'A') + $"({edge[1]}) ");
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
        static void PrintGraphProperties()
        {
            Console.WriteLine("СВОЙСТВА ГРАФА");

            //ПЕТЛИ
            bool loops = false;

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i == j)
                        if (weightMatrix[i, j] != 0)
                        {
                            if (!loops)
                            {
                                Console.Write("В графе есть петли: ");
                                loops = true;
                            }
                                
                            Console.Write("{0}({1}) ", Convert.ToChar(i + 'A'), weightMatrix[i, j]);
                        }
                }
            }

            if (loops) 
                Console.WriteLine();
            else
                Console.WriteLine("В графе нет петель.");


            //Ориентированность
            Console.WriteLine(IsDirectedGraph() ? "Граф ориентированный." : "Граф неориентированный.");

            //Взвешенность
            Console.WriteLine(IsWeightedGraph() ? "Граф взвешенный." : "Граф невзвешенный.");



            Console.ReadKey();
        }

        static bool IsDirectedGraph()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (weightMatrix[i, j] != weightMatrix[j, i]) return true;
                }
            }

            return false;
        }

        static bool IsWeightedGraph()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (weightMatrix[i, j] != 1 && weightMatrix[i, j] != 0 && weightMatrix[i, j] != INF) return true;
                }
            }

            return false;
        }

        static void Error(string message)
        {
            Console.WriteLine($"Ошибка: {message}!");
            Console.Write("Нажмите любую клавишу для продолжения... ");
            Console.ReadKey();
        }
    }
}
