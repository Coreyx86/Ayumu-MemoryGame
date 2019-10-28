using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ayumu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int numOfNeedlesValue = 3;

        private GameSettings.Difficulty difficulty;

        private string _lastGameStats = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public string lastGameStats {
            get
            {
                return _lastGameStats;
            }
            set
            {
                _lastGameStats = value;
                OnPropertyChanged("lastGameStats");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            GameGrid gameGrid = new GameGrid(new GameSettings
            {
                allowRepeats = (bool)this.bAllowRepeats.IsChecked,
                numOfNeedles = numOfNeedlesValue,
                difficulty = this.difficulty
            }) ;

            gameGrid.ShowDialog();

            this.Stats.Content = "Diffifculty: " + gameGrid.settings.difficulty.ToString() +
                            "\nTime Elapsed: " + Math.Round(gameGrid.data.timeElapsed.TotalSeconds, 3) + "s" + 
                            "\nNumber of Needles: " + gameGrid.settings.numOfNeedles +
                            "\nAverage selection time: " + gameGrid.data.AverageSelectionTime + "s";
            
        }

        private void numOfNeedles_TextChanged(object sender, TextChangedEventArgs e)
        {
            string change = ((TextBox)e.Source).Text;

            if(string.IsNullOrEmpty(change) || string.IsNullOrWhiteSpace(change))
            {
                return;
            }

            if (!int.TryParse(change, out numOfNeedlesValue))
            {
                numOfNeedles.Text = "3";
                numOfNeedlesValue = 3;
            }
            else
            {
                if(numOfNeedlesValue < 3 || numOfNeedlesValue > 9)
                {
                    numOfNeedles.Text = "3";
                    numOfNeedlesValue = 3;
                }
            }
        }

        private void DiffEasy_Checked(object sender, RoutedEventArgs e)
        {
            this.difficulty = GameSettings.Difficulty.EASY;
        }

        private void DiffNormal_Checked(object sender, RoutedEventArgs e)
        {
            this.difficulty = GameSettings.Difficulty.NORMAL;
        }

        private void DiffHard_Checked(object sender, RoutedEventArgs e)
        {
            this.difficulty = GameSettings.Difficulty.HARD;
        }
    }
}
