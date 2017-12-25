using System;
using example.ViewModels;
using MVVMFramework;
using MVVMFramework.Statics;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace example.Pages
{
    public class BarcodePage : ContentPage
    {
        public static readonly string PageKey = PageKeys.BarcodePage;
        private BarcodePageVM ViewModel;
        private ZXingDefaultOverlay overlay;
        private ZXingScannerView zxing;


        public BarcodePage() : base()
        {
            AutomationId = PageKey;
            ViewModel = Initializer.GetDependency<BarcodePageVM>();
            Content = MakeScannerGrid();
            BindingContext = ViewModel;
            DelegateBaseEvents();
        }

        private void DelegateBaseEvents()
        {
            base.Appearing += (object sender, EventArgs e) => ViewModel.Appear();
            base.Disappearing += (object sender, EventArgs e) => ViewModel.Disappear();
        }

        private View MakeScannerGrid()
        {
            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            grid.Children.Add(InitZXingView());
            grid.Children.Add(InitOverlay());
            grid.BindingContext = ViewModel;
            return grid;
        }

        private View InitOverlay()
        {
            overlay = new ZXingDefaultOverlay
            {
                ShowFlashButton = zxing.HasTorch,
                BindingContext = ViewModel
            };

            overlay.FlashButtonClicked += (sender, e) =>
                zxing.IsTorchOn = !zxing.IsTorchOn;

            overlay.SetBinding(ZXingDefaultOverlay.TopTextProperty, "TopText");
            overlay.SetBinding(ZXingDefaultOverlay.BottomTextProperty, "BottomText");
            overlay.SetBinding(ZXingDefaultOverlay.FlashCommandProperty, "FlashOperation");
            return overlay;
        }

        private View InitZXingView()
        {
            zxing = new ZXingScannerView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BindingContext = ViewModel
            };
            zxing.SetBinding(ZXingScannerView.IsScanningProperty, new Binding(nameof(IsScanning)));
            this.SetBinding(IsScanningProperty, nameof(IsScanning));
            zxing.SetBinding(ZXingScannerView.IsAnalyzingProperty, new Binding(nameof(IsAnalyzing)));
            this.SetBinding(IsAnalyzingProperty, nameof(IsAnalyzing));
            zxing.SetBinding(ZXingScannerView.IsTorchOnProperty, new Binding(nameof(IsTorchOn)));
            this.SetBinding(IsTorchOnProperty, nameof(IsTorchOn));

            zxing.SetBinding(ZXingScannerView.ScanResultCommandProperty, "ScanResultCommand");
            zxing.SetBinding(ZXingScannerView.ResultProperty, "ScanResult");
            ViewModel.ShowFlashButton = zxing.HasTorch;
            return zxing;
        }


        public static readonly BindableProperty IsScanningProperty =
            BindableProperty.Create(nameof(IsScanning), typeof(bool), typeof(BarcodePage), true);

        public bool IsScanning
        {
            get { return (bool)GetValue(IsScanningProperty); }
            set { SetValue(IsScanningProperty, value); }
        }

        public static readonly BindableProperty IsTorchOnProperty =
    BindableProperty.Create(nameof(IsTorchOn), typeof(bool), typeof(BarcodePage), true);

        public bool IsTorchOn
        {
            get { return (bool)GetValue(IsTorchOnProperty); }
            set { SetValue(IsTorchOnProperty, value); }
        }


        public static readonly BindableProperty IsAnalyzingProperty =
            BindableProperty.Create(nameof(IsAnalyzing), typeof(bool), typeof(BarcodePage), true);
        public bool IsAnalyzing
        {
            get { return (bool)GetValue(IsAnalyzingProperty); }
            set { SetValue(IsAnalyzingProperty, value); }
        }
    }
}

