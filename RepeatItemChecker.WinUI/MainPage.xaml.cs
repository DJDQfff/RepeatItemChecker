// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace RepeatItemsChecker.WinUI
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage ()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            using (Database_1 database = new Database_1())
            {
                database.Database.Migrate();
                database.SaveChanges();
            }

            MainFrame.Navigate(typeof(Views.Page1));
        }
    }
}