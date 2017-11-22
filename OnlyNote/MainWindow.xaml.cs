﻿using System;
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
        private int _maxRow;
        private int _maxCol;
        private int _rowSpan;

        public int col { get => _col; set => _col = value; }
        public int row { get => _row; set => _row = value; }

        public pos() { }

        public void Initialize()
        {
            _row = 0; _col = 0;
        }

        public void SetMaxLimit(int row, int col)
        {
            _maxRow = row;
            _maxCol = col;
        }

        public void SetRowSpan(int rowSpan)
        {
            this._rowSpan = rowSpan;
        }

        public void MoveToNextPos()
        {
            if (_col >= (_maxCol - 1))
            {
                _col = 0;
                _row += _rowSpan;
            }
            else
                _col++;

            if ((_row >= _maxRow))
                MessageBox.Show("Row limit reached");
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TODOItemList list = new TODOItemList();
        private const int MAX_ROWS = 6;
        private const int MAX_COLS = 4;
        private const int ITEM_ROWSPAN = 3;
        ListBox[,] taskLists = new ListBox[MAX_ROWS, MAX_COLS];
        pos currentPosition = new pos ();

        public MainWindow()
        {
            InitializeComponent();
            list.Populate();
            BindEvents();
            HydrateForm();
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

            currentPosition.SetMaxLimit(MAX_ROWS, MAX_COLS);
            currentPosition.SetRowSpan(ITEM_ROWSPAN);
            currentPosition.Initialize();
            txtCommands.Text = "Ctrl+N = New Task. Ctrl+K = New Category. Ctrl+Up = Move task up. Ctrl+Down = Move task down. Ctrl+D = Delete task. Enter = Open Task";

            foreach (string category in categories)
            {
                AddCategoryControls(category);
                UpdateTaskListControl(currentPosition);
                currentPosition.MoveToNextPos();
            }
        }

        private void AddCategoryControls(string category)
        {
            Label lblNew = new Label();
            lblNew.Name = ("lbl" + category).Replace(" ", "");
            lblNew.Content = category;
            lblNew.IsTabStop = false;
            Grid.SetColumn(lblNew, currentPosition.col);
            Grid.SetRow(lblNew, currentPosition.row);
            grdMain.Children.Add(lblNew);

            ListBox newList = new ListBox();
            newList.Name = ("lst" + category).Replace(" ", "");
            Grid.SetColumn(newList, currentPosition.col);
            Grid.SetRow(newList, currentPosition.row + 1);
            grdMain.Children.Add(newList);
            newList.Tag = category;
            newList.KeyUp += LstTasks_KeyUp;

            newList.IsTabStop = true;
            int tabIndex = (currentPosition.row * MAX_COLS) + currentPosition.col;
            newList.TabIndex = tabIndex;

            //TODO: move out
            if (tabIndex == 0)
                newList.Focus();

            taskLists[currentPosition.row, currentPosition.col] = newList;

            //-----------------------
            //        <Rectangle x:Name="rctDormant" Grid.Row="2" Grid.Column="0" Fill="Red" Height="10" Width="25" Margin="10,0,0,0" Stroke="Transparent" VerticalAlignment="Center" HorizontalAlignment="Left"/>
            Rectangle rctNewDormant = new Rectangle();
            rctNewDormant.Name = "rctDormant" + category.Replace(" ", "");
            Grid.SetRow(rctNewDormant, currentPosition.row + 2);
            Grid.SetColumn(rctNewDormant, currentPosition.col);
            rctNewDormant.Fill = new SolidColorBrush(Color.FromRgb(125,0,0)); 
            rctNewDormant.Height = 10;
            rctNewDormant.Width = 25;
            rctNewDormant.Margin = new Thickness(10, 0, 0, 0);
            rctNewDormant.Stroke = new SolidColorBrush(Color.FromRgb(125, 0, 0));
            rctNewDormant.VerticalAlignment = VerticalAlignment.Center;
            rctNewDormant.HorizontalAlignment = HorizontalAlignment.Left;
            grdMain.Children.Add(rctNewDormant);

            TextBlock txtDormant = new TextBlock();
            txtDormant.Text = "XX";
            txtDormant.Name = "txtDormant" + category.Replace(" ", "");
            txtDormant.Margin = new Thickness(45, 0, 0, 0);
            txtDormant.Width = 25;
            txtDormant.TextAlignment = TextAlignment.Center;
            txtDormant.HorizontalAlignment = HorizontalAlignment.Left;
            txtDormant.VerticalAlignment = VerticalAlignment.Center;
            txtDormant.TextWrapping = TextWrapping.NoWrap;
            Grid.SetRow(txtDormant, currentPosition.row + 2);
            Grid.SetColumn(txtDormant, currentPosition.col);
            grdMain.Children.Add(txtDormant);

            Rectangle rctUntouched = new Rectangle();
            rctUntouched.Name = "rctUntouched" + category.Replace(" ", "");
            Grid.SetRow(rctUntouched, currentPosition.row + 2);
            Grid.SetColumn(rctUntouched, currentPosition.col);
            rctUntouched.Fill = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            rctUntouched.Height = 10;
            rctUntouched.Width = 25;
            rctUntouched.Margin = new Thickness(80, 0, 0, 0);
            rctUntouched.Stroke = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            rctUntouched.VerticalAlignment = VerticalAlignment.Center;
            rctUntouched.HorizontalAlignment = HorizontalAlignment.Left;
            grdMain.Children.Add(rctUntouched);

            TextBlock txtUntouched = new TextBlock();
            txtUntouched.Text = "XX";
            txtUntouched.Name = "txtUntouched" + category.Replace(" ", "");
            txtUntouched.Margin = new Thickness(115, 0, 0, 0);
            txtUntouched.Width = 25;
            txtUntouched.TextAlignment = TextAlignment.Center;
            txtUntouched.HorizontalAlignment = HorizontalAlignment.Left;
            txtUntouched.VerticalAlignment = VerticalAlignment.Center;
            txtUntouched.TextWrapping = TextWrapping.NoWrap;
            Grid.SetRow(txtUntouched, currentPosition.row + 2);
            Grid.SetColumn(txtUntouched, currentPosition.col);
            grdMain.Children.Add(txtUntouched);

            Rectangle rctOthers = new Rectangle();
            rctOthers.Name = "rctOthers" + category.Replace(" ", "");
            Grid.SetRow(rctOthers, currentPosition.row + 2);
            Grid.SetColumn(rctOthers, currentPosition.col);
            rctOthers.Fill = new SolidColorBrush(Color.FromRgb(1, 1, 1)); ;
            rctOthers.Height = 10;
            rctOthers.Width = 25;
            rctOthers.Margin = new Thickness(150, 0, 0, 0);
            rctOthers.Stroke = new SolidColorBrush(Color.FromRgb(27, 82, 87)); ;
            rctOthers.VerticalAlignment = VerticalAlignment.Center;
            rctOthers.HorizontalAlignment = HorizontalAlignment.Left;
            grdMain.Children.Add(rctOthers);

            TextBlock txtOthers = new TextBlock();
            txtOthers.Text = "XX";
            txtOthers.Name = "txtOthers" + category.Replace(" ", "");
            txtOthers.Margin = new Thickness(185, 0, 0, 0);
            txtOthers.Width = 25;
            txtOthers.TextAlignment = TextAlignment.Center;
            txtOthers.HorizontalAlignment = HorizontalAlignment.Left;
            txtOthers.VerticalAlignment = VerticalAlignment.Center;
            txtOthers.TextWrapping = TextWrapping.NoWrap;
            Grid.SetRow(txtOthers, currentPosition.row + 2);
            Grid.SetColumn(txtOthers, currentPosition.col);
            grdMain.Children.Add(txtOthers);
            //-----------------------
        }

        private void ShowNotes(TODOItem todo)
        {
            if (todo != null)
            {
                wndNotes wnd = new wndNotes();
                wnd.txtNotes.Text = todo.Notes;
                wnd.ShowDialog();
                list.AddNotes(todo.ID, wnd.txtNotes.Text);
                //todo.Notes = wnd.txtNotes.Text;
            }
        }

        public void UpdateTaskListControl(pos thisPosition)
        {
            ListBox thisList = taskLists[thisPosition.row, thisPosition.col];
            string category = thisList.Tag as string;

            thisList.ItemsSource = list.FilterByCategory(category);
            thisList.DisplayMemberPath = "Task";
            thisList.SelectedValuePath = "ID";
        }

        public void UpdateTaskListControl(ListBox thisList)
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
                list.AddNewCategory(newValue);
                AddCategoryControls(newValue);
                UpdateTaskListControl(currentPosition);
                currentPosition.MoveToNextPos();

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
                list.AddTask(thisCategory, newValue);
                UpdateTaskListControl(thisListBox);
            }
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (e.Key == Key.I))
            {
                ListBox thisListBox = e.Source as ListBox;
                string thisCategory = thisListBox.Tag as string;
                wndNotes wndMultiText = new wndNotes();
                wndMultiText.ShowDialog();
                string newValues = wndMultiText.txtNotes.Text;
                list.ImportTasks(thisCategory, newValues);
                UpdateTaskListControl(thisListBox);
            }
            if (((Keyboard.IsKeyDown(Key.LeftCtrl)) || (Keyboard.IsKeyDown(Key.RightCtrl))) && (e.Key == Key.D))
            {
                ListBox thisListBox = e.Source as ListBox;
//                ListBoxItem thisListItem = thisListBox.SelectedItem as ListBoxItem;
                TODOItem selectedTaskItem = thisListBox.SelectedItem as TODOItem;
                list.DeleteTask(selectedTaskItem);
                UpdateTaskListControl(thisListBox);
            }
            if (((Keyboard.IsKeyDown(Key.LeftCtrl)) || (Keyboard.IsKeyDown(Key.RightCtrl))) && (e.Key == Key.Up))
            {
                ListBox thisListBox = e.Source as ListBox;
                int selectedIndex = thisListBox.SelectedIndex;
                if (selectedIndex > 0)
                {
                    //ListBoxItem thisListItem = thisListBox.SelectedItem as ListBoxItem;
                    TODOItem selectedTaskItem = thisListBox.SelectedItem as TODOItem;

                    //ListBoxItem destinationListItem = thisListBox.Items[selectedIndex - 1] as ListBoxItem;
                    TODOItem destinationTaskItem = thisListBox.Items[selectedIndex - 1] as TODOItem;

                    list.SwapTask(selectedTaskItem, destinationTaskItem);
                    UpdateTaskListControl(thisListBox);
                }
            }
            if (((Keyboard.IsKeyDown(Key.LeftCtrl)) || (Keyboard.IsKeyDown(Key.RightCtrl))) && (e.Key == Key.Down))
            {
                ListBox thisListBox = e.Source as ListBox;
                ListBoxItem thisListItem = thisListBox.SelectedItem as ListBoxItem;
                TODOItem selectedTaskItem = thisListItem.DataContext as TODOItem;
                list.DeleteTask(selectedTaskItem);
                UpdateTaskListControl(thisListBox);
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
