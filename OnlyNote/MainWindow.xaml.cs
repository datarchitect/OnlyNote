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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TODOItemList list = new TODOItemList();
        private const int MAX_ROWS = 4;
        private const int MAX_COLS = 4;
        ListBox[,] taskLists = new ListBox[MAX_ROWS, MAX_COLS];
        int currentRow = 0, currentCol = 0;

        public MainWindow()
        {
            InitializeComponent();
            list.Populate();
            BindEvents();
            HydrateForm();
            UpdateTaskLists();
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
                ShowCategory(category);
            }

            int tabIndexCount = 0;
            for (int row = 0; row < MAX_ROWS; row++)
                for (int col = 0; col < MAX_COLS; col++)
                {
                    ListBox thisList = taskLists[row, col];
                    if (thisList != null)
                    {
                        thisList.IsTabStop = true;
                        thisList.TabIndex = tabIndexCount;

                        if (tabIndexCount == 0)
                            thisList.Focus();

                        tabIndexCount++;
                    }
                }
        }

        private ListBox ShowCategory(string category)
        {
            if ((currentRow == MAX_ROWS) || (currentCol == MAX_COLS))
                MessageBox.Show("Row / Col limit reached");

            Label lblNew = new Label();
            lblNew.Name = ("lbl" + category).Replace(" ", "");
            lblNew.Content = category;
            lblNew.IsTabStop = false;
            Grid.SetColumn(lblNew, currentCol);
            Grid.SetRow(lblNew, currentRow);
            grdMain.Children.Add(lblNew);

            ListBox newList = new ListBox();
            newList.Name = ("lst" + category).Replace(" ", "");
            Grid.SetColumn(newList, currentCol);
            Grid.SetRow(newList, currentRow + 1);
            grdMain.Children.Add(newList);
            newList.Tag = category;
            //            lstTasks.SelectionChanged += lstTasks_SelectionChanged;
            newList.KeyUp += LstTasks_KeyUp;

            //TODO: move this out
            taskLists[currentRow, currentCol] = newList;

            //TODO: move this out
            list.Add(new TODOItem(category, "Dummy Task", string.Empty));
            UpdateTaskLists(newList, category);

            currentCol++;
            if (currentCol == MAX_COLS)
            {
                if (currentRow < MAX_ROWS)
                {
                    currentRow += 2;
                    currentCol = 0;
                }
            }

            return newList;
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

        public void UpdateTaskLists(ListBox thisList, string category)
        {
            thisList.ItemsSource = list.FilterByCategory(category);
            thisList.DisplayMemberPath = "Task";
            thisList.SelectedValuePath = "ID";
        }

        public void UpdateTaskLists()
        {
            for (int row = 0; row < MAX_ROWS; row++)
                for (int col = 0; col < MAX_COLS; col++)
                {
                    ListBox thisList = taskLists[row, col];
                    if (thisList != null)
                    {
                        string category = thisList.Tag as string;
                        thisList.ItemsSource = list.FilterByCategory(category);
                        thisList.DisplayMemberPath = "Task";
                        thisList.SelectedValuePath = "ID";
                    }
                }
        }

        //private void lstTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //}

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.K))
            {
                AddNew wnd = new AddNew();
                wnd.txtNewValue.Text = string.Empty;
                wnd.ShowDialog();
                string newValue = wnd.txtNewValue.Text;
                ShowCategory(newValue);
            }
        }

        private void LstTasks_KeyUp(object sender, KeyEventArgs e)
        {
            ListBox thisListBox = e.Source as ListBox;
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.N))
            {
                AddNew wnd = new AddNew();
                wnd.ShowDialog();
                string newValue = wnd.txtNewValue.Text;

                list.Add(new TODOItem(thisListBox.Tag as string, newValue, string.Empty));

                UpdateTaskLists();
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
