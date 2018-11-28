using System;

namespace Edge_Collapse
{
    class Program
    {
        public static void Main(string[] args)
        {
            ObjHandler handler = new ObjHandler();
            handler.LoadVertices(@"C:\Users\HOLOARCH2\Desktop\EdgeDecimator\bunny.obj");
            EdgeCollapser.Run(10000, 7);
            handler.WriteObj();
            Console.WriteLine(Globals.FACES.Count);
        }
    }
}
