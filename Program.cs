using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K_NNalgorytm
{
    internal class Program
    {
        public static double[][] PobierzDane(string sciezka)
        {
            double[][] tablicaProb = System.IO.File
            .ReadAllLines(sciezka)
            .Where(linia => !string.IsNullOrWhiteSpace(linia))
            .Select(linia => linia.Split(' ').Select(x => double.Parse(x.Replace('.', ','))).ToArray())
            .ToArray();

            return tablicaProb;
        }


        public static double[][] Normalizacja(double[][] Probki) { 
            double [][] Znormalizowane = new double[Probki.Length][];
            double[] min = new double[Probki[0].Length-1]; //są trzy kolumny wiec trzy minima
            double[] max = new double[Probki[0].Length - 1];

            
            return Znormalizowane;
        }



        static void Main(string[] args)
        {
            BazaProbek = PobierzDane("dane.txt");
            foreach(double[] element in BazaProbek)
            {
                foreach(double value in element)
                {
                    Console.Write(value+"  ");
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
        public static double[][] BazaProbek = new double[][]
        {           
        };
    }
}
