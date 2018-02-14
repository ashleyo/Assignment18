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
[assembly:InternalsVisibleTo("UnitTestProject1")]

namespace Assignment18
{

    class HashString
    {
        public static HashString Origin = new HashString(new string('0', 2 * Block.HASHLEN));

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
        private static uint nextblockid = 1;
        public static readonly int HASHLEN = 20;   //Should be in some other class
        private static UnicodeEncoding enc = new UnicodeEncoding();
        private HashAlgorithm sha = new SHA1CryptoServiceProvider();

        //Fields
        private Block previousBlock;

        //Constructors

        // no parms builds start block
        public Block(String Data = "")
        {
            previousBlock = null;
            ID = nextblockid++;
            Nonce = 0;
            this.Data = Data;
            PreviousHash = HashString.Origin.Value;
            PropertyChanged += InternalChangeHandler;
            ReHash();
        }

        public Block(Block prior, String Data = "")
        {
            previousBlock = prior;
            ID = nextblockid++;
            Nonce = 0;
            this.Data = Data;
            PreviousHash = previousBlock.MyHash;
            PropertyChanged += InternalChangeHandler;
            previousBlock.PropertyChanged += PreviousHashChangeHandler;
            ReHash();
        }

        //Properties
        private uint id;
        public uint ID { get => id; set => SetField(ref id, value, nameof(ID)); }

        private uint nonce;
        public uint Nonce { get => nonce; set => SetField(ref nonce, value, nameof(Nonce)); }

        private string data;
        public string Data
        {
            get => data;
            set => SetField(ref data, value, nameof(Data));
        }

        private HashString previousHash;
        public string PreviousHash
        {
            get => previousHash.Value;
            set
            {
                previousHash = new HashString(value);
                NotifyPropertyChanged();
            }
        }

        private HashString myHash;
        public string MyHash
        {
            get { return myHash.Value; }
            private set { myHash = new HashString(value); NotifyPropertyChanged(); }
        }

        private bool signed;
        public bool Signed
        {
            get => signed;
            private set => SetField(ref signed, value, nameof(Signed));
        }

        //Property Change Handlers
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
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
            if ("ID Nonce Data PreviousHash".Contains(e.PropertyName)) ReHash();
        }

        public void PreviousHashChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MyHash")
            {
                PreviousHash = previousBlock.MyHash;
            }
        }

        //Methods
        public bool IsSigned()
        {
            Signed = String.Equals("0000", MyHash.Substring(0, 4));
            return Signed;
        }

        public void ReHash()
        {
            using (MemoryStream mstream = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(mstream);
                mstream.Seek(0, SeekOrigin.Begin);
                bw.Write(enc.GetBytes(ID.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(Nonce.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(Data.ToString().ToCharArray()));
                bw.Write(enc.GetBytes(PreviousHash.ToString().ToCharArray()));
                mstream.Seek(0, SeekOrigin.Begin);
                MyHash = new HashString(sha.ComputeHash(mstream)).Value;
                Signed = IsSigned();
            }
        }

        public void Mine()
        {
            Nonce = 0;
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
