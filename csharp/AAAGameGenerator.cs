using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

using System.IO;

namespace MazeGenerator2
{
    class Program
    {
        static int scale = 8;
        static int ID = 0;

        static void Main(string[] args)
        {

            int width = 64;
            int height = 64;

            Random rand = new Random();

            bool[,] ItemArray = new bool[width + 1, height + 1];

            for (int x = 0; x < width + 1; x++)
            {
                for (int y = 0; y < height + 1; y++)
                {
                    if (rand.Next(10) < 5)
                    {
                        ItemArray[x, y] = true;
                    }
                    else
                    {
                        ItemArray[x, y] = false;
                    }
                }
            }

            StreamWriter writer = new StreamWriter("out.vmf");

            writer.WriteLine(File.ReadAllText("head.txt"));

            Bitmap bmp = new Bitmap(width * scale + 1, height * scale + 1);
            Graphics gra = Graphics.FromImage(bmp);

            gra.Clear(Color.Gray);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ItemArray[x, y] && ItemArray[x + 1, y])
                    {
                        DrawLine(gra, true, x, y, x + 1, y);
                        add_brush(writer, x * scale - 4, y * scale - 4, 0, ((x + 1) - x) * scale + 4, 4, 32);
                    }

                    if (ItemArray[x, y] && ItemArray[x, y + 1])
                    {
                        DrawLine(gra, true, x, y, x, y + 1);
                        add_brush(writer, x, y, 0, x, ((y + 1) - y), 32);
                    }
                }
            }

            List<Point> Buildings = new List<Point>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (ItemArray[x, y] && ItemArray[x + 1, y])
                    {
                        DrawLine(gra, false, x, y, x + 1, y);
                    }

                    if (ItemArray[x, y] && ItemArray[x, y + 1])
                    {
                        DrawLine(gra, false, x, y, x, y + 1);
                    }

                    if (ItemArray[x, y] && ItemArray[x, y + 1] && ItemArray[x + 1, y] && ItemArray[x + 1, y + 1])
                    {
                        Buildings.Add(new Point(x * scale - 3, y * scale - 3));
                    }
                }
            }

            foreach (Point item in Buildings)
            {
                gra.FillRectangle(Brushes.Black, new Rectangle(item.X - 1, item.Y - 1, scale + 4, scale + 4));

                //add_brush(writer, item.X - 1, item.Y - 1, 0, scale + 4, scale + 4, rand.Next(10, 15));
            }

            foreach (Point item in Buildings)
            {
                gra.FillRectangle(Brushes.White, new Rectangle(item, new Size(scale + 2, scale + 2)));
            }

            bmp.Save("out.png");

            writer.WriteLine(File.ReadAllText("foot.txt"));

            writer.Close();
        }

        static void add_brush(StreamWriter writer, int x0, int y0, int z0, int x1, int y1, int z1)
        {
            if (x0 > x1)
            {
                add_brush(writer, x1, y0, z0, x0, y1, z1);
                return;
            }
            if (y0 > y1)
            {
                add_brush(writer, x0, y1, z0, x1, y0, z1);
                return;
            }
            if (z0 > z1)
            {
                add_brush(writer, x0, y0, z1, x1, y1, z0);
                return;
            }
            writer.Write("solid\n{\n");
            writer.Write("\"id\" \"" + (ID++).ToString() + "\"\n");
            add_side(writer, -x0 * 16, y1 * 16, z1 * 16, x1 * 16, y1 * 16, z1 * 16, x1 * 16, -y0 * 16, z1 * 16, "dev/dev_blendmeasure2");
            add_side(writer, -x0 * 16, -y0 * 16, z0 * 16, x1 * 16, -y0 * 16, z0 * 16, x1 * 16, y1 * 16, z0 * 16, "dev/dev_blendmeasure2");
            add_side(writer, -x0 * 16, y1 * 16, z1 * 16, -x0 * 16, -y0 * 16, z1 * 16, -x0 * 16, -y0 * 16, z0 * 16, "dev/dev_blendmeasure2");
            add_side(writer, x1 * 16, y1 * 16, z0 * 16, x1 * 16, -y0 * 16, z0 * 16, x1 * 16, -y0 * 16, z1 * 16, "dev/dev_blendmeasure2");
            add_side(writer, x1 * 16, y1 * 16, z1 * 16, -x0 * 16, y1 * 16, z1 * 16, -x0 * 16, y1 * 16, z0 * 16, "dev/dev_blendmeasure2");
            add_side(writer, x1 * 16, -y0 * 16, z0 * 16, -x0 * 16, -y0 * 16, z0 * 16, -x0 * 16, -y0 * 16, z1 * 16, "dev/dev_blendmeasure2");
            writer.Write("}\n");
        }

        static void add_side(StreamWriter writer, int x0, int y0, int z0, int x1, int y1, int z1, int x2, int y2, int z2, string material)
        {
            writer.WriteLine("side");
            writer.Write("{\n");
            writer.Write("\"id\" \"" + (ID++).ToString() + "\"\n");
            writer.WriteLine("\"plane\" \"(" + x0.ToString() + " " + y0.ToString() + " " + z0.ToString() + ") (" +
                                x1.ToString() + " " + y1.ToString() + " " + z1.ToString() + ") (" + x2.ToString() + " " + y2.ToString() + " " + z2.ToString() + ")\"\n");
            writer.WriteLine("\"material\" \"" + material + "\"\n");
            writer.WriteLine("\"uaxis\" \"[1 0 0 0] 0.25\"\n");
            writer.WriteLine("\"vaxis\" \"[0 -1 0 0] 0.25\"\n");
            writer.WriteLine("\"rotation\" \"0\"\n");
            writer.WriteLine("\"lightmapscale\" \"16\"\n");
            writer.WriteLine("}\n");
        }

        static void DrawLine(Graphics gra, bool outline, int x, int y, int x2, int y2)
        {
            if (outline)
            {
                gra.FillRectangle(Brushes.Black, new Rectangle(x * scale - 4, y * scale - 4, (x2 - x) * scale + 4, (y2 - y) * scale + 4));
            }
            else
            {
                gra.FillRectangle(Brushes.Red, new Rectangle(x * scale - 3, y * scale - 3, (x2 - x) * scale + 2, (y2 - y) * scale + 2));
            }
        }
    }
}
