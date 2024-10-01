using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace BlazorInWPF
{
    public partial class MainWindow : Window
    {
        // ObservableCollection to hold tasks (auto-updates UI when changed)
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            // Initialize the collection
            Tasks = new ObservableCollection<TaskItem>();
            // Set the DataContext to this window, allowing us to bind the task list to the UI
            this.DataContext = this;
        }

        // Add Task Button click event
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the task input is not empty
            if (!string.IsNullOrWhiteSpace(TaskInput.Text))
            {
                // Add new task to the collection
                Tasks.Add(new TaskItem { Description = TaskInput.Text });
                // Clear the input box
                TaskInput.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Please enter a task description.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Remove Task Button click event
        private void RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = (TaskItem)TaskListView.SelectedItem;
            if (selectedTask != null)
            {
                Tasks.Remove(selectedTask);
            }
            else
            {
                MessageBox.Show("Please select a task to remove.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // Model class representing a Task
    public class TaskItem : INotifyPropertyChanged
    {
        private string _description;
        private bool _isCompleted;

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }

        // Event to notify when property changes (needed for binding)
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
