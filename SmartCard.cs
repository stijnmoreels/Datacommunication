using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nmct.datacom.colordome.hardware
{
    public class SmartCard
    {
        public String[] Readers { get; set; }

        /*  #region IMPORT
          private int nContext;				//Card reader context handle - DWORD
          private int nCard;
          private int nActiveProtocol;        //T0/T1 

          // WinSCard API's to be imported
          [DllImport("WinScard.dll")]
          public static extern int SCardEstablishContext(uint dwScope, int nNotUsed1,
              int nNotUsed2, ref int phContext);
          [DllImport("WinScard.dll")]
          public static extern int SCardReleaseContext(int phContext);
          [DllImport("WinScard.dll")]
          public static extern int SCardConnect(int hContext, string cReaderName,
              uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref int ActiveProtocol);
          [DllImport("WinScard.dll")]
          public static extern int SCardDisconnect(int hCard, int Disposition);
          [DllImport("WinScard.dll")]
          public static extern int SCardListReaderGroups(int hContext, ref string cGroups, ref int nStringSize);
          [DllImport("WinScard.dll")]
          public static extern int SCardListReaders(int hContext, string cGroups,
              ref string cReaderLists, ref int nReaderCount);
          [DllImport("WinScard.dll")]
          public static extern int SCardFreeMemory(int hContext, string cResourceToFree);
          [DllImport("WinScard.dll")]
          public static extern int SCardGetAttrib(int hContext, uint dwAttrId,
              ref byte[] bytRecvAttr, ref int nRecLen);
          #endregion

          private int SystemContext, Context, ReturnCode, Protocol, Card, Readercount, SendLength, ReceiveLength;
          private byte[] SendBuffer = new byte[256], ReceiveBuffer = new byte[256];
          private ModWinsCard.SCARD_IO_REQUEST Request;

          public SmartCard()
          {
              InitSmartCard();
          }

          #region INIT
          private void InitSmartCard()
          {
              if (!EstablishContext())
                  return;

              char[] delimiter = new char[1];
              string groupList = Convert.ToChar(0).ToString();
              if (!GetListReaderGroups(ref delimiter, ref groupList))
                  return;

              string[] groups = groupList.Split(delimiter);
              string readerList = Convert.ToChar(0).ToString();
              int readerCount = -1;
              if (!GetListReaders(ref readerList, groups, readerCount))
                  return;

              this.Readers = readerList.Split(delimiter).ToList<string>();
          }

          private bool EstablishContext()
          {
              this.SystemContext = 2;
              int notInUse1 = 0;
              int notInUse2 = 0;
              this.Context = 0;

              int returnContext = SCardEstablishContext(2, notInUse1, notInUse2, ref this.Context);
              if (returnContext != 0)
              {
                  MessageBox.Show("Error returned by SCardEstablishContext()");
                  return false;
              }
              return true;
          }

          private bool GetListReaderGroups(ref char[] delimiter, ref string groupList)
          {
              delimiter = new char[1];
              delimiter[0] = Convert.ToChar(0);

              int stringSize = -1;	//SCARD_AUTOALLOCATE
              int returnReaderGroups = SCardListReaderGroups(this.Context, ref groupList, ref stringSize);
              if (returnReaderGroups != 0)
              {
                  MessageBox.Show("Error returned by SCardListReaderGroups()");
                  return false;
              }
              return true;
          }

          private bool GetListReaders(ref string readerList, string[] groups, int readerCount)
          {
              int returnListReaeders = SCardListReaders(this.Context, groups[0], ref readerList, ref readerCount);
              if (returnListReaeders != 0)
              {
                  MessageBox.Show(" Error returned by SCardListReaders()");
                  return false;
              }
              return true;
          }
          #endregion*/

        /* public bool Connect(string reader)
         {
             int connect = SCardConnect(this.Context, reader, ModWinsCard.SCARD_SHARE_SHARED,
                 ModWinsCard.SCARD_PROTOCOL_T0, ref Card, ref Protocol);
             int transaction = ModWinsCard.SCardBeginTransaction(Card);

             ClearArrays();
             (new List<byte>() { 0xFF, 0xA4, 0x0, 0x0, 0x01, 0x01 })
                .ForEach(i => SendBuffer.Add(i));

             SendLength = 6;
             Request.cbPciLength = 8;
             Request.dwProtocol = Protocol;
             ReceiveLength = ReceiveBuffer.Count;

             int returnCardType = ModWinsCard.SCardTransmit(Card, ref Request, ref SendBuffer.ToArray()[0], SendLength,
                 ref Request, ref ReceiveBuffer.ToArray()[0], ref ReceiveLength);
             return returnCardType == 0 ? true : false;
             int connect = SCardConnect(this.nContext, cboReaders.SelectedItem.ToString(), ModWinsCard.SCARD_SHARE_SHARED, ModWinsCard.SCARD_PROTOCOL_T0, ref hCard, ref Protocol);
             int transaction = ModWinsCard.SCardBeginTransaction(hCard);
         }

         public bool WriteData(string text)
         {
             byte[] array = System.Text.Encoding.ASCII.GetBytes(text);
             (new List<byte>() { 0xFF, 0xD0, 0x20, 0x0, 0x04 })
                 .ForEach(i => SendBuffer.Add(i)); // Write Data
             array.ToList<byte>()
                 .ForEach(i => SendBuffer.Add(i)); // Add text to SendBuffer

             SendLength = SendBuffer.Count;
             ReceiveLength = ReceiveBuffer.Count;
             int returnWrite = ModWinsCard.SCardTransmit(Card, ref Request, ref SendBuffer.ToArray()[0], SendLength,
                 ref Request, ref ReceiveBuffer.ToArray()[0], ref ReceiveLength);
             return returnWrite == 0 ? ClearArrays() : false;
         }

         public string ReadData()
         {
             (new List<byte>() { 0xFF, 0xB0, 0x0, 0x0, 0xFF })
                 .ForEach(i => SendBuffer.Add(i)); // Read Data 

             SendLength = SendBuffer.Count;
             ReceiveLength = ReceiveBuffer.Count;
             int read = ModWinsCard.SCardTransmit(Card, ref Request, ref SendBuffer.ToArray()[0], SendLength,
                 ref Request, ref ReceiveBuffer.ToArray()[0], ref ReceiveLength);
             if (read != 0)
                 return string.Empty;

             List<byte> list = new List<byte>();
             foreach (byte b in ReceiveBuffer)
             {
                 list.Add(b);
                 if (b == 144)
                     break;
             }
             return Encoding.ASCII.GetString(list.ToArray(), 0, list.Count);
         }

         private bool ClearArrays()
         {
             SendBuffer = new List<byte>();
             ReceiveBuffer = new List<byte>();
             return true;
         }

         public void CloseConnection()
         {
             ModWinsCard.SCardEndTransaction(Card, 0);
             ModWinsCard.SCardDisconnect(Card, 0);
             ModWinsCard.SCardReleaseContext(Context);
         }*/
        private int nContext;				//Card reader context handle - DWORD
        private int nCard;
        private int nActiveProtocol;        //T0/T1 

        // WinSCard API's to be imported
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(uint dwScope, int nNotUsed1,
            int nNotUsed2, ref int phContext);
        [DllImport("WinScard.dll")]
        public static extern int SCardReleaseContext(int phContext);
        [DllImport("WinScard.dll")]
        public static extern int SCardConnect(int hContext, string cReaderName,
            uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref int ActiveProtocol);
        [DllImport("WinScard.dll")]
        public static extern int SCardDisconnect(int hCard, int Disposition);
        [DllImport("WinScard.dll")]
        public static extern int SCardListReaderGroups(int hContext, ref string cGroups, ref int nStringSize);
        [DllImport("WinScard.dll")]
        public static extern int SCardListReaders(int hContext, string cGroups,
            ref string cReaderLists, ref int nReaderCount);
        [DllImport("WinScard.dll")]
        public static extern int SCardFreeMemory(int hContext, string cResourceToFree);
        [DllImport("WinScard.dll")]
        public static extern int SCardGetAttrib(int hContext, uint dwAttrId,
            ref byte[] bytRecvAttr, ref int nRecLen);


        int retCode, Protocol, hContext, hCard, Readercount; byte[] ReaderListBuff = new byte[262]; byte[] ReaderGroupBuff; bool diFlag; ModWinsCard.SCARD_IO_REQUEST ioRequest; int sendLen, RecvLen; byte[] RecvBuff = new byte[262]; byte[] SendBuff = new byte[262];
        string cGroupList; int nStringSize;

        int context;
        int nNotUsed1, nNotUsed2;

        public SmartCard()
        {
            //InitializeComponent();

            //First step in using smart cards is CSardEstablishContext()
            this.context = 2;		//system
            this.nNotUsed1 = 0;
            this.nNotUsed2 = 0;
            this.nContext = 0;		//handle to context - SCARDCONTEX

            int nRetVal1 = SCardEstablishContext(2, nNotUsed1, nNotUsed2, ref this.nContext);
            if (nRetVal1 != 0)
            {
                // MessageBox.Show("u cardreader is niet aangesloten");
                return;
            }

            //next we split up the null delimited strings into a string array
            char[] delimiter = new char[1];
            delimiter[0] = Convert.ToChar(0);

            //Next we need to call the  SCardListReaderGroups() to find reader groups to use
            string cGroupList = "" + Convert.ToChar(0);
            int nStringSize = -1;	//SCARD_AUTOALLOCATE
            int nRetVal2 = SCardListReaderGroups(this.nContext, ref cGroupList, ref nStringSize);
            if (nRetVal2 != 0)
            {
                // MessageBox.Show("u cardreader is niet aangesloten");
                return;
            }

            string[] cGroups = cGroupList.Split(delimiter);

            string cReaderList = "" + Convert.ToChar(0);
            int nReaderCount = -1;

            // Get the reader list
            int nRetVal4 = SCardListReaders(this.nContext, cGroups[0], ref cReaderList, ref nReaderCount);
            if (nRetVal4 != 0)
            {
                // MessageBox.Show("u cardreader is niet aangesloten");
                return;
            }

            Readers = cReaderList.Split(delimiter);


        }

        /* private void btnConnect_Click(object sender, RoutedEventArgs e)
         {
             int connect = SCardConnect(this.nContext, cboReaders.SelectedItem.ToString(), ModWinsCard.SCARD_SHARE_SHARED, ModWinsCard.SCARD_PROTOCOL_T0, ref hCard, ref Protocol);
            int transaction = ModWinsCard.SCardBeginTransaction(hCard);

            SendBuff[0] = 0xFF;//start 
             SendBuff[1] = 0xA4;//instruction card type 
             SendBuff[2] = 0x0; //memory adres 
             SendBuff[3] = 0x0; 
             SendBuff[4] = 0x01; 
             SendBuff[5] = 0x01;//I2c 1
             sendLen = 6;
             ioRequest.cbPciLength = 8;
             ioRequest.dwProtocol = Protocol;
             RecvLen = RecvBuff.Length;
             int test = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);

           
            
             Array.Clear(SendBuff, 0, 262); 
             Array.Clear(RecvBuff, 0, 262); 
             SendBuff[0] = 0xFF;//start 
             SendBuff[1] = 0xD0;//instruction write 
             SendBuff[2] = 0x0; //memory adres 
             SendBuff[3] = 0x0; 
             SendBuff[4] = 0x04; 
             SendBuff[5] = 0x74; 
             SendBuff[6] = 0x69; 
             SendBuff[7] = 0x73;
             SendBuff[8] = 0x74;
             sendLen = 9;
             RecvLen = RecvBuff.Length;

             int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            
            

             //read
             Array.Clear(SendBuff, 0, 262);
             Array.Clear(RecvBuff, 0, 262);
             SendBuff[0] = 0xFF;//start 
             SendBuff[1] = 0xB0;//instruction write 
             SendBuff[2] = 0x0; //memory adres 
             SendBuff[3] = 0x0;
             SendBuff[4] = 0x04;
            
             sendLen = 5;
             RecvLen = RecvBuff.Length;

             int read = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            

         }*/
        #region CardOnSecure
        public bool Connect(string reader)
        {
            int connect = SCardConnect(this.nContext, reader, ModWinsCard.SCARD_SHARE_SHARED, ModWinsCard.SCARD_PROTOCOL_T0, ref hCard, ref Protocol);
            int transaction = ModWinsCard.SCardBeginTransaction(hCard);

            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0xA4;//instruction card type 
            SendBuff[2] = 0x0; //memory adres 
            SendBuff[3] = 0x0;
            SendBuff[4] = 0x01;
            SendBuff[5] = 0x01;//I2c 1
            //SendBuff[5] = 0x06 //code kaart
            sendLen = 6;
            ioRequest.cbPciLength = 8;
            ioRequest.dwProtocol = Protocol;
            RecvLen = RecvBuff.Length;
            int test = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            bool succes = ReadPresentationErrorCounter(test);
            return succes;


            /* Array.Clear(SendBuff, 0, 262);
             Array.Clear(RecvBuff, 0, 262);
             SendBuff[0] = 0xFF;//start 
             SendBuff[1] = 0xD0;//instruction write 
             SendBuff[2] = 0x00; //memory adres 
             SendBuff[3] = 0x0;
             SendBuff[4] = 0x04;
             SendBuff[5] = 0x74;
             SendBuff[6] = 0x69;
             SendBuff[7] = 0x73;
             SendBuff[8] = 0x74;
             sendLen = 9;
             RecvLen = RecvBuff.Length;

             int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            
            


             //read
             Array.Clear(SendBuff, 0, 262);
             Array.Clear(RecvBuff, 0, 262);
             SendBuff[0] = 0xFF;//start 
             SendBuff[1] = 0xB0;//instruction read 
             SendBuff[2] = 0x00; //memory adres 
             SendBuff[3] = 0x0;
             SendBuff[4] = 0xFF;

             sendLen = 5;
             RecvLen = RecvBuff.Length;

             int read = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);*/
        }


        public bool WriteData(string text)
        {
            try
            {
                Array.Clear(SendBuff, 0, 262);
                Array.Clear(RecvBuff, 0, 262);
                byte[] array = System.Text.Encoding.ASCII.GetBytes(text);
                int i = 5;
                SendBuff[0] = 0xFF;//start 
                SendBuff[1] = 0xD0;//instruction write 
                SendBuff[2] = 0x00; //memory adres 
                SendBuff[3] = 0x0;
                SendBuff[4] = Convert.ToByte(text.Length);
                foreach (byte b in array)
                {
                    SendBuff[i] = b;
                    i += 1;
                }
                sendLen = i;
                RecvLen = RecvBuff.Length;
                int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
                bool succes = ReadPresentationErrorCounter(write);
                return succes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void ClearCard()
        {
            Array.Clear(SendBuff, 0, 262);
            Array.Clear(RecvBuff, 0, 262);
            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0xD0;//instruction write 
            SendBuff[2] = 0x00; //memory adres 
            SendBuff[3] = 0x0;
            SendBuff[4] = 255;
            sendLen = 260;
            RecvLen = RecvBuff.Length;
            int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            bool succes = ReadPresentationErrorCounter(write);
        }


        public string ReadData()
        {
            Array.Clear(SendBuff, 0, 262);
            Array.Clear(RecvBuff, 0, 262);
            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0xB0;//instruction write 
            SendBuff[2] = 0x00; //memory adres 
            SendBuff[3] = 0x0;
            SendBuff[4] = 0xFF;

            sendLen = 5;
            RecvLen = RecvBuff.Length;

            int read = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            List<Byte> list = new List<byte>();
            foreach (byte b in RecvBuff)
            {
                if (b == 0)
                    break;
                list.Add(b);
                if (b == 144)
                {
                    break;
                }
            }
            byte[] arr = list.ToArray();

            bool succes = ReadPresentationErrorCounter(read);
            if (succes)
                return Encoding.UTF8.GetString(arr, 0, arr.Length);

            return null;
        }
        #endregion

        public bool ReadPresentationErrorCounter(int code)
        {

            if (code == 0)
            {
                return true;
            }
            if (!SearchSuccesCode(RecvBuff))
                return false;

            Array.Clear(SendBuff, 0, 262);
            Array.Clear(RecvBuff, 0, 262);


            return true;
        }

        private bool SearchSuccesCode(byte[] buff)
        {
            byte succes = 144;
            foreach (byte b in buff)
            {
                if (b.Equals(succes))
                    return true;
            }
            return false;
        }


        private void close()
        {
            //base.OnClosed(e);
            ModWinsCard.SCardEndTransaction(hCard, 0);
            ModWinsCard.SCardDisconnect(hCard, 0);
            ModWinsCard.SCardReleaseContext(hContext);
        }
        #region CardSecure
        public bool ConnectSecure(string reader)
        {
            int connect = SCardConnect(this.nContext, reader, ModWinsCard.SCARD_SHARE_SHARED, ModWinsCard.SCARD_PROTOCOL_T0, ref hCard, ref Protocol);
            int transaction = ModWinsCard.SCardBeginTransaction(hCard);

            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0xA4;//instruction card type 
            SendBuff[2] = 0x0; //memory adres 
            SendBuff[3] = 0x0;
            SendBuff[4] = 0x01;
            SendBuff[5] = 0x06; //code kaart
            sendLen = 6;
            ioRequest.cbPciLength = 8;
            ioRequest.dwProtocol = Protocol;
            RecvLen = RecvBuff.Length;
            int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            bool succes = ReadPresentationErrorCounter(write);

            return succes;

        }
        public bool CheckCode(byte[] code)
        {
            Array.Clear(SendBuff, 0, 262);
            Array.Clear(RecvBuff, 0, 262);
            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0x20;//instruction write 
            SendBuff[2] = 0x00; //memory adres 
            SendBuff[3] = 0x00;
            SendBuff[4] = 0x03;
            SendBuff[5] = code[0];
            SendBuff[6] = code[1];
            SendBuff[7] = code[2];
            sendLen = 8;
            RecvLen = RecvBuff.Length;

            int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            bool succes = ReadPresentationErrorCounter(write);

            return succes;
        }


        public bool WriteDataSecure(string text)
        {
            try
            {
                Array.Clear(SendBuff, 0, 262);
                Array.Clear(RecvBuff, 0, 262);
                byte[] array = System.Text.Encoding.ASCII.GetBytes(text);
                int i = 5;
                SendBuff[0] = 0xFF;//start 
                SendBuff[1] = 0xD0;//instruction write 
                SendBuff[2] = 0x00;
                SendBuff[3] = 32;//memory adres 
                SendBuff[4] = Convert.ToByte(text.Length);
                foreach (byte b in array)
                {
                    SendBuff[i] = b;
                    i += 1;
                }
                sendLen = i;
                RecvLen = RecvBuff.Length;
                int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
                bool succes = ReadPresentationErrorCounter(write);
                return succes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ClearCardSecure()
        {
            try
            {
                Array.Clear(SendBuff, 0, 262);
                Array.Clear(RecvBuff, 0, 262);
                SendBuff[0] = 0xFF;//start 
                SendBuff[1] = 0xD0;//instruction write 
                SendBuff[2] = 0x00;
                SendBuff[3] = 32;//memory adres 
                SendBuff[4] = 200;
                sendLen = 205;
                RecvLen = RecvBuff.Length;
                int write = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
                bool succes = ReadPresentationErrorCounter(write);
                return succes;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string ReadDataSecure()
        {
            Array.Clear(SendBuff, 0, 262);
            Array.Clear(RecvBuff, 0, 262);
            SendBuff[0] = 0xFF;//start 
            SendBuff[1] = 0xB0;//instruction write 
            SendBuff[2] = 0x00;//memory adres 
            SendBuff[3] = 32;
            SendBuff[4] = 0xFF;

            sendLen = 5;
            RecvLen = RecvBuff.Length;

            int read = ModWinsCard.SCardTransmit(hCard, ref ioRequest, ref SendBuff[0], sendLen, ref ioRequest, ref RecvBuff[0], ref RecvLen);
            List<Byte> list = new List<byte>();
            foreach (byte b in RecvBuff)
            {
                if (b == 0)
                    break;
                list.Add(b);
                if (b == 144)
                {
                    break;
                }
            }
            byte[] arr = list.ToArray();

            bool succes = ReadPresentationErrorCounter(read);
            if (succes)
                return Encoding.UTF8.GetString(arr, 0, arr.Length);

            return null;
        }
        #endregion
    }
}
