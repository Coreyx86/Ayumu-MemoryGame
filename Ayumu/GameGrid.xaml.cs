using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Ayumu
{
    public class GameSessionData
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

        public TimeSpan timeElapsed
        {
            get
            {
                return endTime - startTime;
            }
        }

        public int NeedlesCount { get; set; }

        public List<DateTime> selectionTimeHistory { get; set; } = new List<DateTime>();

        public double AverageSelectionTime
        {
            get
            {
                if(selectionTimeHistory != null && selectionTimeHistory.Count > 1)
                {
                    TimeSpan tot = selectionTimeHistory.Last() - selectionTimeHistory[0];

                    return Math.Round(tot.TotalSeconds / selectionTimeHistory.Count, 3);
                }

                return 0;
            }
        }

        public GameSessionData()
        {

        }
    }
    public class GameSettings
    {
        public enum Difficulty
        {
            EASY,
            NORMAL,
            HARD
        }

        public Difficulty difficulty { get; set; }

        public bool allowRepeats { get; set; }

        public int numOfNeedles { get; set; }

        public GameSettings() { }
    }
    /// <summary>
    /// Interaction logic for GameGrid.xaml
    /// </summary>

    public partial class GameGrid : Window
    {
        public GameSettings settings { get; set; } = new GameSettings();

        public GameSessionData data { get; set; } = new GameSessionData();

        private List<Cell> Needles { get; set; }
        private List<Cell> Haystack { get; set; }
        private List<Cell> SelectedNeedles { get; set; }

        public GameGrid()
        {
            InitializeComponent();
            Needles = new List<Cell>();
            Haystack = new List<Cell>();
            SelectedNeedles = new List<Cell>();
        }

        public GameGrid(GameSettings settings)
        {
            InitializeComponent();

            this.settings = settings;

            Needles = new List<Cell>();
            Haystack = new List<Cell>();
            SelectedNeedles = new List<Cell>();

        }

        private void GameGrid_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGrid();
        }

        private void AddSelectedCell(Cell cell)
        {
            DateTime tick = DateTime.Now;
            if(SelectedNeedles.Count == 0)
            {
                data.startTime = tick;
            }
            else if(SelectedNeedles.Count == Needles.Count - 1)
            {
                data.endTime = tick;
            }

            data.selectionTimeHistory.Add(tick);
            SelectedNeedles.Add(cell);
            cell.cellType = Cell.CellType.HAYSTACK;
        }

        private void CellBtn_Click(object sender, RoutedEventArgs e)
        {
            Cell cell = (Cell)((Button)sender).Tag;
            //SelectedNeedles.Reverse();

            //Search needles list to see if the selected cell a needle, if it is...
            if (Needles.Find( x => x.coord.CompareTo(cell.coord) == 1) != null)
            {
                if (SelectedNeedles.Count > 0)
                {
                    Cell lastSelected = SelectedNeedles.Last();
                    int correctIndex = Needles.FindLastIndex(x => x.value.CompareTo(lastSelected.value) == 0);

                    //If repeats are valid then the next value CAN be the same as the last.
                    if (settings.allowRepeats && lastSelected.value.CompareTo(cell.value) == 0)
                    {
                        AddSelectedCell(cell);
                    }
                    //If the last value is less than the next
                    else if (lastSelected.value.CompareTo(cell.value) == -1)
                    {

                        //if the selected cell is greater than the previous but is higher than the correct value then we failed

                        if ((!settings.allowRepeats && cell.value.CompareTo(Needles[correctIndex + 1].value) == 1) ||
                            //Or if repeats are allowed, if the selected cell is higher than the correct cell value or if the selected...
                            //..value is correct but not all of the previous value has been selected then we have failed
                            (   settings.allowRepeats && (cell.value.CompareTo(Needles[correctIndex + 1].value) == 1 || 
                                    ( cell.value.CompareTo(Needles[correctIndex + 1].value) == 0 && 
                                        SelectedNeedles.FindAll(x => x.value == lastSelected.value).Count < Needles.FindAll(x => x.value == lastSelected.value).Count))))
                        {
                            MessageBox.Show("Failed");
                            Window.GetWindow(this).Close();
                            return;
                        }

                        AddSelectedCell(cell);
                    }
                    else
                    {
                        MessageBox.Show("FAILED");
                        Window.GetWindow(this).Close();

                        return;
                    }
                }
                else
                {
                    //If the selected cell is not the first index of the Needles list (sorted by ascending value order and by coordinates) then its not in numerical order.

                    if ((settings.allowRepeats && Needles.FindIndex(x => x.value == cell.value) != 0) 
                        || (!settings.allowRepeats && Needles.FindIndex(x => x.coord.CompareTo(cell.coord) == 1) != 0))
                    {
                        if(Needles.FindIndex( x => x.value == cell.value ) != 0)
                        {
                            MessageBox.Show("Failed");
                            Window.GetWindow(this).Close();
                            return;
                        }
                    }

                    //Handle some of the difficult settings here. If you're playing above easy, the first selection will hide the rest.
                    if(settings.difficulty > GameSettings.Difficulty.EASY)
                    {
                        foreach(Cell c in Needles)
                        {
                            c.cellType = Cell.CellType.HAYSTACK;
                        }
                    }

                    AddSelectedCell(cell);
                }

                cell.num.IsEnabled = false;

                if(SelectedNeedles.Count == Needles.Count)
                {
                    MessageBox.Show("Winner winner chicken dinner!");
                    Window.GetWindow(this).Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("FAiled");
                Window.GetWindow(this).Close();

                return;
            }
        }


        private void InitializeGrid()
        {
            //TODO:
            Random rand = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 1337);

            //Determine our needle cordinates before hand.
            List<Coordinate> needleCordinates = new List<Coordinate>();
            List<int> randomValues = new List<int>();

            for (int i = 0; i < settings.numOfNeedles; i++)
            {
                Coordinate buffer = new Coordinate(rand.Next(1, 6), rand.Next(1, 9));
                while (needleCordinates.Find(x => x.CompareTo(buffer) == 1) != null)
                {
                    buffer = new Coordinate(rand.Next(1, 6), rand.Next(1, 9));
                }

                needleCordinates.Add(buffer);

                if (settings.allowRepeats)
                {
                    randomValues.Add(rand.Next(1, 10));
                }
            }

            if(!settings.allowRepeats)
            {
                for(int i = 1; i < 10; i++)
                {
                    randomValues.Add(i);
                }

                randomValues.Shuffle();
                randomValues = randomValues.GetRange(0, settings.numOfNeedles);
            }


            for (int row = 1; row < 6; row++)
            {
                for (int col = 1; col < 9; col++)
                {
                    //Initialize a new Cell object with the random value and the current row,col 
                    Cell cell = new Cell(randomValues.Count > 0 ? randomValues.Last() : 0);
                    cell.coord = new Coordinate(row, col);

                    if (needleCordinates.Find(x => x.CompareTo(cell.coord) == 1) != null)
                    {
                        cell.cellType = Cell.CellType.NEEDLE;

                        if(randomValues.Count > 0) {
                            randomValues.RemoveAt(randomValues.Count - 1);
                        }
                        
                    }
                    else
                    {
                        cell.cellType = Cell.CellType.HAYSTACK;
                    }

                    //Add the cell to its respected list. 
                    if (cell.cellType == Cell.CellType.HAYSTACK)
                    {
                        if (settings.difficulty == GameSettings.Difficulty.EASY || settings.difficulty == GameSettings.Difficulty.NORMAL)
                        {
                            continue;
                        }

                        Haystack.Add(cell);
                    }
                    else
                    {
                        Needles.Add(cell);
                    }

                    cell.num.Click += CellBtn_Click;
                    cell.num.Tag = cell;

                    //Add the Cell to the GameGrid UI.
                    Grid.SetRow(cell, row - 1);
                    Grid.SetColumn(cell, col - 1);

                    this.grid.Children.Add(cell);
                }
            }

            //Sort the Needles
            Needles.Sort(delegate(Cell x, Cell y)
            {
                if (x == null && y == null) { return 0; }
                else if (x.value == y.value) { return 0; }
                else if (x == null) { return -1; }
                else if (y == null) { return 1; }
                else
                {
                    int retVal = x.value.CompareTo(y.value);

                    if (retVal == 0)
                    {
                        return x.coord.CompareTo(y.coord);
                    }
                    else
                    {
                        return retVal;
                    }
                }
            });
        }
    }
}
