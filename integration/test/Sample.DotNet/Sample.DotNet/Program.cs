using System;

namespace Sample.DotNet
{
    class Program
    {
        const string TEXT = "Hello world from (dev) build";
        const int NUMBER = 73;

        static void Main(string[] args)
        {
            Console.WriteLine($"{TEXT} ### {NUMBER}");
        }
    }
}
