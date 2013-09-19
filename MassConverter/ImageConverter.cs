using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using FreeImageAPI;

namespace MassConverter {
    public class ImageConverter {
        public MainWindow MainWindow { get; set; }

        public ImageConverter(MainWindow mainWindow) {
            MainWindow = mainWindow;
        }

        public void ConvertImagesFromFolder(string sourceFolder, string outputFolder, List<SupportedImageFormats> sourceFormats, SupportedImageFormats outputFormat) {
            MainWindow.StateControl(true);
            MainWindow.TxtProgressText.Visibility = Visibility.Visible;
            MainWindow.TxtProgressText.Text = "Search files, please wait...";
            MainWindow.TxtProgressText.Foreground = new SolidColorBrush(Colors.Blue);
            DoEvents();
            var foundFiles = new List<string>();
            foreach (var sourceFormat in sourceFormats) {
                var mask = string.Format("*.{0}", sourceFormat.ToString().ToLower());
                foundFiles.AddRange(Directory.GetFiles(sourceFolder, mask, SearchOption.AllDirectories));
            }
            MainWindow.PbProgress.Visibility = Visibility.Visible;
            MainWindow.PbProgress.Value = 0;
            MainWindow.PbProgress.Maximum = foundFiles.Count;
            MainWindow.HasStop = false;
            foreach (var foundFile in foundFiles) {
                if (MainWindow.HasStop) break;
                MainWindow.TxtProgressText.Text = string.Format("{2:0.0}% Convert image {0} to {1}...",
                    MainWindow.PbProgress.Value,
                    MainWindow.PbProgress.Maximum,
                    MainWindow.PbProgress.Value / MainWindow.PbProgress.Maximum * 100);
                MainWindow.TxtProgressText.Foreground = new SolidColorBrush(Colors.SeaGreen);
                MainWindow.PbProgress.Value++;
                DoEvents();
                using (var fs = File.Open(foundFile, FileMode.Open)) {
                    FREE_IMAGE_FORMAT targetFormat;
                    switch (MainWindow.OutputFormat) {
                        case SupportedImageFormats.Dds: targetFormat = FREE_IMAGE_FORMAT.FIF_DDS; break;
                        case SupportedImageFormats.Png: targetFormat = FREE_IMAGE_FORMAT.FIF_PNG; break;
                        case SupportedImageFormats.Jpg: targetFormat = FREE_IMAGE_FORMAT.FIF_JPEG; break;
                        case SupportedImageFormats.Gif: targetFormat = FREE_IMAGE_FORMAT.FIF_GIF; break;
                        case SupportedImageFormats.Bmp: targetFormat = FREE_IMAGE_FORMAT.FIF_BMP; break;
                        default: continue;
                    }
                    var tmp = foundFile.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    var foundFileExt = tmp[tmp.Length - 1];
                    var outputFileName = foundFile.Replace(string.Format(@"{0}\", sourceFolder), "")
                                        .Replace(@"\", "_")
                                        .Replace(string.Format(".{0}", foundFileExt), string.Format(".{0}", outputFormat.ToString().ToLower()));
                    outputFileName = string.Format(@"{0}\{1}", outputFolder, outputFileName);
                    try {
                        var output = FreeImage.LoadFromStream(fs);
                        if (output.IsNull) continue;
                        FreeImage.Save(targetFormat, output, outputFileName, FREE_IMAGE_SAVE_FLAGS.DEFAULT);
                    } catch (Exception ex) { }
                    ClearMemory();
                }
            }
            MainWindow.PbProgress.Visibility = Visibility.Collapsed;
            MainWindow.TxtProgressText.Visibility = Visibility.Collapsed;
            MainWindow.StateControl(false);
        }

        public static void DoEvents() {
            var f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg) {
                var fr = arg as DispatcherFrame;
                if (fr != null) fr.Continue = false;
            },
            f);
            Dispatcher.PushFrame(f);
        }
        [DllImport("kernel32.dll")]
        public static extern bool SetProcessWorkingSetSize(IntPtr handle, int minimumWorkingSetSize, int maximumWorkingSetSize);

        public static void ClearMemory() {
            GC.Collect();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }
    }
}
