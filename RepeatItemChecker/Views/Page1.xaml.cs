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
        internal ObservableCollection<StorageFile> StorageFiles = new ObservableCollection<StorageFile>();
        public ObservableCollection<StorageFolder> StorageFolders = new ObservableCollection<StorageFolder>();

        internal ObservableCollection<FoldersConfiguration> ConfigurationFiles = new ObservableCollection<FoldersConfiguration>();

        public ObservableCollection<RepeatItemGroup> RepeatPairs = new ObservableCollection<RepeatItemGroup>();

        private StorageFolder configurationFileFolder;

        private FoldersConfiguration currentConfiguration;
        public Page1 ()
        {
            this.InitializeComponent();
             configurationFileFolder = Windows.Storage.ApplicationData.Current.LocalFolder .CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists).AsTask().Result;

        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
                var files = await configurationFileFolder.GetFilesAsync();
                foreach (var file in files)
                {
                    var confile = FoldersConfiguration.Read(file.Path);

                    ConfigurationFiles.Add(confile);
                }
        }

        private async void PickFolder (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await StorageItemPicker.PickSingleFolderAsync(Windows.Storage.Pickers.PickerLocationId.Desktop);
            if (folder != null)
            {

                if (!StorageFolders.Contain(folder))
            {            
                var token = FutureAccessList.Add(folder);
                currentConfiguration.AddToken(token);

                StorageFolders.Add(folder);
            }

            }


        }

        private async void Start (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ProgressRingUI.Visibility = Windows.UI.Xaml.Visibility.Visible;

            FoldersConfiguration.SaveFile(configurationFileFolder.Path, currentConfiguration);

            StorageFiles.Clear();
            RepeatPairs.Clear();

            foreach (var folder in StorageFolders)
            {
                var files = await folder.GetAllStorageFiles();

                foreach (var file in files)
                {
                    StorageFiles.Add(file);
                }
            }



            var c = StorageFiles.GroupBy(n => n.GetBasicPropertiesAsync().AsTask().Result.Size);
            foreach (var cc in c)
            {
                if (cc.Count() > 1)
                {
                    var item = new RepeatItemGroup(cc);
                    RepeatPairs.Add(item);
                }
            }

            ProgressRingUI.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
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

            
            DeleteStorageFileInRootObservable(file);


            await file.DeleteAsync(StorageDeleteOption.Default);
            
            button.IsEnabled = false;
        }

        private async void AddConfiguration (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if DEBUG
            await Windows.System.Launcher.LaunchFolderAsync(configurationFileFolder);
#endif

            var text = NewConFileNameInput.Text;
            Guid guid = Guid.NewGuid();
            var file = await configurationFileFolder.CreateFileAsync(guid.ToString(), CreationCollisionOption.GenerateUniqueName);
            FoldersConfiguration folderConFile = new FoldersConfiguration( guid.ToString(),text);
            ConfigurationFiles.Add(folderConFile);

            var a = ConfigurationComboBox.Items.IndexOf(folderConFile);
            ConfigurationComboBox.SelectedIndex = a;

            NewConFileNameInput.Text = String.Empty;

        }

        private async void RemoveConFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (ConfigurationFiles.Count != 0)
            {

            var item = ConfigurationComboBox.SelectedItem as FoldersConfiguration;
            var file = await configurationFileFolder.GetFileAsync(item.Guid);
            await file.DeleteAsync( StorageDeleteOption.PermanentDelete);
            ConfigurationFiles.Remove(item);
            }
        }

        private async void ConfigurationComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if(ConfigurationComboBox.SelectedItem != null)
            {
                RepeatPairs.Clear();
                StorageFolders.Clear();
                StorageFiles.Clear();

                currentConfiguration = ConfigurationComboBox.SelectedItem as FoldersConfiguration;

                foreach(var token in currentConfiguration.FolderTokens)
                {
                    var folder=await FutureAccessList.GetFolderAsync(token);
                    StorageFolders.Add(folder);

                }         

            }
        }

        private void RemoveFolder (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // TODO
        }

        private void DeleteStorageFileInRootObservable (StorageFile storageFile)
        {
            for(int index = RepeatPairs.Count-1; index >= 0; index--)
            {
                var item=RepeatPairs[index];

                var count = item.TryRemoveItem(storageFile);

                if (count == 1)
                {
                    RepeatPairs.Remove(item);
                }
            }
        }
       
    }

    
}