/*
    Class name:  Program.cs
    Authors:     Danielle Miike, Priscilla Peron, Renato Medeiros
    Date:        April/7/2022
    Description: Host class based on class examples.
 */
using System;
using System.ServiceModel;
using MemoryGameLibrary;
namespace MemoryGameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost servHost = null;
            try
            {
                servHost = new ServiceHost(typeof(MemoryGame));
                servHost.Open();
                Console.WriteLine("Service started. Press any key to quit.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                servHost?.Close();
            }
        }
    }
}
