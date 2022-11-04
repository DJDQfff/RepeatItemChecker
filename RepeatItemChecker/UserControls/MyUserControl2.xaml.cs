using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace RepeatItemChecker.UserControls
{
    public sealed partial class MyUserControl2 : UserControl
    {
        public StorageFile storageFile1;

        public MyUserControl2 ()
        {
            this.InitializeComponent();
        }

        public MyUserControl2 (StorageFile storageFile) : this()
        {
            storageFile1 = storageFile;
        }

        public async Task Initial ()
        {
            var a = await storageFile1.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(a);
            Thumbnail.Source = bitmapImage;
        }

        private async void Button_Click (object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchFileAsync(storageFile1);
        }
    }
}