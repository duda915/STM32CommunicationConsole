using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KonsolaKomunikacyjnaWpf
{
    public class PrzebiegModel : INotifyPropertyChanged
    {
        
        private string tytul;
        private ObservableCollection<DataPoint> punkty;
        private bool rejestruj;
        private int probka;
        private int ilosc_probek;
        private bool stop;
        private LineSeries ch1;
        private LineSeries ch2;
        private LineSeries ch3;
        private LineSeries ch4;
        private string startprzycisk;
        private double wartoscsrednia;
        private StreamWriter f;
        private List<double> okresprobek;
        private double czestotliwoscprobkowania;
        private bool parzyste;
        private double czas;
        private LinearAxis pozioma;
        private LinearAxis pionowa;

        private int kanaly;

        private bool onch1;
        private bool onch2;
        private bool onch3;
        private bool onch4;

        private ObservableCollection<string> analizakanal;
        private string wybranyanalizakanal;


        private ObservableCollection<string> typrejestracji;

        public ObservableCollection<string> Typrejestracji
        {
            get { return typrejestracji; }
            set
            {
                if (typrejestracji != value)
                {
                    typrejestracji = value;
                    OnPropertyChanged("Typrejestracji");
                }
            }
        }

        public ObservableCollection<string> Analizakanal
        {
            get { return analizakanal; }
            set
            {
                if (analizakanal != value)
                {
                    analizakanal = value;
                    OnPropertyChanged("Analizakanal");
                }
            }
        }

        private ObservableCollection<string> sampletime;

        public ObservableCollection<string> Sampletime
        {
            get { return sampletime; }
            set
            {
                if (sampletime != value)
                {
                    sampletime = value;
                    OnPropertyChanged("Sampletime");
                }
            }
        }

        private string wybranarejestracja;

        public string Wybranarejestracja
        {
            get { return wybranarejestracja; }
            set
            {
                if (wybranarejestracja != value)
                {
                    wybranarejestracja = value;

                    UstawBool();
                    OnPropertyChanged("Wybranarejestracja");
                }
            }
        }

        public string Wybranyanalizakanal
        {
            get { return wybranyanalizakanal; }
            set
            {
                if (wybranyanalizakanal != value)
                {
                    wybranyanalizakanal = value;
                    ObliczSredniaCzestotliwoscProbkowania(Wybranyanalizakanal);
                    ObliczWartoscSrednia(Wybranyanalizakanal);
                    OnPropertyChanged("Wybranyanalizakanal");
                }
            }
        }

        private string wybranysampletime;

        public string Wybranysampletime
        {
            get { return wybranysampletime; }
            set
            {
                if (wybranysampletime != value)
                {
                    wybranysampletime = value;

                    OnPropertyChanged("Wybranysampletime");
                }
            }
        }

        private bool rejestracjaonline;

        public bool Rejestracjaonline
        {
            get { return rejestracjaonline; }
            set
            {
                if (rejestracjaonline != value)
                {
                    rejestracjaonline = value;
                    OnPropertyChanged("Rejestracjaonline");
                }
            }
        }

        private bool rejestracjaprzerwanie;
        public bool Rejestracjaprzerwanie
        {
            get { return rejestracjaprzerwanie; }
            set
            {
                if (rejestracjaprzerwanie != value)
                {
                    rejestracjaprzerwanie = value;
                    OnPropertyChanged("Rejestracjaprzerwanie");
                }
            }
        }

        private UInt32 dzielnik;

        public UInt32 Dzielnik
        {
            get { return dzielnik; }
            set
            {
                if (dzielnik != value)
                {
                    dzielnik = value;
                    OnPropertyChanged("Dzielnik");
                }
            }
        }



        public PlotModel Przebieg { get; private set; }

        public string Tytul
        {
            get { return tytul; }
            set
            {
                if (tytul != value)
                {
                    tytul = value;
                    OnPropertyChanged("Tytul");
                }
            }
        }

        public string Startprzycisk
        {
            get { return startprzycisk; }
            set
            {
                if (startprzycisk != value)
                {
                    startprzycisk = value;
                    OnPropertyChanged("Startprzycisk");
                }
            }
        }

        public ObservableCollection<DataPoint> Punkty
        {
            get { return punkty; }
            set
            {
                if (punkty != value)
                {
                    punkty = value;
                    OnPropertyChanged("Punkty");
                }
            }
        }

        public bool Rejestruj
        {
            get { return rejestruj; }
            set
            {
                if (rejestruj != value)
                {
                    rejestruj = value;
                    OnPropertyChanged("Rejestruj");
                }
            }
        }

        public int Probka
        {
            get { return probka; }
            set
            {
                if (probka != value)
                {
                    probka = value;
                    OnPropertyChanged("Probka");
                }
            }
        }

        public int Ilosc_probek
        {
            get { return ilosc_probek; }
            set
            {
                if (ilosc_probek != value)
                {
                    ilosc_probek = value;
                    OnPropertyChanged("Ilosc_probek");
                }
            }
        }

        public bool Stop
        {
            get { return stop; }
            set
            {
                if (stop != value)
                {
                    stop = value;
                    OnPropertyChanged("Stop");
                }
            }
        }

        public bool Onch1
        {
            get { return onch1; }
            set
            {
                if (onch1 != value)
                {
                    onch1 = value;
                    PokazKanaly();
                    OnPropertyChanged("Onch1");
                }
            }
        }

        public bool Onch2
        {
            get { return onch2; }
            set
            {
                if (onch2 != value)
                {
                    onch2 = value;
                    PokazKanaly();
                    OnPropertyChanged("Onch2");
                }
            }
        }

        public bool Onch3
        {
            get { return onch3; }
            set
            {
                if (onch3 != value)
                {
                    onch3 = value;
                    PokazKanaly();
                    OnPropertyChanged("Onch3");
                }
            }
        }

        public bool Onch4
        {
            get { return onch4; }
            set
            {
                if (onch4 != value)
                {
                    onch4 = value;
                    PokazKanaly();
                    OnPropertyChanged("Onch4");
                }
            }
        }


        public LineSeries Ch1
        {
            get { return ch1; }
            set
            {
                if (ch1 != value)
                {
                    ch1 = value;
                    OnPropertyChanged("Ch1");
                }
            }
        }

        public LineSeries Ch2
        {
            get { return ch2; }
            set
            {
                if (ch2 != value)
                {
                    ch2 = value;
                    OnPropertyChanged("Ch2");
                }
            }
        }

        public LineSeries Ch3
        {
            get { return ch3; }
            set
            {
                if (ch3 != value)
                {
                    ch3 = value;
                    OnPropertyChanged("Ch3");
                }
            }
        }

        public LineSeries Ch4
        {
            get { return ch4; }
            set
            {
                if (ch4 != value)
                {
                    ch4 = value;
                    OnPropertyChanged("Ch4");
                }
            }
        }

        public double Wartoscsrednia
        {
            get { return wartoscsrednia; }
            set
            {
                if (wartoscsrednia != value)
                {
                    wartoscsrednia = value;
                    OnPropertyChanged("Wartoscsrednia");
                }
            }
        }

        public double Czestotliwoscprobkowania
        {
            get { return czestotliwoscprobkowania; }
            set
            {
                if (czestotliwoscprobkowania != value)
                {
                    czestotliwoscprobkowania = value;
                    OnPropertyChanged("Czestotliwoscprobkowania");
                }
            }
        }

        public PrzebiegModel()
        {
            Dzielnik = 1;
            czas = 0;
            parzyste = false;
            Czestotliwoscprobkowania = 0;
            okresprobek = new List<double>();
            Wartoscsrednia = 0;
            Startprzycisk = "Start";
            
            Przebieg = new PlotModel();
            Przebieg.TextColor = OxyColors.White;
            Przebieg.DefaultFont = "Century Gothic";
            Przebieg.PlotAreaBorderColor = OxyColors.White;

            Ch1 = new LineSeries();
            Ch1.Title = "ADC CH1[V]";
            Ch1.Color = OxyColors.Green;
            Ch2 = new LineSeries();
            Ch2.Title = "ADC CH2[V]";
            Ch2.Color = OxyColors.SteelBlue;
            Ch3 = new LineSeries();
            Ch3.Title = "ADC CH3[V]";
            Ch3.Color = OxyColors.Goldenrod;
            Ch4 = new LineSeries();
            Ch4.Title = "ADC CH4[V]";
            Ch4.Color = OxyColors.OrangeRed;


            Przebieg.Series.Add(Ch1);
            Przebieg.Series.Add(Ch2);
            Przebieg.Series.Add(Ch3);
            Przebieg.Series.Add(Ch4);

            Stop = true;
            Ilosc_probek = 1000;
            Probka = 0;
            Rejestruj = false;
            Tytul = "Sygnał ADC";
            Przebieg.Title = Tytul;
            pozioma = new LinearAxis();
            pozioma.Position = AxisPosition.Bottom;
            pozioma.Title = "t[us]";
            pozioma.TicklineColor = OxyColors.White;
            Przebieg.Axes.Add(pozioma);


            pionowa = new LinearAxis();
            pionowa.Position = AxisPosition.Left;
            pionowa.TicklineColor = OxyColors.White;
            Przebieg.Axes.Add(pionowa);
            Punkty = new ObservableCollection<DataPoint>();
            Punkty.CollectionChanged += Punkty_CollectionChanged;

            PrzypiszTypyRejestracji();
            PrzypiszTypySampleTime();
            Dodajkanalyanalizy();
            Rejestracjaonline = true;
            Rejestracjaprzerwanie = false;

            Onch1 = true;
            Onch2 = true;
            Onch3 = true;
            Onch4 = true;
            kanaly = 0;

            Prezentacja();
        }

        public void ParsujPunkty(List<string> ZbiorWartosci)
        {
            double val = 0;
            foreach(string s in ZbiorWartosci)
            {
                if (Probka >= Ilosc_probek - 1)
                {
                    Rejestruj = false;
                    Przebieg.InvalidatePlot(true);
                    ObliczWartoscSrednia(Wybranyanalizakanal);
                    ObliczSredniaCzestotliwoscProbkowania(Wybranyanalizakanal);
                    return;
                }
                try
                {
                    if (s.Substring(0, 1) == "1")
                    {
                        if (Double.TryParse(s.Substring(2), out val))
                        {
                            if (s.Substring(1, 1) == "t")
                            {
                                czas = val;
                            }
                            else if (s.Substring(1, 1) == "s")
                            {
                                DodajPunkt(czas, val * 3.3 / 4095, 1);
                                Probka++;
                            }

                        }
                    }

                    else if (s.Substring(0, 1) == "2")
                    {
                        if (Double.TryParse(s.Substring(2), out val))
                        {
                            if (s.Substring(1, 1) == "t")
                            {
                                czas = val;
                            }
                            else if (s.Substring(1, 1) == "s")
                            {
                                DodajPunkt(czas, val * 3.3 / 4095, 2);
                                Probka++;
                            }

                        }
                    }

                    else if (s.Substring(0, 1) == "3")
                    {
                        if (Double.TryParse(s.Substring(2), out val))
                        {
                            if (s.Substring(1, 1) == "t")
                            {
                                czas = val;
                            }
                            else if (s.Substring(1, 1) == "s")
                            {
                                DodajPunkt(czas, val * 3.3 / 4095, 3);
                                Probka++;
                            }

                        }
                    }
                    else if (s.Substring(0, 1) == "4")
                    {
                        if (Double.TryParse(s.Substring(2), out val))
                        {
                            if (s.Substring(1, 1) == "t")
                            {
                                czas = val;
                            }
                            else if (s.Substring(1, 1) == "s")
                            {
                                DodajPunkt(czas, val * 3.3 / 4095, 4);
                                Probka++;
                            }

                        }
                    }
                }
                catch { }


            }
            
            
        }
            

        public void DodajPunkt(double t, double f, int channel)
        {
            switch (channel)
            {
                case 1:
                    {
                        Ch1.Points.Add(new DataPoint(t, f));
                        break;
                    }
                case 2:
                    {
                        Ch2.Points.Add(new DataPoint(t, f));
                        break;
                    }
                case 3:
                    {
                        Ch3.Points.Add(new DataPoint(t, f));
                        break;
                    }
                case 4:
                    {
                        Ch4.Points.Add(new DataPoint(t, f));
                        break;
                    }
                default:
                    { break; }
            }
           
        }

        public void UsunPunkty()
        {
            Ch1.Points.Clear();
            Ch2.Points.Clear();
            Ch3.Points.Clear();
            Ch4.Points.Clear();
            
        }

        public void Resetprobka()
        {
            Probka = 0;
        }

        private void ObliczWartoscSrednia(string channel)
        {
            double suma = 0;
            if (channel == "CH1")
            {
                foreach (DataPoint dp in Ch1.Points)
                    suma += dp.Y;
                Wartoscsrednia = Math.Round(suma / Ch1.Points.Count(), 3);
            }
            else if (channel == "CH2")
            {
                foreach (DataPoint dp in Ch2.Points)
                    suma += dp.Y;
                Wartoscsrednia = Math.Round(suma / Ch2.Points.Count(), 3);
            }
            else if (channel == "CH3")
            {
                foreach (DataPoint dp in Ch3.Points)
                    suma += dp.Y;
                Wartoscsrednia = Math.Round(suma / Ch3.Points.Count(), 3);
            }
            else if (channel == "CH4")
            {
                foreach (DataPoint dp in Ch4.Points)
                    suma += dp.Y;
                Wartoscsrednia = Math.Round(suma / Ch4.Points.Count(), 3);
            }
        }

        private void ObliczSredniaCzestotliwoscProbkowania(string channel)
        {
            okresprobek.Clear();
            double roznica;
            if (channel == "CH1")
            {
                for (int i = 1; i < Ch1.Points.Count; i++)
                {
                    roznica = Ch1.Points[i].X - Ch1.Points[i - 1].X;
                    okresprobek.Add(roznica);
                }
                Czestotliwoscprobkowania = Math.Round(1 / (okresprobek.Sum() / okresprobek.Count() / 1000000), 3);
            }

            else if (channel == "CH2")
            {
                for (int i = 1; i < Ch2.Points.Count; i++)
                {
                    roznica = Ch2.Points[i].X - Ch2.Points[i - 1].X;
                    okresprobek.Add(roznica);
                }
                Czestotliwoscprobkowania = Math.Round(1 / (okresprobek.Sum() / okresprobek.Count() / 1000000), 3);
            }

            else if (channel == "CH3")
            {
                for (int i = 1; i < Ch3.Points.Count; i++)
                {
                    roznica = Ch3.Points[i].X - Ch3.Points[i - 1].X;
                    okresprobek.Add(roznica);
                }
                Czestotliwoscprobkowania = Math.Round(1 / (okresprobek.Sum() / okresprobek.Count() / 1000000), 3);
            }

            else if (channel == "CH4")
            {
                for (int i = 1; i < Ch4.Points.Count; i++)
                {
                    roznica = Ch4.Points[i].X - Ch4.Points[i - 1].X;
                    okresprobek.Add(roznica);
                }
                Czestotliwoscprobkowania = Math.Round(1 / (okresprobek.Sum() / okresprobek.Count() / 1000000), 3);
            }
        }

        public void ZapiszDoPliku(Uri sciezka, string x, string y)
        {
            f = new StreamWriter(sciezka.AbsolutePath);

            if (Onch1)
            {

                f.WriteLine(Ch1.Title);
                f.WriteLine(x + "\t\t\t" + y);

                foreach (DataPoint dp in Ch1.Points)
                {
                    f.Write("{0}\t\t\t{1}\n", dp.X, dp.Y);
                }
            }

            if (Onch2)
            {

                f.WriteLine(Ch2.Title);
                f.WriteLine(x + "\t\t\t" + y);

                foreach (DataPoint dp in Ch2.Points)
                {
                    f.Write("{0}\t\t\t{1}\n", dp.X, dp.Y);
                }
            }

            if (Onch3)
            {

                f.WriteLine(Ch3.Title);
                f.WriteLine(x + "\t\t\t" + y);

                foreach (DataPoint dp in Ch3.Points)
                {
                    f.Write("{0}\t\t\t{1}\n", dp.X, dp.Y);
                }
            }

            if (Onch4)
            {

                f.WriteLine(Ch4.Title);
                f.WriteLine(x + "\t\t\t" + y);

                foreach (DataPoint dp in Ch4.Points)
                {
                    f.Write("{0}\t\t\t{1}\n", dp.X, dp.Y);
                }
            }


            f.Close();
        }

        public void ZapiszDoPliku(Uri sciezka, LineSeries punkty, string tytul, string x, string y)
        {
            f = new StreamWriter(sciezka.AbsolutePath);

            f.WriteLine(tytul);
            f.WriteLine(x + "\t\t\t" + y);

            foreach (DataPoint dp in punkty.Points)
            {
                f.Write("{0}\t\t\t{1}\n", dp.X, dp.Y);
            }



            f.Close();
        }

        private void PrzypiszTypyRejestracji()
        {
            Typrejestracji = new ObservableCollection<string>();
            Typrejestracji.Add("Online");
            Typrejestracji.Add("Przerwanie");
            Typrejestracji.Add("DMA");
        }

        private void PrzypiszTypySampleTime()
        {
            Sampletime = new ObservableCollection<string>();
            Sampletime.Add("1,5 cykla");
            Sampletime.Add("7,5 cykla");
            Sampletime.Add("13,5 cykla");
            Sampletime.Add("28,5 cykla");
            Sampletime.Add("41,5 cykla");
            Sampletime.Add("55,5 cykla");
            Sampletime.Add("71,5 cykla");
            Sampletime.Add("239,5 cykla");
        }

        private void UstawBool()
        {
            if(Wybranarejestracja == "Online")
            {
                Rejestracjaonline = true;
                Rejestracjaprzerwanie = false;
                Dzielnik = 1;
            }
            else if(Wybranarejestracja == "Przerwanie")
            {
                Rejestracjaonline = false;
                Rejestracjaprzerwanie = true;
                Ilosc_probek = 2000;
            }
            else if (Wybranarejestracja == "DMA")
            {
                Rejestracjaonline = false;
                Rejestracjaprzerwanie = false;
                Dzielnik = 1;
                Ilosc_probek = 5100;
            }
        }

        public string ParsujSampleTime(string s)
        {
            if (s == "1,5 cykla")
                return "1";
            else if (s == "7,5 cykla")
                return "2";
            else if (s == "13,5 cykla")
                return "3";
            else if (s == "28,5 cykla")
                return "4";
            else if (s == "41,5 cykla")
                return "5";
            else if (s == "55,5 cykla")
                return "6";
            else if (s == "71,5 cykla")
                return "7";
            else if (s == "239,5 cykla")
                return "8";
            else
                return "1";
        }

        private void PokazKanaly()
        {
            Przebieg.Series.Clear();
            if (Onch1)
                Przebieg.Series.Add(Ch1);
            if (Onch2)
                Przebieg.Series.Add(Ch2);
            if (Onch3)
                Przebieg.Series.Add(Ch3);
            if (Onch4)
                Przebieg.Series.Add(Ch4);
            Przebieg.InvalidatePlot(true);
        }

        public string WlaczoneKanaly()
        {
            kanaly = 0;
            string s;
            if (Onch1)
                kanaly |= 0b1;
            if (Onch2)
                kanaly |= 0b10;
            if (Onch3)
                kanaly |= 0b100;
            if (Onch4)
                kanaly |= 0b1000;

            s = kanaly.ToString();
            return s;
        }

        private void Prezentacja()
        {
            for(int i = 0; i < 2000; i++)
            {
                double t = i;
                Ch1.Points.Add(new DataPoint(t, Math.Sin(2 * Math.PI * 0.005 * t) + 1 + 0.1 * Math.Sin(2 * Math.PI * 0.05 * t)));
                Ch2.Points.Add(new DataPoint(t, Math.Sin(2 * Math.PI * 0.005 * t) + 1 *0.1 * Math.Sin(2 * Math.PI * 0.05 * t)));
                Ch3.Points.Add(new DataPoint(t, Math.Sin(2 * Math.PI * 0.005 * t) + 2 - 0.1 * Math.Sin(2 * Math.PI * 0.05 * t)));
                Ch4.Points.Add(new DataPoint(t, Math.Sin(2 * Math.PI * 0.005 * t) - 1 + 0.1 * Math.Sin(2 * Math.PI * 0.05 * t)));
            }
        }

        private void Dodajkanalyanalizy()
        {
            Analizakanal = new ObservableCollection<string>();
            Analizakanal.Add("CH1");
            Analizakanal.Add("CH2");
            Analizakanal.Add("CH3");
            Analizakanal.Add("CH4");

        }



        private void Punkty_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Punkty");
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
