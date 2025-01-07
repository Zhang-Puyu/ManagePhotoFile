using System.Windows;

namespace PhotoTools.Application.Dialog
{
    /// <summary>
    /// ConverExistedFileDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConverExistedFileDialog : Window
    {
        public ConverExistedFileDialog(in IEnumerable<string> strs)
        {
            InitializeComponent();

            foreach (var str in strs)
                ListFiles.Items.Add(str);
        }

        public new bool DialogResult { get; private set; } = false;

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
