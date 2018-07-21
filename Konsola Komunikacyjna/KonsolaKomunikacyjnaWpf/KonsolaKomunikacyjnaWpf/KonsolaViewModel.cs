using System.Collections.Generic;

namespace KonsolaKomunikacyjnaWpf
{
    using Microsoft.Win32;
    using OxyPlot;
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    public class KonsolaViewModel : INotifyPropertyChanged
    {
        private PolaczenieModel _pmodel;
        public PolaczenieModel Pmodel
        {
            get { return _pmodel; }
            set
            {
                _pmodel = value;
                OnPropertyChanged("Pmodel");
            }
        }

        private TerminalModel _tmodel;
        public TerminalModel Tmodel
        {
            get { return _tmodel; }
            set
            {
                _tmodel = value;
                OnPropertyChanged("Tmodel");
            }
        }

        private PrzebiegModel _przmodel;
        public PrzebiegModel Przmodel
        {
            get { return _przmodel; }
            set
            {
                _przmodel = value;
                OnPropertyChanged("Przmodel");
            }
        }

        private string przyciskpolacz;
        public string Przyciskpolacz
        {
            get { return przyciskpolacz; }
            set
            {
                if (przyciskpolacz != value)
                {
                    przyciskpolacz = value;
                    OnPropertyChanged("Przyciskpolacz");
                }
            }
        }

        private FFTModel _fftmodel;
        public FFTModel Fftmodel
        {
            get { return _fftmodel; }
            set
            {
                _fftmodel = value;
                OnPropertyChanged("Fftmodel");
            }
        }

        private string tytulokna;
        public string Tytulokna
        {
            get { return tytulokna; }
            set
            {
                if (tytulokna != value)
                {
                    tytulokna = value;
                    OnPropertyChanged("Tytulokna");
                }
            }
        }

        Thread skanujcom;

        public ICommand PolaczCmd { get; set; }
        public ICommand WyczyscCmd { get; set; }
        public ICommand WyslijTerminal { get; set; }
        public ICommand TestCmd { get; set; }
        public ICommand StartCmd { get; set; }
        public ICommand ZapiszCmd { get; set; }
        public ICommand UstawieniaCmd { get; set; } 
        public ICommand PokazTerminalCmd { get; set; }
        public ICommand UstawieniaPrzebiegCmd { get; set; }
        public ICommand AnalizaCmd { get; set; }
        public ICommand FFTCmd { get; set; }
        public ICommand CofnijPlotCmd { get; set; }

        public KonsolaViewModel()
        {
            Tytulokna = "Konsola komunikacyjna STM32 - Niepołączony";
            Przyciskpolacz = "Połącz";

            Pmodel = new PolaczenieModel ();
            Tmodel = new TerminalModel();
            Przmodel = new PrzebiegModel();
            Fftmodel = new FFTModel();

            PolaczCmd = new RelayCommand(pars => Polacz());
            WyczyscCmd = new RelayCommand(pars => Wyczysc());
            WyslijTerminal = new RelayCommand(pars => Wyslij());
            TestCmd = new RelayCommand(pars => Testuj());
            StartCmd = new RelayCommand(pars => Start());
            ZapiszCmd = new RelayCommand(pars => Zapisz());
            UstawieniaCmd = new RelayCommand(pars => Ustaw());
            PokazTerminalCmd = new RelayCommand(pars => PokazTerminal());
            UstawieniaPrzebiegCmd = new RelayCommand(pars => UstawPrzebieg());
            AnalizaCmd = new RelayCommand(pars => Analiza());
            FFTCmd = new RelayCommand(pars => Fft());
            CofnijPlotCmd = new RelayCommand(pars => Cofnij());
        }

        private void Polacz()
        {
            Pmodel.Polacz_port();


            if (Pmodel.Polaczony == true)
            {
                Tytulokna = "Konsola komunikacyjna STM32 - Połączony " + Pmodel.Wybranyport;
                skanujcom = new Thread(SkanujPort);
                skanujcom.Start();
                Przyciskpolacz = "Rozłącz";
            }
            else
            {
                Tytulokna = "Konsola komunikacyjna STM32 - Niepołączony";
                Przyciskpolacz = "Połącz";
                Tmodel.Tab = 0;
            }
            
        }

        private void Wyczysc()
        {
            Tmodel.WyczyscTerminal();
            Tmodel.WyczyscBufor();
        }

        private void Wyslij()
        {
            Tmodel.WyslijWiadomosc(Tmodel.Wiadomosc, Pmodel.stmcom);
        }

        private void Testuj()
        {
            Tmodel.Test(Pmodel.stmcom);
        }

        private void Start()
        {
            Fftmodel.Plottab = 0;
            if(Przmodel.Stop == true)
            {
                Pmodel.WyczyscBufor();
                Przmodel.Startprzycisk = "Stop";
                Przmodel.Stop = false;
                Przmodel.UsunPunkty();
                Przmodel.Resetprobka();

                Tmodel.WyslijWiadomosc(":channel", Pmodel.stmcom);
                Tmodel.WyslijWiadomosc(Przmodel.WlaczoneKanaly(), Pmodel.stmcom);

                Tmodel.WyslijWiadomosc(":sampletime", Pmodel.stmcom);
                Tmodel.WyslijWiadomosc(Przmodel.ParsujSampleTime(Przmodel.Wybranysampletime), Pmodel.stmcom);

                if (Przmodel.Wybranarejestracja == "Online")
                {
                    Tmodel.WyslijWiadomosc(":start", Pmodel.stmcom);
                }
                else if(Przmodel.Wybranarejestracja == "Przerwanie")
                {
                    Tmodel.WyslijWiadomosc(":pstart", Pmodel.stmcom);
                    Tmodel.WyslijWiadomosc(Przmodel.Dzielnik.ToString(), Pmodel.stmcom);
                }
                else if (Przmodel.Wybranarejestracja == "DMA")
                {
                    Tmodel.WyslijWiadomosc(":dmastart", Pmodel.stmcom);
                }

                Przmodel.Rejestruj = true;
            }
            else
            {
                Przmodel.Startprzycisk = "Start";
                
                Tmodel.WyslijWiadomosc(":stop", Pmodel.stmcom);
                Tmodel.WyslijWiadomosc(":stop", Pmodel.stmcom);
                Przmodel.Rejestruj = false;
                Pmodel.WyczyscBufor();
            }
            
        }

        private void Zapisz()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Pliki tekstowe (*.txt)|*.txt";
            if(dialog.ShowDialog() == true)
            {
                if(Fftmodel.Plottab == 0)
                    Przmodel.ZapiszDoPliku(new Uri(dialog.FileName), "t[us]", "ADC[V]");
                else if(Fftmodel.Plottab == 1)
                    Przmodel.ZapiszDoPliku(new Uri(dialog.FileName), Fftmodel.Punkty, "Transformata Fouriera", "f[Hz]", "Skladowa[V]");
            }
        }

