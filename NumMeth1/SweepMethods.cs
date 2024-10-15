using System;
using System.IO;

namespace NumMeth1
{
    public class SweepMethods
    {
        public static Vector ClassicalShuttle(TridiagonalMatrix matrix, Vector d, out Vector l, out Vector m)
        {
            if (matrix.Length != d.Length)
                throw new ArgumentException("Матрица и вектор должны быть одинакового размера!");

            int size = matrix.Length;

            m = new Vector(size + 1);
            l = new Vector(size + 1);

            m[1] = 0;
            l[1] = 0;

            double denom;

            for (int i = 1; i <= size; i++)
            {
                denom = matrix.Main[i] - matrix.Lower[i] * l[i];

                if (denom == 0)
                {
                    throw new DivideByZeroException("Попытка деления на ноль!");
                }

                l[i + 1] = matrix.Upper[i] / denom;
                m[i + 1] = (d[i] - matrix.Lower[i] * m[i]) / denom;
            }

            Vector x = new Vector(size);

            x[size] = m[size + 1];

            for (int i = size - 1; i >= 1; --i)
            {
                x[i] = m[i + 1] - l[i + 1] * x[i + 1];
            }

            return x;
        }

        public static Vector SpecialShuttle(SpecialMatrix matrix, Vector f, Vector exactX = null, StreamWriter writer = null)
        {
            Vector x = new Vector(matrix.Length);
            double R;

            // 1 этап

            int stage = 1;

            int rowK = matrix.Length - matrix.K + 1;

            for (int i = 1; i < rowK; ++i)
            {
                R = matrix.Main[i];
                matrix.Main[i] = 1;

                matrix.Upper[i] /= R;
                matrix.Vertical[i] /= R;
                f[i] /= R;

                R = matrix.Lower[i + 1];
                matrix.Lower[i + 1] = 0;

                matrix.Main[i + 1] -= R * matrix.Upper[i];
                matrix.Vertical[i + 1] -= R * matrix.Vertical[i];

                f[i + 1] -= R * f[i];
            }

            if (exactX != null)
            {
                writer.WriteLine($"Норма вектора abs(A*x_точн – f) после {stage} этапа: " +
                    $"{String.Format("{0:0.0###e+00}", (matrix * exactX - f).Norm())}\n");
            }

            // 2 этап

            ++stage;

            for (int i = matrix.Length; i > rowK; --i)
            {
                R = matrix.Main[i];
                matrix.Main[i] = 1;

                matrix.Lower[i] /= R;
                matrix.Vertical[i] /= R;
                f[i] /= R;

                R = matrix.Upper[i - 1];
                matrix.Upper[i - 1] = 0;

                matrix.Main[i - 1] -= R * matrix.Lower[i];
                matrix.Vertical[i - 1] -= R * matrix.Vertical[i];

                f[i - 1] -= R * f[i];             
            }

            if (exactX != null)
            {
                writer.WriteLine($"Норма вектора abs(A*x_точн – f) после {stage} этапа: " +
                    $"{String.Format("{0:0.0###e+00}", (matrix * exactX - f).Norm())}\n");
            }

            // 3 этап

            ++stage;

            R = matrix.Vertical[rowK];
            matrix.Vertical[rowK] = 1;
            f[rowK] /= R;

            for (int i = 1; i < rowK; ++i)
            {
                f[i] -= matrix.Vertical[i] * f[rowK];
                matrix.Vertical[i] = 0;
            }

            for (int i = rowK + 1; i <= matrix.Length; ++i)
            {
                f[i] -= matrix.Vertical[i] * f[rowK];
                matrix.Vertical[i] = 0;
            }

            if (exactX != null)
            {
                writer.WriteLine($"Норма вектора abs(A*x_точн – f) после {stage} этапа: " +
                    $"{String.Format("{0:0.0###e+00}", (matrix * exactX - f).Norm())}\n");
            }

            // 4 этап

            x[matrix.K - 1] = f[rowK + 1];
            x[matrix.K] = f[rowK];
            x[matrix.K + 1] = f[rowK - 1];

            for (int i = rowK - 2; i >= 1; --i)
            {
                x[matrix.Length - i + 1] = f[i] - matrix.Upper[i] * x[matrix.Length - i];
            }

            for (int i = rowK + 2; i <= matrix.Length; ++i)
            {
                x[matrix.Length - i + 1] = f[i] - matrix.Lower[i] * x[matrix.Length - i + 2];
            }

            if (exactX != null)
            {
                writer.WriteLine("\nТочный вектор решения:\n");
                exactX.Print(writer);
                writer.WriteLine("\nВычисленный вектор решения:\n");
                x.Print(writer);
                writer.WriteLine();
            }

            matrix.Harmonize();

            return x;
        }
    }
}
