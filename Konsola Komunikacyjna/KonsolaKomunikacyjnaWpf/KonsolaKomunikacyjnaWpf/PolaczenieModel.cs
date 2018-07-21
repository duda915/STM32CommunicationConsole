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
    

    public class PolaczenieModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> port;
        private ObservableCollection<string> com_baud;
        private ObservableCollection<string> com_dlugosc;
        private ObservableCollection<string> com_parity;
        private ObservableCollection<string> com_stop;
        private ObservableCollection<string> com_handshake;

        private string wybranyport;
        private string wybranybaud;
        private string wybraneparity;
        private string wybranadlugosc;
        private string wybranystop;
        private string wybranyhandshake;
        private bool polaczony;

        public SerialPort stmcom { get; private set; }

        public ObservableCollection<string> Port
        {
            get { return port; }
            set
            {
                if(port != value)
                {
                    port = value;
                    OnPropertyChanged("Port");
                }
            }
        }

        public ObservableCollection<string> Com_baud
        {
            get { return com_baud; }
            set
            {
                if (com_baud != value)
                {
                    com_baud = value;
                    OnPropertyChanged("Com_baud");
                }
            }
        }

        public ObservableCollection<string> Com_dlugosc
        {
            get { return com_dlugosc; }
            set
            {
                if (com_dlugosc != value)
                {
                    com_dlugosc = value;
                    OnPropertyChanged("Com_dlugosc");
                }
            }
        }

        public ObservableCollection<string> Com_parity
        {
            get { return com_parity; }
            set
            {
                if (com_parity != value)
                {
                    com_parity = value;
                    OnPropertyChanged("Com_parity");
                }
            }
        }

        public ObservableCollection<string> Com_stop
        {
            get { return com_stop; }
            set
            {
                if (com_stop != value)
                {
                    com_stop = value;
                    OnPropertyChanged("Com_stop");
                }
            }
        }

        public ObservableCollection<string> Com_handshake
        {
            get { return com_handshake; }
            set
            {
                if (com_handshake != value)
                {
                    com_handshake = value;
                    OnPropertyChanged("Com_handshake");
                }
            }
        }

        public string Wybranyport
        {
            get { return wybranyport; }
            set
            {
                if(wybranyport != value)
                {
                    wybranyport = value;
                    OnPropertyChanged("Wybranyport");
                }
            }
        }

        public string Wybranybaud
        {
            get { return wybranybaud; }
            set
            {
                if (wybranybaud != value)
                {
                    wybranybaud = value;
                    OnPropertyChanged("Wybranybaud");
                }
            }
        }

        public string Wybraneparity
        {
            get { return wybraneparity; }
            set
            {
                if (wybraneparity != value)
                {
                    wybraneparity = value;
                    OnPropertyChanged("Wybraneparity");
                }
            }
        }

        public string Wybranadlugosc
        {
            get { return wybranadlugosc; }
            set
            {
                if (wybranadlugosc != value)
                {
                    wybranadlugosc = value;
                    OnPropertyChanged("Wybranadlugosc");
                }
            }
        }

        public string Wybranystop
        {
            get { return wybranystop; }
            set
            {
                if (wybranystop != value)
                {
                    wybranystop = value;
                    OnPropertyChanged("Wybranystop");
                }
            }
        }

        public string Wybranyhandshake
        {
            get { return wybranyhandshake; }
            set
            {
                if (wybranyhandshake != value)
                {
                    wybranyhandshake = value;
                    OnPropertyChanged("Wybranyhandshake");
                }
            }
        }

        public bool Polaczony
        {
            get { return polaczony; }
            set
            {
                if (polaczony != value)
                {
                    polaczony = value;
                    OnPropertyChanged("Polaczony");
                }
            }
        }

        public PolaczenieModel()
        {
            Polaczony = false;
            stmcom = new SerialPort();
            PrzypiszBaud();
            PrzypiszDlugosc();
            PrzypiszHandshake();
            PrzypiszStop();
            PrzypiszParity();
            PrzypiszPorty();
        }

        private void PrzypiszBaud()
        {
            Com_baud = new ObservableCollection<string>();
            Com_baud.Add("9600");
            Com_baud.Add("115200");
            Com_baud.Add("230400");
            Com_baud.Add("460800");
            Com_baud.Add("921600");
        }

        private void PrzypiszDlugosc()
        {
            Com_dlugosc = new ObservableCollection<string>();
            Com_dlugosc.Add("8");
            Com_dlugosc.Add("9");

        }

        private void PrzypiszParity()
        {
            Com_parity = new ObservableCollection<string>(Enum.GetNames(typeof(Parity)));
        }

        private void PrzypiszHandshake()
        {
            Com_handshake = new ObservableCollection<string>(Enum.GetNames(typeof(Handshake)));
        }

        private void PrzypiszStop()
        {
            Com_stop = new ObservableCollection<string>(Enum.GetNames(typeof(StopBits)));
        }

        private void PrzypiszPorty()
        {
            Port = new ObservableCollection<string> { };
            foreach (string s in SerialPort.GetPortNames())
            {
                Port.Add(s);
            }
        }

        public void WyczyscBufor()
        {
            stmcom.DiscardInBuffer();
        }

        public void Polacz_port()
        {
            if (Polaczony == false)
            {
                stmcom.PortName = Wybranyport;
                stmcom.BaudRate = int.Parse(Wybranybaud);
                stmcom.Parity = (Parity)Enum.Parse(typeof(Parity), Wybraneparity, true);
                stmcom.DataBits = int.Parse(Wybranadlugosc);
                stmcom.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Wybranystop, true);
                stmcom.Handshake = (Handshake)Enum.Parse(typeof(Handshake), Wybranyhandshake, true);
                stmcom.ReadTimeout = 1;
                stmcom.Open();
                Polaczony = true;
                
            }
            else
            {
                stmcom.Close();
                Polaczony = false;
                
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
