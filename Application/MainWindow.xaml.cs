using System.Windows;
using System.Windows.Controls;
using PhotoTools.Application.Pages;

namespace PhotoTools.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            pages[nameof(CopyPage)]   = new CopyPage();
            pages[nameof(RenamePage)] = new RenamePage();

            frame.Navigate(pages[nameof(CopyPage)]);
        }

        private readonly Dictionary<string, Page> pages = new Dictionary<string, Page>();

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem?.Tag is string tag && pages.TryGetValue(tag, out var page))
            {
                _ = frame.Navigate(page);
            }
        }
    }
}