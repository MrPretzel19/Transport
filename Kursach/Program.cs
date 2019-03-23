using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursach
{
    /* Постановка:
     * Создать два класса, отвечающих за создание массива транспортной задачи
     * и вычисление его потенциалов.
     * 
     * -------------------------------------------------------------------------------------------
     * Класс FirstBasis
     * 
     * Методы:
     * Initialize - Запускает процедуру создания массива транспортной задачи.
     *           Включает в себя процедуру ввода как значений поставок, так и цен на клетках.
     * SetBasis - Расставляет базисы в транспортной задаче, основываясь на двух массивах поставок.
     * 
     * --------------------------------------------------------------------------------------------
     * Класс Potencials
     * 
     * Методы:
     * Potencials - Конструктор класса. Требует на вход массив транспортной задачи. 
     *            Присваивает его значение внутриклассовому массиву.
     * DeterminCoef - Вычисляет коэффициенты массива транспортной задачи, распределяя их в два вспомогательных массива.
     * DeterminPotenc - Вычисляет координату клетки с наименьшей потенциальной стоимостью.
     * 
     * ---------------------------------------------------------------------------------------------
     * Класс TranspMass
     * 
     * Содержит переменные-ячейки для создания массива транспортной задачи:
     * 1)potenial
     * 2)price
     * 3)basis
     * 
     * ---------------------------------------------------------------------------------------------
     * Общие переменные:
     * 
     * transpMatr - массив транспортной задачи.
     * 
     */

    public class TranspMass
    {
        public static int?[] coefA = new int?[5];
        public static int?[] coefB = new int?[5];

        public static int?[] Pri = new int?[5];
        public static int?[] Pos = new int?[5];

        public static int? Gamma;
        public static int K;
        public static int Z;

        public int _price { get; set; }
        public int? _potencial { get; set; }
        public int? _basis { get; set; }
        public char? _sign { get; set; }
        public TranspMass(int price)
        {
            _price = price;
        }
    }



    class Program
    {
        static int i, j;
        static TranspMass[,] tm;

        static void Main(string[] args)
        {
            FirstBasis transpMatr = new FirstBasis();
            tm = transpMatr.Initialize();
            Potencials potencMatr = new Potencials(transpMatr.tm);

            // создаем переменную класса Loop для работы с циклом
            Loop lo = new Loop();
                        // создаем переменную класса Statistics для подсчета
                        // омеги, Zожидаемого, Z, и k
            Statistics Statis = new Statistics();

            TranspMass.Gamma = -1;
            Console.Clear();
            //lo.Vyvod(transpMatr.tm);
            List<CirlcleType> tree = new List<CirlcleType>();
            Kords koord = new Kords();
            int kol = 1;
            koord = potencMatr.DetermCoef(tm);
            while (TranspMass.Gamma < 0)
            {
                
                Console.WriteLine("План №{0} Новые точки цикла {1} {2}", kol++, koord.i, koord.j);
                tree = lo.Circle(koord.i, koord.j, transpMatr.tm);
                if (tree[0].flag == false)
                {
                    Console.WriteLine("Невозможно построить цикл, в данном плане");
                    break;
                }
                lo.Vyvod(transpMatr.tm);
                Statis.CountZ(transpMatr.tm, transpMatr.tm.GetLength(0), transpMatr.tm.GetLength(1));
                NewPlan plan = new NewPlan(transpMatr.tm, tree);
                transpMatr.tm = plan.UpdatePlan();

                // второй раз выводится таблица уже с перемещенным минимумом в точку начала цикла

                Statis.OutParametr();
                lo.ClearSign(transpMatr.tm);
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                koord = potencMatr.DetermCoef(tm);
            }
            Console.WriteLine("План №{0}", kol);
            lo.Vyvod(transpMatr.tm);
            Statis.CountZ(transpMatr.tm, transpMatr.tm.GetLength(0), transpMatr.tm.GetLength(1));
            Console.WriteLine("Z = {0}", TranspMass.Z);
            Console.WriteLine("Конец программы, для выхода нажмите Esc");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
    class Kords
    {
        public int i;
        public int j;

    }

    class Potencials
    {
        private TranspMass[,] tm;

        public Potencials(TranspMass[,] a)
        {
            tm = a;
        }


        //Работает. Вычисляет коэффициенты и потенциалы текущего плана.
        public Kords DetermCoef(TranspMass[,] t)
        {


            Array.Clear(TranspMass.coefA, 0, TranspMass.coefA.Length);
            Array.Clear(TranspMass.coefB, 0, TranspMass.coefB.Length);
            for(int i = 0; i < t.GetLength(0); i++)
            {
                for(int j = 0; j < t.GetLength(1); j++)
                {
                    t[i, j]._potencial = null;
                }
            }
            TranspMass.coefA[0] = 0;
            int cA = 1;
            int cB = 0;


            //Console.WriteLine("Коорд.[{0},{1}] coefA[{2}] = {3}", 0, 0, 0, TranspMass.coefA[0]);
            //Проход по матрице происходит, пока не будут заполены все коэффициенты 
            while (cA != tm.GetLength(0) || cB != tm.GetLength(1))
            {
                for (int i = 0; i < tm.GetLength(0); i++)
                {
                    for (int j = 0; j < tm.GetLength(1); j++)
                    {
                        if (tm[i, j]._basis != null)//Условие вычисления новых коэффициетов транспортной задачи
                        {
                            if (TranspMass.coefA[i] != null & TranspMass.coefB[j] == null) //Если известен только один коэффициент, то вычисляет второй
                            {
                                TranspMass.coefB[j] = tm[i, j]._price - TranspMass.coefA[i];
                                // Console.WriteLine("Коорд.[{0},{1}] coefB[{2}] = {3}", i, j, i, TranspMass.coefB[j]);
                                cB++;
                            }
                            else if (TranspMass.coefB[j] != null & TranspMass.coefA[i] == null)
                            {
                                TranspMass.coefA[i] = tm[i, j]._price - TranspMass.coefB[j];
                                // Console.WriteLine("Коорд.[{0},{1}] coefA[{2}] = {3}", i, j, j, TranspMass.coefA[i]);
                                cA++;
                            }


                        }
                    }
                }
            }
            TranspMass.Gamma = null;
            Kords omega = new Kords();
            //Расставляет потенциалы по плану
            for (int i = 0; i < tm.GetLength(0); i++)
            {
                for (int j = 0; j < tm.GetLength(1); j++)
                {
                    if (tm[i, j]._basis == null)
                    {
                        tm[i, j]._potencial = TranspMass.coefA[i] + TranspMass.coefB[j];
                        //Console.WriteLine("Потенциал на коорд.[{0},{1}] = {2}", i, j, tm[i,j]._potencial);
                    }
                    else
                    {
                        tm[i, j]._potencial = tm[i, j]._price;
                    }
                }
            }
            //Вычисляет первое значение разности потенциала и цены
            int i1 = tm.GetLength(0) - 1;
            int j1 = tm.GetLength(1) - 1;

            while (TranspMass.Gamma == null)
            {
                if (tm[i1, j1]._basis == null)
                {
                    TranspMass.Gamma = tm[i1, j1]._price - tm[i1, j1]._potencial;
                    //Console.WriteLine("max на коорд.[{0},{1}] = {2}", i1, j1, tm[i1, j1]._potencial);
                    omega.i = i1; omega.j = j1;
                }
                j1--;
            }
            //Вычисляет минимальную разность цены и потенциала на плане
            //Ставит значение выходных координат в null при отсутствии отрицательной разности

            for (j1 = tm.GetLength(1) - 1; j1 >= 0; j1--)
            {
                //Организуется цикл с неимененным значением j1, чтобы не проходить уже пройденное расстояние дважды
                for (i1 = tm.GetLength(1) - 1; i1 >= 0; i1--)
                {
                    if (tm[i1, j1]._price - tm[i1, j1]._potencial <= TranspMass.Gamma)
                    {
                        TranspMass.Gamma = tm[i1, j1]._price - tm[i1, j1]._potencial;
                        omega.i = i1; omega.j = j1;
                    }
                }
            }
            //Console.WriteLine("омега = {0}, на координатах [{1}, {2}]",TranspMass.Gamma, omega.i, omega.j);
            //Console.ReadKey();
            if (TranspMass.Gamma < 0)
            {
                return omega;
            }
            else
            {
                return null;
            }
        }
        //Метод вычисления потециалов на ячейках текущего плана
        //Также вычисляет минимальную разность цены и потенциала среди всех ячеек текущего плана
        //Кординаты полученной ячейки дает на выход
    }

    class FirstBasis
    {

        public int i, j;

        public TranspMass[,] tm;
        public int[] post;
        public int[] pol;

        public TranspMass[,] Initialize()
        {
            i = 5;
            j = 5;
            //Тут создать массив
            tm = new TranspMass[i, j];
            post = new int[i];
            pol = new int[j];

            //Построение и вывод заготовки матрицы для ввода различных значений
            //Вывод пояснений о вводе будет появляться на 7 строке консоли
            //Под пояснениями будет матрица
            Console.SetCursorPosition(0, 7);
            postrMatr();
            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Введите получателей:");
            {
                int otst = 0;
                for (int k = 0; k < i; k++)
                {
                    Console.SetCursorPosition(6 + (j * 6), 9 + otst);
                    pol[k] = Convert.ToInt32(Console.ReadLine());
                    TranspMass.Pri[k] = pol[k];
                    otst += 2;

                }
            }

            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Введите поставки:");
            {
                int otst = 0;
                for (int k = 0; k < i; k++)
                {
                    Console.SetCursorPosition(6 + otst, 9 + (i * 2));
                    post[k] = Convert.ToInt32(Console.ReadLine());
                    TranspMass.Pos[k] = post[k];
                    otst += 6;
                }
            }

            Console.SetCursorPosition(0, 7);
            Console.WriteLine("Введите цены на клетках:");
            {
                int otsty = 0;
                int otstx = 0;
                for (int k = 0; k < i; k++)
                {
                    otstx = 0;

                    for (int l = 0; l < j; l++)
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        Console.SetCursorPosition(8 + otstx, 9 + otsty);
                        tm[k, l] = new TranspMass(Convert.ToInt32(Console.ReadLine()));
                        //tm[k, l] = new TranspMass(2);
                        otstx += 6;
                    }
                    otsty += 2;
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            SetBasis(post, pol);
            return tm;
        }

        //Необходим метод подсчета Z
        public void SetBasis(int[] A, int[] B)
        {
            int i = 0;
            int j = 0;
            while (j < B.Length & i < A.Length)
            {
                if ((A[j] - B[i]) > 0)
                {
                    tm[i, j]._basis = B[i];
                    A[j] -= B[i];
                    Console.WriteLine("i = {0} j = {1} basis = {2} ", i + 1, j + 1, tm[i, j]._basis);
                    i++;
                }
                else if ((A[j] - B[i]) <= 0)
                {
                    tm[i, j]._basis = A[j];
                    B[i] -= A[j];
                    Console.WriteLine("i = {0} j = {1} basis = {2} ", i + 1, j + 1, tm[i, j]._basis);
                    j++;
                }
            }
        }

        public void postrMatr()
        {
            int otst = 0;
            Console.SetCursorPosition(0, 8);
            int l = 0;
            while (l <= i)
            {
                int k = 0;
                switch (l)
                {
                    case 0:
                        Console.SetCursorPosition(0, 8);
                        Console.Write("i\\j |");
                        while (k < j)
                        {
                            Console.Write("{0,-5}|", "B" + (k + 1));
                            k++;
                        }
                        Console.Write(" Al(i)");
                        l++;
                        break;

                    default:
                        Console.SetCursorPosition(0, 8 + l + otst);
                        Console.Write("{0,-4}|", "A" + l);
                        while (k < j)
                        {
                            Console.Write("     |");
                            k++;
                        }
                        otst++;
                        Console.SetCursorPosition(0, 8 + l + otst);
                        k = 0;
                        Console.Write("    |");
                        while (k < j)
                        {
                            Console.Write("     |");
                            k++;
                        }
                        l++;
                        break;

                }
            }
            {
                int k = 0;
                Console.SetCursorPosition(0, 8 + l + otst);
                Console.Write("{0,-3}|", "B(j)");
                while (k < j)
                {
                    Console.Write("     |");
                    k++;
                }
            }
        }
    }

    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * */
    public class CirlcleType
    {
        public int i { get; set; }
        public int j { get; set; }
        public bool flag = true;
    }


    public class Loop
    {

        bool changeway = true;

        //***************************************************************
        //* Vyvod - вывод плана в таблицу.
        //***************************************************************
        //* Формальный параметр:
        //*    tm         - переменная, содержащая матрицы с базисами, 
        //*                 поставками, ценами и знаками, проставленные цилом,
        //*                 в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    end        - количество строк;
        //*    i          - счетчик строк;
        //*    j          - счетчик столбцов;
        //*    lpotenc    - количество символов содержащихся в переменной potenc;
        //*    lprice     - количество символов содержащихся в переменной price;
        //*    potenc     - строковый тип переменной potenc;
        //*    price      - строковый тип переменной price;
        //*    длин       - количество символов содержащихся в переменной tm[i, j]._basis.
        //***************************************************************
        public void Vyvod(TranspMass[,] tm)
        {
            Console.WriteLine("──────┬────────┬────────┬────────┬────────┬────────┬────────┬───────┐");
            Console.WriteLine("      |   a1   |   a2   |   a3   |   a4   |   a5   |    ai  |   A   |");
            Console.WriteLine("──────┼────────┼────────┼────────┼────────┼────────┼────────┼───────┤");
            int end = tm.GetLength(0) - 1;
            for (int i = 0; i <= end; i++)
            {
                Console.Write("B{0}    |", i +1);
                for (int j = 0; j <= end; j++)
                {
                    Console.Write("{0,3}  {1,3}|", tm[i, j]._potencial, tm[i, j]._price);
                }
                Console.Write("        |       |");
                Console.WriteLine();
                Console.Write("      |");
                for (int j = 0; j <= end; j++)
                {
                    Console.Write("    {0,1}   |", tm[i, j]._sign);
                }
                Console.Write("  {0,3}   |  {1,3}  |", TranspMass.coefA[i], TranspMass.Pri[i]);
                Console.WriteLine();
                Console.Write("      |");
                for (int j = 0; j <= end; j++)
                {
                    Console.Write("  {0,3}   |",  tm[i, j]._basis);
                }
                Console.Write("        |       |");
                Console.WriteLine();
                if (i < 4)
                Console.WriteLine("──────┼────────┼────────┼────────┼────────┼────────┼────────┼───────┤");
            }
            Console.WriteLine("──────┼────────┼────────┼────────┼────────┼────────┼────────┴───────┘");
            Console.Write("  bj  |");
            for (int j = 0; j <= end; j++)
            {
                Console.Write("  {0,3}   |", TranspMass.coefB[j]);
            }
            Console.WriteLine();
            Console.WriteLine("──────┼────────┼────────┼────────┼────────┼────────┤");
            Console.Write("  B   |");
            for (int j = 0; j <= end; j++)
            {
                Console.Write("   {0}   |",  TranspMass.Pos[j]);
            }
            Console.WriteLine();
            Console.WriteLine("──────┴────────┴────────┴────────┴────────┴────────┘");

        }



        //***************************************************************
        //* Circle - строит список с координатами, по которым проходится цикл.
        //***************************************************************
        //* Формальные параметры:
        //*    i             - начальная координата цикла, по строкам;
        //*    j             - начальная координата цикла, по столбцу;
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    flag          - флаг для цикла;
        //*    tree          - список с координатами(цикл);
        //*    направление   - переменная, по которой определяется начало поиска
        //*                    базиса по горизонтали или вертикали сначала;
        //*    колсменнаправ - по этому числу опеределяется метод поиска;
        //*    sign          - переменная которая расставляет + или -.
        //***************************************************************
        public List<CirlcleType> Circle(int i, int j, TranspMass[,] tm)
        {
            bool flag = true;
            List<CirlcleType> tree = new List<CirlcleType>();
                        //заносим начало цикла
            tree.Add(new CirlcleType() { i = i, j = j });
            if ((i == 0) && (j == 3))
            {
                flag = true;
            }
            bool направление = false;
            byte колсменнаправ = 1;
                        //начинаем цикл
            while (flag == true)
            {
                        // проверяем на замкнутость
                if (((tree.Count - 1 > 3) && (tree[0].i == tree[tree.Count - 1].i) && (tree[0].j == tree[tree.Count - 1].j)) || (колсменнаправ == 9))
                {
                    break;
                }
                if (колсменнаправ == 8)
                    Console.WriteLine(колсменнаправ);
                if (направление)
                {
                    if ((колсменнаправ == 1) || (колсменнаправ == 2) || (колсменнаправ == 5) || (колсменнаправ == 6))
                        tree = CircleGoriz(tree, tree[0].i, tree[0].j, tm);
                    else
                        tree = UnCircleGoriz(tree, tree[0].i, tree[0].j, tm); 
                }
                else
                {
                    if ((колсменнаправ == 1) || (колсменнаправ == 2) || (колсменнаправ == 3) || (колсменнаправ == 4))
                        tree = CircleVert(tree, tree[0].i, tree[0].j, tm);
                    else
                        tree = UnCircleVert(tree, tree[0].i, tree[0].j, tm);
                }
                направление = !направление;

                        //если не построился цикл то меняем метод поиска координат 
                if (changeway == false)
                {
                    колсменнаправ++;
                    
                    направление = колсменнаправ % 2 == 0 ? true : false;
                    changeway = true;
                    tree.Clear();
                    tree.Add(new CirlcleType() { i = i, j = j });
                }
            }
                        //проверяем построился ли цикл
                        //если нет то flag = false в 0 элементе
            if (tree.Count() > 3)
            {
                        //ставим null на всей матрицы содержащей знаки

                        //расставляем знаки по координатам, где прошел цикл
                char sign = '+';
                flag = true;
                foreach (CirlcleType от in tree)
                {
                    if (flag)
                    {
                        tm[от.i, от.j]._sign = sign;
                        flag = false;
                        sign = '-';
                    }
                    else
                    {
                        tm[от.i, от.j]._sign = sign;
                        flag = true;
                        sign = '+';
                    }
                }
            }
            else tree[0].flag = false;
                        
            return tree;
        }



        //***************************************************************
        //* CircleVert - поиск координаты в список для цикла, проходит сверху вниз.
        //***************************************************************
        //* Формальные параметры:
        //*    tree          - переменная с координатами прошедшего цикла;
        //*    i             - начальная координата цикла, по строкам;
        //*    j             - начальная координата цикла, по столбцу;
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    колэлвспи     - последний индекс списке tree;
        //*    i1            - счетчик по строкам.
        //***************************************************************
        private List<CirlcleType> CircleVert(List<CirlcleType> tree, int i, int j, TranspMass[,] tm)
        {

                int колэлвспи = tree.Count - 1;
                        // проходит сверху вниз
                for (int i1 = 0; i1 < tm.GetLength(0); i1++)
                {
                        // проверяем есть ли базис в клетке или количество итерации более 1 и это клетка с началом 
                    if (((tm[i1, tree[колэлвспи].j]._basis != null) && (i1 != tree[колэлвспи].i))
                        || ((tree[колэлвспи].j == j) && (i1 == i) && (колэлвспи > 2)))
                    {
                        // если да то заносим данный элемент в наш список
                        tree.Add(new CirlcleType() { i = i1, j = tree[колэлвспи].j });
                        break;
                    }
                    if ((колэлвспи > 2) && (j == tree[колэлвспи].j))
                    {
                        tree.Add(new CirlcleType() { i = i, j = j });
                        break;
                    }
                        // если не нашел пути то изменяем направления цикла при начале прохода цикла
                    if (i1 == 4) changeway = false;

                }
            return tree;
        }

        //***************************************************************
        //* UnCircleVert - поиск координаты в список для цикла, проходит снизу вверх.
        //***************************************************************
        //* Формальные параметры:
        //*    tree          - переменная с координатами прошедшего цикла;
        //*    i             - начальная координата цикла, по строкам;
        //*    j             - начальная координата цикла, по столбцу;
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    колэлвспи     - последний индекс списке tree;
        //*    i1            - счетчик по строкам.
        //***************************************************************
        private List<CirlcleType> UnCircleVert(List<CirlcleType> tree, int i, int j, TranspMass[,] tm)
        {

                int колэлвспи = tree.Count - 1;
                        // проходит снизу вверх
                for (int i1 = tm.GetLength(0) - 1; 0 <= i1; i1--)
                {
                        // проверяем есть ли базис в клетке или количество итерации более 1 и это клетка с началом 
                    if (((tm[i1, tree[колэлвспи].j]._basis != null) && (i1 != tree[колэлвспи].i))
                        || ((tree[колэлвспи].j == j) && (i1 == i) && (колэлвспи - 1 > 2)))
                    {
                        // если да то заносим данный элемент в наш список
                        tree.Add(new CirlcleType() { i = i1, j = tree[колэлвспи].j });
                        break;
                    }
                    if ((колэлвспи > 2) && (j == tree[колэлвспи].j))
                    {
                        tree.Add(new CirlcleType() { i = i, j = j });
                        break;
                    }
                        // если не нашел пути то изменяем направления цикла при начале прохода цикла
                    if (i1 == 0) changeway = false;
                }
            return tree;
        }

        //***************************************************************
        //* CircleGoriz - поиск координаты в список для цикла, проходит слева на право.
        //***************************************************************
        //* Формальные параметры:
        //*    tree          - переменная с координатами прошедшего цикла;
        //*    i             - начальная координата цикла, по строкам;
        //*    j             - начальная координата цикла, по столбцу;
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    колэлвспи     - последний индекс списке tree;
        //*    j1            - счетчик по столбцу.
        //***************************************************************
        private List<CirlcleType> CircleGoriz(List<CirlcleType> tree, int i, int j, TranspMass[,] tm)
        {

                int колэлвспи = tree.Count - 1;
                        // проходит слева на право
                for (int j1 = 0; j1 < tm.GetLength(1); j1++)
                {
                        // проверяем есть ли базис в клетке или количество итерации более 1 и это клетка с началом 
                    if (((tm[tree[колэлвспи].i, j1]._basis != null) && (j1 != tree[колэлвспи].j))
                        || ((j1 == j) && (tree[колэлвспи].i == i) && (колэлвспи > 2)))
                    {
                        // если да то заносим данный элемент в наш список
                        tree.Add(new CirlcleType() { i = tree[колэлвспи].i, j = j1 });
                        break;
                    }
                    if ((колэлвспи > 2) && (i == tree[колэлвспи].i))
                    {
                        tree.Add(new CirlcleType() { i = i, j = j });
                        break;
                    }
                        // если не нашел пути то изменяем направления цикла при начале прохода цикла
                    if (j1 == 4) changeway = false;
                }

            return tree;
        }

        //***************************************************************
        //* UnCircleGoriz - поиск координаты в список для цикла, проходит справа налево.
        //***************************************************************
        //* Формальные параметры:
        //*    tree          - переменная с координатами прошедшего цикла;
        //*    i             - начальная координата цикла, по строкам;
        //*    j             - начальная координата цикла, по столбцу;
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    колэлвспи     - последний индекс списке tree;
        //*    j1            - счетчик по столбцу.
        //***************************************************************
        private List<CirlcleType> UnCircleGoriz(List<CirlcleType> tree, int i, int j, TranspMass[,] tm)
        {

                int колэлвспи = tree.Count - 1;
                        // проходит справо на лево
                for (int j1 = tm.GetLength(1)-1;0 <= j1; j1--)
                {
                        // проверяем есть ли базис в клетке или количество итерации более 1 и это клетка с началом 
                    if (((tm[tree[колэлвспи].i, j1]._basis != null) && (j1 != tree[колэлвспи].j))
                        || ((j1 == j) && (tree[колэлвспи].i == i) && (колэлвспи-1 > 2)))
                    {
                        // если да то заносим данный элемент в наш список
                        tree.Add(new CirlcleType() { i = tree[колэлвспи].i, j = j1 });
                        break;
                    }
                    if ((колэлвспи > 2) && (i == tree[колэлвспи].i))
                    {
                        tree.Add(new CirlcleType() { i = i, j = j });
                        break;
                    }
                        // если не нашел пути то изменяем направления цикла при начале прохода цикла
                    if (j1 == 0) changeway = false;
                }

            return tree;
        }

        //***************************************************************
        //* ClearSign - поиск координаты в список для цикла, проходит справа налево.
        //***************************************************************
        //* Формальный параметр:
        //*    tm            - переменная, содержащая матрицы с базисами, 
        //*                    поставками, ценами и знаками, проставленные цилом,
        //*                    в данной коодинате.
        //***************************************************************
        //* Локальные переменные: 
        //*    i1           - счетчик строк;
        //*    j1           - счетчик столбцов.
        //***************************************************************
        public void ClearSign(TranspMass[,] tm)
        {
            for (int i1 = 0; i1 < tm.GetLength(0); i1++)
            {
                for (int j1 = 0; j1 < tm.GetLength(0); j1++)
                {
                    tm[i1, j1]._sign = null;
                }
            }
        }
    }


    class NewPlan
    {
        private TranspMass[,] tm;
        private int min;
        public int n, m;
        List<CirlcleType> nul_b;

        public NewPlan(TranspMass[,] a, List<CirlcleType> null_base)
        {
            tm = a;
            nul_b = null_base;
        }
        public TranspMass[,] UpdatePlan()
        {
            min = (int)tm[nul_b[1].i, nul_b[1].j]._basis; // первая минимальная координата по циклу равна минимуму
            int min_i = nul_b[1].i;
            int min_j = nul_b[1].j;
            for (int i = 2; i < nul_b.Count - 1; i++) // начинаем со след элемента
            {
                if ((tm[nul_b[i].i, nul_b[i].j]._sign == '-') && (tm[nul_b[i].i, nul_b[i].j]._basis < min))
                {
                    min_i = nul_b[i].i;
                    min_j = nul_b[i].j;
                    min =(int) tm[min_i, min_j]._basis;
                }
            }
            for (int i = 0; i < nul_b.Count - 1; i++)
            {
                if (tm[nul_b[i].i, nul_b[i].j]._sign == '-')
                    tm[nul_b[i].i, nul_b[i].j]._basis -= min;
                else
                    tm[nul_b[i].i, nul_b[i].j]._basis += min;
            }
            tm[min_i, min_j]._basis = null;             // последнее минимальной координате присваиваем null
            tm[nul_b[0].i, nul_b[0].j]._basis = min;
            TranspMass.K = min;// переносим в начальную точку min
            return tm;
        }
    }


    class Statistics
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         *  Класс для подсчета стоимости текущего плана и новой ожидаемой стоимости.
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         *  Формальные параметры:
         *       a - переменная, хранящая матрицы с базисами,
         *           поставками, ценами и знаками в одной ячейке;
         *     n,m - размерность передаваемой матрицы 
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
         *   Локальные переменные:
         *      Z - стоимость текущего плана;
         *      K - максимальная разница между ценой и потенциалом среди всех клеток;
         *  Gamma - минимальный базис в ячейке, отмеченный знаком минус;
         *    min - вспомогательная переменная для посчета K;
         *  count - вспомогательная переменная для подсчета Gamma;
         *     Z2 - ожидаемая стоимость нового плана.
         * * * * * */

        public void CountZ(TranspMass[,] a, int n, int m)
        {
            int Z = 0;
            // Проходим по всей таблице
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // Подсчитываем сумму плана
                    if (a[i, j]._basis != null)
                        Z += a[i, j]._price *(int)a[i, j]._basis;
                }
            }
            TranspMass.Z = Z;
        }
        public void OutParametr()
        {
            Console.WriteLine("Z = {0}", TranspMass.Z);
            Console.WriteLine("K = {0}", TranspMass.K);
            Console.WriteLine("Gamma = {0}", TranspMass.Gamma);
            // Подсчет ожидаемой стоимости
            int Z2 = TranspMass.Z + TranspMass.K * (int)TranspMass.Gamma;
            Console.WriteLine("Z ожидаемое = {0}", Z2);
        }
    }





}



