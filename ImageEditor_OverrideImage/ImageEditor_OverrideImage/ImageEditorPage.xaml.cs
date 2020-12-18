using Syncfusion.UI.Xaml.ImageEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ImageEditor_OverrideImage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageEditorPage : Page
    {
        private MessageDialog showDialog;      
        public ImageEditorPage()
        {
            this.InitializeComponent();
            
            editor.Image = App.Stream.AsRandomAccessStream();
            editor.ImageSaving += Editor_ImageSaving;
        }


        private void Editor_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            args.Cancel = true;

            Dialog(args.Stream);
        }

        private async void Dialog(Stream stream)
        {
            showDialog = new MessageDialog("Image has been Override" + "" + App.FilePath);
            showDialog.Commands.Add(new UICommand("Yes")
            {
                Id = 0
            });
            showDialog.Commands.Add(new UICommand("No")
            {
                Id = 1
            });
            var result = await showDialog.ShowAsync();
            if ((int)result.Id == 0)
            {
                Save(stream);
            }
        }

        public async void Save(Stream stream)
        {
            var filePath = App.FilePath;
            IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
            var wbm = new WriteableBitmap(600, 800);
            await wbm.SetSourceAsync(randomAccessStream);
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".jpg");
            string PathName = Path.GetDirectoryName(filePath);
            string FileName = Path.GetFileName(filePath);
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(@"" + PathName);
            if (folder != null)
            {
                StorageFile file = await folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);
                using (var storageStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, storageStream);
                    var pixelStream = wbm.PixelBuffer.AsStream();
                    var pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wbm.PixelWidth, (uint)wbm.PixelHeight, 200, 200, pixels);
                    await encoder.FlushAsync();
                }
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
    }
}
