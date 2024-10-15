using System;
using System.IO;

public class Vector
{
    private double[] data;

    public int Length { get => data.Length; }

    private static Random rand = new Random();

    public Vector() {}

    public Vector(double[] src, int size)
    {
        data = new double[size];
        for (int i = 0; i < size; ++i)
        {
            data[i] = src[i];
        }
    }

    public Vector(Vector src)
    {
        int size = src.Length;
        data = new double[size];

        for (int i = 0; i < size; ++i)
        {
            data[i] = src[i];
        }
    }

    public Vector(int size)
    {
        data = new double[size];
    }

    public void FillRandom(double low, double high)
    {
        for (int i = 1; i <= data.Length; ++i)
        {
            this[i] = rand.NextDouble() * (high - low) + low;
        }
    }

    public void FillRandom(Func<Random, int, double> lambda)
    {
        for (int i = 1; i <= data.Length; ++i)
        {
            this[i] = lambda(rand, i);
        }
    }

    public static Vector operator+(Vector v1, Vector v2)
    {
        if (v1.Length != v2.Length)
        {
            throw new ArgumentException("Векторы должны быть одинакового размера!");
        }

        Vector result = new Vector(v1.Length);

        for (int i = 0; i < v1.Length; ++i)
        {
            result.data[i] = v1.data[i] + v2.data[i];
        }

        return result;
    }

    public static Vector operator-(Vector v1, Vector v2)
    {
        if (v1.Length != v2.Length)       
        {
            throw new ArgumentException("Векторы должны быть одинакового размера!");
        }

        Vector result = new Vector(v1.Length);

        for (int i = 0; i < v1.Length; ++i)
        {
            result.data[i] = v1.data[i] - v2.data[i];
        }

        return result;
    }

    public double Norm()
    {
        double max = Math.Abs(data[1]);

        foreach (var item in data)
        {
            if (Math.Abs(item) > max)
            {
                max = Math.Abs(item);
            }
        }
        return max;
    }

    public double this[int index]
    {
        get { return data[index - 1]; } // Индексация с 1
        set { data[index - 1] = value; } // Индексация с 1
    }

    public void Fill(StreamReader reader, int size)
    {
        data = new double[size];

        string line = reader.ReadLine();

        var str = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

        int i = 0;

        foreach (string s in str)
        {
            data[i] = Convert.ToDouble(s);
            ++i;
        }
    }

    public void Fill()
    {
        data = new double[Console.Read()];

        for (int i = 0; i < data.Length; ++i)
            data[i] = Convert.ToDouble(Console.ReadLine());
    }

    public override string ToString()
    {
        return $"({string.Join(", ", data)})";
    }

    public void Print(StreamWriter writer, bool isWriteSize = false, int mantissa = 4, int padding = 1)
    {
        if (isWriteSize)
            writer.WriteLine(Length);

        for (int i = 0; i < data.Length; i++)
        {
            //writer.Write($"{String.Format("{0:0.0###}", data[i])} ");

            writer.Write((Math.Round(data[i], mantissa).ToString() + " ").PadRight(padding));
        }
        writer.WriteLine();
    }
}
