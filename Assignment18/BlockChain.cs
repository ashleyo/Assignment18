using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Assignment18
{
    
    class BlockChain
    {
        public ObservableCollection<Block> Blocks { get; }

        public BlockChain()
        {
            Blocks = new ObservableCollection<Block>();
            Block B = new Block(); //starting block
            Blocks.Add(B);
            foreach (int i in Enumerable.Range(1, 4))
            {
               B = new Block(B);
               Blocks.Add(B);
            }
        }
    }
}
