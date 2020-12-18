# How-to-override-the-original-image-with-the-edited-image-in-an-image-editor-SfImageEditor-UWP
By default, saved image from SfImageEditor will choose the path from Pictures directory of your environment as mentioned in below

N> The saved image will be added to default pictures library “C:\Users\<your name>\Pictures\Saved Pictures”.

This article explains how to override the existing original image with the edited image in [Syncfusion UWP SfImageEditor control](https://help.syncfusion.com/uwp/image-editor/getting-started) with follows.

Subscribe the ImageSaving event with initializing image editor control and get the stream from it and pass that to the below method to change the saved path location

[C#]

```
public async void Save(Stream stream)
        {
            IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
            var wbm = new WriteableBitmap(600, 800);
            await wbm.SetSourceAsync(randomAccessStream);
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".jpg");
            string PathName = Path.GetDirectoryName(FilePath);
            string FileName = Path.GetFileName(FilePath);
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
```

## See also

[How to save the image in SfImageEditor control programmatically](https://help.syncfusion.com/uwp/image-editor/saveandresetevents#using-code)

[How to customize the default toolbar in SfImageEditor UWP](https://help.syncfusion.com/uwp/image-editor/toolbarcustomization)

[How to enable the zooming in SfImageEditor UWP](https://help.syncfusion.com/uwp/image-editor/zooming#enable-zooming)
