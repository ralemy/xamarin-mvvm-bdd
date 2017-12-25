using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MVVMFramework;
using MVVMFramework.Statics;
using ZXing;

namespace example.ViewModels
{
    public class BarcodePageVM : ViewModelBase
    {
        private string _TopText = UIStrings.TopTextDefault;
        public String TopText
        {
            get => _TopText;
            set => Set(() => TopText, ref _TopText, value);
        }

        private string _BottomText = UIStrings.BottomTextDefault;
        public String BottomText
        {
            get => _BottomText;
            private set => Set(() => BottomText, ref _BottomText, value);
        }

        private bool _IsTorchOn = false;
        public bool IsTorchOn
        {
            get => _IsTorchOn;
            set => Set(() => IsTorchOn, ref _IsTorchOn, value);
        }

        internal void Disappear()
        {
            IsAnalyzing = false;
            IsScanning = false;
        }

        internal void Appear()
        {
            IsScanning = true;
            IsAnalyzing = true;
            TopText = UIStrings.TopTextDefault;
            BottomText = UIStrings.BottomTextDefault;
        }

        private bool _IsScanning = false;
        public bool IsScanning
        {
            get => _IsScanning;
            set => Set(() => IsScanning, ref _IsScanning, value);
        }

        private bool _IsAnalyzing = false;
        public bool IsAnalyzing
        {
            get => _IsAnalyzing;
            set => Set(() => IsAnalyzing, ref _IsAnalyzing, value);
        }

        private bool _ShowFlashButton = true;
        public bool ShowFlashButton
        {
            get => _ShowFlashButton;
            set => Set(() => ShowFlashButton, ref _ShowFlashButton, value);
        }

        private Result _scanResult;
        public Result ScanResult
        {
            get => _scanResult;
            set => Set(() => ScanResult, ref _scanResult, value);
        }


        private INavigationService navigator;
        private IUIRunner runner;

        public ICommand ScanResultCommand { get; private set; }
        public ICommand FlashOperation { get; private set; }

        public BarcodePageVM(INavigationService navigator)
        {
            this.navigator = navigator;
            this.runner = Initializer.GetDependency<IUIRunner>();
            ScanResultCommand = new RelayCommand<Result>(OnScanResult);
            FlashOperation = new RelayCommand(OperationFlash);
        }

        private void OperationFlash()
        {
        }

        public void OnScanResult(Result r)
        {
            IsAnalyzing = false;
            IsScanning = false;
            runner.RunOnUIThread(navigator.GoBack);
        }
    }
}
