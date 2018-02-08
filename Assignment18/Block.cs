using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Security.Cryptography;

namespace Assignment18
{

    class HashString
    {
        public HashString(String theString)
        {
            if (theString.Length != 40)
                throw new InvalidDataException($"Bad length, expected 40, got {theString.Length} ");
            Value = theString;
        }

        public string Value { get; private set; }
        
        public HashString(byte[] byteArray)
        {
            if (byteArray.Length != Block.HASHLEN) throw new ArgumentOutOfRangeException();
            StringBuilder builder = new StringBuilder(Block.HASHLEN * 2);
            foreach (byte b in byteArray) builder.AppendFormat("{0:x2}", b);
            Value = builder.ToString();
        }
    }


    /// <summary>
    /// Represents a block in a blockchain. Has five properties: ID, Nonce, Data, 
    /// PreviousHash (of previous block in chain), MyHash (realtime hash of the previous).
    /// This class implements IPropertyChanged in order to allow for easy data binding, 
    /// more unusually it listens to its own events (via InternalChangeHandler) in order
    /// to recalculate MyHash whenever any of the other four properties changes.
    /// </summary>
    class Block : INotifyPropertyChanged
    {
        //static
        static int nextblockid = 1;
        public static readonly int HASHLEN = 20;   //Should be in some other class

        //buffer
        private UnicodeEncoding enc = new UnicodeEncoding();
        private HashAlgorithm sha = new SHA1CryptoServiceProvider();

        //Constructors

        // no parms builds start block
        public Block(String Data = "")
        {
            ID = nextblockid++;
            Nonce = 0;
            this.Data = Data;
            PreviousHash = new string('0', 2*HASHLEN);
            PropertyChanged += InternalChangeHandler;
            ReHash();
            Mine();
            
        }

        public Block(Block prior, String Data="")
        {
            ID = nextblockid++;
            Nonce = 0;
            this.Data = Data;
            PreviousHash = prior.MyHash;
            PropertyChanged += InternalChangeHandler;
            ReHash();
            Mine();        
        }

        //Properties
        private int id;
        public int ID { get => id; set => SetField(ref id, value, nameof(ID));  }

        private int nonce;
        public int Nonce { get => nonce; set => SetField(ref nonce, value, nameof(Nonce)); }

        private string data;
        public string Data {
            get => data;
            set => SetField(ref data, value, nameof(Data)); }

        private HashString previousHash;
        public string PreviousHash {
            get => previousHash.Value; 
            set {
                previousHash = new HashString(value);
                NotifyPropertyChanged();
            }
        }

        private HashString myHash;
        public string MyHash
        {
            get { return myHash.Value; }
            private set { myHash = new HashString(value); }
        }

        //Property Change Handlers
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName="" )
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        public void InternalChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "MyHash") ReHash();
        }

        //Methods
        public bool IsSigned() => String.Equals("0000", MyHash.Substring(0, 4));

        public void ReHash()
        {
            using (MemoryStream mstream = new MemoryStream()) {
                BinaryWriter bw = new BinaryWriter(mstream);
                mstream.Seek(0, SeekOrigin.Begin);
                bw.Write(enc.GetBytes(ID.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(Nonce.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(Data.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(PreviousHash.ToString().ToCharArray()));
                mstream.Seek(0, SeekOrigin.Begin);
                MyHash = new HashString(sha.ComputeHash(mstream)).Value;
            }
        }

        public void Mine() {
            while (!this.IsSigned()) { Nonce++; }
        }

        public override string ToString() =>
$@"
{ID}
{Nonce}
{Data}
{PreviousHash}
{MyHash}
";
    }
}
