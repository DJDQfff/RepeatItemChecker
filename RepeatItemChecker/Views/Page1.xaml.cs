using System;
using System.Collections.ObjectModel;
using System.Linq;

using MyUWPLibrary;

using RepeatItemChecker.UserControls;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemChecker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        public ObservableCollection <StorageFile> storageFiles = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFolder> storageFolders = new ObservableCollection<StorageFolder>();

        public Page1 ()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            #region 选择要遍历的文件夹
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
            #region 遍历选择的文件夹
            foreach (var folder in storageFolders)
            {
                var files = await folder.GetAllStorageFiles();

                foreach (var file in files)
                {
                    storageFiles.Add(file);
                }
            }

            #region 按文件大小分组排序
            var c = storageFiles.GroupBy(async n => (await n.GetBasicPropertiesAsync()).Size);
            foreach (var cc in c)
            {
                MyUserControl1 myUserControl1 = new MyUserControl1(cc.ToArray());
                await myUserControl1.Initial();
            }
        }

        private async void Button_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var j = sender as Button;
            var a = j.DataContext as StorageFile;
            await Windows.System.Launcher.LaunchFileAsync(a);
        }
    }
}