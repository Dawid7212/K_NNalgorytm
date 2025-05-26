using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K_NNalgorytm
{
    delegate double Metryka(double[] Probka1, double[] Probka2);
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

        public static double Euklidesowa(double[] Probka1, double[] Probka2)
        {
            double odleglosc=0;
            for (int i =0; i<Probka1.Length-1; i++)
            {
                odleglosc += Math.Pow(Probka1[i] - Probka2[i],2);
            }
            odleglosc = Math.Sqrt(odleglosc);
            return odleglosc;
        }
        public static double Manhatan(double[] Probka1, double[] Probka2)
        {
            double odleglosc = 0;
            for (int i = 0; i < Probka1.Length - 1; i++)
            {
                odleglosc += Math.Abs(Probka1[i] - Probka2[i]);
            }
            return odleglosc;
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
            Metryka m = Euklidesowa;
            double wynik = m(znormalizowane[0], znormalizowane[1]);
            Console.WriteLine("Metryka testowa wynik: "+wynik);
            int K = 8;

            for (int i = 0; i<znormalizowane.Length; i++)
            {
                double[] probkaTestowa = znormalizowane[i];
                List<(int index, double odleglosc)> odleglosci = new List<(int, double)>(); // odleglosci przechowywyane w listach, bo można je łatwo sortować :)

                for (int j = 0; j < znormalizowane.Length; j++)
                {
                    if (znormalizowane[j] == probkaTestowa)//probka testowa pomijana w klasfikacji
                    {
                        continue;
                    }

                    double odleglosc = Euklidesowa(probkaTestowa, znormalizowane[j]);
                    odleglosci.Add((j, odleglosc));
                }

                
                odleglosci = odleglosci.OrderBy(x => x.odleglosc).ToList();//rosnąco, po odległości - bo łatwo wyświetlić i skasyfikować zaczynając od indeksu 0 
                double[] najblizszeSasiady = new double[K];
                Console.WriteLine("\nProbka "+i+" : ");
                for (int k = 0; k < K; k++)
                {
                    int indexSasiada = odleglosci[k].index;
                    double odlegloscSasiada = odleglosci[k].odleglosc;
                    Console.WriteLine("Njabliższy sasiad " + (k + 1) + ": indeks " + indexSasiada + ", odleglosc = " + odlegloscSasiada + ", klasa = " + znormalizowane[indexSasiada][znormalizowane[indexSasiada].Length - 1]);
                    najblizszeSasiady[k] = indexSasiada;
                }

                var grupy = odleglosci
                    .Take(K)
                    .Select(x => znormalizowane[x.index].Last())
                    .GroupBy(x => x)
                    .Select(g => new { Klasa = g.Key, Liczba = g.Count() })
                    .OrderByDescending(x => x.Liczba)
                    .ThenBy(x => x.Klasa)
                    .ToList();

                double najczestszaKlasa;
                if (grupy.Count > 1 && grupy[0].Liczba == grupy[1].Liczba)//jesli remis to bedzie klasa 0
                {
                    najczestszaKlasa = double.NaN;
                    Console.WriteLine("Brak jednoznacznej decyzji (remis)");
                }
                else
                {
                    najczestszaKlasa = grupy.First().Klasa;
                    Console.WriteLine($"Najczęściej występująca klasa: {najczestszaKlasa}");
                }

            }

            Console.ReadKey();
        }
        
        public static double[][] BazaProbek = new double[][]
        {           
        };
    }
}
