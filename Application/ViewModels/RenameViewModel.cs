using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace PhotoTools.Application.ViewModels
{
    public partial class RenameViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
        private string _workPath = string.Empty;
        [RelayCommand]
        private void ChooseWorkPath()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Multiselect = false,
                Title = "选择遍历路径",
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                WorkPath = dialog.FileName;

                if (!string.IsNullOrEmpty(Pattern.Trim()))
                    OrignalFileList = new ObservableCollection<string>
                        (Directory.GetFiles(WorkPath, Pattern).Select(Path.GetFileName));
                else
                    OrignalFileList = new ObservableCollection<string>
                        (Directory.GetFiles(WorkPath).Select(Path.GetFileName));

                OrignalFileCount = OrignalFileList.Count;
            }
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RenameCommand))]
        private ObservableCollection<string> _orignalFileList = new();
        [ObservableProperty]
        private ObservableCollection<string> _renamedFileList = new();

        [ObservableProperty]
        private string _addedSuffix = string.Empty;
        [ObservableProperty]
        private string _addedPrefix = string.Empty;
        [ObservableProperty]
        private string _orignalText = string.Empty;
        [ObservableProperty]
        private string _replacedText = string.Empty;

        [ObservableProperty]
        private string _pattern = "*.*";

        [ObservableProperty]
        private int _orignalFileCount = 0;
        [ObservableProperty]
        private int _reanamedFileCount = 0;

        private bool CanRename => !string.IsNullOrEmpty(WorkPath) && OrignalFileList.Count > 0;
        [RelayCommand(CanExecute = nameof(CanRename))]
        private void Rename()
        {
            List<string> renamedFileList = new List<string>();
            Parallel.ForEach(OrignalFileList, originalFileName =>
            {
                string targetFileName = AddedPrefix + originalFileName.Replace(OrignalText, ReplacedText) + AddedSuffix;
                if (originalFileName != targetFileName)
                {
                    File.Move(Path.Combine(WorkPath, originalFileName), Path.Combine(WorkPath, targetFileName));
                    renamedFileList.Add(targetFileName);
                }
            });
      
            RenamedFileList   = new ObservableCollection<string>(renamedFileList);
            ReanamedFileCount = RenamedFileList.Count;
        }
    }
}