        private void Ustaw()
        {
            Tmodel.Tab = 0;
        }

        private void PokazTerminal()
        {
            Tmodel.Tab = 1;
        }
        private void UstawPrzebieg()
        {
            Tmodel.Tab = 2;
        }

        private void Analiza()
        {
            Tmodel.Tab = 3;
        }

        private void Fft()
        {
            if(Przmodel.Wybranyanalizakanal == "CH1")
                Fftmodel.WykonajFFT(Przmodel.Ch1, Przmodel.Czestotliwoscprobkowania);
            else if(Przmodel.Wybranyanalizakanal == "CH2")
                Fftmodel.WykonajFFT(Przmodel.Ch2, Przmodel.Czestotliwoscprobkowania);
            else if(Przmodel.Wybranyanalizakanal == "CH3")
                Fftmodel.WykonajFFT(Przmodel.Ch3, Przmodel.Czestotliwoscprobkowania);
            else if (Przmodel.Wybranyanalizakanal == "CH4")
                Fftmodel.WykonajFFT(Przmodel.Ch4, Przmodel.Czestotliwoscprobkowania);
            else { }

            Fftmodel.Plottab = 1;
        }
        private void Cofnij()
        {
            Fftmodel.Plottab = 0;
        }

        private void SkanujPort()
        {
            while (true)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    
                    Tmodel.Odbierz(Pmodel.stmcom, Przmodel.Rejestruj);

                    if (Przmodel.Rejestruj && !Przmodel.Stop)
                    {
                        Przmodel.Przebieg.InvalidatePlot(true);
                        Przmodel.Przebieg.Title = "Pobieram...";
                        Przmodel.ParsujPunkty(Tmodel.Wartosci);
                    }

                    else if (!Przmodel.Rejestruj && !Przmodel.Stop)
                    {
                        Przmodel.Przebieg.Title = "Sygnał ADC";
                        Przmodel.Przebieg.InvalidatePlot(true);
                        Przmodel.Stop = true;
                        Tmodel.WyslijWiadomosc(":stop", Pmodel.stmcom);
                        Tmodel.WyslijWiadomosc(":stop", Pmodel.stmcom);

                        Przmodel.Startprzycisk = "Start";
                    }
                    Tmodel.WyczyscBufor();
                }));
                System.Threading.Thread.Sleep(50);

                



                if (Pmodel.Polaczony != true)
                    return;

            }  
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


    }
}
