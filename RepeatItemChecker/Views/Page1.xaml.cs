using System;
using System.Collections.ObjectModel;
using System.Linq;

using FolderGroupDB;

using MyUWPLibrary;

using RepeatItemChecker.Models;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static Windows.Storage.AccessCache.StorageApplicationPermissions;

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

        internal ObservableCollection<FoldersGroup> Groups = new ObservableCollection<FoldersGroup>();

        public ObservableCollection<RepeaStorageFileGroup> RepeatPairs = new ObservableCollection<RepeaStorageFileGroup>();

        private StorageFolder configurationFileFolder;

        private FoldersGroup currentConfiguration;

        private Database_1 database;
        public Page1 ()
        {
            this.InitializeComponent();
            database= new Database_1 ();
            configurationFileFolder = Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("Configuration" , CreationCollisionOption.OpenIfExists).AsTask().Result;
        }

        protected override  void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

                var groups = database.folderGroups.ToArray();
                foreach (var group in groups)
                {
                    Groups.Add(group);
                }
        }

        private async void PickFolder (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var folder = await StorageItemPicker.OpenSingleFolderAsync();
            if (folder != null)
            {
                if (!StorageFolders.Contain(folder))
                {
                    var token = FutureAccessList.Add(folder);
                    currentConfiguration.AddToken(token);

                        database.SaveChanges();

                    StorageFolders.Add(folder);
                }
            }
        }

        private async void Start (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            ProgressRingUI.Visibility = Windows.UI.Xaml.Visibility.Visible;

            StorageFiles.Clear();
            RepeatPairs.Clear();

            foreach (var folder in StorageFolders)
            {
                var files = await folder.GetAllStorageItems();

                foreach (var file in files.Item2)
                {
                    StorageFiles.Add(file);
                }
            }

            var c = StorageFiles.GroupBy(n => n.GetBasicPropertiesAsync().AsTask().Result.Size);
            foreach (var cc in c)
            {
                if (cc.Count() > 1)
                { 
                    var item = new RepeaStorageFileGroup(cc);
                    RepeatPairs.Add(item);
                }
            }

            ProgressRingUI.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void Button_Click_LaunchFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var j = sender as Button;
            var a = j.DataContext as StorageFile;
            await Windows.System.Launcher.LaunchFileAsync(a);
        }

        private async void DeleteFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var file = button.DataContext as StorageFile;

            DeleteStorageFileInRootObservable(file);

            await file.DeleteAsync(StorageDeleteOption.Default);

            button.IsEnabled = false;
        }

        private void AddConfiguration (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var text = NewConFileNameInput.Text;
            FoldersGroup folderConFile = new FoldersGroup(text);


                database.Add(folderConFile);
                database.SaveChanges();

            Groups.Add(folderConFile);

            var a = Groups.IndexOf(folderConFile);

            ConfigurationComboBox.SelectedIndex = a;

            NewConFileNameInput.Text = String.Empty;
        }

        private void RemoveConFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Groups.Count != 0)
            {
                var item = ConfigurationComboBox.SelectedItem as FoldersGroup;


                    database.Remove(item);
                    database.SaveChanges();

                Groups.Remove(item);
            }
        }

        private async void ConfigurationComboBox_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            if (ConfigurationComboBox.SelectedItem != null)
            {
                RepeatPairs.Clear();
                StorageFolders.Clear();
                StorageFiles.Clear();

                currentConfiguration = ConfigurationComboBox.SelectedItem as FoldersGroup;

                foreach (var token in currentConfiguration.Tokens)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        var folder = await FutureAccessList.GetFolderAsync(token);
                        StorageFolders.Add(folder);
                    }
                }
            }
        }

        private void RemoveFolder (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var item = ConfigurationComboBox.SelectedItem as FoldersGroup;
            if (item != null)
            {
                    item.ClearAllTokens();
                    database.SaveChanges();

                StorageFolders.Clear();
            }
        }

        private void DeleteStorageFileInRootObservable (StorageFile storageFile)
        {
            for (int index = RepeatPairs.Count - 1 ; index >= 0 ; index--)
            {
                var item = RepeatPairs[index];

                var count = item.TryRemoveItem(storageFile);

                if (count == 1)
                {
                    RepeatPairs.Remove(item);
                }
            }
        }

    }
}