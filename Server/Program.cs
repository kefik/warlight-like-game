using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GameObjectsLib;
using GameObjectsLib.Game;
using GameObjectsLib.GameMap;

namespace Server
{
    [Serializable]
    class A : ISerializable 
    {
        public A() { }
        public int Id { get; set; }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Id), Id);
        }

        private A(SerializationInfo info, StreamingContext context)
        {
            Id = (int)info.GetValue(nameof(Id), typeof(int));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var formatter = new BinaryFormatter();
            A a = new A()
            {
                Id = 10
            };

            FileStream s = new FileStream("a.txt", FileMode.Create);
            formatter.Serialize(s, a);
            s.Close();

            FileStream fs = new FileStream("a.txt", FileMode.Open);
            A newA = (A)formatter.Deserialize(fs);
            fs.Close();

            Console.WriteLine(newA.Id);
        }
    }
}
