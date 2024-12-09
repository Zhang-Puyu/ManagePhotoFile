using System.Windows.Controls;
using PhotoTools.Application.ViewModels;

namespace PhotoTools.Application.Pages
{
    /// <summary>
    /// RenamePage.xaml 的交互逻辑
    /// </summary>
    public partial class RenamePage : Page
    {
        public RenamePage()
        {
            InitializeComponent();
            DataContext = new RenameViewModel();
        }
    }
}
