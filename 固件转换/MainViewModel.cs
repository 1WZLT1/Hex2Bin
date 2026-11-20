using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

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

        private string _BinDocumentAddress;

        public string BinDocumentAddress
        {
            get => _BinDocumentAddress;
            set
            {
                if (_BinDocumentAddress == value) return;
                _BinDocumentAddress = value;
                OnPropertyChanged(nameof(BinDocumentAddress));
            }

        }

        public ICommand OnHexDocumentAddressClick { get; }
        public ICommand OnBinDocumentAddressClick { get; }
        public ICommand ConvertFirmwareCommand { get; }
        public ICommand DocumentAddressSelect { get; }
        public ICommand BinDocumentAddressSelect { get; }
        public MainViewModel()
        {
            OnHexDocumentAddressClick = new RelayCommand(OnHexDocumentAddressClick_Function);
            OnBinDocumentAddressClick = new RelayCommand(OnBinDocumentAddressClick_Function);
            ConvertFirmwareCommand    = new RelayCommand(ConvertFirmwareCommand_Function);
            DocumentAddressSelect     = new RelayCommand(DocumentAddressSelect_Function);
            BinDocumentAddressSelect  = new RelayCommand(BinDocumentAddressSelect_Function);
        }

        void OnHexDocumentAddressClick_Function()
        {
     
        }

        void OnBinDocumentAddressClick_Function()
        {

        }

        public class HexRecord
        {
            public byte Length { get; set; } 
            public ushort Offset { get; set; }
            public byte Type { get; set; } 
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public byte Checksum { get; set; }
        }

        Dictionary<int, HexRecord> records = new Dictionary<int, HexRecord>();

        void ConvertFirmwareCommand_Function()
        {
            if (HexDocumentAddress == null) return;

            int line_index = 0;
            int min_index  = 0;
            UInt32 BaseAddress = 0xFFFFFFFF;
            UInt32 NowAddress  = 0x0000;
            records.Clear();
            foreach (string line in File.ReadLines(HexDocumentAddress))
            {
                if (line.StartsWith(":"))
                {
                    byte length = Convert.ToByte(line.Substring(1, 2), 16);

                    HexRecord record = new HexRecord
                    {
                        Length = length,
                        Offset = Convert.ToUInt16(line.Substring(3, 4), 16),
                        Type = Convert.ToByte(line.Substring(7, 2), 16),
                        Data = Convert.FromHexString(line.Substring(9, length * 2))
                    };

                    if (record.Type == 0x04)
                    {
                        NowAddress = ((UInt32)record.Data[0] << 24) | ((UInt32)record.Data[1] << 16);
                        if (BaseAddress > NowAddress)
                        {
                            BaseAddress = NowAddress;
                            min_index = line_index;
                        }
                    }

                    records.Add(line_index, record);
                    line_index += 1;
                }
            }

            UInt16 minOffset = 0xFFFF;
            int minOffsetIndex = -1;

            for (int i = min_index + 1; i < records.Count; i++)
            {
                HexRecord record = records[i];
                if (record.Type == 0x00 && record.Length > 0)
                {
                    if (minOffsetIndex == -1 || record.Offset < minOffset)
                    {
                        minOffset = record.Offset;
                        minOffsetIndex = i;
                    }
                }
            }

            if (minOffsetIndex >= 0)
            {
                UInt32 startAddress = BaseAddress + minOffset;
                Debug.WriteLine($"起始地址：0x{startAddress:X8}");
            }
        }

        void DocumentAddressSelect_Function()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "HEX 文件 (*.hex)|*.hex|所有文件 (*.*)|*.*",
                Multiselect = false /*一次只能打开单个文件*/
            };

            if (dialog.ShowDialog() == true)
            {
                HexDocumentAddress = dialog.FileName;
                Console.WriteLine("HexDocumentAddress" + HexDocumentAddress);
            }
        }

        void BinDocumentAddressSelect_Function()
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                BinDocumentAddress = dialog.FolderName;
                Console.WriteLine("BinDocumentAddress" + BinDocumentAddress);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
