using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static double ObliczanieDokladnosci(double[][] bazaProbek, double[] noweKlasy)
        {
            int poprawne = 0;

            for (int i = 0; i < noweKlasy.Length; i++)
            {
                double prawdziwaKlasa = bazaProbek[i][bazaProbek[i].Length - 1];
                double przewidzianaKlasa = noweKlasy[i];

                if (!double.IsNaN(przewidzianaKlasa) && przewidzianaKlasa == prawdziwaKlasa)
                {
                    poprawne += 1;
                }
            }
            double dokladnosc = poprawne / (double)noweKlasy.Length * 100;
            return dokladnosc;
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
        public static double Czebyszewa(double[] Probka1, double[] Probka2)
        {
            double max = 0;
            for (int i = 0; i < Probka1.Length; i++)
            {
                double aktualniePrzetwarzany = Math.Abs(Probka1[i] - Probka2[i]);
                if (aktualniePrzetwarzany > max)
                    max = aktualniePrzetwarzany;
            }
            return max;
        }
        public static double Minkowskiego(double[] Probka1, double[] Probka2)
        {
            double p = 3; // jak p > 2 to kładzie większy nacisk na większe różnice
            double suma = 0;
            for (int i = 0; i < Probka1.Length; i++)
            {
                suma += Math.Pow(Math.Abs(Probka1[i] - Probka2[i]), p);
            }
            return Math.Pow(suma, 1.0 / p);
        }
        public static double ZLogarytmem(double[] Probka1, double[] Probka2)
        {
            double suma = 0;
            for (int i = 0; i < Probka1.Length; i++)
            {
                suma += Math.Abs(Math.Log(Probka1[i]) - Math.Log(Probka2[i]));
            }
            return suma;
        }

        public static double KNN(double[][] znormalizowane, int K, double[] probkaTestowa, int i, Metryka M)
        {
            
            List<(int index, double odleglosc)> odleglosci = new List<(int, double)>(); // odleglosci przechowywyane w listach, bo można je łatwo sortować :)

            for (int j = 0; j < znormalizowane.Length; j++)
            {
                if (i == j)//probka testowa pomijana w klasfikacji
                {
                    continue;
                }
                double odleglosc = M(probkaTestowa, znormalizowane[j]);
                odleglosci.Add((j, odleglosc));
            }
            odleglosci = odleglosci.OrderBy(x => x.odleglosc).ToList();//rosnąco, po odległości - bo łatwo wyświetlić i skasyfikować zaczynając od indeksu 0 
           // Console.WriteLine("\nProbka " + i + " : ");
            for (int k = 0; k < K; k++)
            {
                int indexSasiada = odleglosci[k].index;
                double odlegloscSasiada = odleglosci[k].odleglosc;
                //Console.WriteLine("Njabliższy sasiad " + (k + 1) + ": indeks " + indexSasiada + ", odleglosc = " + odlegloscSasiada + ", klasa = " + znormalizowane[indexSasiada][znormalizowane[indexSasiada].Length - 1]);
            }
            
            var grupy = odleglosci
                .Take(K) //pobiera k-pierwszch elementow z listy
                .Select(x => znormalizowane[x.index].Last())//dla kazdego z nich pobiera klase, Last() pobiera ostatni element z wiersza/probki (no czyli klase), czyli x to klasa  
                .GroupBy(x => x)// grupowanie sasiadow wg ich klasy, powstaja grupy probek, każda grupa dla innej klasy
                .Select(g => new { numerKlasy = g.Key, LiczbaWystapienKlasy = g.Count() })//dla kazdej grupy tworzony jest anonimowy obiekt z dwoma polami: neKlasy i LiczbaWystopienKlasy
                .OrderByDescending(x => x.LiczbaWystapienKlasy)//sortowanie grupy malejąco według liczby sąsiadów w danej klasie, czyli najczesciej wystepujaca klasa idzie na przod listy
                .ThenBy(x => x.numerKlasy)//a jak dwie klasy maja tyle samo wystapien (no remis), to sortuje je rosnąco po numerze klasy, czyli pierwsza będzie klasa o niższym numerze
                .ToList();

            double najczestszaKlasa;
            if (grupy.Count > 1 && grupy[0].LiczbaWystapienKlasy == grupy[1].LiczbaWystapienKlasy)//jesli remis to bedzie oznaczenie Nan
            {
                najczestszaKlasa = double.NaN;
                //Console.WriteLine("Remis, czyli: "+najczestszaKlasa);
            }
            else
            {
                najczestszaKlasa = grupy.First().numerKlasy; // dzieki OrderByDescending na poczatek klasy idzie najliczniejsza klasa
              //  Console.WriteLine("Najczęściej występująca klasa: "+najczestszaKlasa);
            }

            return najczestszaKlasa;
        }
        public static void WyswietlZawartosc(double[][] Baza)
        {
            foreach (double[] element in Baza)
            {
                foreach (double value in element)
                {
                    Console.Write(value + "  ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            
            BazaProbek = PobierzDane("dane.txt");
            //WyswietlZawartosc(BazaProbek);


            double[][] znormalizowane = Normalizacja(BazaProbek);
            //WyswietlZawartosc(znormalizowane);
            int jakaMetryka = 1;
            int K = 0;
            Metryka M = Euklidesowa;
            while (jakaMetryka !=0)
            {
                Console.WriteLine("Wybierz metrykę i zatwierdz enterem:");
                Console.WriteLine("1) metryka Euklidesowa");
                Console.WriteLine("2) metryka Manhatan");
                Console.WriteLine("3) metryka Czebyszewa");
                Console.WriteLine("4) metryka Minkowskiego");
                Console.WriteLine("5) metryka Z logarytmem");
                Console.WriteLine("0) zakończ działanie programu");
                jakaMetryka = int.Parse(Console.ReadLine());
                Console.WriteLine("Podaj parametr K i zatwierdz enterem:");
                K = int.Parse(Console.ReadLine());           
                if (jakaMetryka == 1)
                {
                    M = Euklidesowa;
                }
                else if (jakaMetryka == 2)
                {
                    M = Manhatan;
                }
                else if (jakaMetryka == 3)
                {
                    M = Czebyszewa;
                }
                else if (jakaMetryka == 4)
                {
                    M = Minkowskiego;
                }
                else if (jakaMetryka == 5)
                {
                    M = ZLogarytmem;
                }
                else if (jakaMetryka == 0)
                {
                    continue;
                }
                else
                {
                    Console.WriteLine("Wprowadzono zły numer metryki, kliknij enter aby przejsc dalej");
                    Console.ReadKey();
                    continue;
                }
                //var metryki = typeof(Metryka).GetMethods().Where(m =>
               // m.ReturnType == typeof(double) && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(double[])
                //).ToArray();//nazwa m jest dowolna
                //comboBox1.DataSource = metryki;
                //comboBox1.DisplayMember = "Name";
                //Metryka N = (Metryka)Delegate.CreateDelegate(typeof(N));
                //double wynik = M(znormalizowane[0], znormalizowane[1]);
                //Console.WriteLine("Metryka testowa wynik: "+wynik);
                double[] NoweKlasy = new double[BazaProbek.Length];
                for (int i = 0; i < znormalizowane.Length; i++)
                {
                    double[] probkaTestowa = znormalizowane[i];
                    NoweKlasy[i] = KNN(znormalizowane, K, probkaTestowa, i, M);


                }
                double dokladnosc = ObliczanieDokladnosci(BazaProbek, NoweKlasy);
                Console.WriteLine("dokladnosc: " + dokladnosc + "%\n");
            }
            
            Console.ReadKey();
            //Wnioski:
            //Najskuteczniejszymi metrykami są Minkowskiego dla p = 3 i Czebyszewa, bo obie dają dokładność na poziomie 100 % dla k w zakresie 1 - 97
            //kolejną skuteczną metryką jest "Z Logarytmem" bo dlae dokładność na poziomie 99,33 % dla k = 13 i 16
            //nieco mniej skutecznymi metrykami są Manhatan dająca dokł 95,33 % dla k = 3 i Euklidesowa dająca dokł 96,66 % dla k = 7
        }
        public static double[][] BazaProbek = new double[][]
        {           
        };
    }
}
