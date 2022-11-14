using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using MyUWPLibrary;

using RepeatItemChecker.Models;

using Windows.Storage;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemChecker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        internal ObservableCollection<StorageFile> storageFiles = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFolder> storageFolders = new ObservableCollection<StorageFolder>();

        internal ObservableCollection<FolderConFile> folderConFiles = new ObservableCollection<FolderConFile>();

        public ObservableCollection<ItemGroup> pairs = new ObservableCollection<ItemGroup>();

        private StorageFolder ConfigurationFolder;

        private FolderConFile CurrentConfiguration;
        public Page1 ()
        {
            this.InitializeComponent();
             ConfigurationFolder = Windows.Storage.ApplicationData.Current.LocalFolder .CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists).AsTask().Result;

        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
                var files = await ConfigurationFolder.GetFilesAsync();
                foreach (var file in files)
                {
                    var confile = FolderConFile.Read(file.Path);

                    folderConFiles.Add(confile);
                }
        }

        private async void PickFolder (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await StorageItemPicker.PickSingleFolderAsync(Windows.Storage.Pickers.PickerLocationId.Desktop);
            if (folder != null)
            {

                if (!storageFolders.Contains(folder))
            {            
                var token = FutureAccessList.Add(folder);
                CurrentConfiguration.AddToken(token);

                storageFolders.Add(folder);
            }

            }


        }

        private async void Start (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FolderConFile.SaveFile(ConfigurationFolder.Path, CurrentConfiguration);

            storageFiles.Clear();
            pairs.Clear();

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

        private async void AddConfiguration (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

#if DEBUG
            await Windows.System.Launcher.LaunchFolderAsync(ConfigurationFolder);
#endif

            var text = NewConFileNameInput.Text;
            Guid guid = Guid.NewGuid();
            var file = await ConfigurationFolder.CreateFileAsync(guid.ToString(), CreationCollisionOption.GenerateUniqueName);
            FolderConFile folderConFile = new FolderConFile( guid.ToString(),text);
            folderConFiles.Add(folderConFile);

            var a = ConfigurationComboBox.Items.IndexOf(folderConFile);
            ConfigurationComboBox.SelectedIndex = a;
        }

        private async void RemoveConFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (folderConFiles.Count != 0)
            {

            var item = ConfigurationComboBox.SelectedItem as FolderConFile;
            var file = await ConfigurationFolder.GetFileAsync(item.Guid);
            await file.DeleteAsync( StorageDeleteOption.PermanentDelete);
            folderConFiles.Remove(item);
            }
        }

        private async void ConfigurationComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if(ConfigurationComboBox.SelectedItem != null)
            {
                pairs.Clear();
                storageFolders.Clear();
                storageFiles.Clear();

                CurrentConfiguration = ConfigurationComboBox.SelectedItem as FolderConFile;

                foreach(var token in CurrentConfiguration.FolderTokens)
                {
                    var folder=await FutureAccessList.GetFolderAsync(token);
                    storageFolders.Add(folder);

                }         

            }
        }

        private void RemoveFolder (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO
        }

       
    }

    
}