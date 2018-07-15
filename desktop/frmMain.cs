using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace ReaderDemo
{
    public partial class frmMain : Form
    {
        #region RFID Variables

        public string _strCurrentRFIDAnt1 = "";
        public string _strCurrentRFIDAnt2 = "";
        public string _strCurrentRFIDAnt3 = "";
        public string _strCurrentRFIDAnt4 = "";

        public List<string> _lstScannedValidRfid = new List<string>();
        public List<string> _lstWrittenValidRfid = new List<string>();

        string _strLengthTagData = "8";//textBox6
        string _strAddressTagData = "0";//textBox5
        public int res, k, cmd;
        public byte Mask; 
        public int m_hScanner = -1, m_hSocket = -1, OK = 0;
        public uint hostport;
        public int ComMode = 0;
        public int HardVersion, SoftVersion;
        public byte connect_OK = 0;
        public int AutoPort;
        public int m_antenna_sel;
        public int nidEvent, mem, ptr, len;
        public byte[] mask = new byte[96];
        public int nBaudRate = 0, Interval, EPC_Word;
        public int Read_times;
        public int count_test = 0;
        public byte[,] TagBuffer = new byte[100, 130];
        public byte[] IDTemp = new byte[120];
        public byte[,] TagNumber = new byte[100, 9];
        public byte[] AccessPassWord = new byte[4];

        Reader Reader = new Reader();
        //基本参数和自动参数
        public Reader.ReaderBasicParam gBasicParam = new Reader.ReaderBasicParam();
        public Reader.ReaderAutoParam gAutoParam = new Reader.ReaderAutoParam();

        public byte gFre = 0;
        public byte[] gReaderID = new byte[33];
        int iantennaIndex = 0;

        #endregion

        #region RFID Settings

        public string _strReaderIP = "", _strServerIP = "";
        public string _Ant1 = "read", _Ant2 = "read", _Ant3 = "read", _Ant4 = "read";
        public uint _intRfidPort = 1969;
        public int _intStatusA = 0, _intStatusB = 0, _intStatusC = 0, _intStatusD = 0;
        #endregion

        #region Tara variables

            string _strfldPCSerialNo = "", _strfldRFIDIPaddress = "",  _strfldCurrentStatus = "false", _strfldLastCommand = "";
            string _strLocalIP = "";
            uint _strfldRFIDPortNo = 0;

            TaraWebservice.NyxSoft_AMS_Services ws = new ReaderDemo.TaraWebservice.NyxSoft_AMS_Services();

            List<string> _lstScannedRfid = new List<string>();
            string[] _strScannedRfidForHashing;
            HashSet<string> hash = new HashSet<string>();

        #endregion

        public frmMain()
        {
            InitializeComponent();
            AlignLabels();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            
            int i;

            string strtemp = "";
            double Freq = 0, jumpFreq = 0, temp = 0;

            Reader.SetAppHwnd((int)this.Handle);

            //6B与6C
            radioButton36.Text = "ISO18000-6B";
            radioButton46.Text = "ISO18000-6C";

            //暂时不用其它类型标签
            radioButton38.Visible = false;
            radioButton39.Visible = false;

            //读卡的持续时间
            comboBox1.Items.Clear();
            comboBox1.Items.Add("10ms");
            comboBox1.Items.Add("20ms");
            comboBox1.Items.Add("30ms");
            comboBox1.Items.Add("40ms");
            comboBox1.SelectedIndex = 0;


            comboBox2.SelectedIndex = 3;


            Freq = 865.00;
            jumpFreq = 0.500;

            for (i = 0; i < 7; i++)
            {
                temp = Freq + i * jumpFreq;
                strtemp = string.Format("{0,0:D}{1,0:s}{2,7:F03}", i, ":", temp);

                comboBox14.Items.Add(strtemp);
                comboBox15.Items.Add(strtemp);
            }


            Freq = 902.00;
            jumpFreq = 0.500;


            for (i = 6; i < 59; i++)
            {

                temp = Freq + (i - 6) * jumpFreq;
                strtemp = string.Format("{0,0:D}{1,0:s}{2,7:F03}", i + 1, ":", temp);
                comboBox14.Items.Add(strtemp);
                comboBox15.Items.Add(strtemp);
            }

            comboBox14.SelectedIndex = 7;
            comboBox15.SelectedIndex = 59;

            __btnConnect.Enabled = true;
            __btnDisconnect.Enabled = false;


#if ENABLE_R2000_protocol
            textBox35.Text = "192.168.0.178";
            textBox36.Text = "4001";
            radioButton43.Checked = false;
            radioButton42.Checked = true;//网口
#endif
            _strLocalIP = GetLocalIP();
            _strfldPCSerialNo = GetLogicalHardDiskID();

            __timerCheckServer.Enabled = true;

            //Thread _threadCheckCommand = new Thread(CheckServer);
            //_threadCheckCommand.IsBackground = true;
            //_threadCheckCommand.Start();

            //__timerCheckServer.Enabled = true;

            //CheckServer();

        }

        /// <summary>
        /// Used to get the HDDID of the PC
        /// </summary>
        /// <returns></returns>
        public string GetLogicalHardDiskID()
        {
            string hdd = string.Empty;

            try
            {
                ManagementClass partionsClass = new ManagementClass("Win32_LogicalDisk");
                ManagementObjectCollection partions = partionsClass.GetInstances();
                foreach (ManagementObject partion in partions)
                {
                    hdd = Convert.ToString(partion["VolumeSerialNumber"]);

                    if (hdd != string.Empty)
                        return hdd;
                }

            }
            catch
            { }

            return hdd;
        }

        /// <summary>
        /// Get local IP
        /// </summary>
        private string GetLocalIP()
        {
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                return "127.0.0.1";
            }

            return "127.0.0.1";
        }

        /// <summary>
        /// Constantly checking the server
        /// </summary>
        private void CheckServer()
        {
            DataTable _dtWebserviceData = new DataTable();

            //while (true)
            //{
                try
                {

                //_strScannedRfidForHashing = _lstScannedRfid.ToArray();//convert scanned rfid to array
                //hash = new HashSet<string>(_strScannedRfidForHashing);//hash the array
                
                string[] _strarrTemp = hash.ToArray();//send this hash to server


                string _strResults = ws.TravelHistory_SaveRecord(_strarrTemp, "NYXSOFT2018");

                if(_strResults.Split('`')[0] == "true")
                    hash.Clear();

                
                //if activate command
                //ConnectReader()
                _dtWebserviceData = ws.LoadRFIDSetting_Where(_strfldPCSerialNo, "NYXSOFT2018");
                    //"`" + fldPCSerialNo + "`" + fldRFIDIPaddress + "`" + fldRFIDPortNo + "`" + fldCurrentStatus + "`" + fldLastCommand
                    //LoadRFIDSetting_Where
                    //RFIDSetting_SaveRecord

                    if (_dtWebserviceData.Rows.Count <= 0)
                    {
                        string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + "" + "`" + "" + "`" + _strfldCurrentStatus + "`" + "", "NYXSOFT2018");
                    }
                    else//validate result from server
                    {
                        string __strHddID = _dtWebserviceData.Rows[0][1].ToString();
                        string __strReaderIP = _dtWebserviceData.Rows[0][2].ToString();
                        uint __strReaderPort = Convert.ToUInt16(_dtWebserviceData.Rows[0][3].ToString());
                        string __srtCurrentStatus = _dtWebserviceData.Rows[0][4].ToString();
                        string __strLastCommand = _dtWebserviceData.Rows[0][5].ToString();

                    if (__strLastCommand == "true")
                    {
                        if (string.IsNullOrEmpty(_strfldRFIDIPaddress) || _strfldRFIDPortNo == 0)
                        {
                            //connect reader
                            _strfldRFIDIPaddress = __strReaderIP;
                            _strfldRFIDPortNo = __strReaderPort;
                            ConnectReader();
                        }
                        else if ((_strfldRFIDIPaddress != __strReaderIP) || (_strfldRFIDPortNo != __strReaderPort))
                        {
                            //disconnect and reconnect reader
                            DisconnectReader();

                            _strfldRFIDIPaddress = __strReaderIP;
                            _strfldRFIDPortNo = __strReaderPort;
                            ConnectReader();

                        }

                        if (connect_OK == 1)
                        {
                            if(__lblRfidStatus.Text == "Disconnected")
                                ConnectReader();

                            if (__btnStartReading.Text == "Stop")//already reading
                            {
                                string _strResultDC = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Reading" + "`" + "", "NYXSOFT2018");
                            }
                            else
                            {
                                //start reading
                                StartReading();
                                string _strResultDC = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Reading" + "`" + "", "NYXSOFT2018");
                            }
                        }
                        else
                        {
                            //connect reader and start reading
                            ConnectReader();
                            StartReading();
                            string _strResultDC = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Reading" + "`" + "", "NYXSOFT2018");
                        }
                    }
                    else
                    {
                        _strfldRFIDIPaddress = __strReaderIP;
                        _strfldRFIDPortNo = __strReaderPort;
                        DisconnectReader();
                        string _strResultDC = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Reader Disconnected" + "`" + "", "NYXSOFT2018");

                    }
                }


                    //timer1.Enabled = false;


                    //Thread.Sleep(15000);
                }
                catch (Exception ex)
                { }
            //}


        }

        /// <summary>
        /// Connect to reader
        /// </summary>
        private void ConnectReader()
        {
            //read first the config
            ReadConfig();
            _lstScannedValidRfid.Clear();
            //连接设备按钮功能
            int i;
            byte HardVer = 0;
            //szPort = comboBox19.Text;
            //readerip = textBox35.Text;
            //readerport = Convert.ToUInt16(textBox36.Text);
            //hostip = textBox37.Text;
            //hostport = Convert.ToUInt16(textBox38.Text);

            //connect_OK = 0;//0--no connect, 1--connected

            ComMode = 1;//网口模式 //set to TCP/IP

            //res = Reader.Net_ConnectScanner(ref m_hSocket, _strReaderIP, _intRfidPort, _strServerIP, 26782);
            res = Reader.Net_ConnectScanner(ref m_hSocket, _strfldRFIDIPaddress, _strfldRFIDPortNo, _strLocalIP, 26782);

            if ((res != OK) && (ComMode == 1))
            {
                //MessageBox.Show("No reader connected to the network", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult1 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "No reader connected to the network" + "`" + "", "NYXSOFT2018");
                return;
            }

            //连上了
#if ENABLE_R2000_protocol
            this.tabControl1.SelectedIndex = 2;//进入第3页,ISO180006C
#else
            res = Reader.Net_GetReaderVersion(m_hSocket, ref HardVersion, ref SoftVersion);

            if (res != OK)
            {
                connect_OK = 0;

                //MessageBox.Show("Can't get reader version!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult3 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Cant get reader version" + "`" + "", "NYXSOFT2018");
                return;
            }

            HardVer = (byte)HardVersion;
            string _strHWVersion = String.Format("{0,0:D02}{1,0:D02}", (byte)(HardVersion >> 8), (byte)HardVersion);
            string _strSWVersion = String.Format("{0,0:D02}{1,0:D02}", (byte)(SoftVersion >> 8), (byte)SoftVersion);

            res = Reader.Net_ReadBasicParam(m_hSocket, ref gBasicParam);

            if (res != OK)
            {

                //MessageBox.Show("Connect Reader Fail!(BasicParam)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            res = Reader.Net_GetFrequencyRange(m_hSocket, ref gFre);

            if (res != OK)
            {
                //MessageBox.Show("Connect Reader Fail!(Frequency)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult2 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Connect Reader Fail(Frequency)" + "`" + "", "NYXSOFT2018");

                return;
            }

            res = Reader.Net_ReadAutoParam(m_hSocket, ref gAutoParam);

            if (res != OK)
            {
                //MessageBox.Show("Connect Reader Fail!(AutoParam)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult4 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Connect Reader Fail!(AutoParam)" + "`" + "", "NYXSOFT2018");

                return;
            }

            res = Reader.Net_GetReaderID(m_hSocket, gReaderID);

            if (res != OK)
            {
                //MessageBox.Show("Connect Reader Fail!(GetReaderID)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult6 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Connect Reader Fail(GetReaderID)" + "`" + "", "NYXSOFT2018");

                return;
            }

            //6.是否设置自动模式
            if (gBasicParam.WorkMode == 0)
            {
                res = Reader.Net_AutoMode(m_hSocket, 1);

                if (res != OK)
                {
                    //MessageBox.Show("Connect Reader Fail!(AutoMode)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    string _strResult5 = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Connect Reader Fail(AutoMode)" + "`" + "", "NYXSOFT2018");

                    return;
                }
            }



            //连上了的
            //基本参数更新
            OnReadParameter(0);

            //自动参数更新
            OnReadParameter(1);
#endif
            //弹出提示成功
            connect_OK = 1;//0--no connect, 1--connected
            __btnConnect.Enabled = false;
            __btnDisconnect.Enabled = true;
            __btnStartReading.Enabled = true;

            //MessageBox.Show("Connect reader success!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Connected" + "`" + "", "NYXSOFT2018");

            __lblRfidStatus.Text = "Connected";
            __lblRfidStatus.ForeColor = Color.Green;
        }

        private void __btnConnect_Click(object sender, EventArgs e)
        {
            
        }

        private void ReadConfig()
        {
            try
            {
                string[] _strConfig = File.ReadAllLines(Application.StartupPath + "\\config.cfg");

                _strReaderIP = _strConfig[0].Split(';')[1].ToString();//READERIP settings
                _strServerIP = _strConfig[1].Split(';')[1].ToString();//SERVERIP settings
                _intRfidPort = Convert.ToUInt16(_strConfig[2].Split(';')[1].ToString());//PORT settings

                if (_strConfig[3].Split(';')[1].ToString().ToLower() == "write")//ANTENNA1
                    _Ant1 = "write";
                else if (_strConfig[3].Split(';')[1].ToString().ToLower() == "read")
                    _Ant1 = "read";
                else
                    _Ant1 = "none";

                if (_strConfig[4].Split(';')[1].ToString().ToLower() == "write")//ANTENNA2
                    _Ant2 = "write";
                else if (_strConfig[4].Split(';')[1].ToString().ToLower() == "read")
                    _Ant2 = "read";
                else
                    _Ant2 = "none";

                if (_strConfig[5].Split(';')[1].ToString().ToLower() == "write")//ANTENNA3
                    _Ant3 = "write";
                else if (_strConfig[5].Split(';')[1].ToString().ToLower() == "read")
                    _Ant3 = "read";
                else
                    _Ant3 = "none";

                if (_strConfig[6].Split(';')[1].ToString().ToLower() == "write")//ANTENNA4
                    _Ant4 = "write";
                else if (_strConfig[6].Split(';')[1].ToString().ToLower() == "read")
                    _Ant4 = "read";
                else
                    _Ant4 = "none";


                _intStatusA = int.Parse(_strConfig[7].Split(';')[1].ToString());//Pre defined cycle count for status A
                _intStatusB = int.Parse(_strConfig[8].Split(';')[1].ToString());//Pre defined cycle count for status A
                _intStatusC = int.Parse(_strConfig[9].Split(';')[1].ToString());//Pre defined cycle count for status A
                _intStatusD = int.Parse(_strConfig[10].Split(';')[1].ToString());//Pre defined cycle count for status A


            }
            catch(Exception ex)
            { }
        }

        public void OnReadParameter(int iOpt)	//iOpt,0--基本参数，1--自动参数
        {
            if (0 == iOpt)
            {
                //基本参数更新
                //版本
                string _strSWVersion = String.Format("{0,0:D02}{1,0:D02}", (byte)(HardVersion >> 8), (byte)HardVersion);
                string _strHWVersion = String.Format("{0,0:D02}{1,0:D02}", (byte)(SoftVersion >> 8), (byte)SoftVersion);

                //bps
                //comboBox13.SelectedIndex = gBasicParam.BaudRate - 4;
                //int[] br = new int[] { 9600, 19200, 38400, 57600, 115200 };
                //nBaudRate = br[gBasicParam.BaudRate - 4];


                //最小频率
                //最大频率
                int imin = gBasicParam.Min_Frequence - 1;
                int imax = gBasicParam.Max_Frequence - 1;
                OnCbnSetFre(gFre, imin, imax);//因为通过国家来，每个国家的频率有些不一样的

                //RF power
                string _strRFpower = gBasicParam.Power.ToString();

                //Address of Reader (1-254)
                string _strAddressOfReader = gBasicParam.ReaderAddress.ToString();


                //Max Tags of once Reading(1-100)
                string _strMaxTagOnceRead = gBasicParam.NumofCard.ToString();

                //工作模式
                //0-自动,1-Command
                switch (gBasicParam.WorkMode)
                {
                    case 0:
                        radioButton29.Checked = true;
                        break;
                    case 1:
                        radioButton31.Checked = true;
                        break;
                }


                //读写器ID
                string _strReaderID = "";
                for (int i = 0; i < 10; i++)
                {
                    _strReaderID += (char)gReaderID[i];
                }


                //buzzer
                checkBox13.Checked = Convert.ToBoolean(gBasicParam.EnableBuzzer);

                //哪种标签
                //0-6B,1-6C
                if ((gBasicParam.TagType & 0x01) == 1)
                {
                    radioButton36.Checked = true;
                }

                if ((gBasicParam.TagType & 0x04) == 4)
                {
                    radioButton46.Checked = true;

                }


                //读卡的持续时间
                comboBox1.SelectedIndex = gBasicParam.ReadDuration;

                //读写器的IP
                string _strReaderIP = "";
                _strReaderIP = gBasicParam.IP1.ToString();
                _strReaderIP += ".";
                _strReaderIP += gBasicParam.IP2.ToString();
                _strReaderIP += ".";
                _strReaderIP += gBasicParam.IP3.ToString();
                _strReaderIP += ".";
                _strReaderIP += gBasicParam.IP4.ToString();

                //读写器的端口
                string _strReaderPort = "";
                int m_ReaderPort = (((int)gBasicParam.Port1) << 8) + (int)gBasicParam.Port2;
                _strReaderPort = m_ReaderPort.ToString();

                //MASK
                string _strMask = "";
                _strMask = gBasicParam.Mask1.ToString();
                _strMask += ".";
                _strMask += gBasicParam.Mask2.ToString();
                _strMask += ".";
                _strMask += gBasicParam.Mask3.ToString();
                _strMask += ".";
                _strMask += gBasicParam.Mask4.ToString();

                //Gateway
                string _strGateway = "";
                _strGateway = gBasicParam.Gateway1.ToString();
                _strGateway += ".";
                _strGateway += gBasicParam.Gateway2.ToString();
                _strGateway += ".";
                _strGateway += gBasicParam.Gateway3.ToString();
                _strGateway += ".";
                _strGateway += gBasicParam.Gateway4.ToString();

                //MAC
                string MACstr,_strMacAdd;
                MACstr = "";
                MACstr = gBasicParam.MAC1.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC2.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC3.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC4.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC5.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC6.ToString("X02");

                _strMacAdd = MACstr;




            }
            else if (1 == iOpt)
            {
                //自动参数更新
                //天线1-4
                if ((gAutoParam.Antenna & 0x01) == 1)//gAutoParam->Antenna & 0x01
                    checkBox14.Checked = true;
                else
                    checkBox14.Checked = false;

                if (((gAutoParam.Antenna >> 1) & 0x01) == 1)//gAutoParam->Antenna & 0x02
                    checkBox15.Checked = true;
                else
                    checkBox15.Checked = false;

                if (((gAutoParam.Antenna >> 2) & 0x01) == 1)//gAutoParam->Antenna & 0x04
                    checkBox16.Checked = true;
                else
                    checkBox16.Checked = false;

                if (((gAutoParam.Antenna >> 3) & 0x01) == 1)//gAutoParam->Antenna & 0x08
                    checkBox17.Checked = true;
                else
                    checkBox17.Checked = false;



                //工作模式
                //0-定时模式,1-触发模式
                switch (gAutoParam.AutoMode)
                {
                    case 0:
                        radioButton23.Checked = true;
                        break;
                    case 1:
                        radioButton30.Checked = true;
                        break;
                }

                //输出格式
                //0-简化,1-标准
                switch (gAutoParam.OutputManner)
                {
                    case 0:
                        radioButton40.Checked = true;
                        break;
                    case 1:
                        radioButton41.Checked = true;
                        break;
                }

                //输出端口
                //0-RS232,1-RS485,2-RJ45,3-Wiegand26,4-Wiegand34
                switch (gAutoParam.OutInterface)
                {
                    case 0:
                        radioButton35.Checked = true;
                        break;
                    case 1:
                        radioButton34.Checked = true;
                        break;
                    case 2:
                        radioButton37.Checked = true;
                        break;
                    case 3:
                        radioButton32.Checked = true;
                        break;
                    case 4:
                        radioButton33.Checked = true;
                        break;
                }

                //触发模式
                //0-低电平，1-高电平
                switch (gAutoParam.TriggerMode)
                {
                    case 0:
                        radioButton24.Checked = true;
                        break;
                    case 1:
                        radioButton25.Checked = true;
                        break;
                }

                //定时间隔
                //0-10ms，1-20ms，2-30ms，3-50ms，4-100ms。缺省值为2。每隔设定时间主动读取一次标签。
                switch (gAutoParam.Interval)
                {
                    case 0:
                        radioButton51.Checked = true;
                        break;
                    case 1:
                        radioButton52.Checked = true;
                        break;
                    case 2:
                        radioButton53.Checked = true;
                        break;
                    case 3:
                        radioButton54.Checked = true;
                        break;
                    case 4:
                        radioButton55.Checked = true;
                        break;
                }

                //主机IP
                string _strHostIP = "";
                _strHostIP = gAutoParam.HostIP1.ToString();
                _strHostIP += ".";
                _strHostIP += gAutoParam.HostIP2.ToString();
                _strHostIP += ".";
                _strHostIP += gAutoParam.HostIP3.ToString();
                _strHostIP += ".";
                _strHostIP += gAutoParam.HostIP4.ToString();



                //主机PORT
                string _strHostPort = "";//textBox44
                int m_HostPort = 0;
                m_HostPort = (int)(gAutoParam.Port1 << 8) + (int)gAutoParam.Port2;
                _strHostPort = m_HostPort.ToString();
                AutoPort = m_HostPort;


                //检测报警
                if (gAutoParam.Alarm == 1)
                {
                    checkBox6.Checked = true;
                }
                else
                {
                    checkBox6.Checked = false;
                }

                //控制继电器
                if (gAutoParam.EnableRelay == 1)
                    checkBox21.Checked = true;
                else
                    checkBox21.Checked = false;


                //通知条件
                byte m_Condiona = gAutoParam.Report_Condition;	//0-立即通知,1-定时通知,2-增加新标签,3-减少标签,4-标签数变化		
                //////////////////////////////////////////////////////////////////////////
                //保留时间
                string _strKeepTime = "";//textBox10
                int m_PersistenceTime = 0;
                m_PersistenceTime = gAutoParam.TimeH;
                m_PersistenceTime = (m_PersistenceTime << 8) + gAutoParam.TimeL;
                _strKeepTime = m_PersistenceTime.ToString();

                //保留数目
                string _strNoOfTags = "";
                int m_LenofList = 0;
                m_LenofList = gAutoParam.NumH;
                m_LenofList = (m_LenofList << 8) + gAutoParam.NumL;
                _strNoOfTags = m_LenofList.ToString();
                //间隔时间, 通知间隔
                int shValue = 0;////读卡总超时1
                string _strNotifInterval = "";//textBox12
                shValue = gAutoParam.Report_Interval;
                _strNotifInterval = shValue.ToString();


                //选哪根天线,0-一根天线,1-四根天线
                if (iantennaIndex == 0)
                {
                    //直接---0,保留时间为0, 通知间隔为 用户自定义
                    //标准---1,保留时间为0, 通知间隔为 用户自定义
                    //定时---2,保留时间为用户自定义, 通知间隔为0,定时与标准互换
                    switch (m_Condiona)
                    {
                        case 0://直接
                            m_PersistenceTime = gAutoParam.TimeH;
                            m_PersistenceTime = (m_PersistenceTime << 8) + gAutoParam.TimeL;
                            _strKeepTime = m_PersistenceTime.ToString();

                            shValue = gAutoParam.Report_Interval;
                            _strNotifInterval = shValue.ToString();

                            break;
                        case 1://标准
                            m_PersistenceTime = gAutoParam.TimeH;
                            m_PersistenceTime = (m_PersistenceTime << 8) + gAutoParam.TimeL;
                            _strKeepTime = m_PersistenceTime.ToString();

                            shValue = gAutoParam.Report_Interval;
                            _strNotifInterval = shValue.ToString();

                            break;
                        case 2://定时
                            m_PersistenceTime = gAutoParam.TimeH;
                            m_PersistenceTime = (m_PersistenceTime << 8) + gAutoParam.TimeL;
                            _strNotifInterval = m_PersistenceTime.ToString();

                            shValue = gAutoParam.Report_Interval;
                            _strKeepTime = shValue.ToString();
                            break;

                    }

                }// end of if ( iantennaIndex == 0 )


                //是否通知主机
                if (gAutoParam.Report_Output == 1)
                {
                    checkBox7.Checked = true;
                }
                else
                {
                    checkBox7.Checked = false;
                }



                //WiegandWidth(*10us)
                string _strPulseWidth = "";//textBox31
                _strPulseWidth = gAutoParam.WiegandWidth.ToString();
                //WiegandBetween(*10us)
                string _strPulseInterval = "";//textBox30
                _strPulseInterval = gAutoParam.WiegandInterval.ToString();


                //卡的类型
                //0-标签本身ID，1-用户自定义
                switch (gAutoParam.IDPosition)
                {
                    case 0:
                        radioButton47.Checked = true;
                        break;
                    case 1:
                        radioButton48.Checked = true;
                        break;
                }

                //卡号起始地址
                string _strStartIDAddress = "";
                _strStartIDAddress = gAutoParam.ID_Start.ToString();


            }

        }

        public void OnCbnSetFre(int sel, int imin, int imax)
        {
            int i = 0;
            int k = 0;
            int iCount = 0;
            long iValue = 0;
            string strFreqTmp;
            Reader.tagReaderFreq tmpFreq;

            //频率计算公式
            //级数 = 50;
            //步进 = 500KHz;
            //起始频率 = 902750;
            //902750 + 级数*步进;
            comboBox14.Items.Clear();
            comboBox15.Items.Clear();
            switch (sel)
            {//国家频率字符串, //级数, 步进, 起始频率, 公式：902750 + 级数*步进
                case 0://{"0---FCC（美国）", 63, 400, 902600},								//(0)
                    tmpFreq.chFreq = "0---FCC（美国）";
                    tmpFreq.iGrade = 63;
                    tmpFreq.iSkip = 400;
                    tmpFreq.dwFreq = 902600;
                    break;

                case 1://{"1---ETSI EN 300-220（欧洲300-220）", -1, -1, -1},				//(1)
                    tmpFreq.chFreq = "1---ETSI EN 300-220（欧洲300-220）";
                    tmpFreq.iGrade = -1;
                    tmpFreq.iSkip = -1;
                    tmpFreq.dwFreq = -1;
                    break;

                case 2://{"2---ETSI EN 302-208（欧洲302-208）", 4, 600, 865700},			//(2)
                    tmpFreq.chFreq = "2---ETSI EN 302-208（欧洲302-208）";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 600;
                    tmpFreq.dwFreq = 865700;
                    break;

                case 3://"3---HK920-925香港", 10, 500, 920250},							//(3)
                    tmpFreq.chFreq = "3---HK920-925香港";
                    tmpFreq.iGrade = 10;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 902750;
                    break;

                case 4://{"4---TaiWan 922-928台湾", 12, 500, 922250},						//(4)
                    tmpFreq.chFreq = "4---TaiWan 922-928台湾";
                    tmpFreq.iGrade = 12;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 922250;
                    break;

                case 5://{"5---Japan 952-954日本", 0, 0, 0},								//(5)
                    tmpFreq.chFreq = "5---Japan 952-954日本";
                    tmpFreq.iGrade = 0;
                    tmpFreq.iSkip = 0;
                    tmpFreq.dwFreq = 0;
                    break;

                case 6://{"6---Japan 952-955日本", 14,200, 952200},						//(6)
                    tmpFreq.chFreq = "6---Japan 952-955日本";
                    tmpFreq.iGrade = 14;
                    tmpFreq.iSkip = 200;
                    tmpFreq.dwFreq = 952200;
                    break;

                case 7://{"7---ETSI EN 302-208欧洲", 4, 600, 865700},						//(7)
                    tmpFreq.chFreq = "7---ETSI EN 302-208欧洲";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 600;
                    tmpFreq.dwFreq = 865700;
                    break;

                case 8://{"8---Korea 917-921韩国", 6, 600, 917300},						//(8)
                    tmpFreq.chFreq = "8---Korea 917-921韩国";
                    tmpFreq.iGrade = 6;
                    tmpFreq.iSkip = 600;
                    tmpFreq.dwFreq = 917300;
                    break;

                case 9://{"0---FCC（美国）", 8, 500, 919250},					//(9)
                    tmpFreq.chFreq = "0---FCC（美国）";
                    tmpFreq.iGrade = 8;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 919250;
                    break;

                case 10://{"0---FCC（美国）", 16, 250, 920625},						//(10)
                    tmpFreq.chFreq = "0---FCC（美国）";
                    tmpFreq.iGrade = 16;
                    tmpFreq.iSkip = 250;
                    tmpFreq.dwFreq = 920625;
                    break;

                case 11://{"0---FCC（美国）", 4, 1200, 952400},						//(11)
                    tmpFreq.chFreq = "0---FCC（美国）";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 1200;
                    tmpFreq.dwFreq = 952400;
                    break;

                case 12://{"12--South Africa 915-919南美", 17, 200, 915600},				//(12)
                    tmpFreq.chFreq = "12--South Africa 915-919南美";
                    tmpFreq.iGrade = 17;
                    tmpFreq.iSkip = 200;
                    tmpFreq.dwFreq = 915600;
                    break;

                case 13://{"13--Brazil 902-907/915-928巴西", 35, 500, 902750},				//(13)
                    tmpFreq.chFreq = "13--Brazil 902-907/915-928巴西";
                    tmpFreq.iGrade = 35;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 902750;
                    break;

                case 14://{"14--Thailand 920-925泰国", -1, -1, -1},						//(14)
                    tmpFreq.chFreq = "14--Thailand 920-925泰国";
                    tmpFreq.iGrade = -1;
                    tmpFreq.iSkip = -1;
                    tmpFreq.dwFreq = -1;
                    break;

                case 15://{"15--Singapore 920-925新加坡", 10, 500, 920250},				//(15)
                    tmpFreq.chFreq = "15--Singapore 920-925新加坡";
                    tmpFreq.iGrade = 10;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 920250;
                    break;

                case 16://{"16--Australia 920-926澳洲", 12, 500, 920250},					//(16)
                    tmpFreq.chFreq = "16--Australia 920-926澳洲";
                    tmpFreq.iGrade = 12;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 920250;
                    break;

                case 17://{"17--India 865-867印度", 4, 600, 865100},						//(17)
                    tmpFreq.chFreq = "17--India 865-867印度";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 600;
                    tmpFreq.dwFreq = 865100;
                    break;

                case 18://{"18--Uruguay 916-928乌拉圭", 23, 500, 916250},					//(18)
                    tmpFreq.chFreq = "18--Uruguay 916-928乌拉圭";
                    tmpFreq.iGrade = 23;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 916250;
                    break;

                case 19://{"19--Vietnam 920-925越南", 10, 500, 920250},					//(19)
                    tmpFreq.chFreq = "19--Vietnam 920-925越南";
                    tmpFreq.iGrade = 10;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 920250;
                    break;

                case 20://{"20--Israel 915-917", 1, 0, 916250},							//(20)
                    tmpFreq.chFreq = "20--Israel 915-917";
                    tmpFreq.iGrade = 1;
                    tmpFreq.iSkip = 0;
                    tmpFreq.dwFreq = 916250;
                    break;

                case 21://{"21--Philippines 918-920菲律宾", 4, 500, 918250},				//(21)
                    tmpFreq.chFreq = "21--Philippines 918-920菲律宾";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 918250;
                    break;

                case 22://{"22--Canada 902-928加拿大", 42, 500, 902750},					//(22)
                    tmpFreq.chFreq = "22--Canada 902-928加拿大";
                    tmpFreq.iGrade = 42;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 902750;
                    break;

                case 23://{"23--Indonesia 923-925印度尼西亚", 4, 500, 923250},				//(23)
                    tmpFreq.chFreq = "23--Indonesia 923-925印度尼西亚";
                    tmpFreq.iGrade = 4;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 923250;
                    break;

                case 24://{"24--New Zealand 921.5-928新西兰", 11, 500, 922250},			//(24)
                    tmpFreq.chFreq = "24--New Zealand 921.5-928新西兰";
                    tmpFreq.iGrade = 11;
                    tmpFreq.iSkip = 500;
                    tmpFreq.dwFreq = 922250;
                    break;

                default:
                    return;
            }
            k = 0;
            iCount = tmpFreq.iGrade;

            if (22 == sel)
            {
                int[] iArray = new int[3];
                iArray[0] = 902750;
                iArray[1] = 903250;
                iArray[2] = 905750;

                //加拿大的特述
                for (i = 0; i < 3; i++)
                {
                    strFreqTmp = string.Format("{0,2:D}{1,0:D}", i + 1, iArray[i]);
                    comboBox14.Items.Add(strFreqTmp);
                    comboBox15.Items.Add(strFreqTmp);
                }
                tmpFreq.dwFreq = iArray[i - 1];
                iCount = tmpFreq.iGrade - 3;
                k = 3;
            }

            for (i = k; i < iCount + k; i++)
            {
                iValue = tmpFreq.dwFreq + i * tmpFreq.iSkip;
                strFreqTmp = string.Format("{0,2:D}{1,0:S}{2,0:D}", i + 1, "---", iValue);
                comboBox14.Items.Add(strFreqTmp);
                comboBox15.Items.Add(strFreqTmp);
            }

            iCount = tmpFreq.iGrade;
            if (i > 0)
            {
                if (imin == -1 && imax == -1)
                {
                    comboBox14.SelectedIndex = 0;
                    comboBox15.SelectedIndex = iCount - 1;
                }
                else
                {

                    comboBox14.SelectedIndex = imin;
                    comboBox15.SelectedIndex = imax - 1;
                }
            }


        }

        private void DisconnectReader()
        {
            //断开连接设备功能
            if (connect_OK == 0)
                return;

            switch (ComMode)
            {
                case 0:
                case 2:
                    res = Reader.DisconnectScanner(m_hScanner);
                    break;
                case 1:
                    res = Reader.Net_DisconnectScanner(m_hSocket);
                    break;
            }

            if (res != OK)
            {
                //MessageBox.Show("Can't Disconnect Scanner!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Cant Disconnect Scanner!" + "`" + "", "NYXSOFT2018");

                return;
            }
            else
            {
                __btnConnect.Enabled = true;
                __btnDisconnect.Enabled = false;
                __btnStartReading.Enabled = false;

                __lblRfidStatus.Text = "Disconnected";
                __lblRfidStatus.ForeColor = Color.Red;

                //MessageBox.Show("Disconnect Scanner successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Disconnect Scanner successfully" + "`" + "", "NYXSOFT2018");

            }
        }

        private void __btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectReader();
        }

        private void StartReading()
        {
            //列出标签
            if (connect_OK == 0)
                return;
            nidEvent = 1;
            string str, strtemp;
            string _strAddressOfTag = "0", _strStartOfTag = "0";//textBox1.Text,textBox2.Text

            str = "";

            int str_len = 0;
            int i, m, j;

            if (Convert.ToInt32(_strStartOfTag) == 0)
                m = 0;
            else
            {
                m = Convert.ToInt32(_strStartOfTag) / 8;
                if (Convert.ToInt32(_strStartOfTag) % 8 != 0)
                {
                    for (i = 0; i < ((m + 1) * 2 - str_len); i++)
                        str += "0";
                    m++;
                }
            }

            mem = 1;//memory bank EPC


            ptr = Convert.ToInt16(_strAddressOfTag);
            len = Convert.ToInt16(_strStartOfTag);

            for (i = 0; i < m; i++)
            {
                strtemp = str[i * 2].ToString() + str[i * 2 + 1].ToString();
                mask[i] = Convert.ToByte(strtemp);
            }
            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[3];//reading interval 50ms
            if (__btnStartReading.Text == "Start")
            {//select antenna for reading
                m_antenna_sel = 0;
                if (_Ant1 == "read")
                    m_antenna_sel += 1;
                if (_Ant2 == "read")
                    m_antenna_sel += 2;
                if (_Ant3 == "read")
                    m_antenna_sel += 4;
                if (_Ant4 == "read")
                    m_antenna_sel += 8;

                if (m_antenna_sel == 0)
                {
                    //MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Please choose one antenna at least" + "`" + "", "NYXSOFT2018");

                    __btnStartReading.Text = "Start";
                    return;
                }

                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);

                if (res != 0)
                {
                    //MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    string _strResult = ws.RFIDSetting_SaveRecord("`" + _strfldPCSerialNo + "`" + _strfldRFIDIPaddress + "`" + _strfldRFIDPortNo + "`" + "Fail to set antennaPlease try again" + "`" + "", "NYXSOFT2018");

                    __btnStartReading.Text = "Start";
                    return;
                }

                __btnStartReading.Text = "Stop";
                __lblRfidStatus.Text = "Connected";
                __lblRfidStatus.ForeColor = Color.Green;
                __btnDisconnect.Enabled = false;
                Read_times = 0;
                k = 0;
                count_test = 0;
                //listView1.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;
                

            }
            else
            {
                __btnStartReading.Text = "Start";
                timer1.Enabled = false;
                __btnDisconnect.Enabled = true;
                //__timerRfidWriter.Enabled = false;
                __lblRfidStatus.Text = "Connected, Reading Stopped";
                __lblRfidStatus.ForeColor = Color.Red;
            }
        }

        private void __btnStartReading_Click(object sender, EventArgs e)
        {
            StartReading();
        }

        //antenna setup for reading
        bool _isAntenna1 = false, _isAntenna2 = true, _isAntenna3 = false, _isAntenna4 = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            int i, j, nCounter = 0, ID_len = 0, ID_len_temp = 0, be_antenna, success;
            string str, str_temp, strtemp;
            byte[] DB = new byte[128];
            byte[] IDBuffer = new byte[7680];
            bool _isWriteTag = false;//trigger the tag writing

            Read_times++;
            Thread.Sleep(Interval);
            _isWriteTag = false;
            be_antenna = 0;
            switch (Read_times % 4)//activating the antennas
            {
                case 0:
                    if (_Ant1 == "read")
                    {
                        be_antenna = 1;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    else if (_Ant1 == "write")
                    {
                        _isWriteTag = true;
                        be_antenna = 1;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }

                    break;
                case 1:
                    if (_Ant2 == "read")
                    {
                        be_antenna = 2;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    else if (_Ant2 == "write")
                    {
                        _isWriteTag = true;
                        be_antenna = 2;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
                case 2:
                    if (_Ant3 == "read")
                    {
                        be_antenna = 4;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    else if (_Ant3 == "write")
                    {
                        _isWriteTag = true;
                        be_antenna = 4;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
                case 3:
                    if (_Ant4 == "read")
                    {
                        be_antenna = 8;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    else if (_Ant4 == "write")
                    {
                        _isWriteTag = true;
                        be_antenna = 8;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }

                    }
                    break;
            }

            switch (nidEvent)
            {
                case 1:
                    if (be_antenna != 0)
                    {
                        Array.Clear(TagBuffer, 0, TagBuffer.Length);
                        count_test++;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.EPC1G2_ReadLabelID(m_hScanner, mem, ptr, len, mask, IDBuffer, ref nCounter, 0);
                                break;
                            case 1:
                                res = Reader.Net_EPC1G2_ReadLabelID(m_hSocket, mem, ptr, len, mask, IDBuffer, ref nCounter);
                                break;
                        }
                        if (res == OK)//if reading is successful
                        {
                            for (i = 0; i < nCounter; i++)
                            {
                                if (IDBuffer[ID_len] > 32)
                                {
                                    nCounter = 0;
                                    break;
                                }
                                ID_len_temp = IDBuffer[ID_len] * 2 + 1;
                                for (j = 0; j < ID_len_temp; j++)
                                {
                                    TagBuffer[i, j] = IDBuffer[ID_len + j];
                                }
                                ID_len += ID_len_temp;
                            }
                            for (i = 0; i < nCounter; i++)
                            {
                                str = "";
                                strtemp = "";
                                ID_len = TagBuffer[i, 0] * 2;
                                for (j = 0; j < ID_len; j++)
                                {
                                    strtemp = TagBuffer[i, j + 1].ToString("X2");
                                    str += strtemp;
                                }

                                if(be_antenna == 1)//if current antenna activated is Antenna 1
                                    _strCurrentRFIDAnt1 = str;//get the current read tag
                                if (be_antenna == 2)//if current antenna activated is Antenna 2
                                    _strCurrentRFIDAnt2 = str;//get the current read tag
                                if (be_antenna == 4)//if current antenna activated is Antenna 3
                                    _strCurrentRFIDAnt3 = str;//get the current read tag
                                if (be_antenna == 8)//if current antenna activated is Antenna 4
                                    _strCurrentRFIDAnt4 = str;//get the current read tag

                            }

                            string _strRfidToValidate = "";
                            if (!_isWriteTag)//if reader is just assigned to reading
                            {
                                if (be_antenna == 1)//if current antenna activated is Antenna 1
                                    _strRfidToValidate = _strCurrentRFIDAnt1;//get the current read tag
                                if (be_antenna == 2)//if current antenna activated is Antenna 2
                                    _strRfidToValidate = _strCurrentRFIDAnt2;//get the current read tag
                                if (be_antenna == 4)//if current antenna activated is Antenna 3
                                    _strRfidToValidate = _strCurrentRFIDAnt3;//get the current read tag
                                if (be_antenna == 8)//if current antenna activated is Antenna 4
                                    _strRfidToValidate = _strCurrentRFIDAnt4;//get the current read tag

                                //_lstScannedRfid.Add("Monumento~" + _strRfidToValidate);
                                hash.Add("ST0011`" + _strRfidToValidate);


                                /*if (_strRfidToValidate.Length >= 24)
                                {
                                    __lblScannedRFID.Text = _strRfidToValidate;

                                    __lblPalletNum.Text = _strRfidToValidate.Substring(0, 6);//get first 6 characters
                                    string _strTemp = _strRfidToValidate.Substring(6, 12);
                                    string _strDateTime = "";
                                    string _strStatus = "";
                                    bool _isDateValid = false;
                                    bool _isCCValid = false;

                                    if (_strTemp.Length == 12)
                                    {
                                        _strDateTime = _strTemp.Substring(0, 2) + "/" + _strTemp.Substring(2, 2) + "/" + _strTemp.Substring(4, 4) + " " + _strTemp.Substring(8, 2) + ":" + _strTemp.Substring(10, 2);//MM:dd:yyyy HH:mm
                                        DateTime _dtReturn = new DateTime();
                                        _isDateValid = DateTime.TryParse(_strDateTime, out _dtReturn);

                                        if (_isDateValid)
                                            __lblDatetimeLastUsed.Text = _strDateTime;
                                        else
                                            __lblDatetimeLastUsed.Text = "INVALID";
                                    }
                                    else
                                        __lblDatetimeLastUsed.Text = _strRfidToValidate.Substring(7, 12);//from index 7 to 18 for the date time

                                    int _intReturn = 0;
                                    _isCCValid = int.TryParse(_strRfidToValidate.Substring(18, 5), out _intReturn);

                                    if (_isCCValid)
                                        __lblCyclecount.Text = _strRfidToValidate.Substring(18, 5);//from index 18 to 23 for the cycle count
                                    else
                                        __lblCyclecount.Text = "INVALID";

                                    if (_strRfidToValidate.Substring(23, 1) == "A")
                                        _strStatus = "For Maintenance";
                                    else if (_strRfidToValidate.Substring(23, 1) == "B")
                                        _strStatus = "Reach end of life";
                                    else if (_strRfidToValidate.Substring(23, 1) == "C")
                                        _strStatus = "Defective Unit";
                                    else if (_strRfidToValidate.Substring(23, 1) == "D")
                                        _strStatus = "OK for use";
                                    else
                                        _strStatus = "INVALID";

                                    __lblStatuscode.Text = _strStatus;//24th character for the status code

                                    if ((_strRfidToValidate.Length < 24) || (!_isDateValid) || (!_isCCValid) || (_strStatus == "INVALID"))
                                    {
                                        __lblPalletNum.Text = "INVALID";
                                    }
                                    else
                                    {
                                        if (!_lstScannedValidRfid.Contains(_strRfidToValidate))
                                            _lstScannedValidRfid.Add(_strRfidToValidate);//if tag is valid and not currently recorded, add it to the list


                                    }
                                }*/

                            }
                            else//if reader is just assigned to writing//write to tag
                            {
                                if (be_antenna == 1)//if current antenna activated is Antenna 1
                                {
                                    if (_lstScannedValidRfid.Contains(_strCurrentRFIDAnt1))
                                        RfidWriter(_strCurrentRFIDAnt1);
                                }

                                if (be_antenna == 2)//if current antenna activated is Antenna 2
                                {
                                    if (_lstScannedValidRfid.Contains(_strCurrentRFIDAnt2))
                                        RfidWriter(_strCurrentRFIDAnt2);
                                }
                                if (be_antenna == 4)//if current antenna activated is Antenna 3
                                {
                                    if (_lstScannedValidRfid.Contains(_strCurrentRFIDAnt3))
                                        RfidWriter(_strCurrentRFIDAnt3);
                                }
                                if (be_antenna == 8)//if current antenna activated is Antenna 4
                                {
                                    if (_lstScannedValidRfid.Contains(_strCurrentRFIDAnt4))
                                        RfidWriter(_strCurrentRFIDAnt4);
                                }


                            }
                        }

                        
                    }
                    break;
            }

            //__timerRfidWriter.Enabled = true;
        }

        private void __btnWrite_Click(object sender, EventArgs e)
        {
            //块写
            if (connect_OK == 0)
                return;
            int i;
            string str;
            //listBox1.Items.Clear();
            string _strTagAccessPass = "00000000";//textBox7
            string _strLenTagDataWrite = "6";//write 6 word data//textBox6
            string _strAddressTagData = "0";// textBox5
            int _intIndexToWrite = 0;//comboBox3
            if (_strTagAccessPass.Length != 8)
            {
                //MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //textBox7.Focus();
                //textBox7.SelectAll();
                return;
            }

            str = _strTagAccessPass;
            for (i = 0; i < 4; i++)
            {
                //以前这种做法不妥,改用下下一句
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
                //AccessPassWord[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }

            //if (radioButton5.Checked == true)
            //    mem = 0;
            //if (radioButton6.Checked == true)
                mem = 1;//lets write on EPC
            //if (radioButton7.Checked == true)
            //    mem = 2;
            //if (radioButton8.Checked == true)
            //    mem = 3;
                
            switch (mem)
            {
                case 0:
                case 2:
                    if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 4)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 4 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //textBox6.Focus();
                        //textBox6.SelectAll();
                        return;
                    }
                    break;
                case 1:
                    if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 6)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //textBox6.Focus();
                        //textBox6.SelectAll();
                        return;
                    }
                    break;
                case 3:
                    if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 8)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //textBox6.Focus();
                        //textBox6.SelectAll();
                        return;
                    }
                    break;
            }

            if (__txtWriteData.Text.Length == 0 || __txtWriteData.Text.Length / 4 < Convert.ToInt16(_strLenTagDataWrite))
            {
                MessageBox.Show("Please input enough Length of Tag Data!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            /////////////////////////////////

            str = __txtWriteData.Text;
            for (i = 0; i < __txtWriteData.Text.Length / 2; i++)
            {
                mask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            m_antenna_sel = 0;
            //select antenna for writing
            if (_Ant1 == "write")
                m_antenna_sel += 1;
            if (_Ant2 == "write")
                m_antenna_sel += 2;
            if (_Ant3 == "write")
                m_antenna_sel += 4;
            if (_Ant4 == "write")
                m_antenna_sel += 8;
            switch (m_antenna_sel)
            {
                case 1:
                case 2:
                case 4:
                case 8:
                    for (i = 0; i < 3; i++)
                    {
                        switch (ComMode)
                        {
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("SetAntenna Fail!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }
            len = Convert.ToInt16(_strLenTagDataWrite);
#if false //发卡机用这个接口，平时可以不用
            if (mem == 1)
            {
                for (i = 0; i < 5; i++)
                {
                    Thread.Sleep(30);
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.EPC1G2_WriteEPC(m_hScanner, Convert.ToByte(textBox6.Text), mask, AccessPassWord, 0);
                            break;
                        case 1:
                            res = Reader.Net_EPC1G2_WriteEPC(m_hSocket, Convert.ToByte(textBox6.Text), mask, AccessPassWord);
                            break;
                        case 2:
                            res = Reader.EPC1G2_WriteEPC(m_hScanner, Convert.ToByte(textBox6.Text), mask, AccessPassWord, RS485Address);
                            break;
                    }
                    if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                        break;
                }
            }
            else
#endif
            {
                if (int.Parse(_strAddressTagData) < 0)
                {
                    MessageBox.Show("Please read first than choose a tag!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                switch (mem)
                {
                    case 0:
                    case 2:
                        if (Convert.ToInt16(_strAddressTagData) < 0 || Convert.ToInt16(_strAddressTagData) > 3)
                        {
                            MessageBox.Show("Please input Location of Tag Address between 0 and 4!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //textBox5.Focus();
                            //textBox5.SelectAll();
                            return;
                        }
                        if ((len + Convert.ToInt16(_strAddressTagData)) > 4)
                        {
                            len = 4 - Convert.ToInt16(_strAddressTagData);
                        }
                        break;
                    case 3:
                        if (Convert.ToInt16(_strAddressTagData) < 0)
                        {
                            MessageBox.Show("Please input Location of Tag Address more than 0!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //textBox5.Focus();
                            //textBox5.SelectAll();
                            return;
                        }
                        break;
                }
                EPC_Word = TagBuffer[_intIndexToWrite, 0];
                //int _intTagBufferLength = TagBuffer.Length;
                //EPC_Word = TagBuffer[0, 0];//first on the list

                for (i = 0; i < TagBuffer[_intIndexToWrite, 0] * 2; i++)
                {
                    IDTemp[i] = TagBuffer[_intIndexToWrite, i + 1];
                }

                for (i = 0; i < 5; i++)
                {
                    Thread.Sleep(30);
                    switch (ComMode)
                    {
                        case 1:
                            res = Reader.Net_EPC1G2_WriteWordBlock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(_strAddressTagData), Convert.ToByte(len), mask, AccessPassWord);
                            break;
                    }
                    if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                        break;
                }
            }

            if (res == OK)
            {
                MessageBox.Show("Write Successfully!");
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Write Fail!";
                if (res == 9)
                    str = "The access password is error!";
                MessageBox.Show(str);
            }
        }

        public void ReportError(ref string temp)
        {
            switch (res)
            {
                case 1:
                    temp = "Connect antenna fail!";
                    break;
                case 2:
                    temp = "No Tag!";
                    break;
                case 3:
                    temp = "Illegal Tag!";
                    break;
                case 4:
                    temp = "Power is not enough!";
                    break;
                case 5:
                    temp = "The memory has been protected!";
                    break;
                case 6:
                    temp = "Check sum error!";
                    break;
                case 7:
                    temp = "Parameter error!";
                    break;
                case 8:
                    temp = "The memory don't exist!";
                    break;
                case 9:
                    temp = "The Access Password is error!";
                    break;
                case 10:
                    temp = "The Kill Password cannot be 000000!";
                    break;
                case 14:
                    temp = "Locked Tags in the field!";
                    break;
                case 30:
                    temp = "Invalid Command!";
                    break;
                case 31:
                    temp = "Other Error!";
                    break;
                default:
                    temp = "Unbeknown Error!";
                    break;
            }
        }

        private void __timerRfidWriter_Tick(object sender, EventArgs e)
        {
            int i, j, nCounter = 0, ID_len = 0, ID_len_temp = 0, be_antenna, success;
            string str, str_temp, strtemp;
            byte[] DB = new byte[128];
            byte[] IDBuffer = new byte[7680];
            string _strCurrentRfidInWriter = "";

            Read_times++;
            Thread.Sleep(Interval);

            be_antenna = 0;
            switch (Read_times % 4)
            {
                case 0:
                    if (_Ant1 == "write")
                    {
                        be_antenna = 1;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
                case 1:
                    if (_Ant2 == "write")
                    {
                        be_antenna = 2;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
                case 2:
                    if (_Ant3 == "write")
                    {
                        be_antenna = 4;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
                case 3:
                    if (_Ant4 == "write")
                    {
                        be_antenna = 8;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, be_antenna);
                                break;
                        }
                    }
                    break;
            }


            ListViewItem item = new ListViewItem();
            switch (nidEvent)
            {
                case 1:
                    if (be_antenna != 0)
                    {
                        Array.Clear(TagBuffer, 0, TagBuffer.Length);
                        count_test++;
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.EPC1G2_ReadLabelID(m_hScanner, mem, ptr, len, mask, IDBuffer, ref nCounter, 0);
                                break;
                            case 1:
                                res = Reader.Net_EPC1G2_ReadLabelID(m_hSocket, mem, ptr, len, mask, IDBuffer, ref nCounter);
                                break;
                        }
                        if (res == OK)
                        {
                            for (i = 0; i < nCounter; i++)
                            {
                                if (IDBuffer[ID_len] > 32)
                                {
                                    nCounter = 0;
                                    break;
                                }
                                ID_len_temp = IDBuffer[ID_len] * 2 + 1;
                                for (j = 0; j < ID_len_temp; j++)
                                {
                                    TagBuffer[i, j] = IDBuffer[ID_len + j];
                                }
                                ID_len += ID_len_temp;
                            }
                            for (i = 0; i < nCounter; i++)
                            {
                                str = "";
                                strtemp = "";
                                ID_len = TagBuffer[i, 0] * 2;
                                for (j = 0; j < ID_len; j++)
                                {
                                    strtemp = TagBuffer[i, j + 1].ToString("X2");
                                    str += strtemp;
                                }

                                for (j = 0; j < k; j++)
                                {
                                    /*strtemp = listView1.Items[j].SubItems[2].Text;
                                    if (ID_len == Convert.ToInt32(strtemp, 16) * 2)
                                    {
                                        str_temp = listView1.Items[j].SubItems[1].Text;
                                        if (str == str_temp)
                                        {
                                            success = Convert.ToInt32(listView1.Items[j].SubItems[3].Text) + 1;
                                            listView1.Items[j].SubItems[3].Text = success.ToString();
                                            break;
                                        }
                                    }*/
                                }
                                if (j == k)
                                {
                                    /*item = listView1.Items.Add((k + 1).ToString(), k);
                                    item.SubItems.Add(str);
                                    item.SubItems.Add(TagBuffer[i, 0].ToString("X2"));
                                    success = 1;
                                    item.SubItems.Add(success.ToString());
                                    item.SubItems.Add(count_test.ToString());
                                    k++;
                                    //listView1.Items.Add(item);*/
                                }
                                _strCurrentRfidInWriter = str;
                            }


                        }

                        //__lblScannedRFID.Text = _strCurrentRFID;

                        if (_lstScannedValidRfid.Contains(_strCurrentRfidInWriter))
                        { 
                            
                        }
                        //for (j = 0; j < k; j++)
                        //listView1.Items[j].SubItems[4].Text = count_test.ToString();
                    }
                    break;
                case 2:
                    if (be_antenna != 0)
                    {
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.EPC1G2_ReadWordBlock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(_strAddressTagData), Convert.ToByte(_strLengthTagData), DB, AccessPassWord, 0);
                                break;
                            case 1:
                                res = Reader.Net_EPC1G2_ReadWordBlock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(_strAddressTagData), Convert.ToByte(_strLengthTagData), DB, AccessPassWord);
                                break;
                        }
                        if (res == OK)
                        {
                            str = "";

                            for (i = 0; i < Convert.ToByte(_strLengthTagData) * 2; i++)
                            {
                                strtemp = DB[i].ToString("X2");
                                str += strtemp;
                            }
                            //listBox1.Items.Add(str);

                        }
                    }
                    break;

                case 3://EAS报警
                    if (be_antenna != 0)
                    {
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.EPC1G2_EasAlarm(m_hScanner, 0);
                                break;
                            case 1:
                                res = Reader.Net_EPC1G2_EasAlarm(m_hSocket);
                                break;
                        }
                        if (res == OK)
                        {
                            //Reader.MessageBeep(-1);

                            Icon icon = new Icon("icon1.ico", 32, 32);
                            //pictureBox1.Image = icon.ToBitmap();
                            //pictureBox1.Image = Image.FromFile("icon1.ico", false);   

                        }
                        else
                        {
                            Icon icon2 = new Icon("icon2.ico", 32, 32);
                            //pictureBox1.Image = icon2.ToBitmap();
                        }

                        //Thread.Sleep(100);
                        //pictureBox1.Image = Image.FromFile("D:\\App\\Reader\\Reader626\\ReaderC#WDemo\\WADemo\\icon2.ico", false);   


                    }
                    break;



                case 5://6B列出标签
                    Array.Clear(TagBuffer, 0, TagBuffer.Length);
                    count_test++;
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.ISO6B_ReadLabelID(m_hScanner, TagNumber, ref nCounter, 0);
                            break;
                        case 1:
                            res = Reader.Net_ISO6B_ReadLabelID(m_hSocket, TagNumber, ref nCounter);
                            break;
                    }
                    if (res == OK)
                    {
                        for (i = 0; i < nCounter; i++)
                        {
                            str = "";
                            //for (j = 0; j < 8; j++)
                            for (j = 1; j <= 8; j++)
                            {
                                str += TagNumber[i, j].ToString("X2");
                            }

                            for (j = 0; j < k; j++)
                            {
                                /*str_temp = listView2.Items[j].SubItems[1].Text;
                                if (str == str_temp)
                                {
                                    success = Convert.ToInt32(listView2.Items[j].SubItems[2].Text) + 1;
                                    listView2.Items[j].SubItems[2].Text = success.ToString();
                                    break;
                                }*/
                            }
                            if (j == k)
                            {
                                /*item = listView2.Items.Add((k + 1).ToString(), k);
                                item.SubItems.Add(str);
                                success = 1;
                                item.SubItems.Add(success.ToString());
                                item.SubItems.Add(count_test.ToString());
                                k++;*/
                            }

                        }

                    }
                    /*for (j = 0; j < k; j++)
                        listView2.Items[j].SubItems[3].Text = count_test.ToString();*/
                    break;
                case 6:
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.ISO6B_ReadByteBlock(m_hScanner, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask, 0);
                            break;
                        case 1:
                            res = Reader.Net_ISO6B_ReadByteBlock(m_hSocket, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask);
                            break;
                    }
                    if (res == OK)
                    {
                        str = "HEX:";
                        for (i = 0; i < len; i++)
                        {
                            str_temp = mask[i].ToString("X2");
                            str += str_temp;
                        }
                        //listBox2.Items.Add(str);
                        //listBox2.Items.Add("---------------------------------");
                    }
                    break;
                case 7:
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.ISO6B_WriteByteBlock(m_hScanner, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask, 0);
                            break;
                        case 1:
                            res = Reader.Net_ISO6B_WriteByteBlock(m_hSocket, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask);
                            break;
                    }
                    if (res == OK)
                    {
                        //listBox2.Items.Add("Write Successfully!");
                        //listBox2.Items.Add("---------------------------------");
                    }
                    else
                    {
                        //listBox2.Items.Add("Write Fail!");
                        //listBox2.Items.Add("---------------------------------");
                    }
                    break;
                case 8:
                    count_test++;
                    switch (ComMode)
                    {
                        case 1:
                            res = Reader.Net_ISO6B_ListSelectedID(m_hSocket, Convert.ToByte(cmd), Convert.ToByte(ptr), Mask, mask, TagNumber, ref nCounter);
                            break;
                    }
                    if (res == OK)
                    {
                        for (i = 0; i < nCounter; i++)
                        {
                            str = "";
                            for (j = 1; j <= 8; j++)
                            {
                                str += TagNumber[i, j].ToString("X2");
                            }

                            for (j = 0; j < k; j++)
                            {
                                /*str_temp = listView2.Items[j].SubItems[1].Text;
                                if (str == str_temp)
                                {
                                    success = Convert.ToInt32(listView2.Items[j].SubItems[2].Text) + 1;
                                    listView2.Items[j].SubItems[2].Text = success.ToString();
                                    break;
                                }*/
                            }
                            if (j == k)
                            {
                                //item = listView2.Items.Add((k + 1).ToString(), k);
                                //item.SubItems.Add(str);
                                //success = 1;
                                //item.SubItems.Add(success.ToString());
                                //item.SubItems.Add(count_test.ToString());
                                //k++;
                            }

                        }

                    }
                    /*for (j = 0; j < k; j++)
                        listView2.Items[j].SubItems[3].Text = count_test.ToString();*/
                    break;

            }
        }

        private void RfidWriter(string _strRfid)
        {
            string _strPalletNum = "", _strDatetime = "", _strCycleCount = "", _strStatusCode = "", _strValueToWrite = "";

            //块写
            if (connect_OK == 0)
                return;
            int i;
            string str;
            //listBox1.Items.Clear();
            string _strTagAccessPass = "00000000";//textBox7
            string _strLenTagDataWrite = "6";//write 6 word data//textBox6
            string _strAddressTagData = "0";// textBox5
            int _intIndexToWrite = 0;//comboBox3
            if (_strTagAccessPass.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //textBox7.Focus();
                //textBox7.SelectAll();
                return;
            }

            str = _strTagAccessPass;
            for (i = 0; i < 4; i++)
            {
                //以前这种做法不妥,改用下下一句
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
                //AccessPassWord[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }

            _strPalletNum = _strRfid.Substring(0, 6);//get first 6 characters for pallet

            string _strTemp = _strRfid.Substring(6, 12);//date time from tag
            _strDatetime = _strTemp.Substring(0, 2) + "/" + _strTemp.Substring(2, 2) + "/" + _strTemp.Substring(4, 4) + " " + _strTemp.Substring(8, 2) + ":" + _strTemp.Substring(10, 2);//MM:dd:yyyy HH:mm

            DateTime _dtNow = DateTime.Now;
            DateTime _Datetime = Convert.ToDateTime(_strDatetime);

            TimeSpan ts = _dtNow - _Datetime;

            int _intDifference = ts.Minutes;

            _strCycleCount = _strRfid.Substring(18, 5);//from index 18 to 23 for the cycle count

            string _strStatus = "";
            _strStatusCode = _strRfid.Substring(23, 1);//read current status code

            if (_intDifference >= 1)//if time interval of last write is two minutes or greater proceed to write
            {
                int _intTempCC = (int.Parse(_strCycleCount) + 1);

                if (_intTempCC == _intStatusA)
                    _strStatus = "A";//maintenance
                else if (_intTempCC == _intStatusB)
                    _strStatus = "B";//reach end of life
                else if (_intTempCC == _intStatusC)
                    _strStatus = "C";//defective
                else if (_intTempCC == _intStatusD)
                    _strStatus = "D";//OK for Use
                else
                    _strStatus = _strStatusCode;//retain

                string _strNewDatetime = DateTime.Now.ToString("MMddyyyyHHmm");

                _strCycleCount = _intTempCC.ToString().PadLeft(5, '0');

                _strValueToWrite = _strPalletNum + _strNewDatetime + _strCycleCount + _strStatus;

                //if (radioButton5.Checked == true)
                //    mem = 0;
                //if (radioButton6.Checked == true)
                mem = 1;//lets write on EPC
                //if (radioButton7.Checked == true)
                //    mem = 2;
                //if (radioButton8.Checked == true)
                //    mem = 3;

                switch (mem)
                {
                    case 0:
                    case 2:
                        if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 4)
                        {
                            MessageBox.Show("Please input Length of Tag Data between 1 and 4 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //textBox6.Focus();
                            //textBox6.SelectAll();
                            return;
                        }
                        break;
                    case 1:
                        if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 6)
                        {
                            MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //textBox6.Focus();
                            //textBox6.SelectAll();
                            return;
                        }
                        break;
                    case 3:
                        if (Convert.ToInt16(_strLenTagDataWrite) < 1 || Convert.ToInt16(_strLenTagDataWrite) > 8)
                        {
                            MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //textBox6.Focus();
                            //textBox6.SelectAll();
                            return;
                        }
                        break;
                }

                if (_strValueToWrite.Length == 0 || _strValueToWrite.Length / 4 < Convert.ToInt16(_strLenTagDataWrite))
                {
                    MessageBox.Show("Please input enough Length of Tag Data!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                /////////////////////////////////

                str = _strValueToWrite;
                for (i = 0; i < _strValueToWrite.Length / 2; i++)
                {
                    mask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
                }

                m_antenna_sel = 0;
                //select antenna for writing
                if (_Ant1 == "write")
                    m_antenna_sel += 1;
                if (_Ant2 == "write")
                    m_antenna_sel += 2;
                if (_Ant3 == "write")
                    m_antenna_sel += 4;
                if (_Ant4 == "write")
                    m_antenna_sel += 8;
                switch (m_antenna_sel)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != OK)
                        {
                            MessageBox.Show("SetAntenna Fail!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                }
                len = Convert.ToInt16(_strLenTagDataWrite);
#if false //发卡机用这个接口，平时可以不用
            if (mem == 1)
            {
                for (i = 0; i < 5; i++)
                {
                    Thread.Sleep(30);
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.EPC1G2_WriteEPC(m_hScanner, Convert.ToByte(textBox6.Text), mask, AccessPassWord, 0);
                            break;
                        case 1:
                            res = Reader.Net_EPC1G2_WriteEPC(m_hSocket, Convert.ToByte(textBox6.Text), mask, AccessPassWord);
                            break;
                        case 2:
                            res = Reader.EPC1G2_WriteEPC(m_hScanner, Convert.ToByte(textBox6.Text), mask, AccessPassWord, RS485Address);
                            break;
                    }
                    if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                        break;
                }
            }
            else
#endif
                {
                    if (int.Parse(_strAddressTagData) < 0)
                    {
                        MessageBox.Show("Please read first than choose a tag!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    switch (mem)
                    {
                        case 0:
                        case 2:
                            if (Convert.ToInt16(_strAddressTagData) < 0 || Convert.ToInt16(_strAddressTagData) > 3)
                            {
                                MessageBox.Show("Please input Location of Tag Address between 0 and 4!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //textBox5.Focus();
                                //textBox5.SelectAll();
                                return;
                            }
                            if ((len + Convert.ToInt16(_strAddressTagData)) > 4)
                            {
                                len = 4 - Convert.ToInt16(_strAddressTagData);
                            }
                            break;
                        case 3:
                            if (Convert.ToInt16(_strAddressTagData) < 0)
                            {
                                MessageBox.Show("Please input Location of Tag Address more than 0!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                //textBox5.Focus();
                                //textBox5.SelectAll();
                                return;
                            }
                            break;
                    }
                    EPC_Word = TagBuffer[_intIndexToWrite, 0];
                    //int _intTagBufferLength = TagBuffer.Length;
                    //EPC_Word = TagBuffer[0, 0];//first on the list

                    for (i = 0; i < TagBuffer[_intIndexToWrite, 0] * 2; i++)
                    {
                        IDTemp[i] = TagBuffer[_intIndexToWrite, i + 1];
                    }

                    for (i = 0; i < 5; i++)
                    {
                        Thread.Sleep(30);
                        switch (ComMode)
                        {
                            case 1:
                                res = Reader.Net_EPC1G2_WriteWordBlock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(_strAddressTagData), Convert.ToByte(len), mask, AccessPassWord);
                                break;
                        }
                        if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                            break;
                    }
                }

                if (res == OK)
                {
                    //MessageBox.Show("Write Successfully!");
                    //_lstWrittenValidRfid.Add(_strRfid);
                    toolStripStatusLabel1.Text = "Write Successfully! " + _strValueToWrite;
                }
                else
                {
                    /*this.ReportError(ref str);
                    if (str == "Unbeknown Error!")
                        str = "Write Fail!";
                    if (res == 9)
                        str = "The access password is error!";
                    MessageBox.Show(str);*/
                }
            }
        }

        private void AlignLabels()
        {

            //__lblCurrentScanned.Location = new Point((this.Width / 2) - (__lblCurrentScanned.Width / 2), ((this.Height / 2) - (__lblCurrentScanned.Height / 2)));


        }

        private void __btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void __timerDateTime_Tick(object sender, EventArgs e)
        {
            __lblTime.Text = DateTime.Now.ToString("hh:mm tt");
            __lblDate.Text = DateTime.Now.ToString("dddd dd, MMMM");
        }

        bool _isHidden = false;
        private void __pbxMenu_Click(object sender, EventArgs e)
        {
            if (!_isHidden)
            {
                __btnConnect.Visible = false;
                __btnDisconnect.Visible = false;
                __btnStartReading.Visible = false;

                _isHidden = true;
            }
            else
            {
                __btnConnect.Visible = true;
                __btnDisconnect.Visible = true;
                __btnStartReading.Visible = true;

                _isHidden = false;
            }
        }

        private void __timerCheckServer_Tick(object sender, EventArgs e)
        {
            __timerCheckServer.Enabled = false;
            __timerCheckServer.Interval = 2000;
            CheckServer();
            __timerCheckServer.Enabled = true;
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

       
    }
}
