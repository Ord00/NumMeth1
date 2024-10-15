using System;
using System.IO;

namespace NumMeth1
{
    public class TridiagonalMatrix
    {
        public Vector Main { get; set; }

        public Vector Lower { get; set; }

        public Vector Upper { get; set; }

        public int Length { get => Main.Length; }

        private void MemoryAllocation(int size)
        {
            Main = new Vector(size);
            Lower = new Vector(size);
            Upper = new Vector(size);
        }

        public TridiagonalMatrix() {}

        public TridiagonalMatrix(int size)
        {
            MemoryAllocation(size);
        }

        public TridiagonalMatrix(Vector lower, Vector main, Vector upper)
        {
            Lower = lower;
            Main = main;
            Upper = upper;
        }

        public static Vector operator*(TridiagonalMatrix matrix, Vector vector)
        {
            if (matrix.Main.Length != vector.Length)
            {
                throw new ArgumentException("Матрица и вектор должны быть одинакового размера!");
            }

            Vector result = new Vector(matrix.Main.Length);

            for (int i = 1; i <= matrix.Main.Length; ++i)
            {
                result[i] = matrix.Main[i] * vector[i]; // Главная диагональ

                if (i > 1)
                {
                    result[i] += matrix.Lower[i] * vector[i - 1]; // Нижняя диагональ
                }
                if (i < matrix.Main.Length)
                {
                    result[i] += matrix.Upper[i] * vector[i + 1]; // Верхняя диагональ
                }
            }
            return result;
        }

        public void FillRandom(double low, double high, bool isWellCond = true)
        {
            Lower.FillRandom(low, high);
            Upper.FillRandom(low, high);

            Lower[1] = 0;
            Upper[Length] = 0;

            if (isWellCond)
            {
                int specMult = 100000;
                Main.FillRandom((Random rand, int i) => rand.NextDouble() * specMult * (high - low) + low);
            }
            else
            {
                Main.FillRandom(low, high);
            }
        }

        //public void FillRandom(double low, double high, bool isWellCond = true)
        //{
        //    if (isWellCond)
        //    {
        //        Lower.FillRandom(low, high);
        //        Lower[1] = 0;

        //        Main.FillRandom((Random rand, int i) =>
        //        {
        //            double res = rand.NextDouble() * (high - Lower[i]) + Lower[i];
        //            if (rand.Next(0, 2) == 0)
        //            {
        //                res += low;
        //            }
        //            else
        //            {
        //                res += Lower[i];
        //            }
        //            return res;
        //        });

        //        Upper.FillRandom((Random rand, int i) => {
        //            double res = rand.NextDouble() * (Math.Abs(Main[i]) - Math.Abs(Lower[i]));
        //            if (rand.Next(0, 2) == 0)
        //            {
        //                res -= Math.Abs(Main[i]) - Math.Abs(Lower[i]);
        //            }
        //            return res;
        //        });

        //        Upper[Length] = 0;
        //    }
        //    else
        //    {
        //        Lower.FillRandom(low, high);
        //        Upper.FillRandom(low, high);

        //        Lower[1] = 0;
        //        Upper[Length] = 0;

        //        Main.FillRandom((Random rand, int i) => {
        //            double res = rand.NextDouble() * Math.Min(high, Math.Abs(Upper[i]) + Math.Abs(Lower[i]));
        //            if (rand.Next(0, 2) == 0)
        //            {
        //                res -= Math.Min(high, Math.Abs(Upper[i]) + Math.Abs(Lower[i]));
        //            }
        //            return res;
        //        });
        //    }
        //}

        public void Fill(StreamReader reader)
        {
            string line = reader.ReadLine();

            MemoryAllocation(Convert.ToInt32(line));

            line = reader.ReadLine();
            var str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            Main[1] = Convert.ToDouble(str[0]);
            Upper[1] = Convert.ToDouble(str[1]);

            for (int i = 2; i <= Length - 1; i++)
            {
                line = reader.ReadLine();
                str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                Lower[i] = Convert.ToDouble(str[i - 2]);
                Main[i] = Convert.ToDouble(str[i - 1]);
                Upper[i] = Convert.ToDouble(str[i]);
            }

            line = reader.ReadLine();
            str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            Lower[Length] = Convert.ToDouble(str[Length - 2]);
            Main[Length] = Convert.ToDouble(str[Length - 1]);

            reader.ReadLine();
        }

        public void Print(StreamWriter writer, int mantissa = 0, int padding = 5)
        {
            writer.Write((Math.Round(Main[1], mantissa).ToString() + " ").PadRight(padding));
            writer.Write((Math.Round(Upper[1], mantissa).ToString() + " ").PadRight(padding));

            for (int i = 2; i < Length; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }

            writer.WriteLine();

            for (int i = 2; i < Length; ++i)
            {
                for (int j = 1; j < i - 1; ++j)
                {
                    writer.Write(("0 ").PadRight(padding));
                }

                writer.Write((Math.Round(Lower[i], mantissa).ToString() + " ").PadRight(padding));
                writer.Write((Math.Round(Main[i], mantissa).ToString() + " ").PadRight(padding));
                writer.Write((Math.Round(Upper[i], mantissa).ToString() + " ").PadRight(padding));

                for (int j = i + 1; j < Length; ++j)
                {
                    writer.Write(("0 ").PadRight(padding));
                }

                writer.WriteLine();
            }

            for (int i = 0; i < Length - 2; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }

            writer.Write((Math.Round(Lower[Length], mantissa).ToString() + " ").PadRight(padding));
            writer.Write((Math.Round(Main[Length], mantissa).ToString() + " ").PadRight(padding));
        }
    }
}
