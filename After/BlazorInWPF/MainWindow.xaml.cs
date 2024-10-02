using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;

namespace BlazorInWPF
{
    public partial class MainWindow : Window
    {
        // ObservableCollection to hold tasks (auto-updates UI when changed)
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            Resources.Add("services", serviceCollection.BuildServiceProvider());

            // Initialize the collection
            Tasks = new ObservableCollection<TaskItem>();
            // Set the DataContext to this window, allowing us to bind the task list to the UI
            this.DataContext = this;

            webview.BlazorWebViewInitialized += BlazorWebViewInitialized;
        }

        private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            webview.WebView.WebMessageReceived += WebView_WebMessageReceived;
        }

        private void WebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            if (e.WebMessageAsJson.Contains("blazorHostEventName"))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var blazorHostEvent = JsonSerializer.Deserialize<BlazorHostEvent>(e.WebMessageAsJson, options);
                if (blazorHostEvent?.BlazorHostEventName == "AddTask")
                {
                    // Ensure the task input is not empty
                    if (!string.IsNullOrWhiteSpace(blazorHostEvent.Content))
                    {
                        // Add new task to the collection
                        Tasks.Add(new TaskItem { Description = blazorHostEvent.Content });
                    }
                    else
                    {
                        MessageBox.Show("Please enter a task description.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
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
