using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Globalization;
using System.ComponentModel;

namespace Assignment18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        BlockChain theChain;

        private static Action NoOp = delegate () { };

        public MainWindow()
        {
            InitializeComponent();

            theChain = new BlockChain();
            this.DataContext = theChain;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button B = sender as Button;
            Block blck = B.DataContext as Block;
            B.IsEnabled = false;
            B.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() => { }));
            //Task.Run((Action)blck.Mine);
            blck.Mine();
            B.IsEnabled = true;
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class SignedToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Brushes.PaleGreen : Brushes.Pink;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection() {
                new KeyGesture(Key.F4, ModifierKeys.Alt)
            }
        );
    }





}



//TODO Would prefer an hourglass or similar to the ferocious numbers when mining .. it looks a little naff
//plus ... nasty crashes if you try to mine two blocks at once so doing it the 'right' way - on a non-UI thread
//opens nasty cans of wormses. Reverted this back to single-threaded with button visibly disabled while mining 
