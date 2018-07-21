using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KonsolaKomunikacyjnaWpf
{
    public class TerminalModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> listaterminal;
        private List<string> odebrane;
        private List<string> wartosci;
        private string wiadomosc;
        private int terminalindex = 0;
        private int tab;

        private bool odswiez;

        private bool cr;
        private bool lf;

        public ObservableCollection<string> Listaterminal
        {
            get { return listaterminal; }
            set
            {
                if (listaterminal != value)
                {
                    listaterminal = value;
                    OnPropertyChanged("Listaterminal");
                }
                
            }
        }

        public List<string> Wartosci
        {
            get { return wartosci; }
            set
            {
                if (wartosci != value)
                {
                    wartosci = value;
                    OnPropertyChanged("Wartosci");
                }

            }
        }

        public List<string> Odebrane
        {
            get { return odebrane; }
            set
            {
                if (odebrane != value)
                {
                    odebrane = value;
                    OnPropertyChanged("Odebrane");
                }

            }
        }

        public string Wiadomosc
        {
            get { return wiadomosc; }
            set
            {
                if(wiadomosc != value)
                {
                    wiadomosc = value;
                    OnPropertyChanged("Wiadomosc");
                }
            }
        }

        public bool CR
        {
            get { return cr; }
            set
            {
                if (cr != value)
                {
                    cr = value;
                    OnPropertyChanged("CR");
                }
            }
        }

        public bool LF
        {
            get { return lf; }
            set
            {
                if (lf != value)
                {
                    lf = value;
                    OnPropertyChanged("LF");
                }
            }
        }

        public int Tab
        {
            get { return tab; }
            set
            {
                if (tab != value)
                {
                    tab = value;
                    OnPropertyChanged("Tab");
                }
            }
        }


        public TerminalModel()
        {
            Tab = 0;
            LF = true;
            CR = true;
            odswiez = false;
            Wartosci = new List<string>();
            Odebrane = new List<string>();
            Listaterminal = new ObservableCollection<string>();
            Listaterminal.CollectionChanged += Listaterminal_CollectionChanged;
            TerminalAdd();
        }

        public void WyslijWiadomosc(string wiadomosc, SerialPort com)
        {
            if (CR == true)
                wiadomosc += "\r";
            if (LF == true)
                wiadomosc += "\n";
            com.Write(wiadomosc);
            TerminalOdswiezanie("TX: " + wiadomosc);
            Wiadomosc = "";
        }

        public void WyczyscTerminal()
        {
            Listaterminal.Clear();
            TerminalAdd();
            terminalindex = 0;
        }

        public void Odbierz(SerialPort com, bool rejestruj)
        {

            try
            {
                //Odczytaj i roznych wiadomosci z portu szeregowego
                for (int i = 0; i < 1000; i++)
                {
                    string odb = com.ReadLine();
                    if (odb != null)
                    {

                        Odebrane.Add(odb);
                        if(rejestruj == true)
                            Wartosci.Add(odb);
                        odswiez = true;



                    }
                }

            }
            catch { }

            //Pokaz 12 ostatnich wiadomosci
            if (odswiez == true)
            {
                for (int i = 12; i >= 0; i--)
                    if (Odebrane.ElementAtOrDefault(Odebrane.Count() - 1 - i) != null)
                    {
                        TerminalOdswiezanie("RX: " + Odebrane.ElementAtOrDefault(Odebrane.Count() - 1 - i));
                    }
                odswiez = false;
            }

            Odebrane.Clear();




        }
        private void TerminalOdswiezanie(string wiadomosc)
        {
            Listaterminal[terminalindex] = wiadomosc;
            terminalindex++;
            if (terminalindex > 12)
                terminalindex = 0;
        }

        private void TerminalAdd()
        {

            for(int i = 0; i < 14; i++)
            {
                Listaterminal.Add("");
            }
        }

        public void Test(SerialPort com)
        {
            WyczyscTerminal();
            CR = true;
            LF = true;
            WyslijWiadomosc(":info", com);
            Tab = 1;
        }

        public void WyczyscBufor()
        {
            Wartosci.Clear();
        }
          

        private void Listaterminal_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Listaterminal");
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
