using RepeatItemChecker.Views;

using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace RepeatItemChecker.UserControls
{
    public sealed partial class MyUserControl1 : UserControl
    {
        public TestClass Con => this.DataContext as TestClass;

        public MyUserControl1 ()
        {
            this.InitializeComponent();
        }
    }
}