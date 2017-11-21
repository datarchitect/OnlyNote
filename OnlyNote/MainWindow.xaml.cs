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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlyNote
{
    public class pos
    {
        private int _row;
        private int _col;

        public int col { get => _col; set => _col = value; }
        public int row { get => _row; set => _row = value; }

        public pos() { }
        public pos(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TODOItemList list = new TODOItemList();
        private const int MAX_ROWS = 4;
        private const int MAX_COLS = 4;
        ListBox[,] taskLists = new ListBox[MAX_ROWS, MAX_COLS];
        pos currentPosition = new pos ( 0, 0 );

        public MainWindow()
        {
            InitializeComponent();
            list.Populate();
            BindEvents();
            HydrateForm();
//            UpdateTaskLists();
        }

        public void BindEvents()
        {
            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                ListBoxItem.MouseDoubleClickEvent,
                new RoutedEventHandler(this.TaskItem_DoubleClicked));

            EventManager.RegisterClassHandler(typeof(ListBoxItem),
                ListBoxItem.KeyDownEvent,
                new KeyEventHandler(this.TaskItem_KeyUp));
        }

        private void HydrateForm()
        {
            List<string> categories = list.GetAllCategories();

            foreach (string category in categories)
            {
                pos thisPosition = AddCategoryControls(category);
                UpdateTaskLists(thisPosition);
            }
        }

        private pos AddCategoryControls(string category)
        {
            pos newPosition = GetNextPos(currentPosition);

            Label lblNew = new Label();
            lblNew.Name = ("lbl" + category).Replace(" ", "");
            lblNew.Content = category;
            lblNew.IsTabStop = false;
            Grid.SetColumn(lblNew, newPosition.col);
            Grid.SetRow(lblNew, newPosition.row);
            grdMain.Children.Add(lblNew);

            ListBox newList = new ListBox();
            newList.Name = ("lst" + category).Replace(" ", "");
            Grid.SetColumn(newList, newPosition.col);
            Grid.SetRow(newList, newPosition.row + 1);
            grdMain.Children.Add(newList);
            newList.Tag = category;
            newList.KeyUp += LstTasks_KeyUp;

            newList.IsTabStop = true;
            int tabIndex = (newPosition.row * MAX_COLS) + newPosition.col;
            newList.TabIndex = tabIndex;

            //TODO: move out
            if (tabIndex == 0)
                newList.Focus();

            taskLists[newPosition.row, newPosition.col] = newList;
            currentPosition = newPosition;

            return newPosition;
        }

        private void AddNewCategory(string newValue)
        {
            list.Add(new TODOItem(newValue, "Dummy Task", string.Empty));

            pos thisPosition = AddCategoryControls(newValue);
            UpdateTaskLists(thisPosition);
        }

        public pos GetNextPos(pos currentPosition)
        {
            if ((currentPosition.row == MAX_ROWS) || (currentPosition.col == MAX_COLS))
                MessageBox.Show("Row / Col limit reached");

            currentPosition.col++;
            if (currentPosition.col == MAX_COLS)
            {
                if (currentPosition.row < MAX_ROWS)
                {
                    currentPosition.row += 2;
                    currentPosition.col = 0;
                }
            }

            return currentPosition;
        }

        private void ShowNotes(TODOItem todo)
        {
            if (todo != null)
            {
                wndNotes wnd = new wndNotes();
                wnd.txtNotes.Text = todo.Notes;
                wnd.ShowDialog();
                todo.Notes = wnd.txtNotes.Text;
            }
        }

        public void UpdateTaskLists(pos thisPosition)
        {
            ListBox thisList = taskLists[thisPosition.row, thisPosition.col];
            string category = thisList.Tag as string;

            thisList.ItemsSource = list.FilterByCategory(category);
            thisList.DisplayMemberPath = "Task";
            thisList.SelectedValuePath = "ID";
        }

        public void UpdateTaskLists(ListBox thisList)
        {
            string category = thisList.Tag as string;

            thisList.ItemsSource = list.FilterByCategory(category);
            thisList.DisplayMemberPath = "Task";
            thisList.SelectedValuePath = "ID";
        }


        //public void UpdateTaskLists()
        //{
        //    for (int row = 0; row < MAX_ROWS; row++)
        //        for (int col = 0; col < MAX_COLS; col++)
        //        {
        //            ListBox thisList = taskLists[row, col];
        //            if (thisList != null)
        //            {
        //                string category = thisList.Tag as string;
        //                thisList.ItemsSource = list.FilterByCategory(category);
        //                thisList.DisplayMemberPath = "Task";
        //                thisList.SelectedValuePath = "ID";
        //            }
        //        }
        //}

        //private void lstTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //}

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.K))
            {
                string newValue = ReadNewValue();
                AddNewCategory(newValue);
            }
        }

        private string ReadNewValue()
        {
            AddNew wnd = new AddNew();
            wnd.txtNewValue.Text = string.Empty;
            wnd.ShowDialog();
            return wnd.txtNewValue.Text;
        }

        private void LstTasks_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.N))
            {
                ListBox thisListBox = e.Source as ListBox;
                string thisCategory = thisListBox.Tag as string;
                string newValue = ReadNewValue();
                list.Add(new TODOItem(thisCategory, newValue, string.Empty));
                UpdateTaskLists(thisListBox);
            }
        }

        private void TaskItem_DoubleClicked(object sender, RoutedEventArgs e)
        {
            ListBoxItem thisListItem = e.Source as ListBoxItem;
            TODOItem selectedTaskItem = thisListItem.DataContext as TODOItem;
            ShowNotes(selectedTaskItem);
        }

        private void TaskItem_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListBoxItem thisListItem = e.Source as ListBoxItem;
                TODOItem selectedTaskItem = thisListItem.DataContext as TODOItem;
                ShowNotes(selectedTaskItem);
            }
        }

    }
}
