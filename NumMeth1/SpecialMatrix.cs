using System;
using System.IO;

namespace NumMeth1
{
    public class SpecialMatrix
    {
        public Vector Main { get; set; }

        public Vector Lower { get; set; }

        public Vector Upper { get; set; }

        public Vector Vertical { get; set; }

        public int K {  get; set; }

        public int Length { get => Main.Length; }

        private void MemoryAllocation(int size)
        {
            Main = new Vector(size);
            Lower = new Vector(size);
            Upper = new Vector(size);
            Vertical = new Vector(size);
        }

        public SpecialMatrix() {}

        public SpecialMatrix(int size, int k)
        {
            MemoryAllocation(size);
            K = k;
        }

        public SpecialMatrix(Vector lower, Vector main, Vector upper, Vector vertical, int k)
        {
            Lower = lower;
            Main = main;
            Upper = upper;
            Vertical = vertical;
            K = k;

            Harmonize();
        }

        public void Harmonize()
        {
            //Vertical[Length - K] = Upper[Length - K];
            //Vertical[Length - K + 1] = Main[Length - K + 1];
            //Vertical[Length - K + 2] = Lower[Length - K + 2];

            Upper[Length - K] = Vertical[Length - K];
            Main[Length - K + 1] = Vertical[Length - K + 1];
            Lower[Length - K + 2] = Vertical[Length - K + 2];
        }

        public static Vector operator*(SpecialMatrix matrix, Vector vector)
        {
            if (matrix.Main.Length != vector.Length)
            {
                throw new ArgumentException("Матрица и вектор должны быть одинакового размера!");
            }

            Vector result = new Vector(matrix.Main.Length);

            int size = matrix.Length;

            for (int i = 1; i <= size; ++i)
            {
                result[i] = matrix.Vertical[i] * vector[matrix.K];

                if (i != size - matrix.K + 1)
                {
                    result[i] += matrix.Main[i] * vector[size - i + 1]; // Главная диагональ
                }

                if (i > 1 && i != size - matrix.K + 2)
                {
                    result[i] += matrix.Lower[i] * vector[size - i + 2]; // Нижняя диагональ
                }

                if (i < size && i != size - matrix.K)
                {
                    result[i] += matrix.Upper[i] * vector[size - i]; // Верхняя диагональ
                }
            }
            return result;
        }

        public void FillRandom(double low, double high, bool isWellCond = true)
        {
            Vertical.FillRandom(low, high);

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

            Harmonize();
        }

        public void Fill(StreamReader reader)
        {
            string line = reader.ReadLine();

            MemoryAllocation(Convert.ToInt32(line));

            line = reader.ReadLine();
            var str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            int j = 1;
            foreach (string s in str)
            {
                if (Convert.ToDouble(s) != 0)
                {
                    K = j;
                    break;
                }
                ++j;
            }

            Vertical[1] = Convert.ToDouble(str[K - 1]);
            Upper[1] = Convert.ToDouble(str[Length - 2]);
            Main[1] = Convert.ToDouble(str[Length - 1]);

            for (int i = 2; i <= Length - 1; i++)
            {
                line = reader.ReadLine();
                str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                Vertical[i] = Convert.ToDouble(str[K - 1]);
                Upper[i] = Convert.ToDouble(str[Length - i - 1]);
                Main[i] = Convert.ToDouble(str[Length - i]);
                Lower[i] = Convert.ToDouble(str[Length - i + 1]);
            }

            line = reader.ReadLine();
            str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            Main[Length] = Convert.ToDouble(str[0]);
            Lower[Length] = Convert.ToDouble(str[1]);
            Vertical[Length] = Convert.ToDouble(str[K - 1]);

            reader.ReadLine();
        }

        public void Print(StreamWriter writer, int mantissa = 0, int padding = 5)
        {
            Harmonize();

            for (int i = 1; i < K; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }

            writer.Write((Math.Round(Vertical[1], mantissa).ToString() + " ").PadRight(padding));

            for (int i = K + 1; i < Length - 1; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }

            writer.Write((Math.Round(Upper[1], mantissa).ToString() + " ").PadRight(padding));
            writer.Write((Math.Round(Main[1], mantissa).ToString() + " ").PadRight(padding));

            writer.WriteLine();

            int min;

            for (int i = 2; i < Length; ++i)
            {
                min = Math.Min(Length - i, K);

                for (int j = 1; j < min; ++j)
                {
                    writer.Write(("0 ").PadRight(padding));
                }

                if (min != K)
                {
                    writer.Write((Math.Round(Upper[i], mantissa).ToString() + " ").PadRight(padding));
                    writer.Write((Math.Round(Main[i], mantissa).ToString() + " ").PadRight(padding));
                    writer.Write((Math.Round(Lower[i], mantissa).ToString() + " ").PadRight(padding));

                    if (min + 3 <= K)
                    {
                        for (int j = min + 3; j < K; ++j)
                        {
                            writer.Write(("0 ").PadRight(padding));
                        }

                        writer.Write((Math.Round(Vertical[i], mantissa).ToString() + " ").PadRight(padding));

                        for (int j = K + 1; j <= Length; ++j)
                        {
                            writer.Write(("0 ").PadRight(padding));
                        }
                    }
                    else
                    {
                        for (int j = min + 3; j <= Length; ++j)
                        {
                            writer.Write(("0 ").PadRight(padding));
                        }
                    }

                    writer.WriteLine();
                }
                else
                {
                    if (i < Length - K || i > Length - K + 2)
                    {
                        writer.Write((Math.Round(Vertical[i], mantissa).ToString() + " ").PadRight(padding));
                    }

                    for (int j = K + 1; j < Length - i; ++j)
                    {
                        writer.Write(("0 ").PadRight(padding));
                    }

                    writer.Write((Math.Round(Upper[i], mantissa).ToString() + " ").PadRight(padding));
                    writer.Write((Math.Round(Main[i], mantissa).ToString() + " ").PadRight(padding));
                    writer.Write((Math.Round(Lower[i], mantissa).ToString() + " ").PadRight(padding));

                    for (int j = Length - i + 3; j <= Length; ++j)
                    {
                        writer.Write(("0 ").PadRight(padding));
                    }

                    writer.WriteLine();
                }
            }

            writer.Write((Math.Round(Main[Length], mantissa).ToString() + " ").PadRight(padding));
            writer.Write((Math.Round(Lower[Length], mantissa).ToString() + " ").PadRight(padding));

            for (int i = 3; i < K; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }

            writer.Write((Math.Round(Vertical[Length], mantissa).ToString() + " ").PadRight(padding));

            for (int i = K + 1; i <= Length; ++i)
            {
                writer.Write(("0 ").PadRight(padding));
            }
        }
    }
}
