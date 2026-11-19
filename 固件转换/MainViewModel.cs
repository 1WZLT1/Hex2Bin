using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace 固件转换
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _HexDocumentAddress;

        public string HexDocumentAddress
        {
            get => _HexDocumentAddress;
            set
            {
                if (_HexDocumentAddress == value) return;
                _HexDocumentAddress = value;
                OnPropertyChanged(nameof(HexDocumentAddress));
            }

        }

        public ICommand OnHexDocumentAddressClick { get; }
        public ICommand OnBinDocumentAddressClick { get; }
        public ICommand ConvertFirmwareCommand { get; }
        public ICommand DocumentAddressSelect { get; }
        public MainViewModel()
        {
            OnHexDocumentAddressClick = new RelayCommand(OnHexDocumentAddressClick_Function);
            OnBinDocumentAddressClick = new RelayCommand(OnBinDocumentAddressClick_Function);
            ConvertFirmwareCommand    = new RelayCommand(ConvertFirmwareCommand_Function);
            DocumentAddressSelect     = new RelayCommand(DocumentAddressSelect_Function);
        }

        void OnHexDocumentAddressClick_Function()
        {
     
        }

        void OnBinDocumentAddressClick_Function()
        {

        }

        void ConvertFirmwareCommand_Function()
        {

        }

        void DocumentAddressSelect_Function()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
