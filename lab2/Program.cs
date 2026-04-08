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
        enum Color {White, Gray, Black}

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
                        "7. Минимум переходов от вершины до остальных вершин (поиск в ширину).\n" +
                        "8. Связность графа и определение циклов (поиск в глубину).\n" +
                        "9. Выход из программы."
                        );
                    Console.WriteLine("Введите номер действия (1 .. 9):");

                    if (!int.TryParse(Console.ReadLine(), out int Case) || Case > 9 || Case < 1)
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
                            BFS();
                            break;
                        case 8:
                            DFS();
                            break;
                        case 9:
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
            Console.WriteLine("Пути:");
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
        static void BFS()
        {
            Console.WriteLine("МИНИМУМ ПЕРЕХОДОВ ОТ ВЕРШИНЫ ДО ОСТАЛЬНЫХ ВЕРШИН (ПОИСК В ШИРИНУ)");
            int startV = GetVertex();
            if (startV == -1) return;

            Parents = new int[N];
            for (int i = 0; i < N; i++) Parents[i] = -1;

            bool[] visited = new bool[N];
            int[] dist = new int[N];
            Queue<int> q = new Queue<int>();
            for (int i = 0; i < N; i++)
            {
                visited[i] = false;
                dist[i] = INF;
            }

            dist[startV] = 0;
            q.Enqueue(startV);
            Console.Write("+{0}({1}) ", Convert.ToChar(startV + 'A'), dist[startV]);
            visited[startV] = true;

            int currentV;
            while (q.Count > 0)
            {
                currentV = q.Dequeue();
                Console.Write("-{0}({1}) ", Convert.ToChar(currentV + 'A'), dist[currentV]);
                for (int i = 0; i < N; i++)
                {
                    int neighbor = M[currentV, i];
                    if (neighbor > 0 && neighbor < INF && !visited[i])
                    {
                        dist[i] = dist[currentV] + 1;
                        q.Enqueue(i);
                        Console.Write("+{0}({1}) ", Convert.ToChar(i + 'A'), dist[i]);
                        Parents[i] = currentV;
                        visited[i] = true;
                    }
                }
            }

            Console.WriteLine("\nМИНИМУМ ПЕРЕХОДОВ:");
            for (int i = 0; i < N; i++)
                Console.Write($"\t{Convert.ToChar(i + 'A')}");
            Console.WriteLine();
            for (int i = 0; i < N; i++)
            {
                if (dist[i] < INF) Console.Write($"\t{dist[i]}");
                else Console.Write($"\t-");
            }
                

            Console.WriteLine();

            Console.WriteLine("МИНИМАЛЬНЫЕ ПЕРЕХОДЫ:");
            PrintWays(startV);
        }
        static void DFS()
        {
            Console.WriteLine("СВЯЗНОСТЬ ГРАФА И ОПРЕДЕЛЕНИЕ ЦИКЛОВ (ПОИСК В ГЛУБИНУ)");

            bool Directed = IsDirectedGraph();
            int[] Components = new int[N];
            Stack<int> GreyPath = new Stack<int>();
            List<int> Cycle = new List<int>();
            Color[] color = new Color[N];
            for (int i = 0; i < N; i++)
            {
                Components[i] = 0;
                color[i] = Color.White;
            }

            int ComponentsCount = 0;
            for (int i = 0; i < N; i++)
            {
                if (Components[i] == 0)
                {
                    ComponentsCount++;
                    GreyPath.Push(i);
                    while(GreyPath.Count > 0)
                    {
                        int currV = GreyPath.Peek();
                        if (color[currV] == Color.White)
                        {
                            color[currV] = Color.Gray;
                            Console.Write("(" + Convert.ToChar(currV + 'A') + " ");
                            Components[currV] = ComponentsCount;
                        }

                        bool FoundWhite = false;
                        for (int j = 0; j < N; j++)
                        {
                            if (M[currV, j] > 0 && M[currV, j] < INF)
                            {
                                if (color[j] == Color.Gray)
                                {
                                    //GreyPath.Pop();
                                    //int Prev = GreyPath.Count != 0 ? GreyPath.Peek() : -1;
                                    int currV_Temp = GreyPath.Pop();
                                    int Prev = GreyPath.Count != 0 ? GreyPath.Peek() : -1;
                                    GreyPath.Push(currV_Temp);

                                    if (Directed || !Directed && j != Prev)
                                    {
                                        Cycle.Clear();
                                        while (j != GreyPath.Peek())
                                            Cycle.Insert(0, GreyPath.Pop());
                                        foreach (int U in Cycle)
                                            GreyPath.Push(U);
                                        Cycle.Insert(0, j);
                                    }
                                }
                                if (color[j] == Color.White)
                                {
                                    GreyPath.Push(j);
                                    FoundWhite = true;
                                    break;
                                }
                            }
                        }
                        if (!FoundWhite)
                        {
                            color[currV] = Color.Black;
                            Console.Write(Convert.ToChar(currV + 'A') + ") ");
                            GreyPath.Pop();
                        }
                    }
                }
            }

            Console.WriteLine();

            if (Cycle.Count == 0) 
                Console.WriteLine("В графе нет циклов.");
            else
            {
                Console.Write("В графе есть цикл: ");
                foreach (int V in Cycle)
                    Console.Write("{0:D} ", Convert.ToChar(V + 'A'));
                Console.WriteLine();
            }

            if (Directed) Console.WriteLine("Граф ориентированный.Связность не определяется.");
            if (!Directed)
            {
                if (ComponentsCount == 1)
                    Console.WriteLine("Граф связный.");
                else
                {
                    Console.WriteLine("Граф не связный. Количество компонент: {0:D}", ComponentsCount);
                    for (int i = 1; i <= ComponentsCount; i++)
                    {
                        Console.WriteLine("{0:D}:", i);
                        for (int j = 0; j < N; j++)
                            if (Components[j] == i) Console.Write("{0:D} ", Convert.ToChar(j + 'A'));
                        Console.WriteLine();
                    }
                }
            }


        }

        static void Error(string message)
        {
            Console.WriteLine($"Ошибка: {message}!");
            Console.Write("Нажмите любую клавишу для продолжения... ");
            Console.ReadKey();
        }
    }
}
