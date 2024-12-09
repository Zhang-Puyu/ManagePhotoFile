using System.Windows.Controls;
using PhotoTools.Application.ViewModels;

namespace PhotoTools.Application.Pages
{
    /// <summary>
    /// CopyPage.xaml 的交互逻辑
    /// </summary>
    public partial class CopyPage : Page
    {
        public CopyPage()
        {
            InitializeComponent();
            DataContext = new CopyViewModel();
        }
    }
}
