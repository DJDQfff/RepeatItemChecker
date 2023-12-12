// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemsChecker.WinUI.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        private ItemsGroupsViewModel<string , string , RepeaStorageFileGroup> _viewModel;

        /// <summary>
        /// 所有要进行比较的文件
        /// </summary>
        private ObservableCollection<string> StorageFiles = new();

        /// <summary>
        /// 所有要进行遍历的文件夹
        /// </summary>
        public ObservableCollection<StorageFolder> StorageFolders = new();

        /// <summary>
        /// 程序存储的检查组合
        /// </summary>
        internal ObservableCollection<FoldersGroup> Groups = new();

        private StorageFolder configurationFileFolder;

        private FoldersGroup currentConfiguration;

        private Database_1 database;

        public Page1 ()
        {
            this.InitializeComponent();
            database = new Database_1();
            configurationFileFolder = ApplicationData.Current.LocalFolder.CreateFolderAsync("Configuration" , CreationCollisionOption.OpenIfExists).AsTask().Result;
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var groups = database.folderGroups.ToArray();
            foreach (var group in groups)
            {
                Groups.Add(group);
            }
        }

        private async void PickFolder (object sender , RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            var handle = WindowNative.GetWindowHandle((App.Current as App).MainWindow);
            InitializeWithWindow.Initialize(picker , handle);
            picker.FileTypeFilter.Add("*");
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                if (!StorageFolders.Contains(folder))
                {
                    var token = FutureAccessList.Add(folder);
                    currentConfiguration.AddToken(token);

                    database.SaveChanges();

                    StorageFolders.Add(folder);
                }
            }
        }

        private void Start (object sender , RoutedEventArgs e)
        {
            LoadingControl.IsLoading = true;
            StorageFiles.Clear();

            foreach (var folder in StorageFolders)
            {
                var files = Directory.GetFiles(folder.Path , "*" , SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    StorageFiles.Add(file);
                }
            }

            Func<string , string> onlyFileSize = n => new FileInfo(n).Length.ToString();

            var bool1 = CheckBox_FileSize.IsChecked;
            var bool2 = CheckBox_Md5.IsChecked;

            _viewModel = new ItemsGroupsViewModel<string , string , RepeaStorageFileGroup>(StorageFiles , onlyFileSize);

            if (bool2.Value)
            {
                Func<string , string> sha256 = (x) =>
                {
                    string hash;
                    using (var Stream = new FileStream(x , FileMode.Open , FileAccess.Read))
                    {
                        using (SHA256 sHA256 = SHA256.Create())
                        {
                            var a = sHA256.ComputeHash(Stream);
                            hash = BitConverter.ToString(a).Replace("-" , "");
                        }
                    }

                    return hash;
                };

                _viewModel = new ItemsGroupsViewModel<string , string , RepeaStorageFileGroup>(_viewModel.AllElements , sha256);
            }

            SameItemList.ItemsSource = _viewModel.RepeatPairs;
            CountTextBlock.Text = _viewModel.Count.ToString();

            LoadingControl.IsLoading = false;
            new ToastContentBuilder().AddText("完成").Show();
        }

        private async void Button_Click_LaunchFile (object sender , RoutedEventArgs e)
        {
            var j = sender as Button;
            switch (j.DataContext)
            {
                case string str:
                    System.Diagnostics.Process.Start("explorer" , str);
                    break;

                case StorageFile a:
                    await Windows.System.Launcher.LaunchFileAsync(a);
                    break;
            }
        }

        private async void DeleteFile (object sender , RoutedEventArgs e)
        {
            var button = sender as Button;
            switch (button.DataContext)
            {
                case StorageFile fil:
                    await fil.DeleteAsync(StorageDeleteOption.Default);
                    _viewModel.DeleteStorageFileInRootObservable(fil.Path);

                    break;

                case string path:
                    File.Delete(path);
                    _viewModel.DeleteStorageFileInRootObservable(path);
                    break;
            }

            button.IsEnabled = false;
        }

        private void AddConfiguration (object sender , RoutedEventArgs e)
        {
            var text = NewConFileNameInput.Text;

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            FoldersGroup folderConFile = new FoldersGroup(text);

            database.Add(folderConFile);
            database.SaveChanges();

            Groups.Add(folderConFile);

            var a = Groups.IndexOf(folderConFile);

            ConfigurationComboBox.SelectedIndex = a;

            NewConFileNameInput.Text = String.Empty;
        }

        private void RemoveConFile (object sender , RoutedEventArgs e)
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
                StorageFolders.Clear();
                StorageFiles.Clear();

                currentConfiguration = ConfigurationComboBox.SelectedItem as FoldersGroup;

                foreach (var token in currentConfiguration.Tokens)
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        try
                        {
                            var folder = await FutureAccessList.GetFolderAsync(token);
                            StorageFolders.Add(folder);
                        }
                        catch { }
                    }
                }
            }
        }

        private void RemoveFolder (object sender , RoutedEventArgs e)
        {
            var item = ConfigurationComboBox.SelectedItem as FoldersGroup;
            if (item != null)
            {
                item.ClearAllTokens();
                database.SaveChanges();

                StorageFolders.Clear();
            }
        }

        private async void Image_Loaded (object sender , RoutedEventArgs e)
        {
            var image = sender as Image;

            switch (image.DataContext)
            {
                case string str:
                    break;

                case StorageFile storageFile:
                    var thumbnail = await storageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(thumbnail);
                    image.Source = bitmapImage;
                    break;
            }
        }
    }
}