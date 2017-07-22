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
    class A : IEnumerable<int>
    {
        public List<int> MyList = new List<int>();
        public IEnumerator<int> GetEnumerator()
        {
            foreach (int item in MyList)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<int> Where(Predicate<int> pred)
        {
            return MyList.Where(item => pred(item));
        }
    }
    class Program
    {
        static  void Main(string[] args)
        {
            A a = new A();
            a.MyList.Add(10);
            a.MyList.Add(5);

            foreach (var item in a.Where(x => x == 10))
            {
                Console.WriteLine(item);
            }
        }
    }
}
