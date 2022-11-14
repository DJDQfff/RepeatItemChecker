using System.Collections.ObjectModel;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace RepeatItemChecker.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public ObservableCollection<TestClass> strings = new ObservableCollection<TestClass>();

        public BlankPage1 ()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TestClass testClass = new TestClass() { Content = 43 };

            strings.Add(testClass);
        }
    }

    public class TestClass
    {
        public ulong Content;
    }
}