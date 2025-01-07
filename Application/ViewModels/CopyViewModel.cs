using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using PhotoTools.Models;
using PhotoTools.Application.Dialog;

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
            JpgPath.IsInvalidPath() && !string.IsNullOrWhiteSpace(JpgSuffix) &&
            RawPath.IsInvalidPath() && !string.IsNullOrWhiteSpace(RawSuffix) &&
            TargetPath.IsInvalidPath();

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
                var dialog = new ConverExistedFileDialog(ExistedRawList.Values);
                if (dialog.ShowDialog() == true)
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
        private void StopWork()
        {
            ContinueFlag = false;
        }

        #endregion

        #region 刷新文件信息
        [RelayCommand]
        private void Refresh()
        {
            JpgList = new ObservableCollection<string>(Directory.GetFiles(JpgPath, $"*.{JpgSuffix}"));
            JpgTotalCount = JpgList.Count;
            // 计算在jpg中找不到对应的raw文件
            JpgResidueCount = 0;
            foreach (var jpg in JpgList)
            {
                string raw = Path.ChangeExtension(jpg, RawSuffix);
                if (!RawList.Contains(Path.Combine(RawPath, Path.GetFileName(raw))))
                    ++JpgResidueCount;
            }


            RawList = new ObservableCollection<string>(Directory.GetFiles(RawPath, $"*.{RawSuffix}"));
            RawTotalCount = RawList.Count;
            // 计算在raw中找不到对应的jpg文件
            RawResidueCount = 0;
            foreach (var raw in RawList)
            {
                string jpg = Path.ChangeExtension(raw, JpgSuffix);
                if (!JpgList.Contains(Path.Combine(JpgPath, Path.GetFileName(jpg))))
                    ++RawResidueCount;
            }
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

                if (RawPath.IsInvalidPath() && !string.IsNullOrWhiteSpace(RawSuffix))
                    Refresh();
                else
                {
                    JpgList = new ObservableCollection<string>(Directory.GetFiles(JpgPath, $"*.{JpgSuffix}"));
                    JpgTotalCount = JpgList.Count;
                    JpgResidueCount = 0;
                }
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
                if (JpgPath.IsInvalidPath() && !string.IsNullOrWhiteSpace(JpgSuffix))
                    Refresh();
                else
                {
                    RawList = new ObservableCollection<string>(Directory.GetFiles(RawPath, $"*.{RawSuffix}"));
                    RawTotalCount = RawList.Count;
                    RawResidueCount = 0;
                }
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
