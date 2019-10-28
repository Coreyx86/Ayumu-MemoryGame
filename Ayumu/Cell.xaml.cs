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
    public class Coordinate : IComparable<Coordinate>
    {
        public int x { get; set; }
        public int y { get; set; }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        //It would make more sense to return 0 if the coords are the same, but oh well. 
        public int CompareTo(Coordinate obj)
        {
            if (obj == null) { return 0; }
            else
            {
                if(this.x == obj.x && this.y == obj.y)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
    /// <summary>
    /// Interaction logic for Cell.xaml
    /// </summary>

    public partial class Cell : UserControl, INotifyPropertyChanged
    {
        public enum CellType
        {
            HAYSTACK,
            NEEDLE
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private CellType _cellType;
        public CellType cellType {
            get
            {
                return _cellType;
            }
            set
            {
                _cellType = value;
                OnPropertyChanged("valuestr");
            }
        }

        public Coordinate coord { get; set; }

        public int value = 0;
        public string valuestr
        {
            get
            {

                return cellType == CellType.HAYSTACK ? "X" : value.ToString();
            }
        }

        public Cell()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public Cell(int value, CellType type = CellType.HAYSTACK)
        {
            InitializeComponent();
            this.DataContext = this;
            this.value = value;
            cellType = type;
        }
    }
}
