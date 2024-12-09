using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;

namespace PhotoTools.Application.ViewModels
{
    public partial class CopyViewModel : ObservableObject
    {
        public CopyViewModel()
        {
            AsyncCopyCommand = new AsyncRelayCommand(CopyWork, CanWork);
            AsyncMoveCommand = new AsyncRelayCommand(MoveWork, CanWork);
        }

        #region 属性
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        string _jpgPath = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        string _jpgSuffix = "JPG";
        [ObservableProperty]
        int _jpgTotalCount = 0;
        [ObservableProperty]
        int _jpgResidueCount = 0;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        string _rawPath = string.Empty;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        string _rawSuffix = "NEF";
        [ObservableProperty]
        int _rawTotalCount = 0;
        [ObservableProperty]
        int _rawResidueCount = 0;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        string _targetPath = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        ObservableCollection<string> _jpgList = new ObservableCollection<string>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AsyncCopyCommand))]
        [NotifyCanExecuteChangedFor(nameof(AsyncMoveCommand))]
        ObservableCollection<string> _rawList = new ObservableCollection<string>();

        [ObservableProperty]
        int _progress = 0;
        [ObservableProperty]
        int _maxProgress = 1;

        [ObservableProperty]
        bool _isIdel = true;

        [ObservableProperty]
        bool _continueFlag = false;
        #endregion

        #region 剪切和复制命令
        private bool CanWork() => JpgList.Count > 0 && 
            !string.IsNullOrWhiteSpace(JpgPath) && !string.IsNullOrWhiteSpace(JpgSuffix) &&
            !string.IsNullOrWhiteSpace(RawPath) && !string.IsNullOrWhiteSpace(RawSuffix) &&
            !string.IsNullOrWhiteSpace(TargetPath);

        public IAsyncRelayCommand AsyncCopyCommand { get; }
        private Task CopyWork()
        {
            _ = DoWork(File.Copy);
            return Task.CompletedTask;
        }

        public IAsyncRelayCommand AsyncMoveCommand { get; }
        private Task MoveWork()
        {
            _ = DoWork(File.Move);
            return Task.CompletedTask;
        }

        private Task DoWork(Action<string, string> work)
        {
            MaxProgress = JpgList.Count;
            Progress = 1;

            IsIdel = false;
            ContinueFlag = true;

            Dictionary<string, string> ExistedRawList = new Dictionary<string, string>();

            for (int i = JpgList.Count - 1; i >= 0; i--)
            {
                if (ContinueFlag)
                {
                    string jpgName = Path.GetFileName(JpgList[i]);
                    string rawName = Path.ChangeExtension(jpgName, RawSuffix);
                    int rawIndex = RawList.IndexOf(Path.Combine(RawPath, rawName));
                    if (rawIndex > -1)
                    {
                        string tarRaw = Path.Combine(TargetPath, rawName);
                        if (File.Exists(tarRaw))
                            ExistedRawList[RawList[rawIndex]] = tarRaw;
                        else
                            work(RawList[rawIndex], tarRaw);

                        JpgList.RemoveAt(i);
                        RawList.RemoveAt(rawIndex);
                    }
                    Progress++;
                }
                else
                {
                    break;
                }
            }

            JpgResidueCount = JpgList.Count;
            RawResidueCount = RawList.Count;

            IsIdel = true;

            if (ExistedRawList.Count > 0)
            {
                string msg = $"以下文件已存在，是否覆盖？\n{string.Join("\n", ExistedRawList)}";
                if (MessageBox.Show(msg, "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Parallel.ForEach(ExistedRawList, pair => 
                    { 
                        File.Delete(pair.Value);
                        work(pair.Key, pair.Value);
                    });
                }
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private void Refresh()
        {
            JpgList = new ObservableCollection<string>(
                Directory.GetFiles(JpgPath, $"*.{JpgSuffix}"));
            JpgTotalCount   = JpgList.Count;
            JpgResidueCount = JpgList.Count;

            RawList = new ObservableCollection<string>(
                Directory.GetFiles(RawPath, $"*.{RawSuffix}"));
            RawTotalCount   = RawList.Count;
            RawResidueCount = RawList.Count;
        }
        #endregion

        #region 选择路径

        [RelayCommand]
        private void ChooseJpgPath()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "请选择jpg路径"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
            {
                JpgPath = dialog.FileName;
                JpgList = new ObservableCollection<string>(
                    Directory.GetFiles(JpgPath, $"*.{JpgSuffix}"));
                JpgTotalCount   = JpgList.Count;
                JpgResidueCount = JpgList.Count;
            }
        }

        [RelayCommand]
        private void ChooseRawPath()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "请选择raw路径"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
            {
                RawPath = dialog.FileName;
                RawList = new ObservableCollection<string>(
                    Directory.GetFiles(RawPath, $"*.{RawSuffix}"));
                RawTotalCount   = RawList.Count;
                RawResidueCount = RawList.Count;
            }
        }

        [RelayCommand]
        private void ChooseTargetPath()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "请选择目标路径"
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok && dialog.FileName != null)
            {
                TargetPath = dialog.FileName;
            }
        }

        #endregion
    }
}
