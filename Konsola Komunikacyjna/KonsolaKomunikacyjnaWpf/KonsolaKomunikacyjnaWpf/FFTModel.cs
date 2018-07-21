using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace KonsolaKomunikacyjnaWpf 
{
    public class FFTModel : INotifyPropertyChanged
    {
        public LineSeries Punkty { get; private set; }
        public PlotModel Fftplot { get; set; }
        private LinearAxis pionowa;
        private LinearAxis pozioma;
        private List<Complex32> probkifourier;
        private int plottab;

        public int Plottab
        {
            get { return plottab; }
            set
            {
                if (plottab != value)
                {
                    plottab = value;
                    OnPropertyChanged("Plottab");
                }
            }
        }

        public FFTModel()
        {
            Plottab = 0;
            Punkty = new LineSeries();
            Fftplot = new PlotModel();
            pozioma = new LinearAxis();
            pionowa = new LinearAxis();
            probkifourier = new List<Complex32>();

            Fftplot.TextColor = OxyColors.White;
            Fftplot.DefaultFont = "Century Gothic";
            Fftplot.PlotAreaBorderColor = OxyColors.White;
            Fftplot.Title = "Transformata Fouriera";

            pozioma.TicklineColor = OxyColors.White;
            pozioma.Position = AxisPosition.Bottom;
            pozioma.Title = "f[Hz]";
            pionowa.TicklineColor = OxyColors.White;
            pionowa.Position = AxisPosition.Left;

            Fftplot.Axes.Add(pozioma);
            Fftplot.Axes.Add(pionowa);
            Fftplot.Series.Add(Punkty);
        }

        public void WykonajFFT(LineSeries probki, double czestprobkowania)
        {
            probkifourier.Clear();
            Punkty.Points.Clear();

            foreach(DataPoint dp in probki.Points)
            {
                probkifourier.Add(new Complex32((float)dp.Y, 0));
            }

            Complex32[] tablicaprobek = new Complex32[probki.Points.Count()];
            tablicaprobek = probkifourier.ToArray();


            Fourier.Forward(tablicaprobek, FourierOptions.NoScaling);
            double[] skala;
            if(czestprobkowania == 0)
                skala = Fourier.FrequencyScale(probki.Points.Count(), 1000000);
            else
                skala = Fourier.FrequencyScale(probki.Points.Count(), czestprobkowania);


            for (int i = 0; i <= probki.Points.Count()/2; i++)
            {   
                if(i<1)
                    Punkty.Points.Add(new DataPoint(skala[i], tablicaprobek[i].Magnitude/probki.Points.Count()));
                else
                    Punkty.Points.Add(new DataPoint(skala[i], 2*tablicaprobek[i].Magnitude / probki.Points.Count()));
            }
            Fftplot.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged = null;
        virtual protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}
