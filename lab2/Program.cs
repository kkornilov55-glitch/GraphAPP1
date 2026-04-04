using System;
using System.IO;
using System.Collections.Generic;

namespace lab2
{
    
    internal class Program
    {
        const byte INF = byte.MaxValue;
        static int N; //Количество вершин графа
        static int[,] M; //Матрица смежности
        static int[] Parents; //Родители вершин

        static void Main(string[] args)
        {
            while (true)
            {
                DirectoryInfo Dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] Files = Dir.GetFiles("*.txt");

                //Проверка
                if (Files.Length == 0)
                {
                    Console.WriteLine("В каталоге приложения не обнаружено текстовых файлов!");
                    return;
                }

                //Выбор файла
                int choiceFile;
                try
                {
                    Console.Clear();

                    //Меню выбора файлов
                    Console.WriteLine("В каталоге приложения обнаружены текстовые файлы:");
                    for (int i = 0; i < Files.Length; i++)
                    {
                        Console.WriteLine("{0}: {1}", i + 1, Files[i].Name);
                    }
                    Console.Write($"Номер файла с описанием графа (1 .. {Files.Length}): ");


                    //Получение номера файла
                    choiceFile = int.Parse(Console.ReadLine());
                    if (choiceFile < 1 || choiceFile > Files.Length)
                    {
                        Error("Неверный индекс файла, файл с таким номером отсутствует");
                        return;
                    }
                }
                catch (FormatException)
                {
                    Error("Неверный индекс файла, числовое преобразование невозможно");
                    return;
                }
                choiceFile--; //Для нас индексация с 0


                //Попытка чтения выбранного файла
                FileInfo mainFile = Files[choiceFile];
                bool ReadOK = ReadGraph(mainFile.Name);
                if (!ReadOK) return;


                //Работа с файлом
                while (true)
                {
                    Console.Clear();
                    //Меню выбора файла
                    Console.WriteLine($"Операции над графом \"{mainFile.Name}\":");
                    Console.WriteLine
                        (
                        "1. Вывод матрицы смежности.\n" +
                        "2. Вывод списка рёбер.\n" +
                        "3. Вывод списков смежности.\n" +
                        "4. Определение свойств графа.\n" +
                        "5. Матрица кратчайших расстояний (алгоритм Флойда - Уоршелла).\n" +
                        "6. Кратчайшее расстояние от вершины до остальных вершин (алгоритм Дейкстры).\n" +
                        "7. Выход из программы."
                        );
                    Console.WriteLine("Введите номер действия (1 .. 7):");

                    if (!int.TryParse(Console.ReadLine(), out int Case) || Case > 7 || Case < 1)
                    {
                        Error("Некорректный номер операции");
                        continue;
                    }

                    Console.WriteLine();
                    switch(Case)
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
                            FloydWarshall();
                            break;
                        case 6:
                            Dijkstra();
                            break;
                        case 7:
                            Console.Write("ВЫХОД ИЗ ПРОГРАММЫ");
                            Console.ReadKey();
                            return;
                        default:
                            Error("-");
                            return;
                    }
                    Console.ReadKey();
                }
            }

        }
        static bool ReadGraph(string FileName)
        {
            const int N_MAX = 20;
            const int LENGTH_MAX = 100;

            StreamReader F = new StreamReader(FileName);

            try
            {
            //Считываем число вершин графа
            N = int.Parse(F.ReadLine() ?? "0");
            if (N < 1 || N > N_MAX)
            {
                Error("Недопустимое число вершин");
                return false;
            }
            M = new int[N, N];

            //Считываем матрицу
            string[] currentStringMass;
            string currentString;

                for (int i = 0; i < N; i++)
                {
                    currentString = F.ReadLine();

                    if (currentString == null)
                    {
                        Error("Некорректный формат данных в файле! Число вершин не соответствует действительности.");
                        return false;
                    }

                    currentStringMass = currentString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    if (currentStringMass.Length != N)
                    {
                        Error("Некорректный формат данных в файле! Число вершин не соответствует действительности.");
                        return false;
                    }

                    for (int j = 0; j < N; j++)
                    {
                        //Недопустимая длинна ребра
                        if (int.Parse(currentStringMass[j]) < 0 || int.Parse(currentStringMass[j]) > LENGTH_MAX)
                        {
                            Error("Некорректная длинна ребра");
                            return false;
                        }

                        //Допустимая длинна ребра
                        int length = int.Parse(currentStringMass[j]);
                        if (length > 0 && length <= LENGTH_MAX)
                        {
                            M[i, j] = length;
                        }
                        else //Отсутствие ребра -> бесконечное расстояние/(A->A, B->B, ... вес == 0)
                        {
                            if (i == j)
                                M[i, j] = 0;
                            else
                                M[i, j] = INF;
                        }
                    }
                }

                //Что-то осталось в файле?
                if (F.ReadToEnd() != string.Empty)
                {
                    Error("Некорректная запись матрицы смежности");
                    return false;
                }
    
                F.Close();
            }
            catch (FormatException)
            {
                Error("Числовое приведение невозможно");
                return false;
            }

            return true;
        }
        static void PrintAdjacencyMatrix()
        {
            Console.WriteLine("МАТРИЦА СМЕЖНОСТИ");
            PrintMatrix(M);
        }
        static void PrintListEdges()
        {
            Console.WriteLine("СПИСОК РЁБЕР");
            Console.WriteLine($"Вершины: A-{Convert.ToChar('A' + N-1)}");

            List <int[]> edges = new List <int[]>();

            //int[] mass = {{Первая вершина},{Вторая вершина},{Длинна ребра}}

            for (int i = 0; i < N;i++)
            {
                for (int j = 0; j < N;j++)
                {
                    if (M[i,j] == INF || M[i, j] == 0)
                        continue;

                    if (IsDirectedGraph())
                    {
                        edges.Add(new int[] { 'A' + i, 'A' + j, M[i, j] });
                    }
                    else //В неориентированном графе, ватрица весов симметрична, поэтому добавляем ребра выше и на главной диагонали
                    {
                        if (i <= j) edges.Add(new int[] { 'A' + i, 'A' + j, M[i, j] });
                    }
                }
            }


            string sep = IsDirectedGraph() ? "->" : "-";
            foreach (int[] edge in edges)
            {
                    if (IsWeightedGraph())
                        Console.WriteLine($"{Convert.ToChar(edge[0])}{sep}{Convert.ToChar(edge[1])} ({edge[2]})");
                    else
                        Console.WriteLine($"{Convert.ToChar(edge[0])}{sep}{Convert.ToChar(edge[1])}");
            }
        }
        static void PrintAdjacencyLists()
        {
            Console.WriteLine("ВЫВОД СПИСКОВ СМЕЖНОСТИ");

            //Массив, списков массивов(Вершина, длинна ребра) ребер исходящих из текущей вершины
            List<int[]>[] AdjacencyLists = new List<int[]> [N];

            //Для всех вершин
            for (int i = 0;i < N;i++)
            {
                AdjacencyLists[i] = new List<int[]>();

                //Смотрим к каким идут пути 
                for (int j = 0; j < N; j++)
                {
                    //Путь равен 0 или бесконечности, значит ребра нет, пропускаем
                    if (M[i, j] == 0 || M[i, j] == INF) continue;

                    //Добавляем путь в список текущей вершины
                    AdjacencyLists[i].Add(new int[] { j, M[i, j] });
                }
            }

            //Вывод списков смежности для каждой вершины
            for (int i = 0; i < N;i++)
            {
                Console.Write(Convert.ToChar(i + 'A') + ": ");

                //Все пути что есть
                foreach (int[] edge in AdjacencyLists[i])
                {
                    Console.Write(Convert.ToChar(edge[0] + 'A') + (IsWeightedGraph() ? $"({edge[1]}) " : " "));
                }
                Console.WriteLine();
            }
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
                    //Элемент на главной диагонали != 0 -> Есть петли
                    if (i == j)
                        if (M[i, j] != 0)
                        {
                            if (!loops) //Если это первая петля прописываем текст
                            {
                                Console.Write("В графе есть петли: ");
                                loops = true;
                            }
                            //Выводим петли и их длины
                            Console.Write("{0}({1}) ", Convert.ToChar(i + 'A'), M[i, j]);
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
        }

        static bool IsDirectedGraph()
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (M[i, j] != M[j, i]) return true;
                }
            }

            return false;
        }

        static bool IsWeightedGraph()
        {
            //Проходимся по всем элементам, если какой-либо != 0 или 1 или бесконечности, значит граф взвешенный
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (M[i, j] != 1 && M[i, j] != 0 && M[i, j] != INF) return true;
                }
            }

            return false;
        }

        static void FloydWarshall()
        {
            Console.WriteLine("МАТРИЦА КРАТЧАЙШИХ РАССТОЯНИЙ (АЛГОРИТМ ФЛОЙДА - УОРШЕЛЛА)");

            //Копируем матрицу игнорируя петли
            int[,] R = new int[N,N];
            for (int i = 0; i < N; ++i)
                for (int j = 0; j < N; ++j)
                    R[i, j] = i == j ? 0 : M[i, j];

            for (int k = 0; k < N; k++)
            {
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < N; j++) 
                        R[i, j] = Math.Min(R[i, j], R[i, k] + R[k, j]);
                }
            }

            PrintMatrix(R);
        }
        static void PrintMatrix(int[,] T)
        {
            //Столбцы
            for (int i = 0; i < N; i++)
            {
                Console.Write("\t" + Convert.ToChar('A' + i));
            }
            Console.WriteLine();

            //Строки
            for (int i = 0; i < N; i++)
            {
                Console.Write(Convert.ToChar('A' + i));

                for (int j = 0; j < N; j++)
                {
                    if (T[i, j] == INF || T[i, j] == 0)
                        Console.Write("\t" + '-');
                    else
                        Console.Write("\t" + T[i, j]);
                }
                Console.WriteLine();
            }
        }
        static void Dijkstra()
        {
            Parents = new int[N];
            for (int i = 0; i < N; i++) Parents[i] = -1;


            Console.WriteLine("КРАТЧАЙШЕЕ РАССТОЯНИЕ ОТ ВЕРШИНЫ ДО ОСТАЛЬНЫХ ВЕРШИН (АЛГОРИТМ ДЕЙКСТРЫ)");
            int S = GetVertex();
            if (S == -1) return;

            int[] Distance = new int[N];
            bool[] Visited = new bool[N];
            for (int i = 0; i < N; ++i)
            {
                Distance[i] = INF;
                Visited[i] = false;
            }
            Distance[S] = 0;

            int MinD;
            do
            {
                MinD = INF;
                int MinV = -1;
                for (int i = 0; i < N; ++i)
                    if (Distance[i] < MinD && !Visited[i])
                    {
                        MinD = Distance[i];
                        MinV = i;
                    }
                if (MinV == -1) break;
                for (int i = 0; i < N; ++i)
                    if (M[MinV, i] < INF && !Visited[i])
                    {
                        int newDist = Distance[MinV] + M[MinV, i];
                        if (newDist < Distance[i])
                        {
                            Distance[i] = newDist;
                            Parents[i] = MinV;
                        }
                    }
                Visited[MinV] = true;
            } while (MinD < INF);

            Console.WriteLine("Кратчайшие расстояния до вершин:");
            PrintByVertices(Distance);
            Console.WriteLine();
            PrintWays(S);
        }
        static int GetVertex()
        {
            char V = ' ', maxLetter = Convert.ToChar('A' + N - 1);

            Console.Write("Введите имя исходной вершины (A-{0:C})", maxLetter);
            
            try
            {
                V = char.ToUpper(char.Parse(Console.ReadLine()));
                if (V > maxLetter || V < 'A') throw new InvalidDataException();
            }
            catch
            {
                Error("Неверно указана вершина");
                return -1;
            }

            return V - 'A';
        }
        static void PrintByVertices(int[] D)
        {
            // Вывод буквенных обозначений вершин (заголовок)
            for (int i = 0; i < N; ++i)
                Console.Write("\t" + Convert.ToChar('A' + i));
            Console.WriteLine();

            // Вывод значений кратчайших расстояний
            for (int i = 0; i < N; ++i)
            {
                if (D[i] == INF)
                    Console.Write("\t" + '-');
                else
                    Console.Write("\t" + D[i]);
            }
            Console.WriteLine();
        }
        static void PrintWays(int startV)
        {
            var way = new List<char>();
            int current;

            Console.WriteLine("Пути:");
            //Для каждой вершины
            for (int i = 0; i < N; i++)
            {
                way.Clear();
                current = i; //Текущая вершина

                if (current == startV) continue;
                if (current == -1) break;

                do
                {
                    //Добавляем буквенное представление в "Путь"
                    way.Add(Convert.ToChar('A' + current));
                    //Меняем текущую на её родителя
                    current = Parents[current];
                } while (current != startV && current != -1);

                //Добрались
                if (current == startV)
                {
                    way.Reverse();
                    Console.Write("{0}: {1} -> {2}", way[^1], Convert.ToChar(startV + 'A'), string.Join(" -> ", way));
                }
                else //Не добрались
                {
                    Console.Write("Из {0} нет пути в {1}", Convert.ToChar(startV + 'A'), Convert.ToChar(i + 'A'));
                }

                Console.WriteLine();
            }
            return;
        }

        static void Error(string message)
        {
            Console.WriteLine($"Ошибка: {message}!");
            Console.Write("Нажмите любую клавишу для продолжения... ");
            Console.ReadKey();
        }
    }
}
