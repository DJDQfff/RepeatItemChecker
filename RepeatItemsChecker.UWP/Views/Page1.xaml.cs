﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using FolderGroupDB;

using Microsoft.Toolkit.Uwp.Notifications;

using MyLibrary.Standard20;

using MyLibrary.UWP;

using RepeatItemsChecker.App.Models;
using GroupedItemsLibrary.ViewModels;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using static Windows.Storage.AccessCache.StorageApplicationPermissions;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemsChecker.App.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page1 : Page
    {
        private ItemsGroupsViewModel<string, StorageFile, RepeaStorageFileGroup> _viewModel;

        /// <summary>
        /// 所有要进行比较的文件
        /// </summary>
        private ObservableCollection<StorageFile> StorageFiles = new ObservableCollection<StorageFile>();

        /// <summary>
        /// 所有要进行遍历的文件夹
        /// </summary>
        public ObservableCollection<StorageFolder> StorageFolders = new ObservableCollection<StorageFolder>();

        /// <summary>
        /// 程序存储的检查组合
        /// </summary>
        internal ObservableCollection<FoldersGroup> Groups = new ObservableCollection<FoldersGroup>();

        private StorageFolder configurationFileFolder;

        private FoldersGroup currentConfiguration;

        private Database_1 database;

        public Page1()
        {
            this.InitializeComponent();
            database = new Database_1();
            configurationFileFolder = Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("Configuration", CreationCollisionOption.OpenIfExists).AsTask().Result;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var groups = database.folderGroups.ToArray();
            foreach (var group in groups)
            {
                Groups.Add(group);
            }
        }

        private async void PickFolder(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

        private async void Start(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadingControl.IsLoading = true;
            StorageFiles.Clear();

            foreach (var folder in StorageFolders)
            {
                var files = await folder.GetAllStorageItems();

                foreach (var file in files.Item2)
                {
                    StorageFiles.Add(file);
                }
            }

            Func<StorageFile, string> onlyFileSize = n => n.GetBasicPropertiesAsync().AsTask().Result.Size.ToString();
            var bool1 = CheckBox_FileSize.IsChecked;
            var bool2 = CheckBox_Md5.IsChecked;

            _viewModel = new ItemsGroupsViewModel<string, StorageFile, RepeaStorageFileGroup>(StorageFiles, onlyFileSize);

            if (bool2.Value)
            {
                Func<StorageFile, string> sha256 = (x) =>
                {
                    string hash;
                    using (Stream stream = x.OpenStreamForReadAsync().Result)
                    {
                        hash = HashComputer.ComputeHash(stream);
                    }

                    return hash;
                };

                _viewModel = new ItemsGroupsViewModel<string, StorageFile, RepeaStorageFileGroup>(_viewModel.AllElements, sha256);
            }

            SameItemList.ItemsSource = _viewModel.RepeatPairs;
            CountTextBlock.Text = _viewModel.Count.ToString();

            LoadingControl.IsLoading = false;
            new ToastContentBuilder()
                .AddText("完成")
                .Show();
        }

        private async void Button_Click_LaunchFile(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var j = sender as Button;
            var a = j.DataContext as StorageFile;
            await Windows.System.Launcher.LaunchFileAsync(a);
        }

        private async void DeleteFile(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var file = button.DataContext as StorageFile;

            _viewModel.DeleteStorageFileInRootObservable(file);

            await file.DeleteAsync(StorageDeleteOption.Default);

            button.IsEnabled = false;
        }

        private async void AddConfiguration(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var text = NewConFileNameInput.Text;

            if (string.IsNullOrEmpty(text))
            {
                await new ContentDialog()
                { Content = "不能为空", CloseButtonText = "OK" }
                .ShowAsync();
            }

            FoldersGroup folderConFile = new FoldersGroup(text);

            database.Add(folderConFile);
            database.SaveChanges();

            Groups.Add(folderConFile);

            var a = Groups.IndexOf(folderConFile);

            ConfigurationComboBox.SelectedIndex = a;

            NewConFileNameInput.Text = String.Empty;
        }

        private void RemoveConFile(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Groups.Count != 0)
            {
                var item = ConfigurationComboBox.SelectedItem as FoldersGroup;

                database.Remove(item);
                database.SaveChanges();

                Groups.Remove(item);
            }
        }

        private async void ConfigurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void RemoveFolder(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var item = ConfigurationComboBox.SelectedItem as FoldersGroup;
            if (item != null)
            {
                item.ClearAllTokens();
                database.SaveChanges();

                StorageFolders.Clear();
            }
        }

        private async void Image_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var image = sender as Image;
            var storagefile = image.DataContext as StorageFile;
            if (storagefile is null)
            {
                return;
            }
            var thumbnail = await storagefile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            image.Source = bitmapImage;
        }
    }
}