using System;
using System.Collections.Generic;

namespace VbitzUtil
{
    struct MatrixPoint
    {
        public int X;
        public int Y;

        public MatrixPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    class Matrix<T>
    {
        private Dictionary<MatrixPoint, T> Objects = new Dictionary<MatrixPoint, T>();

        private int XSize;
        private int YSize;

        public Matrix(int size)
        {
            this.XSize = size;
            this.YSize = size;
        }

        public Matrix(int xsize, int ysize)
        {
            this.XSize = xsize;
            this.YSize = ysize;
        }

        public void SetValue(int x, int y, T value)
        {
            this.CheckBounds(x, y);
            MatrixPoint search = new MatrixPoint(x, y);
            if (Objects.ContainsKey(search))
            {
                Objects[search] = value;
            }
            else
            {
                Objects.Add(search, value);
            }
        }

        private void CheckBounds(int x, int y)
        {
            if (this.XSize < x || this.XSize < 0)
            {
                throw new ArgumentOutOfRangeException("X needs to be less then XSize");
            }
            if (this.YSize < y || this.YSize < 0)
            {
                throw new ArgumentOutOfRangeException("Y needs to be less then YSize");
            }
        }

        public T GetValue(int x, int y)
        {
            this.CheckBounds(x, y);
            MatrixPoint search = new MatrixPoint(x,y);
            if (Objects.ContainsKey(search))
            {
                return Objects[search];
            }
            else
            {
                return default(T);
            }
        }

        public void Extend(int x, int y)
        {
            this.XSize += x;
            this.YSize += y;
        }

        public void Extend(int size)
        {
            this.XSize += size;
            this.YSize += size;
        }

        public List<List<T>> IterateValues()
        {
            List<List<T>> returnValues = new List<List<T>>();

            for (int x = 0; x < this.XSize; x++)
            {
                List<T> values = new List<T>();
                for (int y = 0; y < this.YSize; y++)
                {
                    values.Add(this.GetValue(x, y));
                }
                returnValues.Add(values);
            }

            return returnValues;
        }
    }
}