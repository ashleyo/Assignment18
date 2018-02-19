using System.Collections.ObjectModel;
using System.Linq;
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("UnitTestProject1")]

namespace Assignment18
{
    //The ViewModel for this app. There is no Model as such since no data persists

    /// <summary>
    /// A wrapper around an ObservableCollection of Blocks.
    /// </summary>
    class BlockChain
    {
        /// <summary>
        /// The blocks
        /// </summary>
        public ObservableCollection<Block> Blocks { get; }

        /// <summary>
        /// bool flag indicating whether we are currently in a mining operation or not
        /// set to indicate that no new mining operations should be started
        /// </summary>
        public bool IsMining { get; set; } =  false;

        /// <summary>
        /// Constructor for a block chain. Default data is included for the root block,
        /// The remainder have empty data.
        /// The chain is initially unsigned.
        /// </summary>
        /// <param name="chainLength">Optional. The length of chain required, default 5.
        /// Not trapped. Setting to 1 or less will cause errors. Tested up to 10k
        /// although by this point start-up of the program (construction of the chain) is becoming unacceptably slow.</param>
        /// <param name="initiallySigned">Optional. If set to true the blocks will be mined after creation so that 
        /// they are all initially signed.</param>
        public BlockChain(int chainLength = 5, bool initiallySigned=false)
        {
            Blocks = new ObservableCollection<Block>();
            Block B = new Block("For single lower-case alpha on the origin block 'l' with a nonce of 4294 is quickest to mine and x with 261301 slowest."); //starting block
            Blocks.Add(B);
            foreach (int i in Enumerable.Range(1,chainLength-1))
            {
               B = new Block(B);
               Blocks.Add(B);
            }
            if (initiallySigned)
            {
                foreach (int i in Enumerable.Range(1, chainLength))
                {
                    Blocks[i - 1].Mine();
                }
            }
        }
    }

    
}
