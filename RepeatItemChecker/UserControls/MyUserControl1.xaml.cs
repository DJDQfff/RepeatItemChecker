using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace RepeatItemChecker.UserControls
{
    public sealed partial class MyUserControl1 : UserControl
    {
        public StorageFile[] storageFiles;

        public MyUserControl1 ()
        {
            this.InitializeComponent();
        }

        public MyUserControl1 (params StorageFile[] parts) : this()
        {
            storageFiles = parts;
        }

        public async Task Initial ()
        {
            foreach (var part in storageFiles)
            {
                var control = new MyUserControl2(part);
                await control.Initial();
                stack.Children.Add(control);
            }
        }
    }
}