using Microsoft.Owin.Hosting;
using System;

namespace GoogleSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080/";
            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Server is running - {0}", uri);
                Console.ReadLine();
            }
        }
    }
}
