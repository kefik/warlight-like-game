using System;
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
            Bitmap bmp = new Bitmap(@"C:\Users\Honza\Documents\Skola\Matfyz\BachelorWork\WarlightLikeGame\WinformsUI\bin\Debug\Maps\WorldImageModified1.png");
            

            // Lock the bitmap's bits.  
            BitmapData bmpData =
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

            unsafe
            {
                byte* ptr = (byte*) bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;


                for (int i = 0; i < bytes; i += 3)
                {
                    byte* blue = ptr;
                    byte* green = ptr + 1;
                    byte* red = ptr + 2;

                    if (*red == *green && *green == *blue && *red > 170)
                    {
                        *red = 155;
                        *green = 150;
                        *blue = 122;
                    }
                    ptr += 3;
                }
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            bmp.Save(@"C:\Users\Honza\Documents\Skola\Matfyz\BachelorWork\WarlightLikeGame\WinformsUI\bin\Debug\Maps\WorldImageModified2.png");
            
        }
    }
}
