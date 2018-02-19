using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Assignment18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private BlockChain theChain;

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
            if (!theChain.IsMining) //try to stop some other block mining at same time
                using (new BusyCursor(B))
                {
                    theChain.IsMining = true;
                    B.Dispatcher.Invoke(DispatcherPriority.Input, (Action)(() => { })); //without this UI will not update before starting the mine
                    blck.Mine();
                        //Task.Run((Action)blck.Mine); //In principle this is better, not on UI thread, in practice huge care is needed
                        //with reentrancy when another button press occurs before the first is finished
                        //Also all descendant blocks update needlessly slowing everything down
                    theChain.IsMining = false;
                }      
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

    /// <summary>
    /// ValueConverter. Maps true->PaleGreen and false->Pink. No ConvertBack defined.
    /// </summary>
    public class SignedToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? Brushes.PaleGreen : Brushes.Pink;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Defines an Exit CustomCommand
    /// </summary>
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

    /// <summary>
    /// defines an horglass for use by long running code
    /// </summary>
    /// <remarks>
    /// Usage:
    /// <code>
    /// using (new BusyCursor(b)){ ... } 
    /// </code>
    /// </remarks>
    public class BusyCursor:IDisposable
    {
        private Cursor previousCursor;
        private bool previousEnabled;
        private Control theControl;

        public BusyCursor(Control C)
        {
            theControl = C;
            previousCursor = Mouse.OverrideCursor;
            previousEnabled = theControl.IsEnabled;
            Mouse.OverrideCursor = Cursors.Wait;
            theControl.IsEnabled = false;
        }
        public void Dispose()
        {
            Mouse.OverrideCursor = previousCursor;
            theControl.IsEnabled = previousEnabled;
        }
    }
}
