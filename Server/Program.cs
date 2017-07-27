using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;

namespace Server
{
    class Program
    {
        static  void Main(string[] args)
        {
            Bitmap bmp = new Bitmap(@"C:\Users\Honza\Documents\Skola\Matfyz\BachelorWork\WarlightLikeGame\WinformsUI\bin\Debug\Maps\WorldTemplate.png");

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            byte[] r = new byte[bytes / 3];
            byte[] g = new byte[bytes / 3];
            byte[] b = new byte[bytes / 3];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            
            int stride = bmpData.Stride;
            
            for (int column = 0; column < bmpData.Height; column++)
            {
                for (int row = 0; row < bmpData.Width; row++)
                {
                    if (rgbValues[(column * stride) + (row * 3) + 2] == 78
                        && rgbValues[(column * stride) + (row * 3) + 1] == 24
                        && rgbValues[(column * stride) + (row * 3)] == 86)
                    {
                        //Console.WriteLine($"Red: {r[count - 1]}, Green: {g[count - 1]}, Blue: {b[count - 1]}");
                        Console.WriteLine($"X: {row}, Y: {column}");
                    }
                }
            }
        }
    }
}
