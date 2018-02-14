using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestProject1")]

namespace Assignment18
{
    
    class BlockChain
    {
        public ObservableCollection<Block> Blocks { get; }

        public bool Mining = false;

        public BlockChain()
        {
            Blocks = new ObservableCollection<Block>();
            Block B = new Block("For single lower-case alpha on the origin block 'l' with a nonce of 4294 is quickest to mine and x with 261301 slowest."); //starting block
            Blocks.Add(B);
            foreach (int i in Enumerable.Range(1, 4))
            {
               B = new Block(B);
               Blocks.Add(B);
            }
        }
    }

    
}
