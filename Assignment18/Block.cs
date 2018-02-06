using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Assignment18
{

    class HashString
    {
        public HashString(String theString)
        {
            Value = theString;
        }
        public string Value { get; private set; }
    }


    /// <summary>
    /// Represents a block in a blockchain
    /// </summary>
    class Block : INotifyPropertyChanged
    {
        //static
        static int nextblockid = 1;
        static readonly int HASHLEN = 20;   //Should be in some other class

        //Constructors

        public Block()
        {
            ID = nextblockid++;
            Nonce = 0;
            Data = String.Empty;
            PreviousHash = new string('0', HASHLEN);
            ReHash();
            PropertyChanged += InternalChangeHandler;
        }

        //Properties
        private int id;
        public int ID { get => id; set => SetField(ref id, value, nameof(ID));  }

        private int nonce;
        public int Nonce { get => nonce; set => SetField(ref nonce, value, nameof(Nonce)); }

        private string data;
        public string Data { get => data; set => SetField(ref data, value, nameof(Data)); }

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

        private void InternalChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "MyHash") ReHash();
        }

        //Methods
        public bool IsSigned() => true; // TODO: Implementation needed

        public void ReHash()
        {
            //TODO: Implementation needed
        }

        public void Mine() { } //TODO: Implementation needed

    }
}
