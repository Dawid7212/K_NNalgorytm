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

            for (int i = 0; i < min.Length; i++)
            {
                min[i] = Probki[0][i];
                max[i] = Probki[0][i];
                for (int s = 1; s < Probki.Length; s++)
                {
                    if (Probki[s][i] < min[i])
                    {
                        min[i] = Probki[s][i];
                    }
                    if (Probki[s][i] > max[i])
                    {
                        max[i] = Probki[s][i];
                    }
                }
            }
            for (int s = 0; s < Probki.Length; s++)
            {
                Znormalizowane[s] = new double[Probki[s].Length];
                for (int j = 0; j < Probki[0].Length - 1; j++)
                {
                    if (max[j] == min[j])//  jak wartości sa tekie same to 0.0
                    {
                        Znormalizowane[s][j] = 0.0;
                    }
                    else
                    {
                        Znormalizowane[s][j] = (Probki[s][j] - min[j]) / (max[j] - min[j]);
                    }    
                }
                Znormalizowane[s][Probki[0].Length - 1] = Probki[s][Probki[0].Length - 1]; //ostania kolumna dla klasy
            }
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

            Console.WriteLine();


            double[][] znormalizowane = Normalizacja(BazaProbek);
            foreach (double[] element in znormalizowane)
            {
                foreach (double value in element)
                {
                    Console.Write(value + "  ");
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
