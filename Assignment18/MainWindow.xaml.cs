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

namespace Assignment18
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //put some test code here (how dirty is that!)
            Debug.WriteLine("Started");

            //Create a Block
            Block B = new Block();
            //Check its properties
            B.Data = "Hello World";
            B.Nonce = 42;
            Debug.WriteLine($"Created Block: ID:{B.ID} Nonce:{B.Nonce} Data: {B.Data} PreviousHash: {B.PreviousHash} Hash: {B.MyHash}");
            Debug.WriteLine(B.IsSigned() ? "Block is Signed" : "Block is Unsigned");
            B.Mine();
            Debug.WriteLine($"Created Block: ID:{B.ID} Nonce:{B.Nonce} Data: {B.Data} PreviousHash: {B.PreviousHash} Hash: {B.MyHash}");
            Debug.WriteLine(B.IsSigned() ? "Block is Signed" : "Block is Unsigned");
            Application.Current.MainWindow.Close();
        }
    }
}
