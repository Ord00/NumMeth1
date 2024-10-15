﻿using System;
using System.IO;

namespace NumMeth1
{
    class Program
    {
        static void ClassicalTest(ref int selector)
        {
            FileStream fStreamSrc = new FileStream("Ex1.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fStreamSrc);

            TridiagonalMatrix matrix = new TridiagonalMatrix();
            matrix.Fill(reader);

            Vector d = new Vector();
            d.Fill(reader, matrix.Length);

            reader.Close();
            fStreamSrc.Close();

            while (selector == 2)
            {
                switch (Menu(selector))
                {
                    case 1:
                        {
                            StreamWriter writer = new StreamWriter(Console.OpenStandardOutput());
                            writer.AutoFlush = true;
                            Console.SetOut(writer);

                            writer.WriteLine("\nИсходная матрица:\n");
                            matrix.Print(writer);

                            writer.WriteLine("\n");

                            Vector x = SweepMethods.ClassicalShuttle(matrix, d, out Vector l, out Vector m);

                            writer.WriteLine("Вектор L:\n");
                            l.Print(writer);

                            writer.WriteLine("\n\nВектор M:\n");
                            m.Print(writer);

                            writer.WriteLine("\n\nРешение СЛАУ, полученное с помощью алгоритма прогонки:\n");
                            x.Print(writer);

                            Vector mult = matrix * x;
                            writer.WriteLine("\n\nРезультат умножения матрицы СЛАУ и вектора решения X:\n");
                            mult.Print(writer);

                            writer.WriteLine("\n\nВектор, определяющий правую часть СЛАУ:\n");
                            d.Print(writer);

                            writer.Close();

                            StreamWriter newConsoleWriter = new StreamWriter(Console.OpenStandardOutput())
                            {
                                AutoFlush = true
                            };
                            Console.SetOut(newConsoleWriter);
                        }
                        break;
                    case 2:
                        {
                            Console.WriteLine("\nВведите название файла:\n");
                            Console.Write("->\t");

                            FileStream fStreamDest = new FileStream(Console.ReadLine() + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(fStreamDest);

                            writer.WriteLine("Исходная матрица:\n");
                            matrix.Print(writer);

                            writer.WriteLine("\n");


                            Vector x = SweepMethods.ClassicalShuttle(matrix, d, out Vector l, out Vector m);

                            writer.WriteLine("Вектор L:\n");
                            l.Print(writer);

                            writer.WriteLine("\n\nВектор M:\n");
                            m.Print(writer);

                            writer.WriteLine("\n\nРешение СЛАУ, полученное с помощью алгоритма прогонки:\n");
                            x.Print(writer);

                            Vector mult = matrix * x;
                            writer.WriteLine("\n\nРезультат умножения матрицы СЛАУ и вектора решения X:\n");
                            mult.Print(writer);

                            writer.WriteLine("\n\nВектор, определяющий правую часть СЛАУ:\n");
                            d.Print(writer);

                            writer.Close();
                            fStreamDest.Close();
                        }
                        break;
                    case 0:
                        {
                            selector = 1;
                        }
                        break;
                }
            }
        }

        static void ClassicalComputExp(bool isWellCond = true, bool isWriteTitle = false)
        {
            string[] cond = { " ", "обусловленная", "матрица"};
            cond[0] = isWellCond ? "Хорошо" : "Плохо";

            Console.WriteLine(new String('-', 75));
            if (isWriteTitle)
            {
                string[,] titles = { {"Тип", "обусловленности", "" }, { "Размерность", "СЛАУ", "" } ,
                { "Диапазон", "значений элементов", "матрицы" }, { "Относительная", "погрешность", "" } };
                int size = 3;

                for (int i = 0; i < size; ++i)
                {
                    Console.WriteLine(String.Format($"|{titles[0, i],-20}|{titles[1, i],-15}|{titles[2, i],-20}|{titles[3, i],-15}|"));
                }
                Console.WriteLine(new String('-', 75));
            }

            int numTests = 10;
            int[] rangeDegrees = {-1, 1, 2, 3};
            double high;

            for (int i = 4; i <= 4096; i *= 2)
            {
                TridiagonalMatrix matrix = new TridiagonalMatrix(i);
                Vector exactX = new Vector(i);
                Vector realX;
                Vector d = new Vector(i);

                for (int k = 0; k < rangeDegrees.Length; ++k)
                {
                    high = Math.Pow(10, rangeDegrees[k]);

                    double avgError = 0;

                    for (int j = 0; j < numTests; ++j)
                    {
                        matrix.FillRandom(-high, high, isWellCond);
                        exactX.FillRandom(high, high);
                        d = matrix * exactX;

                        realX = SweepMethods.ClassicalShuttle(matrix, d, out Vector l, out Vector m);

                        avgError += (exactX - realX).Norm();
                    }
                    avgError /= numTests;

                    if (i == 4 && k < 3)
                    {
                        Console.WriteLine(String.Format($"|{cond[k],-20}" +
                            $"|{i,-15}|{$"[{-high}, {high}]",-20}|{String.Format("{0:0.0###e+00}", avgError),-15}|"));
                    }
                    else
                    {
                        Console.WriteLine(String.Format($"|{" ",-20}|{i,-15}" +
                            $"|{$"[{-high}, {high}]",-20}|{String.Format("{0:0.0###e+00}", avgError),-15}|"));
                    }
                }
            }
        }

        static void SpecialTest(ref int selector)
        {
            FileStream fStreamSrc = new FileStream("Ex2.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fStreamSrc);

            SpecialMatrix matrix = new SpecialMatrix();
            matrix.Fill(reader);

            Vector x = new Vector();
            x.Fill(reader, matrix.Length);

            reader.Close();
            fStreamSrc.Close();

            while (selector == 2)
            {
                switch (Menu(selector))
                {
                    case 1:
                        {
                            StreamWriter writer = new StreamWriter(Console.OpenStandardOutput());
                            writer.AutoFlush = true;
                            Console.SetOut(writer);

                            writer.WriteLine("\nИсходная матрица:\n");
                            matrix.Print(writer);

                            writer.WriteLine("\n");

                            Vector d = matrix * x;

                            writer.WriteLine("Результат умножения матрицы СЛАУ и вектора решения X:\n");
                            d.Print(writer);
                            writer.WriteLine("\n");

                            SweepMethods.SpecialShuttle(matrix, d, x, writer);

                            writer.Close();

                            StreamWriter newConsoleWriter = new StreamWriter(Console.OpenStandardOutput())
                            {
                                AutoFlush = true
                            };
                            Console.SetOut(newConsoleWriter);
                        }
                        break;
                    case 2:
                        {
                            Console.WriteLine("\nВведите название файла:\n");
                            Console.Write("->\t");

                            FileStream fStreamDest = new FileStream(Console.ReadLine() + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(fStreamDest);

                            writer.WriteLine("Исходная матрица:\n");
                            matrix.Print(writer);

                            writer.WriteLine("\n");

                            Vector d = matrix * x;

                            writer.WriteLine("Результат умножения матрицы СЛАУ и вектора решения X:\n");
                            d.Print(writer);
                            writer.WriteLine("\n");

                            SweepMethods.SpecialShuttle(matrix, d, x, writer);

                            writer.Close();
                            fStreamDest.Close();
                        }
                        break;
                    case 0:
                        {
                            selector = 1;
                        }
                        break;
                }
            }
        }

        static void SpecialComputExp(bool isWellCond = true, bool isWriteTitle = false)
        {
            string[] cond = { " ", "обусловленная", "матрица" };
            cond[0] = isWellCond ? "Хорошо" : "Плохо";

            Console.WriteLine(new String('-', 75));
            if (isWriteTitle)
            {
                string[,] titles = { {"Тип", "обусловленности", "" }, { "Размерность", "СЛАУ", "" } ,
                { "Диапазон", "значений элементов", "матрицы" }, { "Относительная", "погрешность", "" } };
                int size = 3;

                for (int i = 0; i < size; ++i)
                {
                    Console.WriteLine(String.Format($"|{titles[0, i],-20}|{titles[1, i],-15}|{titles[2, i],-20}|{titles[3, i],-15}|"));
                }
                Console.WriteLine(new String('-', 75));
            }

            int numTests = 10;
            int[] rangeDegrees = { -1, 1, 2, 3 };
            double high;

            for (int i = 4; i <= 4096; i *= 2)
            {
                SpecialMatrix matrix = new SpecialMatrix(i, i / 4 * 3);
                Vector exactX = new Vector(i);
                Vector realX;
                Vector f = new Vector(i);

                for (int k = 0; k < rangeDegrees.Length; ++k)
                {
                    high = Math.Pow(10, rangeDegrees[k]);

                    double avgError = 0;

                    for (int j = 0; j < numTests; ++j)
                    {
                        matrix.FillRandom(-high, high, isWellCond);
                        exactX.FillRandom(high, high);
                        f = matrix * exactX;

                        realX = SweepMethods.SpecialShuttle(matrix, f);

                        avgError += (exactX - realX).Norm();
                    }
                    avgError /= numTests;

                    if (i == 4 && k < 3)
                    {
                        Console.WriteLine(String.Format($"|{cond[k],-20}" +
                            $"|{i,-15}|{$"[{-high}, {high}]",-20}|{String.Format("{0:0.0###e+00}", avgError),-15}|"));
                    }
                    else
                    {
                        Console.WriteLine(String.Format($"|{" ",-20}|{i,-15}" +
                            $"|{$"[{-high}, {high}]",-20}|{String.Format("{0:0.0###e+00}", avgError),-15}|"));
                    }
                }
            }
        }

        public static int Menu(int selector)
        {
            int num = 1;

            Console.WriteLine($"\nВыберите следующее действие:\n");

            switch (selector)
            {
                case 1:
                    {
                        Console.WriteLine($"{num}. Проверить корректность работы алгоритма прогонки");
                        ++num;
                        Console.WriteLine($"{num}. Провести вычислительный эксперимент на основе алгоритма прогонки");
                        ++num;
                        Console.WriteLine($"{num}. Проверить корректность работы модифицированного алгоритма прогонки");
                        ++num;
                        Console.WriteLine($"{num}. Провести вычислительный эксперимент на основе модифицированного алгоритма прогонки");

                        Console.WriteLine($"0. Завершить работу");
                    }
                    break;
                case 2:
                    {
                        Console.WriteLine($"{num}. Вывести результаты проверки в консоль");
                        ++num;
                        Console.WriteLine($"{num}. Вывести результаты проверки в файл");

                        Console.WriteLine($"0. Вернуться в главное меню");
                    }
                    break;
            }
            Console.Write("\n->\t");
            return Convert.ToInt32(Console.ReadLine());
        }

        static void Main()
        {
            bool exit = false;
            int selector = 1;

            while (!exit)
            {
                switch (Menu(selector))
                {
                    case 1:
                        {
                            selector = 2;
                            ClassicalTest(ref selector);
                        }
                        break;
                    case 2:
                        {
                            Console.WriteLine("\nРезультаты вычислительного эксперимента на основе алгоритма прогонки:\n");
                            ClassicalComputExp(true, true);
                            ClassicalComputExp(false, false);
                            Console.WriteLine(new String('-', 75));
                        }
                        break;
                    case 3:
                        {
                            selector = 2;
                            SpecialTest(ref selector);
                        }
                        break;
                    case 4:
                        {
                            Console.WriteLine("\nРезультаты вычислительного эксперимента на основе модифицированного алгоритма прогонки:\n");
                            SpecialComputExp(true, true);
                            SpecialComputExp(false, false);
                            Console.WriteLine(new String('-', 75));
                        }
                        break;
                    case 0:
                        {
                            exit = true;
                        }
                        break;
                }
            }
        }
    }
}
