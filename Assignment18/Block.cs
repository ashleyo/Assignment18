using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
[assembly: InternalsVisibleTo("UnitTestProject1")]

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
        /// <summary>
        /// Default parameterless constructor, should only be used for the first block
        /// of a new chain.
        /// </summary>
        /// <param name="Data">Data may be passed into the contructor, defaults to empty string if not given</param>
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

        /// <summary>
        /// Constructor for a normal block; need to be passed a reference to the the preceding block
        /// </summary>
        /// <param name="prior">Mandatory, reference to preceding block</param>
        /// <param name="Data">Optional, initial data</param>
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
        /// <summary>
        /// unsigned int, ID of block
        /// </summary>
        public uint ID { get => id; private set => SetField<uint>(ref id, value, nameof(ID)); }

        private uint nonce;
        /// <summary>
        /// unsigned uint, nonce of block
        /// public settor but not normally exposed by UI as setting the nonce directly has
        /// no utility
        /// </summary>
        public uint Nonce { get => nonce; set => SetField<uint>(ref nonce, value, nameof(Nonce)); }

        private string data;
        /// <summary>
        /// string representing data for the block
        /// </summary>
        public string Data
        {
            get => data;
            set => SetField<string>(ref data, value, nameof(Data));
        }

        private HashString previousHash;
        /// <summary>
        /// A readonly HashString reflecting the hash of the previous block
        /// If needed as a string HashString.Value should be used
        /// </summary>
        public string PreviousHash
        {
            get => previousHash.Value;
            private set
            {
                previousHash = new HashString(value);
                NotifyPropertyChanged();
            }
        }

        private HashString myHash;
        /// <summary>
        /// A readonly HashString reflecting the hash of this block
        /// If needed as a string HashString.Value should be used
        /// </summary>
        public string MyHash
        {
            get { return myHash.Value; }
            private set { myHash = new HashString(value); NotifyPropertyChanged(); }
        }

        private bool signed;
        /// <summary>
        /// A readonly bool representing the current signed status of the block
        /// </summary>
        public bool Signed
        {
            get => signed;
            private set => SetField<bool>(ref signed, value, nameof(Signed));
        }

        //Property Change Handlers
        /// <summary>
        /// Event raised when any property of the block changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Helper to aid in writing settors for properties that need to raise PropertyChanged
        /// </summary>
        /// <typeparam name="T">type of value to be set: only needed for non built-in types when it controls how 
        /// they are compared. The comparison is used to decide whether the backing field needs modifying</typeparam>
        /// <param name="field">pass-by-reference field to be modified</param>
        /// <param name="value">new value</param>
        /// <param name="propertyName">the name of the property</param>
        /// <returns>boolean representing whether the backing field was modified or not. It will be false if 'value'
        /// was the same as the exisiting value. In such a case no event is fired.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        //internal property changed event handler, rehashes the block if anything affecting the
        //hash is altered
        private void InternalChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if ("ID Nonce Data PreviousHash".Contains(e.PropertyName)) ReHash();
        }

        //internal handler triggered by changes to **previous** block's hash in order to
        //fetch the new value and load it here (where it will in turn trigger a rehash of 
        //this block
        private void PreviousHashChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MyHash")
            {
                PreviousHash = previousBlock.MyHash;
            }
        }

        //Methods

        
        // called to trigger immediate check of current block's signed status
        private bool IsSigned()
        {
            Signed = String.Equals("0000", MyHash.Substring(0, 4));
            return Signed;
        }

        /// <summary>
        /// Intended mostly for internal use, can be called to force recalculation of block's hash
        /// </summary>
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

        /// <summary>
        /// Call to trigger a search for a nonce that will make the current block satisfy the signed criterion
        /// </summary>
        public void Mine()
        {
            Nonce = 0;
            while (!this.IsSigned()) Nonce++;
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
