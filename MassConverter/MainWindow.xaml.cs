using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace MassConverter {
    public partial class MainWindow {
        public List<SupportedImageFormats> SourceFormats { get; set; }
        public SupportedImageFormats OutputFormat { get; set; }
        public bool HasStop { get; set; }

        public MainWindow() {
            InitializeComponent();
            SourceFormats = new List<SupportedImageFormats>();
        }

        private void BtnOpenSourceDirOnClick(object sender, RoutedEventArgs e) {
            var dlg = new VistaFolderBrowserDialog();
            var showDialog = dlg.ShowDialog(this);
            if (showDialog != null && !(bool)showDialog) return;
            TxtSourceDir.Text = dlg.SelectedPath;
        }

        private void BtnOpenOutputDirOnClick(object sender, RoutedEventArgs e) {
            var dlg = new VistaFolderBrowserDialog();
            var showDialog = dlg.ShowDialog(this);
            if (showDialog != null && !(bool)showDialog) return;
            TxtOutputDir.Text = dlg.SelectedPath;
        }

        private void SourceFormatOnChecked(object sender, RoutedEventArgs e) {
            var checkBox = (CheckBox)sender;
            switch ((string)checkBox.Tag) {
                case "dds":
                    if (SourceFormats.All(t => t != SupportedImageFormats.Dds))
                        SourceFormats.Add(SupportedImageFormats.Dds);
                    break;
                case "png":
                    if (SourceFormats.All(t => t != SupportedImageFormats.Png))
                        SourceFormats.Add(SupportedImageFormats.Png);
                    break;
                case "jpg":
                    if (SourceFormats.All(t => t != SupportedImageFormats.Jpg))
                        SourceFormats.Add(SupportedImageFormats.Jpg);
                    break;
                case "gif":
                    if (SourceFormats.All(t => t != SupportedImageFormats.Gif))
                        SourceFormats.Add(SupportedImageFormats.Gif);
                    break;
                case "bmp":
                    if (SourceFormats.All(t => t != SupportedImageFormats.Bmp))
                        SourceFormats.Add(SupportedImageFormats.Bmp);
                    break;
            }
        }

        private void SourceFormatOnUnchecked(object sender, RoutedEventArgs e) {
            var checkBox = (CheckBox)sender;
            switch ((string)checkBox.Tag) {
                case "dds":
                    SourceFormats.Remove(SupportedImageFormats.Dds);
                    break;
                case "png":
                    SourceFormats.Remove(SupportedImageFormats.Png);
                    break;
                case "jpg":
                    SourceFormats.Remove(SupportedImageFormats.Jpg);
                    break;
                case "gif":
                    SourceFormats.Remove(SupportedImageFormats.Gif);
                    break;
                case "bmp":
                    SourceFormats.Remove(SupportedImageFormats.Bmp);
                    break;
            }
        }

        private void OutputFormatOnChecked(object sender, RoutedEventArgs e) {
            var checkBox = (RadioButton)sender;
            switch ((string)checkBox.Tag) {
                case "dds":
                    OutputFormat = SupportedImageFormats.Dds;
                    break;
                case "png":
                    OutputFormat = SupportedImageFormats.Png;
                    break;
                case "jpg":
                    OutputFormat = SupportedImageFormats.Jpg;
                    break;
                case "gif":
                    OutputFormat = SupportedImageFormats.Gif;
                    break;
                case "bmp":
                    OutputFormat = SupportedImageFormats.Bmp;
                    break;
            }
        }

        private void BtnConvertOnClick(object sender, RoutedEventArgs e) {
            if (!Directory.Exists(TxtSourceDir.Text)) {
                MessageBox.Show(this, "Source folder not set!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Directory.Exists(TxtOutputDir.Text)) {
                MessageBox.Show(this, "Output folder not set!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SourceFormats.Count == 0) {
                MessageBox.Show(this, "Please selected at least one source image format!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (OutputFormat == 0) {
                MessageBox.Show(this, "Please selected output image format!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var converter = new ImageConverter(this);
            converter.ConvertImagesFromFolder(TxtSourceDir.Text, TxtOutputDir.Text, SourceFormats, OutputFormat);
        }

        public void StateControl(bool isConverting) {
            GbConvertOptions.IsEnabled = !isConverting;
            BtnOpenSourceDir.IsEnabled = !isConverting;
            BtnOpenOutputDir.IsEnabled = !isConverting;
            BtnConvert.IsEnabled = !isConverting;
            BtnStop.IsEnabled = isConverting;
        }

        private void BtnStopOnClick(object sender, RoutedEventArgs e) {
            HasStop = true;
            StateControl(false);
        }
    }
}
