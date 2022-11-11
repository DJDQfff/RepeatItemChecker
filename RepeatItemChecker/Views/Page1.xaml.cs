using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using MyUWPLibrary;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemChecker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        public ObservableCollection<StorageFile> storageFiles = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFolder> storageFolders = new ObservableCollection<StorageFolder>();

        public ObservableCollection<ItemGroup> pairs = new ObservableCollection<ItemGroup>();

        public Page1 ()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 加载image，目前无用，loaded事件存在问题，不会用，涉及到一个路由事件的东西
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Loaded (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var file = this.DataContext as StorageFile;
            Image image = sender as Image;
            var thumbnail = file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem).AsTask().Result;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            image.Source = bitmapImage;
        }

        private async void PickFolders (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            while (true)
            {
                var folder = await StorageItemPicker.PickSingleFolderAsync(Windows.Storage.Pickers.PickerLocationId.Desktop);
                if (folder != null)
                {
                    storageFolders.Add(folder);
                }
                else
                    break;
            }
        }

        private async void Start (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (var folder in storageFolders)
            {
                var files = await folder.GetAllStorageFiles();

                foreach (var file in files)
                {
                    storageFiles.Add(file);
                }
            }

            var c = storageFiles.GroupBy(n => n.GetBasicPropertiesAsync().AsTask().Result.Size);
            foreach (var cc in c)
            {
                if (cc.Count() > 1)
                {
                    var item = new ItemGroup(cc);
                    pairs.Add(item);
                }
            }
        }

        private async void Button_Click_LaunchFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var j = sender as Button;
            var a = j.DataContext as StorageFile;
            await Windows.System.Launcher.LaunchFileAsync(a);
        }

        private async void DeleteFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var file = button.DataContext as StorageFile;
            await file.DeleteAsync(StorageDeleteOption.Default);
            button.IsEnabled = false;
        }
    }

    public class ItemGroup : IGrouping<ulong, StorageFile>
    {
        private IGrouping<ulong, StorageFile> files;

        public ItemGroup (IGrouping<ulong, StorageFile> _files)
        {
            files = _files;
            StorageFiles = _files.ToArray();
        }

        public ulong Key => files.Key;
        public StorageFile[] StorageFiles { set; get; }

        public IEnumerator<StorageFile> GetEnumerator () => files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator ()
        {
            throw new NotImplementedException();
        }
    }
}