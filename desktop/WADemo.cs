//#####################################################################
//add by martrin 20140110增加华为斯迈尔以前用Reader2600,现在换成四口的
//R2000的，所以要改用R2000协议, 开启用R2000, 否则用以前的
//#define					ENABLE_R2000_protocol

//#####################################################################

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace ReaderDemo
{ 
    
    public partial class MainForm : Form
    { 
     
        //Program Program = new Program();
        Reader Reader = new Reader();
        //基本参数和自动参数
        public Reader.ReaderBasicParam gBasicParam = new Reader.ReaderBasicParam();
        public Reader.ReaderAutoParam gAutoParam = new Reader.ReaderAutoParam();

        public byte gFre = 0;	//取哪个国家的频率
        public byte[] gReaderID  = new byte[33];			//读写器ID
        int iantennaIndex   =   0;	//选哪根天线,0-一根天线,1-四根天线



        public int res, k, Detect, cmd;
        public int m_hScanner = -1, m_hSocket = -1,OK = 0;
        public int iAddress = 0;//485用的,如果值为0则为RS232
        public int nBaudRate = 0, Interval, EPC_Word;
        public string szPort;
        public int HardVersion, SoftVersion;
        public int  hwnd;
        public int nidEvent,mem,ptr,len;
        public int Read_times;
        public int m_antenna_sel,RS485Address;
        public byte Mask; 
        public byte []AccessPassword=new byte[4];
        public byte[] mask = new byte[96];
        public byte[] IDTemp = new byte[120];
        public byte[,] TagBuffer = new byte[100, 130];
        public byte[] AccessPassWord = new byte[4];
        public byte[,] TagNumber = new byte [100,9];
        public int ComMode=0;//0---RS232,1---网口,2---RS485
        public string readerip;
        public uint readerport;
        public string hostip;
        public uint hostport;
        public byte connect_OK = 0;
        public int count_test = 0;
        public  Socket NetSocket = null;
        public  string ReaderIP;
        public  int ReaderPort;
        public  byte[] IPrecebuffer = new byte[64];

        public  Socket EpcSocket = null;
        public int AutoPort;
        public  byte[] Epcrecebuffer = new byte[256];
        public string[] EpcStr=new string [7];

        public delegate void NetInvoke();

        

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int i;

            string strtemp = ""; 
            double Freq = 0, jumpFreq = 0, temp = 0;

            Reader.SetAppHwnd((int)this.Handle);

            //6B与6C
            radioButton36.Text  =   "ISO18000-6B";
            radioButton46.Text  =   "ISO18000-6C";

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


            comboBox9.Items.Clear();
            comboBox9.Items.Add("10ms");
            comboBox9.Items.Add("20ms");
            comboBox9.Items.Add("30ms");
            comboBox9.Items.Add("50ms");
            comboBox9.Items.Add("100ms");
            comboBox9.Items.Add("200ms");
            comboBox9.Items.Add("500ms");
            comboBox9.SelectedIndex = 0;


            //自动模式下，开始和结束
            button14.Enabled = true;
            button15.Enabled = false;


            comboBox2.SelectedIndex = 3;
            comboBox19.SelectedIndex = 0;
            
            


            //得到主机地址
            //得到主机地址
            //得到主机地址
            //IPAddress ips = Dns.GetHostAddresses(Dns.GetHostName())[0];
            //textBox37.Text = ips.ToString();
            IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ips in arrIPAddresses)
            {

                if (ips.AddressFamily.Equals(AddressFamily.InterNetwork))
                {
                    //MessageBox.Show(ips.ToString());
                    textBox37.Text = ips.ToString();
                    DialogResult j = (DialogResult)MessageBox.Show("主机地址是这个IP地址吗(" + textBox37.Text + ")?。", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (j == DialogResult.Yes)
                        break;
                }
            }
            


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
            comboBox13.SelectedIndex = 3;


            radioButton43.Checked = true;
            button26.Enabled = true;
            button27.Enabled = false;
            //停止扫描这个设备
            button13.Enabled = false;

            #if ENABLE_R2000_protocol
            textBox35.Text = "192.168.0.178";
            textBox36.Text = "4001";
            radioButton43.Checked = false;
            radioButton42.Checked = true;//网口
            #endif

            //暂时先隐藏语言，后续再加上
            groupBox22.Visible = false;

        }

  #region EPCC1G2
        #region List Tag ID
        private void button3_Click(object sender, EventArgs e)
        {
            //列出标签
            if (connect_OK == 0)
                return;
            nidEvent = 1;
            string str,strtemp;

            if (Convert.ToInt32(textBox1.Text) > 95 || Convert.ToInt32(textBox1.Text) < 0)
            {
                MessageBox.Show("Please input start address of data between 0 and 95!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox1.Focus();
                textBox1.SelectAll();
                button3.Text = "List Tag ID";
                return;
            }
            if (Convert.ToInt32(textBox2.Text) > 96 || Convert.ToInt32(textBox2.Text) < 0)
            {
                MessageBox.Show("Please input length of data between 0 and 96 !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Focus();
                textBox2.SelectAll();
                button3.Text = "List Tag ID";
                return;
            }
            str = textBox3.Text;
            int str_len = textBox3.Text.Length;
            int i, m,j;

            if (Convert.ToInt32(textBox2.Text) == 0)
                m = 0;
            else
            {
                m = Convert.ToInt32(textBox2.Text) / 8;
                if (Convert.ToInt32(textBox2.Text) % 8 != 0)
                {
                    for (i = 0; i < ((m + 1) * 2 - str_len); i++)
                        str += "0";
                    m++;
                }
            }

            if (radioButton1.Checked == true)
                mem = 0;
            if (radioButton2.Checked == true)
                mem = 1;
            if (radioButton3.Checked == true)
                mem = 2;
            if (radioButton4.Checked == true)
                mem = 3;

            ptr = Convert.ToInt16(textBox1.Text);
            len = Convert.ToInt16(textBox2.Text);

            for (i = 0; i < m; i++)
            {
                strtemp = str[i * 2].ToString() + str[i * 2 + 1].ToString();
                mask[i] = Convert.ToByte(strtemp);
            }
            int []timrinterval=new int []{10,20,30,50,100,200,500};
            Interval = timrinterval[comboBox2.SelectedIndex];
            if (button3.Text == "List Tag ID")
            {
                m_antenna_sel = 0;
                if (checkBox1.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox2.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox3.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox4.Checked == true)
                    m_antenna_sel += 8;

                if (m_antenna_sel == 0)
                {
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    button3.Text = "List Tag ID";
                    return;
                }

                for (i = 0; i < 3; i++)
                {
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                            break;
                        case 1:
                            res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                            break;
                        case 2:
                            res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                            break;
                    }
                    if (res == OK)
                        break;

                }
                if (res != 0)
                {
                    MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    button3.Text = "List Tag ID";
                    return;
                }

                button3.Text = "Stop";
                Read_times = 0;
                k=0;
                count_test = 0;
                listView1.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;
            }
            else
            {
                button3.Text = "List Tag ID";
                timer1.Enabled = false;
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();

                byte  []dada=new byte[1024];
                for (i = 0; i < k; i++)
                {
                    str = listView1.Items[i].SubItems[2].Text;
                    TagBuffer[i, 0] = Convert.ToByte(str, 16);
                    str = listView1.Items[i].SubItems[1].Text;
                    for(j=0;j<str.Length/2;j++)
                    {
                       strtemp = str[j * 2].ToString() + str[j * 2 + 1].ToString();
                       TagBuffer[i, j + 1] = (byte)Convert.ToInt16(strtemp,16);
                    }
                    strtemp = "";
                    strtemp = (i+1).ToString("D2") + ".";
                    comboBox3.Items.Add(strtemp + str);
                    comboBox4.Items.Add(strtemp + str);
                    comboBox5.Items.Add(strtemp + str);
                    comboBox6.Items.Add(strtemp + str);
                    comboBox7.Items.Add(strtemp + str);
                    comboBox8.Items.Add(strtemp + str);

                }
                if (k != 0)
                {
                    comboBox3.SelectedIndex = 0;
                    comboBox4.SelectedIndex = 0;
                    comboBox5.SelectedIndex = 0;
                    comboBox6.SelectedIndex = 0;
                    comboBox7.SelectedIndex = 0;
                    comboBox8.SelectedIndex = 0;

                }
            }

        }
        #endregion
        #region KillerTag
        private void button4_Click(object sender, EventArgs e)
        {
            //杀死标签
            if (connect_OK == 0)
                return;
            int i, j;
            string str, strtemp;
            byte[]KillPassWord=new byte [4];


            if (comboBox4.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (textBox4.TextLength != 8)
            {
                MessageBox.Show("Please input correct killpassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox4.Focus();
                textBox4.SelectAll();
                return;
            }
            str = textBox4.Text;
            for (i = 0; i < 4; i++)
            {
                strtemp = str[i * 2].ToString() + str[i * 2 + 1].ToString();
                KillPassWord[i] = (byte)Convert.ToInt16(strtemp,16);

            }
            EPC_Word = TagBuffer[comboBox4.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox4.SelectedIndex ,0]*2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox4.SelectedIndex,i+1];
            }

            j = (int)MessageBox.Show("Are you sure to kill the tag ?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (j != 1)
                return;

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default :
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }
            for (i = 0; i < 5; i++)
            {
                Thread.Sleep(30);
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_KillTag(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, KillPassWord, 0);
                        break;
                    case 1:
                        res = Reader.Net_EPC1G2_KillTag(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, KillPassWord);
                        break;
                    case 2:
                        res = Reader.EPC1G2_KillTag(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, KillPassWord, RS485Address);
                        break;
                }
                if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                    break;
            }
            if (res == OK)
            {
                MessageBox.Show("kill tag successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Kill tag Fail!";
                if (res == 9)
                    str = "The kill password is error!";
                MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion
        #region Timer
        private void timer1_Tick(object sender, EventArgs e)
        {
            int i, j, nCounter = 0, ID_len = 0, ID_len_temp = 0, be_antenna,success;
            string str, str_temp, strtemp;
            byte[] DB = new byte[128];
            byte[] IDBuffer = new byte[7680];

            Read_times++;
            Thread.Sleep(Interval);

            be_antenna = 0;
            switch (Read_times % 4)
            {
                case 0:
                    if (checkBox1.Checked == true)
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
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, RS485Address);
                                break;
                        }
                    }
                    break;
                case 1:
                    if (checkBox2.Checked == true)
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
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, RS485Address);
                                break;
                        }
                    }
                    break;
                case 2:
                    if (checkBox3.Checked == true)
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
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, RS485Address);
                                break;
                        }
                    }
                    break;
                case 3:
                    if (checkBox4.Checked == true)
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
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, be_antenna, RS485Address);
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
                            case 2:
                                res = Reader.EPC1G2_ReadLabelID(m_hScanner, mem, ptr, len, mask, IDBuffer, ref nCounter, RS485Address);
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
                                    strtemp = listView1.Items[j].SubItems[2].Text;
                                    if (ID_len == Convert.ToInt32(strtemp, 16) * 2)
                                    {
                                        str_temp = listView1.Items[j].SubItems[1].Text;
                                        if (str == str_temp)
                                        {
                                            success = Convert.ToInt32(listView1.Items[j].SubItems[3].Text) + 1;
                                            listView1.Items[j].SubItems[3].Text = success.ToString();
                                            break;
                                        }
                                    }
                                }
                                if (j == k)
                                {
                                    item = listView1.Items.Add((k + 1).ToString(), k);
                                    item.SubItems.Add(str);
                                    item.SubItems.Add(TagBuffer[i, 0].ToString("X2"));
                                    success = 1;
                                    item.SubItems.Add(success.ToString());
                                    item.SubItems.Add(count_test.ToString());
                                    k++;
                                    //listView1.Items.Add(item);
                                }
                            }

                        }
                        for (j = 0; j < k; j++)
                            listView1.Items[j].SubItems[4].Text = count_test.ToString();
                    }
                    break;
                case 2:
                    if (be_antenna != 0)
                    {
                        switch (ComMode)
                        {
                            case 0:
                                res = Reader.EPC1G2_ReadWordBlock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(textBox6.Text), DB, AccessPassWord, 0);
                                break;
                            case 1:
                                res = Reader.Net_EPC1G2_ReadWordBlock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(textBox6.Text), DB, AccessPassWord);
                                break;
                            case 2:
                                res = Reader.EPC1G2_ReadWordBlock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(textBox6.Text), DB, AccessPassWord, RS485Address);
                                break;
                        }
                        if (res == OK)
                        {
                            str = "";
                            for (i = 0; i < Convert.ToByte(textBox6.Text) * 2; i++)
                            {
                                strtemp = DB[i].ToString("X2");
                                str += strtemp;
                            }
                            listBox1.Items.Add(str);

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
                            case 2:
                                res = Reader.EPC1G2_EasAlarm(m_hScanner, RS485Address);
                                break;
                        }
                        if (res == OK)
                        {
                            //Reader.MessageBeep(-1);
                           
                            Icon icon = new Icon("icon1.ico", 32, 32);
                            pictureBox1.Image = icon.ToBitmap();
                            //pictureBox1.Image = Image.FromFile("icon1.ico", false);   

                        }
                        else
                        {
                            Icon icon2 = new Icon("icon2.ico", 32, 32);
                            pictureBox1.Image = icon2.ToBitmap();
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
                        case 2:
                            res = Reader.ISO6B_ReadLabelID(m_hScanner, TagNumber, ref nCounter, RS485Address);
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
                                str_temp = listView2.Items[j].SubItems[1].Text;
                                if (str == str_temp)
                                {
                                    success = Convert.ToInt32(listView2.Items[j].SubItems[2].Text) + 1;
                                    listView2.Items[j].SubItems[2].Text = success.ToString();
                                    break;
                                }
                            }
                            if (j == k)
                            {
                                item = listView2.Items.Add((k + 1).ToString(), k);
                                item.SubItems.Add(str);
                                success = 1;
                                item.SubItems.Add(success.ToString());
                                item.SubItems.Add(count_test.ToString());
                                k++;
                            }

                        }

                    }
                    for (j = 0; j < k; j++)
                        listView2.Items[j].SubItems[3].Text = count_test.ToString();
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
                        case 2:
                            res = Reader.ISO6B_ReadByteBlock(m_hScanner, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask, RS485Address);
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
                        listBox2.Items.Add(str);
                        listBox2.Items.Add("---------------------------------");
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
                        case 2:
                            res = Reader.ISO6B_WriteByteBlock(m_hScanner, IDTemp, Convert.ToByte(ptr), Convert.ToByte(len), mask, RS485Address);
                            break;
                    }
                    if (res == OK)
                    {
                        listBox2.Items.Add("Write Successfully!");
                        listBox2.Items.Add("---------------------------------");
                    }
                    else
                    {
                        listBox2.Items.Add("Write Fail!");
                        listBox2.Items.Add("---------------------------------");
                    }
                    break;
                case 8:
                    count_test++;
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.ISO6B_ListSelectedID(m_hScanner, Convert.ToByte(cmd), Convert.ToByte(ptr), Mask, mask, TagNumber, ref nCounter, RS485Address);
                            break;
                        case 1:
                            res = Reader.Net_ISO6B_ListSelectedID(m_hSocket, Convert.ToByte(cmd), Convert.ToByte(ptr), Mask, mask, TagNumber, ref nCounter);
                            break;
                        case 2:
                            res = Reader.ISO6B_ListSelectedID(m_hScanner, Convert.ToByte(cmd), Convert.ToByte(ptr), Mask, mask, TagNumber, ref nCounter, RS485Address);
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
                                str_temp = listView2.Items[j].SubItems[1].Text;
                                if (str == str_temp)
                                {
                                    success = Convert.ToInt32(listView2.Items[j].SubItems[2].Text) + 1;
                                    listView2.Items[j].SubItems[2].Text = success.ToString();
                                    break;
                                }
                            }
                            if (j == k)
                            {
                                item = listView2.Items.Add((k + 1).ToString(), k);
                                item.SubItems.Add(str);
                                success = 1;
                                item.SubItems.Add(success.ToString());
                                item.SubItems.Add(count_test.ToString());
                                k++;
                            }

                        }

                    }
                    for (j = 0; j < k; j++)
                        listView2.Items[j].SubItems[3].Text = count_test.ToString();
                    break;

            }
        }
        #endregion
        #region ErrorInformation
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
        #endregion
        #region Read
        private void button5_Click(object sender, EventArgs e)
        {
            //块读
            if (connect_OK == 0)
                return;
            int i;
            string str;
            nidEvent = 2;
            if (comboBox3.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt16(textBox5.Text) < 0)
            {
                MessageBox.Show("Please input start address of tag more then 0!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox5.Focus();
                textBox5.SelectAll();
                return;
            }
            if (Convert.ToInt16(textBox6.Text) < 1)
            {
                MessageBox.Show("Please input length of data more than 1!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox6.Focus();
                textBox6.SelectAll();
                return;
            }
            if (textBox7.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                textBox7.SelectAll();
                return;
            }
            str = textBox7.Text;
            for (i = 0; i < 4; i++)
            {
                //以前这种做法不妥,改用下下一句
                //AccessPassWord[i] = Convert.ToByte(str[i * 2] + str[i * 2 + 1]);
                AccessPassWord[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);

                
            }

            EPC_Word = TagBuffer[comboBox3.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox3.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox3.SelectedIndex, i + 1];
            }
            if (radioButton5.Checked  == true)
                mem = 0;
            if (radioButton6.Checked == true)
                mem = 1;
            if (radioButton7.Checked == true)
                mem = 2;
            if (radioButton8.Checked == true)
                mem = 3;


            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox2.SelectedIndex];
            if (button5.Text == "Read")
            {
                m_antenna_sel = 0;
                if (checkBox1.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox2.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox3.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox4.Checked == true)
                    m_antenna_sel += 8;
                switch (m_antenna_sel)
                {
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        button5.Text = "Read";
                        return;
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != 0)
                        {
                            MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            button5.Text = "Read";
                            return;
                        }
                        break;
                }
                button5.Text = "Stop";
                Read_times = 0;
                k = 0;
                listBox1.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;
            }
            else
            {
                button5.Text = "Read";
                timer1.Enabled = false;
            }

        }
        #endregion
        #region Write
        private void button6_Click(object sender, EventArgs e)
        {
            //块写
            if (connect_OK == 0)
                return;
            int i;
            string str;
            listBox1.Items.Clear();

            if (textBox7.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox7.Focus();
                textBox7.SelectAll();
                return;
            }
          
            str = textBox7.Text;
            for (i = 0; i < 4; i++)
            {
                //以前这种做法不妥,改用下下一句
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()),16);
                //AccessPassWord[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
         
            if (radioButton5.Checked == true)
                mem = 0;
            if (radioButton6.Checked == true)
                mem = 1;
            if (radioButton7.Checked == true)
                mem = 2;
            if (radioButton8.Checked == true)
                mem = 3;
            switch (mem)
            {
                case 0:
                case 2:
                    if (Convert.ToInt16(textBox6.Text) < 1 || Convert.ToInt16(textBox6.Text) > 4)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 4 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox6.Focus();
                        textBox6.SelectAll();
                        return;
                    }
                    break;
                case 1:
                    if (Convert.ToInt16(textBox6.Text) < 1 || Convert.ToInt16(textBox6.Text) > 6)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox6.Focus();
                        textBox6.SelectAll();
                        return;
                    }
                    break;
                case 3:
                    if (Convert.ToInt16(textBox6.Text) < 1 || Convert.ToInt16(textBox6.Text) > 8)
                    {
                        MessageBox.Show("Please input Length of Tag Data between 1 and 6 Word !", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textBox6.Focus();
                        textBox6.SelectAll();
                        return;
                    }
                    break;
            }

            if (textBox8.Text.Length == 0 || textBox8.Text.Length / 4 < Convert.ToInt16(textBox6.Text))
            {
                MessageBox.Show("Please input enough Length of Tag Data!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
/////////////////////////////////

            str = textBox8.Text;
            for (i = 0; i < textBox8.Text.Length / 2; i++)
            {
                mask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()),16);
            }

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
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
            len = Convert.ToInt16(textBox6.Text);
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
                if (comboBox3.SelectedIndex < 0)
                {
                    MessageBox.Show("Please read first than choose a tag!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                switch (mem)
                {
                    case 0:
                    case 2:
                        if (Convert.ToInt16(textBox5.Text)<0 || Convert.ToInt16(textBox5.Text)>3)
                        {
                            MessageBox.Show("Please input Location of Tag Address between 0 and 4!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            textBox5.Focus();
                            textBox5.SelectAll();
                            return;
                        }
                        if ((len + Convert.ToInt16(textBox5.Text))>4)
                        {
                            len = 4 - Convert.ToInt16(textBox5.Text);
                        }
                        break;
                    case 3:
                        if (Convert.ToInt16(textBox5.Text) <0)
                        {
                            MessageBox.Show("Please input Location of Tag Address more than 0!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            textBox5.Focus();
                            textBox5.SelectAll();
                            return;
                        }
                        break;
                }
                EPC_Word = TagBuffer[comboBox3.SelectedIndex, 0];
                for (i = 0; i < TagBuffer[comboBox3.SelectedIndex, 0] * 2; i++)
                {
                    IDTemp[i] = TagBuffer[comboBox3.SelectedIndex, i + 1];
                }

                for (i = 0; i < 5; i++)
                {
                    Thread.Sleep(30);
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.EPC1G2_WriteWordBlock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(len), mask, AccessPassWord, 0);
                            break;
                        case 1:
                            res = Reader.Net_EPC1G2_WriteWordBlock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(len), mask, AccessPassWord);
                            break;
                        case 2:
                            res = Reader.EPC1G2_WriteWordBlock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), Convert.ToByte(textBox5.Text), Convert.ToByte(len), mask, AccessPassWord, RS485Address);
                            break;
                    }
                    if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                        break;
                }
            }

            if (res == OK)
            {
                listBox1.Items.Add("Write Successfully!");
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Write Fail!";
                if (res == 9)
                    str = "The access password is error!";
                listBox1.Items.Add(str);
            }
 
        }
        #endregion
        #region SetProtect
        private void button20_Click(object sender, EventArgs e)
        {
            // Set Protect,设置保护
            if (connect_OK == 0)
                return;
            int i, j;
            string str;
            if (comboBox6.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (textBox11.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox11.Focus();
                textBox11.SelectAll();
                return;
            }
            if (Convert.ToInt16(textBox10.Text) < 0)
            {
                MessageBox.Show("Please input start address of tag more then 0!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox10.Focus();
                textBox10.SelectAll();
                return;
            }
            str = textBox11.Text;
            for (i = 0; i < 4; i++)
            {
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }
            ptr = Convert.ToByte(textBox10.Text);
            EPC_Word = TagBuffer[comboBox6.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox6.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox6.SelectedIndex, i + 1];
            }
            str = textBox10.Text;
            j = (int)MessageBox.Show("Are you sure to set protect address" + str + "of User?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (j != 1)
                return;

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_BlockLock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(ptr), AccessPassword, RS485Address);
                        break;
                    case 1:
                        //                       res = Program.Net_EPC1G2_BlockLock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(ptr), AccessPassword);
                        break;
                }
                if ((res == OK) || (res == 5) || (res == 8) || (res == 9))
                    break;
            }
            if (res == OK)
            {
                MessageBox.Show("Set protect successfully!", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Fail to set protect!";
                if (res == 9)
                    str = "The access password is error!";
                MessageBox.Show(str, "ERROR", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region SetblockProtect
        private void button7_Click(object sender, EventArgs e)
        {
            //设置保护
            if (connect_OK == 0)
                return;
            string str;
            int i, j;
            byte locked=0;

            if (comboBox5.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (textBox9.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            str = textBox9.Text;
            for (i = 0; i < 4; i++)
            {
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()),16);
            }

            EPC_Word = TagBuffer[comboBox5.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox5.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox5.SelectedIndex, i + 1];
            }

            j = (int)MessageBox.Show("Are you sure to protect this tag?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (j != 1)
                return;

            if (radioButton9.Checked == true)
            {
                if (radioButton13.Checked == true)
                    mem = 0;
                if (radioButton14.Checked == true)
                    mem = 1;
                if (radioButton15.Checked == true)
                    locked = 4;
                if (radioButton16.Checked == true)
                    locked = 5;
                if (radioButton17.Checked == true)
                    locked = 6;
                if (radioButton18.Checked == true)
                    locked = 7;

            }
            else
            {
                if (radioButton10.Checked == true)
                    mem=2;
                if (radioButton11.Checked == true)
                    mem = 3;
                if (radioButton12.Checked == true)
                    mem = 3;

                if (radioButton19.Checked == true)
                    locked = 0;
                if (radioButton20.Checked == true)
                    locked = 1;
                if (radioButton21.Checked == true)
                    locked = 2;
                if (radioButton22.Checked == true)
                    locked = 3;
            }

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }
            for(i=0;i<5;i++)
	        {
		        Thread.Sleep(30);
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_SetLock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), locked, AccessPassWord,0);
                        break;
                    case 1:
                        res = Reader.Net_EPC1G2_SetLock(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), locked, AccessPassWord);
                        break;
                    case 2:
                        res = Reader.EPC1G2_SetLock(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(mem), locked, AccessPassWord, RS485Address);
                        break;
                }
		        if ((res==OK) || (res == 5) || (res == 8) || (res == 9))
			        break;
	        }
            if (res == OK)
            {
                MessageBox.Show("Set protect successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Fail to set protect!";
                if (res == 9)
                    str = "The accesspassword is error!";
                MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }


        }
        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            radioButton19.Enabled = false;
            radioButton20.Enabled = false;
            radioButton21.Enabled = false;
            radioButton22.Enabled = false;

            radioButton13.Enabled = true;
            radioButton14.Enabled = true;
            radioButton15.Enabled = true;
            radioButton16.Enabled = true;
            radioButton17.Enabled = true;
            radioButton18.Enabled = true;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            radioButton19.Enabled = true;
            radioButton20.Enabled = true;
            radioButton21.Enabled = true;
            radioButton22.Enabled = true;

            radioButton13.Enabled = false;
            radioButton14.Enabled = false;
            radioButton15.Enabled = false;
            radioButton16.Enabled = false;
            radioButton17.Enabled = false;
            radioButton18.Enabled = false;
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            radioButton19.Enabled = true;
            radioButton20.Enabled = true;
            radioButton21.Enabled = true;
            radioButton22.Enabled = true;

            radioButton13.Enabled = false;
            radioButton14.Enabled = false;
            radioButton15.Enabled = false;
            radioButton16.Enabled = false;
            radioButton17.Enabled = false;
            radioButton18.Enabled = false;
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            radioButton19.Enabled = true;
            radioButton20.Enabled = true;
            radioButton21.Enabled = true;
            radioButton22.Enabled = true;

            radioButton13.Enabled = false;
            radioButton14.Enabled = false;
            radioButton15.Enabled = false;
            radioButton16.Enabled = false;
            radioButton17.Enabled = false;
            radioButton18.Enabled = false;
        }
        #endregion
  #endregion
     

        private void button24_Click(object sender, EventArgs e)
        {
            //设置基本参数
            if (connect_OK == 0)
                return;
            int i,WorkMode = 0, port;
            string[] ipinfo = new string[4];
            byte[] readerip = new byte[4];
            byte[] mask = new byte[4];
            byte[] gateway = new byte[4];
            byte[] MAC = new byte[6];
            string str;



            Reader.ReaderBasicParam BasicParam = new Reader.ReaderBasicParam();




            //读写器的端口
            port = Convert.ToInt16(textBox40.Text);
            ////读写器的IP
            ipinfo = textBox28.Text.Split(new Char[] { '.' });
            for (i = 0; i < 4; i++)
            {
                readerip[i] = Convert.ToByte(ipinfo[i]);
            }
            //MASK
            ipinfo = textBox41.Text.Split(new Char[] { '.' });
            for (i = 0; i < 4; i++)
            {
                mask[i] = Convert.ToByte(ipinfo[i]);
            }
            //Gateway
            ipinfo = textBox42.Text.Split(new Char[] { '.' });
            for (i = 0; i < 4; i++)
            {
                gateway[i] = Convert.ToByte(ipinfo[i]);
            }

            //MAC
            str = "";
            str = textBox16.Text;
            for (i = 0; i < 6; i++)
            {
                MAC[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }
            
            if (comboBox14.SelectedIndex > comboBox15.SelectedIndex)
            {
                MessageBox.Show("The Min of Frequency can't be greater than the Max of Frequency!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt32(textBox27.Text) > 63 || Convert.ToInt32(textBox27.Text) < 0)
            {
                MessageBox.Show("Please input RF power between 20 and 30!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox27.Focus();
                textBox27.SelectAll();
                return;
            }
            if (Convert.ToInt32(textBox29.Text) > 100 || Convert.ToInt32(textBox29.Text) < 0)
            {
                MessageBox.Show("Please input Max. tags of once Reading between 1 and 100!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox29.Focus();
                textBox29.SelectAll();
                return;
            }

            if (Convert.ToInt32(textBox34.Text) > 254 || Convert.ToInt32(textBox34.Text) < 1)
            {
                MessageBox.Show("Please input reader address between 1 and 254 !", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox34.Focus();
                textBox34.SelectAll();
                return;
            }


            if (Convert.ToInt32(textBox31.Text) < 1 || Convert.ToInt32(textBox31.Text) > 255)
            {
                MessageBox.Show("Please input pulse width between 1 and 255!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox31.Focus();
                textBox31.SelectAll();
                return;
            }
            if (Convert.ToInt32(textBox30.Text) < 1 || Convert.ToInt32(textBox30.Text) > 255)
            {
                MessageBox.Show("Please input pulse interval between 1 and 255!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox30.Focus();
                textBox30.SelectAll();
                return;
            }
 

            if (Convert.ToInt32(textBox33.Text) < 0 || Convert.ToInt32(textBox33.Text) > 8)
            {
                MessageBox.Show("Please input interval of reading for standard between 0 and 8!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox33.Focus();
                textBox33.SelectAll();
                return;
            }


            
            int  Enablebuzzer = 0;

            if (checkBox13.Checked == true)
                Enablebuzzer += 1;
            if (checkBox20.Checked == true)
                Enablebuzzer += 8;

            int iValue = 0;
            //bps
            BasicParam.BaudRate = (byte)(comboBox13.SelectedIndex + 4);

            //最小频率
            //最大频率
            int imin = (byte)(comboBox14.SelectedIndex);
            int imax = (byte)(comboBox15.SelectedIndex);
            if (imin < 0)
            {
                imin = 0;
            }
            if (imax < 0)
            {
                imax = 0;
            }
            //因为通过国家来，每个国家的频率有些不一样的
            BasicParam.Min_Frequence = (byte)(imin+1);
            BasicParam.Max_Frequence = (byte)(imax+1);

            //RF power
            iValue = Convert.ToByte(textBox27.Text);
            if ( iValue < 0 || iValue > 63 )
            {
                iValue = 63;
            }
            BasicParam.Power    =   (byte)iValue;


            //Address of Reader (1-254)
            iValue = Convert.ToByte(textBox34.Text);
            BasicParam.ReaderAddress    =   (byte)iValue;

            //Max Tags of once Reading(1-100)
            iValue = Convert.ToByte(textBox29.Text);
            BasicParam.NumofCard = (byte)iValue;

            //Command execution times

            //工作模式
            //0-自动,1-Command
            if (radioButton29.Checked == true)
                WorkMode = 0;
            if (radioButton31.Checked == true)
                WorkMode = 1;
            BasicParam.WorkMode = (byte)WorkMode;


            //buzzer
            if (checkBox13.Checked == true)
                Enablebuzzer += 1;
            BasicParam.EnableBuzzer	=	(byte)Enablebuzzer;
            

	
	        //哪种标签,m_IsoChoose			=	-1;	//0-6B,1-6C
	        BasicParam.TagType	=	0x00;
	        if ( radioButton46.Checked == true )//6C
	        {
                BasicParam.TagType = 0x04;
            }
            if ( radioButton36.Checked == true )//6B
            {
                //选中了6B则为0x01
		        BasicParam.TagType	= 0x01;
	        }
	        


            //读卡的持续时间
            BasicParam.ReadDuration = (byte)comboBox1.SelectedIndex;

            //读写器的IP
            BasicParam.IP1 = readerip[0];
            BasicParam.IP2 = readerip[1];
            BasicParam.IP3 = readerip[2];
            BasicParam.IP4 = readerip[3]; 
            //读写器的端口
            BasicParam.Port1 = (byte)((port >> 8) & 0xFF);
            BasicParam.Port2 = (byte)(port & 0xFF);

            //MASK
            BasicParam.Mask1 = mask[0];
            BasicParam.Mask2 = mask[1];
            BasicParam.Mask3 = mask[2];
            BasicParam.Mask4 = mask[3];

            //Gateway
            BasicParam.Gateway1 = gateway[0];
            BasicParam.Gateway2 = gateway[1];
            BasicParam.Gateway3 = gateway[2];
            BasicParam.Gateway4 = gateway[3];

            //MAC
            BasicParam.MAC1 = MAC[0];
            BasicParam.MAC2 = MAC[1];
            BasicParam.MAC3 = MAC[2];
            BasicParam.MAC4 = MAC[3];
            BasicParam.MAC5 = MAC[4];
            BasicParam.MAC6 = MAC[5];

             

            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.WriteBasicParam(m_hScanner, ref BasicParam, 0);
                        break;
                    case 1:
                        res = Reader.Net_WriteBasicParam(m_hSocket, ref BasicParam);
                        break;
                    case 2:
                        res = Reader.WriteBasicParam(m_hScanner, ref BasicParam, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Fail to update Reader BasicParameter!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            int[] br = new int[] { 9600, 19200, 38400, 57600, 115200 };
            nBaudRate = br[BasicParam.BaudRate - 4];

            gBasicParam = BasicParam;
            MessageBox.Show("Update Basic Parameter successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;

            

        }

        private void button17_Click(object sender, EventArgs e)
        {
            //更新自动参数
            if (connect_OK == 0)
                return;
            int i, Ant, port;
            string[] ipinfo = new string[4];
            byte[] readerip = new byte[4];
            byte[] mask = new byte[4];
            byte[] gateway = new byte[4];
            byte[] MAC = new byte[6];
            



            //Reader.ReaderBasicParam BasicParam = new Reader.ReaderBasicParam();
            
            Reader.ReaderAutoParam AutoParam = new Reader.ReaderAutoParam();
            
            //天线1-4
            Ant = 0;
            if (checkBox14.Checked == true)
                Ant += 1;
            if (checkBox15.Checked == true)
                Ant += 2;
            if (checkBox16.Checked == true)
                Ant += 4;
            if (checkBox17.Checked == true)
                Ant += 8;
            AutoParam.Antenna   =   (byte)Ant;


            //工作模式
            //0-定时模式,1-触发模式
            int AutoMode = 0;
            if (radioButton23.Checked == true)
                AutoMode = 0;
            if (radioButton30.Checked == true)
                AutoMode = 1;
            AutoParam.AutoMode = (byte)AutoMode;


            //输出格式
            //0-简化,1-标准
            int OutMode =   0;
            if (radioButton40.Checked == true)
                OutMode = 0;
            if (radioButton41.Checked == true)
                OutMode = 1;
            AutoParam.OutputManner = (byte)OutMode;

            //输出端口
            //0-RS232,1-RS485,2-RJ45,3-Wiegand26,4-Wiegand34
            int OutInterface = 0;

            if (radioButton35.Checked == true)
                OutInterface = 0;
            if (radioButton34.Checked == true)
                OutInterface = 1;
            if (radioButton37.Checked == true)
                OutInterface = 2;
            if (radioButton32.Checked == true)
                OutInterface = 3;
            if (radioButton33.Checked == true)
                OutInterface = 4;
            AutoParam.OutInterface = (byte)OutInterface;


            //触发模式
            //0-低电平，1-高电平
            int TriggerMode = 0;
            if (radioButton24.Checked == true)
                TriggerMode = 0;
            if (radioButton25.Checked == true)
                TriggerMode = 1;
            AutoParam.TriggerMode = (byte)TriggerMode;


            //定时间隔
            //0-10ms，1-20ms，2-30ms，3-50ms，4-100ms。缺省值为2。每隔设定时间主动读取一次标签。
            int iInterval = 0;
            if (radioButton51.Checked == true)
                iInterval = 0;
            if (radioButton52.Checked == true)
                iInterval = 1;
            if (radioButton53.Checked == true)
                iInterval = 2;
            if (radioButton54.Checked == true)
                iInterval = 3;
            if (radioButton55.Checked == true)
                iInterval = 4;
            AutoParam.Interval = (byte)iInterval;
            
            
            //主机IP
            ipinfo = textBox43.Text.Split(new Char[] { '.' });
            AutoParam.HostIP1 = Convert.ToByte(ipinfo[0]);
            AutoParam.HostIP2 = Convert.ToByte(ipinfo[1]);
            AutoParam.HostIP3 = Convert.ToByte(ipinfo[2]);
            AutoParam.HostIP4 = Convert.ToByte(ipinfo[3]);


            //主机PORT
            port = Convert.ToInt32(textBox44.Text);

            AutoParam.Port1 = (byte)((port >> 8) & 0xFF);
            AutoParam.Port2 = (byte)(port & 0xFF);
            AutoPort = port;

            //检测报警
            if (checkBox6.Checked == true)
                AutoParam.Alarm = 1;
            else
                AutoParam.Alarm = 0;


            //控制继电器
            if (checkBox21.Checked == true)
            {
                AutoParam.EnableRelay = 1;
            }
            else
            {
                AutoParam.EnableRelay = 0;
            }
            
            //对应四天线的
            //通知条件, //0-立即通知,1-定时通知,2-增加新标签,3-减少标签,4-标签数变化
            int ReportCondition = 0;
            if (radioButton26.Checked == true)
                ReportCondition = 0;
            if (radioButton28.Checked == true)
                ReportCondition = 1;
            if (radioButton27.Checked == true)
                ReportCondition = 2;
            if (radioButton45.Checked == true)
                ReportCondition = 3;
            if (radioButton44.Checked == true)
                ReportCondition = 4;
            AutoParam.Report_Condition = (byte)ReportCondition;

            //////////////////////////////////////////////////////////////////////////
            //这里注意一下这三个参数
            //保留时间
            int m_PersistenceTime = 0;
            m_PersistenceTime = Convert.ToByte(textBox10.Text);
            AutoParam.TimeH = (byte)(m_PersistenceTime >> 8);
            AutoParam.TimeL = (byte)m_PersistenceTime;

            //保留数目
            int m_LenofList = 0;
            m_LenofList = Convert.ToByte(textBox11.Text);
            AutoParam.NumH = (byte)(m_LenofList >> 8);
            AutoParam.NumL = (byte)m_LenofList;

            //间隔时间, 通知间隔
            int shValue	=	0;
            shValue =   (int)Convert.ToInt16(textBox12.Text);
            AutoParam.Report_Interval = Convert.ToByte(textBox12.Text);

            if ( iantennaIndex == 0 )
	        {
		        //直接---0,保留时间为0, 通知间隔为 用户自定义
		        //标准---1,保留时间为0, 通知间隔为 用户自定义
		        //定时---2,保留时间为用户自定义, 通知间隔为0,定时与标准互换
		        switch ( ReportCondition )
		        {
		        case 0:
			        AutoParam.TimeH		=	0;
			        AutoParam.TimeL		=	0;
			        AutoParam.Report_Interval	=	(byte)shValue;
			        break;
		        case 1:
			        AutoParam.TimeH		=	0;
			        AutoParam.TimeL		=	0;
			        AutoParam.Report_Interval	=	(byte)shValue;
			        break;
		        case 2:
                    AutoParam.TimeH = (byte)((shValue >> 8) & 0xFF);
                    AutoParam.TimeL = (byte)(shValue & 0xFF);
			        AutoParam.Report_Interval	=	(byte)0;
			        break;

		        }

	        }
            //////////////////////////////////////////////////////////////////////////

            //是否通知主机
            if (checkBox7.Checked == true)
            {
                AutoParam.Report_Output = 1;
            }
            else
            {
                AutoParam.Report_Output = 0;
            }


            //WiegandWidth(*10us)    
            AutoParam.WiegandWidth = Convert.ToByte(textBox31.Text);
            //WiegandBetween(*10us)
            AutoParam.WiegandInterval = Convert.ToByte(textBox30.Text);

            //卡的类型
            //0-标签本身ID，1-用户自定义
            int IDPosition = 0;
            if (radioButton47.Checked == true)
                IDPosition = 0;
            if (radioButton48.Checked == true)
                IDPosition = 1;
            AutoParam.IDPosition = (byte)IDPosition;

            //卡号起始地址
            textBox33.Text = gAutoParam.ID_Start.ToString();
            AutoParam.ID_Start = Convert.ToByte(textBox33.Text);



            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.WriteAutoParam(m_hScanner, ref AutoParam, 0);
                        break;
                    case 1:
                        res = Reader.Net_WriteAutoParam(m_hSocket, ref AutoParam);
                        break;
                    case 2:
                        res = Reader.WriteAutoParam(m_hScanner, ref AutoParam, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Fail to update Reader AutoParameter!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            MessageBox.Show("Update Auto Parameter successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

            


            


            //int[] br = new int[] { 9600, 19200, 38400, 57600, 115200 };
            //int baud = br[Param.BaudRate - 4];

            //nBaudRate = baud;

            //Reader.DCB dcb = new Reader.DCB();
            //Reader.GetCommState(m_hScanner, ref dcb);
            //dcb.BaudRate = baud;
            //Reader.SetCommState(m_hScanner, ref dcb);

            //Reader.PurgeComm(m_hScanner, 0x04);
            //Reader.PurgeComm(m_hScanner, 0x08);

            //switch (ComMode)
            //{
            //    case 0:
            //        res = Reader.Reboot(m_hScanner, RS485Address);
            //        break;
            //    case 1:
            //        res = Reader.Net_Reboot(m_hSocket);
            //        break;
            //}

            //if (res == OK)
            //{
            //    MessageBox.Show("Update Reader Parameter successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("Fail to reboot reader!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}

            return;
        }

        private void button29_Click_1(object sender, EventArgs e)
        {
            //恢复缺省参数
            for (int i = 0; i < 3; i++ )
            {
                switch (ComMode)
                {
                    case 0://RS232
                        res = Reader.ReadFactoryParameter(m_hScanner, 0);
                        break;

                    case 1://网口
                        res = Reader.Net_ReadFactoryParameter(m_hSocket);
                        break;

                    case 2://RS485
                        res = Reader.ReadFactoryParameter(m_hScanner, RS485Address);
                        break;

                }
                if (res == OK)
                {
                    break;
                }

            }
            if ( res == OK )
            {
                MessageBox.Show("Update Default Parameter successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Update Default Parameter Fail!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;
        }

        public int ReaderParam()
        {
            int i = 0;
            byte []readerIP =new byte[4];
            byte []mask =new byte[4];
            byte []gateway =new byte[4];
            int port=0;
            Reader.ReaderBasicParam Param = new Reader.ReaderBasicParam();
            Reader.ReaderAutoParam AutoParam = new Reader.ReaderAutoParam(); 
            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ReadBasicParam(m_hScanner, ref Param, RS485Address);
                        break;
                    case 1:
                        res = Reader.Net_ReadBasicParam(m_hSocket, ref Param);
                        break;
                }
                if (res == OK)
                    break;
            }

            if (res !=OK)
            {
                MessageBox.Show("Fail to read parameter of reader!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return res;
            }

            int[] br = new int[] { 9600, 19200, 38400, 57600, 115200 };
            nBaudRate = br[Param.BaudRate - 4];

            comboBox13.SelectedIndex = Param.BaudRate - 4;
            textBox27.Text = Param.Power.ToString();
            comboBox14.SelectedIndex = Param.Min_Frequence;
            comboBox15.SelectedIndex = Param.Max_Frequence;
            textBox29.Text = Param.NumofCard.ToString();
            textBox34.Text = Param.ReaderAddress.ToString();

            switch (Param.WorkMode)
            {
                case 0:
                    radioButton29.Checked = true;
                    break;
                case 1:
                    radioButton31.Checked = true;
                    break;
            }

            switch (Param.TagType)
            {
                case 1:
                    radioButton36.Checked = true;
                    break;
                case 2:
                    radioButton38.Checked = true;
                    break;
                case 4:
                    radioButton46.Checked = true;
                    break;
                case 8:
                    radioButton39.Checked = true;
                    break;
            }

            if ((Param.EnableBuzzer & 0x01) == 1)
                checkBox13.Checked = true;
            else
                checkBox13.Checked = false;

            if (((Param.EnableBuzzer >> 3) & 0x01) == 1)
                checkBox20.Checked = true;
            else
                checkBox20.Checked = false;

            port = (int)(Param.Port1 << 8) + (int)Param.Port2;

            textBox28.Text = Param.IP1.ToString();
            textBox28.Text += ".";
            textBox28.Text += Param.IP2.ToString();
            textBox28.Text += ".";
            textBox28.Text += Param.IP3.ToString();
            textBox28.Text += ".";
            textBox28.Text += Param.IP4.ToString();

            textBox40.Text = port.ToString();

            textBox41.Text = Param.Mask1.ToString();
            textBox41.Text += ".";
            textBox41.Text += Param.Mask2.ToString();
            textBox41.Text += ".";
            textBox41.Text += Param.Mask3.ToString();
            textBox41.Text += ".";
            textBox41.Text += Param.Mask4.ToString();

            textBox42.Text = Param.Gateway1.ToString();
            textBox42.Text += ".";
            textBox42.Text += Param.Gateway2.ToString();
            textBox42.Text += ".";
            textBox42.Text += Param.Gateway3.ToString();
            textBox42.Text += ".";
            textBox42.Text += Param.Gateway4.ToString();

            string MACstr;
            MACstr = "";
            MACstr = Param.MAC1.ToString("X02");
            MACstr = MACstr + Param.MAC2.ToString("X02");
            MACstr = MACstr + Param.MAC3.ToString("X02");
            MACstr = MACstr + Param.MAC4.ToString("X02");
            MACstr = MACstr + Param.MAC5.ToString("X02");
            MACstr = MACstr + Param.MAC6.ToString("X02");

            textBox16.Text = MACstr;

            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ReadAutoParam(m_hScanner, ref AutoParam, RS485Address);
                        break;
                    case 1:
                        res = Reader.Net_ReadAutoParam(m_hSocket, ref AutoParam);
                        break;
                }
                if (res == OK)
                    break;
            }

            if (res != OK)
            {
                MessageBox.Show("Fail to read parameter of reader!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return res;
            }

            switch (AutoParam.AutoMode)
            {
                case 0:
                    radioButton23.Checked = true;
                    break;
                case 1:
                    radioButton30.Checked = true;
                    break;
            }

            int PersistenceTime;
            PersistenceTime =AutoParam.TimeH;
            PersistenceTime = (PersistenceTime << 8) + AutoParam.TimeL;
            textBox10.Text = PersistenceTime.ToString(); 

            comboBox18.SelectedIndex = AutoParam.Interval;

            int LenofList;
            LenofList = AutoParam.NumH;
            LenofList = (LenofList << 8) + AutoParam.NumL;
            textBox11.Text = LenofList.ToString();

            switch (AutoParam.OutputManner)
            {
                case 0:
                    radioButton40.Checked = true;
                    break;
                case 1:
                    radioButton41.Checked = true;
                    break;
            }

            switch (AutoParam.OutInterface)
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

            textBox31.Text = AutoParam.WiegandWidth.ToString();
            textBox30.Text = AutoParam.WiegandInterval.ToString();
            textBox33.Text = AutoParam.ID_Start.ToString();
            switch (AutoParam.IDPosition)
            {
                case 0:
                    radioButton47.Checked = true;
                    break;
                case 1:
                    radioButton48.Checked = true;
                    break;
            }


            textBox12.Text = AutoParam.Report_Interval.ToString();

            switch (AutoParam.Report_Condition)
            {
                case 0:
                    radioButton26.Checked = true;
                    break;
                case 1:
                    radioButton28.Checked = true;
                    break;
                case 2:
                    radioButton27.Checked = true;
                    break;
                case 3:
                    radioButton45.Checked = true;
                    break;
                case 4:
                    radioButton44.Checked = true;
                    break;
            }

            if ((AutoParam.Report_Output & 0x01) == 1)
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }

            if ((AutoParam.Antenna & 0x01) == 1)
                checkBox14.Checked = true;
            else
                checkBox14.Checked = false;

            if (((AutoParam.Antenna >> 1) & 0x01) == 1)
                checkBox15.Checked = true;
            else
                checkBox15.Checked = false;

            if (((AutoParam.Antenna >> 2) & 0x01) == 1)
                checkBox16.Checked = true;
            else
                checkBox16.Checked = false;

            if (((AutoParam.Antenna >> 3) & 0x01) == 1)
                checkBox17.Checked = true;
            else
                checkBox17.Checked = false;

             switch (AutoParam.TriggerMode)
            {
                case 0:
                    radioButton24.Checked = true;
                    break;
                case 1:
                    radioButton25.Checked = true;
                    break;
            }

            if(AutoParam.Alarm ==1)
            {
                checkBox6.Checked = true;
            }
            else
            {
                checkBox6.Checked = false;
            }

            if (AutoParam.EnableRelay == 1)
                 checkBox21.Checked = true;
            else
                 checkBox21.Checked = false;

            textBox43.Text = AutoParam.HostIP1.ToString();
            textBox43.Text += ".";
            textBox43.Text += AutoParam.HostIP2.ToString();
            textBox43.Text += ".";
            textBox43.Text += AutoParam.HostIP3.ToString();
            textBox43.Text += ".";
            textBox43.Text += AutoParam.HostIP4.ToString();
            port = (int)(AutoParam.Port1 << 8) + (int)AutoParam.Port2;
            textBox44.Text = port.ToString();
            AutoPort = port;

           int FilterAddress=0;
           int FilterLength=0;
           byte[] FilterMask = new byte[64];
           int l = 0;
           string str, str_temp;

            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetReportFilter(m_hScanner, ref FilterAddress, ref FilterLength, FilterMask, RS485Address);
                       
                        break;
                    case 1:
                        res = Reader.Net_GetReportFilter(m_hSocket, ref FilterAddress, ref FilterLength, FilterMask);
                        break;
                }
                if (res == OK) break;
            }
            if (res != OK)
            {
                MessageBox.Show("Read ReportFilter Parameter Fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return res;
            }

            textBox13.Text = FilterAddress.ToString();
            textBox14.Text = FilterLength.ToString(); 

            if (FilterLength % 8 == 0)
            {
                l = FilterLength / 8;
            }
            else
            {
                l = FilterLength / 8 + 1;
            }

            str = "";
            for (i = 0; i < l; i++)
            {
                str_temp = FilterMask[i].ToString("X02");
                str += str_temp;
            }
            textBox15.Text = str;

            byte[] ReaderID = new byte[12];
            switch (ComMode)
            {
                case 0:
                    res = Reader.GetReaderID(m_hScanner, ReaderID, RS485Address);
                    break;
                case 1:
                    res = Reader.Net_GetReaderID(m_hSocket, ReaderID);
                    break;
            }
            textBox39.Text = "";
            for (i = 0; i < 10; i++)
            {
                textBox39.Text += (char)ReaderID[i];
            }
            return OK;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            //取得继电器状态
            if (connect_OK == 0)
                return;
            int Relay = 0;
            int Relay1 = 0;
            
            switch (ComMode)
            {
                case 0:
                    res = Reader.GetRelay(m_hScanner, ref Relay, 0);
                    break;
                case 1:
                    res = Reader.Net_GetRelay(m_hSocket, ref Relay);
                    break;
                case 2:
                    res = Reader.GetRelay(m_hScanner, ref Relay, RS485Address);
                    break;
            }
            if (res == OK)
            {
                Relay1 = Relay;
                //m_Relay = Relay & 0x01;
                //m_Relay1 = (Relay >> 1) & 0x01;

                if ( 1 == Relay )
                {
                    checkBox18.Checked = true;
                }
                else
                {
                    checkBox18.Checked = false;
                }

                //if ( 2 == Relay )
                //{
                //    checkBox19.Checked = true;
                //}


                MessageBox.Show("Get relay successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("Get relay unsuccessfully! Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            return;

        }

        private void button25_Click(object sender, EventArgs e)
        {
            //设置继电器
            if (connect_OK == 0)
                return;
            int Relay = 0;
            if (checkBox18.Checked == true)
                Relay += 1;
            if (checkBox19.Checked == true)
                Relay += 2;
            switch (ComMode)
            {
                case 0:
                    res = Reader.SetRelay(m_hScanner, Relay, 0);
                    break;
                case 1:
                    res = Reader.Net_SetRelay(m_hSocket, Relay);
                    break;
                case 2:
                    res = Reader.SetRelay(m_hScanner, Relay, RS485Address);
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Set relay unsuccessfully! Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                MessageBox.Show("Set relay successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return;

        }

        private void button26_Click(object sender, EventArgs e)
        {
            //连接设备按钮功能
            int i;
            byte HardVer=0;
            szPort = comboBox19.Text;
            readerip = textBox35.Text;
            readerport = Convert.ToUInt16(textBox36.Text);
            hostip = textBox37.Text;
            hostport = Convert.ToUInt16(textBox38.Text);

            connect_OK = 0;//0--no connect, 1--connected

            if (radioButton42.Checked == true)
            {
                ComMode = 1;//网口模式
            }
            if (radioButton43.Checked == true)
            {
                ComMode = 0;//RS232模式
            }
            if (radioButton56.Checked == true)
            {
                ComMode = 2;//RS485模式
                //RS485地址
                iAddress = Convert.ToUInt16(textBox17.Text);
            }
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ConnectScanner(ref m_hScanner, szPort, ref nBaudRate);
                        break;
                    case 1:
                        res = Reader.Net_ConnectScanner(ref m_hSocket, readerip, readerport, hostip, hostport);
                        //OK;
                        break;
                    case 2:
                        res = Reader.ConnectScanner485(ref m_hScanner, szPort, nBaudRate, iAddress);
                        break;
                }
                if (res == OK)
                    break;
            }
            if ((res != OK) && (ComMode == 0))
            {
                MessageBox.Show("None Reader connect to the COM!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if ((res != OK) && (ComMode == 1))
            {
                MessageBox.Show("None Reader connect to the RJ45!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //连上了
#if ENABLE_R2000_protocol
            this.tabControl1.SelectedIndex = 2;//进入第3页,ISO180006C
#else
            //1.读版本
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetReaderVersion(m_hScanner, ref HardVersion, ref SoftVersion, 0);
                        break;
                    case 1:
                        res = Reader.Net_GetReaderVersion(m_hSocket, ref HardVersion, ref SoftVersion);
                        break;
                    case 2:
                        res = Reader.GetReaderVersion(m_hScanner, ref HardVersion, ref SoftVersion, iAddress);
                        //Thread.Sleep(300);
                        break;
                    
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                connect_OK = 0;
                MessageBox.Show("Can't get reader version!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            HardVer = (byte)HardVersion;
            textBox25.Text = String.Format("{0,0:D02}{1,0:D02}", (byte)(HardVersion >> 8), (byte)HardVersion);
            textBox26.Text = String.Format("{0,0:D02}{1,0:D02}", (byte)(SoftVersion >> 8), (byte)SoftVersion);

            //2.读基本参数
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ReadBasicParam(m_hScanner, ref gBasicParam, 0);
                        break;
                    case 1:
                        res = Reader.Net_ReadBasicParam(m_hSocket, ref gBasicParam);
                        break;
                    case 2:
                        res = Reader.ReadBasicParam(m_hScanner, ref gBasicParam, iAddress);
                        
                        break;

                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                
                MessageBox.Show("Connect Reader Fail!(BasicParam)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                
                return;
            }

            //3.取国家频率
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetFrequencyRange(m_hScanner, ref gFre, 0);
                        break;
                    case 1:
                        res = Reader.Net_GetFrequencyRange(m_hSocket, ref gFre);
                        break;
                    case 2:
                        res = Reader.GetFrequencyRange(m_hScanner, ref gFre, iAddress);

                        break;

                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Connect Reader Fail!(Frequency)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            //4.取自动参数
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ReadAutoParam(m_hScanner, ref gAutoParam, 0);
                        break;
                    case 1:
                        res = Reader.Net_ReadAutoParam(m_hSocket, ref gAutoParam);
                        break;
                    case 2:
                        res = Reader.ReadAutoParam(m_hScanner, ref gAutoParam, iAddress);

                        break;

                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Connect Reader Fail!(AutoParam)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            //5.取读写器ID
            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetReaderID(m_hScanner, gReaderID, 0);
                        break;
                    case 1:
                        res = Reader.Net_GetReaderID(m_hSocket, gReaderID);
                        break;
                    case 2:
                        res = Reader.GetReaderID(m_hScanner, gReaderID, iAddress);

                        break;

                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Connect Reader Fail!(GetReaderID)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            //6.是否设置自动模式
            if ( gBasicParam.WorkMode == 0 )
            {
                for (i = 0; i < 3; i++)
                {
                    switch (ComMode)
                    {
                        case 0:
                            res = Reader.AutoMode(m_hScanner, 1, 0);
                            break;
                        case 1:
                            res = Reader.Net_AutoMode(m_hSocket, 1);
                            break;
                        case 2:
                            res = Reader.AutoMode(m_hScanner, 1, iAddress);

                            break;

                    }
                    if (res == OK)
                        break;
                }
                if (res != OK)
                {
                    MessageBox.Show("Connect Reader Fail!(AutoMode)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }



            //读参数
            //ReaderParam();

            this.tabControl1.SelectedIndex = 1;//进入第二页

            //连上了的
            //基本参数更新
            OnReadParameter(0);

            //自动参数更新
            OnReadParameter(1);
#endif
            //弹出提示成功
            connect_OK = 1;//0--no connect, 1--connected
            button26.Enabled = false;
            button27.Enabled = true;

            MessageBox.Show("Connect reader success!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

        }


        //sel为国家频率下标,imin为最小频率下标,imax为最大频率下标, -1为最小为最小，最大为最大
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
	        switch(sel)
	        {//国家频率字符串, //级数, 步进, 起始频率, 公式：902750 + 级数*步进
                case 0://{"0---FCC（美国）", 63, 400, 902600},								//(0)
                tmpFreq.chFreq  =   "0---FCC（美国）";
                tmpFreq.iGrade	=	63;
		        tmpFreq.iSkip	=	400;
		        tmpFreq.dwFreq	=	902600;
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
	        iCount	=	tmpFreq.iGrade;
    		
	        if ( 22 == sel )
	        {
		        int[] iArray = new int[3];
                iArray[0]  =   902750;
                iArray[1]  =   903250;
                iArray[2]  =   905750;
                
		        //加拿大的特述
		        for (i = 0; i < 3; i++)
		        {
                    strFreqTmp = string.Format("{0,2:D}{1,0:D}", i+1, iArray[i]);
                    comboBox14.Items.Add(strFreqTmp);
                    comboBox15.Items.Add(strFreqTmp);
		        }
		        tmpFreq.dwFreq	=	iArray[i-1];
		        iCount			=	tmpFreq.iGrade - 3;
		        k = 3;
	        }
    		
	        for (i = k; i < iCount + k; i++)
	        {
		        iValue	=	tmpFreq.dwFreq + i * tmpFreq.iSkip;
                strFreqTmp = string.Format("{0,2:D}{1,0:S}{2,0:D}", i + 1,"---", iValue);
                comboBox14.Items.Add(strFreqTmp);
                comboBox15.Items.Add(strFreqTmp);
	        }
    		
	        iCount	=	tmpFreq.iGrade;
	        if ( i > 0 )
	        {
		        if ( imin == -1 && imax == -1 )
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


        //基本参数---0, 自动参数---1
        public void OnReadParameter(int iOpt)	//iOpt,0--基本参数，1--自动参数
        {
            if ( 0 == iOpt )
            {
                //基本参数更新
                //版本
		        textBox25.Text = String.Format("{0,0:D02}{1,0:D02}", (byte)(HardVersion >> 8), (byte)HardVersion);
                textBox26.Text = String.Format("{0,0:D02}{1,0:D02}", (byte)(SoftVersion >> 8), (byte)SoftVersion);

                //bps
                comboBox13.SelectedIndex = gBasicParam.BaudRate - 4;
                int[] br = new int[] { 9600, 19200, 38400, 57600, 115200 };
                nBaudRate = br[gBasicParam.BaudRate - 4];
                

                //最小频率
                //最大频率
                int imin = gBasicParam.Min_Frequence - 1;//6
                int imax = gBasicParam.Max_Frequence - 1;//58
                OnCbnSetFre(gFre, imin, imax);//因为通过国家来，每个国家的频率有些不一样的

                //RF power
                textBox27.Text = gBasicParam.Power.ToString();//25

                //Address of Reader (1-254)
                textBox34.Text = gBasicParam.ReaderAddress.ToString();//1


                //Max Tags of once Reading(1-100)
                textBox29.Text = gBasicParam.NumofCard.ToString();//10

                //工作模式
                //0-自动,1-Command
                switch (gBasicParam.WorkMode)
                {
                    case 0:
                        radioButton29.Checked = true;
                        break;
                    case 1:
                        radioButton31.Checked = true;//1
                        break;
                }


                //读写器ID
                
                textBox39.Text = "";
                for (int i = 0; i < 10; i++)
                {
                    textBox39.Text += (char)gReaderID[i];
                }


                //buzzer
                checkBox13.Checked = Convert.ToBoolean(gBasicParam.EnableBuzzer);

                //哪种标签
                //0-6B,1-6C
                if ( (gBasicParam.TagType & 0x01) == 1)
                {
                    radioButton36.Checked = true;//true
                }
                    
                if ( (gBasicParam.TagType & 0x04) == 4)
                {
                    radioButton46.Checked = true;//true

                }


                //读卡的持续时间
                comboBox1.SelectedIndex = gBasicParam.ReadDuration;//0

                //读写器的IP
                textBox28.Text = gBasicParam.IP1.ToString();
                textBox28.Text += ".";
                textBox28.Text += gBasicParam.IP2.ToString();
                textBox28.Text += ".";
                textBox28.Text += gBasicParam.IP3.ToString();
                textBox28.Text += ".";
                textBox28.Text += gBasicParam.IP4.ToString();
                //读写器的端口
                int m_ReaderPort = (((int)gBasicParam.Port1) << 8) + (int)gBasicParam.Port2;
                textBox40.Text = m_ReaderPort.ToString();

                //MASK
                textBox41.Text = gBasicParam.Mask1.ToString();
                textBox41.Text += ".";
                textBox41.Text += gBasicParam.Mask2.ToString();
                textBox41.Text += ".";
                textBox41.Text += gBasicParam.Mask3.ToString();
                textBox41.Text += ".";
                textBox41.Text += gBasicParam.Mask4.ToString();

                //Gateway
                textBox42.Text = gBasicParam.Gateway1.ToString();
                textBox42.Text += ".";
                textBox42.Text += gBasicParam.Gateway2.ToString();
                textBox42.Text += ".";
                textBox42.Text += gBasicParam.Gateway3.ToString();
                textBox42.Text += ".";
                textBox42.Text += gBasicParam.Gateway4.ToString();

                //MAC
                string MACstr;
                MACstr = "";
                MACstr = gBasicParam.MAC1.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC2.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC3.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC4.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC5.ToString("X02");
                MACstr = MACstr + gBasicParam.MAC6.ToString("X02");

                textBox16.Text = MACstr;




            }
            else if ( 1 == iOpt )
            {
                //自动参数更新
		        //天线1-4
                if ((gAutoParam.Antenna & 0x01) == 1)//gAutoParam->Antenna & 0x01
                    checkBox14.Checked = true;//true
                else
                    checkBox14.Checked = false;

                if (((gAutoParam.Antenna >> 1) & 0x01) == 1)//gAutoParam->Antenna & 0x02
                    checkBox15.Checked = true;//true
                else
                    checkBox15.Checked = false;

                if (((gAutoParam.Antenna >> 2) & 0x01) == 1)//gAutoParam->Antenna & 0x04
                    checkBox16.Checked = true;
                else
                    checkBox16.Checked = false;//false

                if (((gAutoParam.Antenna >> 3) & 0x01) == 1)//gAutoParam->Antenna & 0x08
                    checkBox17.Checked = true;
                else
                    checkBox17.Checked = false;//false



                //工作模式
                //0-定时模式,1-触发模式
                switch (gAutoParam.AutoMode)
                {
                    case 0:
                        radioButton23.Checked = true;//true
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
                        radioButton41.Checked = true;//true
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
                        radioButton37.Checked = true;//true
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
                        radioButton24.Checked = true;//true
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
                        radioButton53.Checked = true;//true
                        break;
                    case 3:
                        radioButton54.Checked = true;
                        break;
                    case 4:
                        radioButton55.Checked = true;
                        break;
                }

                //主机IP
                
                textBox43.Text = gAutoParam.HostIP1.ToString();
                textBox43.Text += ".";
                textBox43.Text += gAutoParam.HostIP2.ToString();
                textBox43.Text += ".";
                textBox43.Text += gAutoParam.HostIP3.ToString();
                textBox43.Text += ".";
                textBox43.Text += gAutoParam.HostIP4.ToString();//192.168.0.61

                

                //主机PORT
                int m_HostPort = 0;
                m_HostPort = (int)(gAutoParam.Port1 << 8) + (int)gAutoParam.Port2;//5000
                textBox44.Text = m_HostPort.ToString();//5000
                AutoPort = m_HostPort;//5000


                //检测报警
                if (gAutoParam.Alarm == 1)
                {
                    checkBox6.Checked = true;
                }
                else
                {
                    checkBox6.Checked = false;//false
                }

                //控制继电器
                if (gAutoParam.EnableRelay == 1)
                    checkBox21.Checked = true;
                else
                    checkBox21.Checked = false;//false


                //通知条件
                byte m_Condiona = gAutoParam.Report_Condition;//0	//0-立即通知,1-定时通知,2-增加新标签,3-减少标签,4-标签数变化		
                //////////////////////////////////////////////////////////////////////////
		        //保留时间
		        int m_PersistenceTime	=	0;
		        m_PersistenceTime	=	gAutoParam.TimeH;//1
		        m_PersistenceTime	=	(m_PersistenceTime<<8)+gAutoParam.TimeL;//257
		        textBox10.Text = m_PersistenceTime.ToString(); //257
		        //保留数目
		        int m_LenofList	=	0;
		        m_LenofList		=	gAutoParam.NumH;//1
		        m_LenofList		=	(m_LenofList<<8)+gAutoParam.NumL;//257
                textBox11.Text  = m_LenofList.ToString();//257
		        //间隔时间, 通知间隔
		        int shValue	=	0;////读卡总超时1
		        shValue =	gAutoParam.Report_Interval;//120
		        textBox12.Text  =   shValue.ToString();//120


                //选哪根天线,0-一根天线,1-四根天线
                if ( iantennaIndex == 0 )//0
		        {
			        //直接---0,保留时间为0, 通知间隔为 用户自定义
			        //标准---1,保留时间为0, 通知间隔为 用户自定义
			        //定时---2,保留时间为用户自定义, 通知间隔为0,定时与标准互换
			        switch ( m_Condiona )//0
			        {
			        case 0://直接
				        m_PersistenceTime	=	gAutoParam.TimeH;//1
				        m_PersistenceTime	=	(m_PersistenceTime<<8)+gAutoParam.TimeL;//257
				        textBox10.Text = m_PersistenceTime.ToString(); //257

				        shValue =	gAutoParam.Report_Interval;//120
				        textBox12.Text  =   shValue.ToString();//120
        				
				        break;
			        case 1://标准
				        m_PersistenceTime	=	gAutoParam.TimeH;
				        m_PersistenceTime	=	(m_PersistenceTime<<8)+gAutoParam.TimeL;
				        textBox10.Text = m_PersistenceTime.ToString(); 
        				
				        shValue =	gAutoParam.Report_Interval;
				        textBox12.Text  =   shValue.ToString();
        				
				        break;
			        case 2://定时
                        m_PersistenceTime = gAutoParam.TimeH;
                        m_PersistenceTime = (m_PersistenceTime << 8) + gAutoParam.TimeL;
                        textBox12.Text = m_PersistenceTime.ToString();

                        shValue = gAutoParam.Report_Interval;
                        textBox10.Text = shValue.ToString();
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
                    checkBox7.Checked = false;//false
                }
                


                //WiegandWidth(*10us)
                textBox31.Text = gAutoParam.WiegandWidth.ToString();//40
                //WiegandBetween(*10us)
                textBox30.Text = gAutoParam.WiegandInterval.ToString();//200


                //卡的类型
                //0-标签本身ID，1-用户自定义
                switch (gAutoParam.IDPosition)
                {
                    case 0:
                        radioButton47.Checked = true;//true
                        break;
                    case 1:
                        radioButton48.Checked = true;
                        break;
                }

                //卡号起始地址
                textBox33.Text = gAutoParam.ID_Start.ToString();//0

		        
            }

        }

        private void button27_Click(object sender, EventArgs e)
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
                MessageBox.Show("Can't DisconnectScanner!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                button26.Enabled = true;
                button27.Enabled = false;
                MessageBox.Show("DisconnectScanner successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        private void listView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox35.Text = listView4.SelectedItems[0].SubItems[1].Text;
                textBox36.Text = listView4.SelectedItems[0].SubItems[2].Text;
            }
            catch
            {
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //设置读写器时间
            if (connect_OK == 0)
                return;
            Reader.ReaderDate time;
            DateTime dt = DateTime.Now;
            time.Year = (byte)((dt.Year - 2000) & 0x00FF);
            time.Month = (byte)dt.Month;
            time.Day = (byte)dt.Day;
            time.Hour = (byte)dt.Hour;
            time.Minute = (byte)dt.Minute;
            time.Second = (byte)dt.Second;
            switch (ComMode)
            {
                case 0:
                    res = Reader.SetReaderTime(m_hScanner, ref time, 0);
                    break;
                case 1:
                    res = Reader.Net_SetReaderTime(m_hSocket, ref time);
                    break;
                case 2:
                    res = Reader.SetReaderTime(m_hScanner, ref time, iAddress);
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Can't set reader time!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                textBox45.Text = "Set reader time successfully!";
            }
         }

        private void button28_Click(object sender, EventArgs e)
        {
            //取得读写器时间
            if (connect_OK == 0)
                return;
            int i;
            Reader.ReaderDate time = new Reader.ReaderDate();
            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetReaderTime(m_hScanner, ref time, 0);
                        break;
                    case 1:
                        res = Reader.Net_GetReaderTime(m_hSocket, ref time);
                        break;
                    case 2:
                        res = Reader.GetReaderTime(m_hScanner, ref time, iAddress);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Can't get reader time!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            //textBox45.Text = "Reader time is ";
            textBox45.Text = "";
            textBox45.Text += time.Year.ToString("D2");
            textBox45.Text += "-";
            textBox45.Text += time.Month.ToString("D2"); 
            textBox45.Text += "-";
            textBox45.Text += time.Day.ToString("D2"); 
            textBox45.Text += " ";
            textBox45.Text += time.Hour.ToString("D2"); 
            textBox45.Text += ":";
            textBox45.Text += time.Minute.ToString("D2");
            textBox45.Text += ":";
            textBox45.Text += time.Second.ToString("D2");
        }

        private void button29_Click(object sender, EventArgs e)
        {
            if (connect_OK == 0)
                return;
            int i,j;
            j = (int)MessageBox.Show("Do you determine to recover the factory parameter?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (j != 1)
                return;

           progressBar1.Visible = true;
           progressBar1.Value = 0;
           progressBar1.Step = 20;
           progressBar1.PerformStep();

           for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ReadFactoryParameter(m_hScanner, 0);
                        break;
                    case 1:
                        res = Reader.Net_ReadFactoryParameter(m_hSocket);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Fail to recover default parameters!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Thread.Sleep(500);
            progressBar1.PerformStep();
            switch (ComMode)
            {
                case 0:
                    Reader.Reboot(m_hScanner,0);
                    break;
                case 1:
                    Reader.Net_Reboot(m_hSocket);
                    break;
            }
            if (ComMode == 0)
               Reader.DisconnectScanner(m_hScanner);

           Thread.Sleep(500);
           progressBar1.PerformStep();
           if (ComMode == 0)
            {
                for (i = 0; i < 5; i++)
                {
                    Thread.Sleep(100);
                    res = Reader.ConnectScanner(ref m_hScanner, szPort, ref nBaudRate);
                    if (res == OK)
                        break;
                 }
            }
            if (res != OK)
            {
                MessageBox.Show("Can't connect the reader!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            progressBar1.PerformStep();
            res = ReaderParam();
            if (res != OK)
            {
                progressBar1.Visible = false;
                return;
            }
            progressBar1.Visible = false;
            MessageBox.Show("Set default parameter successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //设置过滤器
            int i = 0;
            int str_len = 0, m = 0, filterLength = 0, filterAddress;
            string str;
            byte[] FilterMask = new byte[64];

            str = textBox15.Text;
            str_len = str.Length;
            filterLength = Convert.ToByte(textBox14.Text);
            filterAddress = Convert.ToByte(textBox13.Text);

            if (filterLength == 0)
                m = 0;
            else
            {
                m = filterLength / 8;
                if (filterLength % 8 != 0)
                {
                    for (i = 0; i < ((m + 1) * 2 - str_len); i++)
                        str += "0";
                    m++;
                }
            }
            filterLength = Convert.ToByte(textBox14.Text);


            for (i = 0; i < m; i++)
            {
                FilterMask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            if (filterAddress + filterLength > 96)
                filterLength = 8 - filterAddress;

            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.SetReportFilter(m_hScanner, filterAddress, filterLength, FilterMask, 0);
                        break;
                    case 1:
                        res = Reader.Net_SetReportFilter(m_hSocket, filterAddress, filterLength, FilterMask);
                        break;
                    case 2:
                        res = Reader.SetReportFilter(m_hScanner, filterAddress, filterLength, FilterMask, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                MessageBox.Show("Fail to update Reader AutoParameter!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            //取得过滤器
            int i=0;
            int FilterAddress = 0;
            int FilterLength = 0;
            byte[] FilterMask = new byte[64];
            int l = 0;
            string str, str_temp;

            for (i = 0; i < 3; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.GetReportFilter(m_hScanner, ref FilterAddress, ref FilterLength, FilterMask, 0);
                        break;

                    case 1:
                        res = Reader.Net_GetReportFilter(m_hSocket, ref FilterAddress, ref FilterLength, FilterMask);
                        break;

                    case 2:
                        res = Reader.GetReportFilter(m_hScanner, ref FilterAddress, ref FilterLength, FilterMask, RS485Address);
                        break;

                }
                if (res == OK) break;
            }
            if (res != OK)
            {
                MessageBox.Show("Read ReportFilter Parameter Fail!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return ;
            }

            textBox13.Text = FilterAddress.ToString();
            textBox14.Text = FilterLength.ToString();

            if (FilterLength % 8 == 0)
            {
                l = FilterLength / 8;
            }
            else
            {
                l = FilterLength / 8 + 1;
            }

            str = "";
            for (i = 0; i < l; i++)
            {
                str_temp = FilterMask[i].ToString("X");
                str += str_temp;
            }
            textBox15.Text = str;

        }

        private void button11_Click(object sender, EventArgs e)
        {
            //开启自动模式
            for (int i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.AutoMode(m_hScanner, 1, 0);
                       
                        break;
                    case 1:
                        res = Reader.Net_AutoMode(m_hSocket, 1);
                        break;

                    case 2:
                        res = Reader.AutoMode(m_hScanner, 1, RS485Address);

                        break;

                }
                if (res == OK)
                    break;
            }

            if (res == OK)
            {
                MessageBox.Show("Start Auto Mode successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Start Auto Mode fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //停止自动模式
            for (int i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.AutoMode(m_hScanner, 0, 0);

                        break;
                    case 1:
                        res = Reader.Net_AutoMode(m_hSocket, 0);
                        break;

                    case 2:
                        res = Reader.AutoMode(m_hScanner, 0, RS485Address);
                        break;

                }
                if (res == OK)
                    break;
            }

            if (res == OK)
            {
                MessageBox.Show("Stop Auto Mode successfully!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Stop Auto Mode fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            //开始扫描设备
            button12.Enabled = false;
            button13.Enabled = true;
            try
            {
                int i = 0;
                //IPAddress ia = Dns.GetHostAddresses(Dns.GetHostName())[0];
                IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                //IPEndPoint Info = new IPEndPoint(IPAddress.Any, 4444);
                for (i = 0; i < arrIPAddresses.Length; i++)
                {
                    if (textBox37.Text == arrIPAddresses[i].ToString())
                    {
                        break;
                    }
                }
                
                IPEndPoint Info = new IPEndPoint(arrIPAddresses[i], 4444);
                NetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                NetSocket.Bind(Info);

                NetSocket.BeginReceive(IPrecebuffer, 0, IPrecebuffer.Length, SocketFlags.None, new AsyncCallback(IPReceiveCallBack), null);
            }
            catch (Exception ex)
            {
                button12.Enabled = true;
                MessageBox.Show(ex.ToString() , "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private  void IPReceiveCallBack(IAsyncResult AR)
        {

            try
            {
                //结束挂起的异步读取，返回接收到的字节数。 AR，它存储此异步操作的状态信息以及所有用户定义数据
                int REnd = NetSocket.EndReceive(AR);

                if (REnd > 0)
                {

                    ReaderIP = "";
                    ReaderIP = IPrecebuffer[3].ToString() + "." + IPrecebuffer[4].ToString() + "." + IPrecebuffer[5].ToString() + "." + IPrecebuffer[6].ToString();

                    ReaderPort = IPrecebuffer[7];
                    ReaderPort = (ReaderPort << 8) + IPrecebuffer[8];

                    NetInvoke mi = new NetInvoke(UpdateListView);
                    this.BeginInvoke(mi);

                   
                }

                NetSocket.BeginReceive(IPrecebuffer, 0, IPrecebuffer.Length, 0, new AsyncCallback(IPReceiveCallBack), null);

            }
            catch
            {

            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //停止扫描这个设备
            button13.Enabled = false;
            button12.Enabled = true;
            NetSocket.Close();
        }

        private void UpdateListView()
        {
            int  row = 0;
            int count = 0;
            string  str;

            row = listView4.Items.Count;
            str = "";
            count = 0;
            for (int i = 0; i < row; i++)
            {
                str = listView4.Items[i].SubItems[1].Text;
                if (ReaderIP == str)
                {
                    count = 1;
                    break;
                }

            }
            if (count == 0)
            {
                ListViewItem myitem = new ListViewItem();
                myitem = listView4.Items.Add(row.ToString(), row);
                myitem.SubItems.Add(ReaderIP);
                myitem.SubItems.Add(ReaderPort.ToString());
            }

        }

        

        private void button14_Click(object sender, EventArgs e)
        {

            //自动模式下，开始和结束
            button14.Enabled = true;
            button15.Enabled = false;


            listView5.Items.Clear(); 

            for(int i=0;i<5;i++)
            {
                switch (ComMode)
                {
                case 0:
                    res = Reader.AutoMode(m_hScanner, 1, 0);
                    break;
                case 1:
                    res = Reader.Net_AutoMode(m_hSocket, 1);
                    break;
                case 2:
                    res = Reader.AutoMode(m_hScanner, 1, RS485Address);
                    break;
                }
                if(res==OK)
                {
                    break;
                }
            }
            if (res!=OK)
            {
                
                MessageBox.Show("Start Auto Mode fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (ComMode == 1)
            {
                try
                {
                    button14.Enabled = false;
                    button15.Enabled = true;
                    #if aaa
                    //IPAddress ia = Dns.GetHostAddresses(Dns.GetHostName())[0];
                    IPAddress[] arrIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                    //IPEndPoint Info = new IPEndPoint(ia, AutoPort);
                    //IPEndPoint Info = new IPEndPoint(IPAddress.Any, AutoPort);
                    IPEndPoint Info = new IPEndPoint(arrIPAddresses[0], AutoPort);
                    EpcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    EpcSocket.Bind(Info);

                    EpcSocket.BeginReceive(Epcrecebuffer, 0, Epcrecebuffer.Length, SocketFlags.None, new AsyncCallback(EpcReceiveCallBack), null);
                    #endif

                    IPEndPoint Info = new IPEndPoint(IPAddress.Any, AutoPort);
                    EpcSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    EpcSocket.Bind(Info);

                    EpcSocket.BeginReceive(Epcrecebuffer, 0, Epcrecebuffer.Length, SocketFlags.None, new AsyncCallback(EpcReceiveCallBack), null);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            else
            {
                try
                {
                    button14.Enabled = false;
                    button15.Enabled = true;

                    Reader.DisconnectScanner(m_hScanner);

                    serialPort1.BaudRate = nBaudRate;
                    serialPort1.PortName = szPort;
                    serialPort1.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }

        }

        private void EpcReceiveCallBack(IAsyncResult AR)
        {

            //string str;
            //int i;

            try
            {
                //结束挂起的异步读取，返回接收到的字节数。 AR，它存储此异步操作的状态信息以及所有用户定义数据
                int REnd = EpcSocket.EndReceive(AR);

                if (REnd > 0)
                {
                    NetInvoke mi = new NetInvoke(UpdateEpcView);
                    this.BeginInvoke(mi);
                }

                EpcSocket.BeginReceive(Epcrecebuffer, 0, Epcrecebuffer.Length, 0, new AsyncCallback(EpcReceiveCallBack), null);

            }
            catch
            {

            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int len=0;
            string str;
            str=serialPort1.ReadLine();
            len = str.Length; 
            if (len > 0)
            {
                Epcrecebuffer=Encoding.UTF8.GetBytes(str);
                NetInvoke mi = new NetInvoke(UpdateEpcView);
                this.BeginInvoke(mi);
            }
        }


        private void UpdateEpcView()
        {
            string str;
            int i;
            int row = 0;
            if (Epcrecebuffer[0] == 0x44)
            {
                //第一次读取的时间
                str = "";
                for (i = 0; i < 19; i++)
                {
                    str += (char)Epcrecebuffer[0x05 + i];
                }
                EpcStr[1] = str;


                //最后一次读取的时间
                str = "";
                for (i = 0; i < 19; i++)
                {
                    str += (char)Epcrecebuffer[0x1f + i];
                }
                EpcStr[2] = str;


                //读取的次数
                str = "";
                for (i = 0; i < 5; i++)
                {
                    str += (char)Epcrecebuffer[0x3a + i];
                }
                EpcStr[3] = str;


                //天线编号
                str = "";
                for (i = 0; i < 2; i++)
                {
                    str += (char)Epcrecebuffer[0x45 + i];
                }
                EpcStr[4] = str;

                //标签类型
                str = "";
                for (i = 0; i < 2; i++)
                {
                    str += (char)Epcrecebuffer[0x4e + i];
                }
                EpcStr[5] = str;


                //EPC
                str = "";
                for (i = 0; i < 74; i++)
                {
                    str += (char)Epcrecebuffer[0x56 + i];
                }
                EpcStr[6] = str;

            }
            else
            {
                str = "";

                //读取的次数
                for (i = 0; i < 5; i++)
                {
                    str += (char)Epcrecebuffer[i];
                }
                EpcStr[3] = str;

                //天线编号
                str = "";
                for (i = 0; i < 2; i++)
                {
                    str += (char)Epcrecebuffer[0x07 + i];
                }
                EpcStr[4] = str;

                //标签类型
                str = "";
                for (i = 0; i < 2; i++)
                {
                    str += (char)Epcrecebuffer[0x0b + i];
                }
                EpcStr[5] = str;

                //EPC
                str = "";
                for (i = 0; i < 74; i++)
                {
                    str += (char)Epcrecebuffer[0x0f + i];
                }
                EpcStr[6] = str;


            }

            row = listView5.Items.Count;
            EpcStr[0] = row.ToString(); 
            listView5.Items.Add(new ListViewItem(EpcStr));
            listView5.EnsureVisible(listView5.Items.Count - 1);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            //停止按钮
            if (ComMode == 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    res = Reader.Net_AutoMode(m_hSocket, 0);
 
                    if (res == OK)
                    {
                        break;
                    }
                }
                if (res != OK)
                {
                    MessageBox.Show("Start Auto Mode fail!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                EpcSocket.Close();
            }
            else
            {
                serialPort1.Close(); 
                for (int i = 0; i < 5; i++)
                {
                    int iBaudRate = nBaudRate;
                    res = Reader.ConnectScanner(ref m_hScanner, szPort, ref iBaudRate);
                    if (res == OK)
                    {
                        button14.Enabled = true;
                        button15.Enabled = false;
                        break;
                    }
                }
            }

            button14.Enabled = true;
            button15.Enabled = false;
        }

        

        private void radioButton49_Click(object sender, EventArgs e)
        {
            //英文语言切换
            nBaudRate = 0;
        }

        private void radioButton50_Click(object sender, EventArgs e)
        {
            //中文语言切换
            nBaudRate = 0;
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        
        #region READEPROTECT
        private void button19_Click(object sender, EventArgs e)
        {
            //设置读保护Lock Read
            if (connect_OK == 0)
                return;
            int i;
            string str;
            if (comboBox8.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (textBox13.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox13.Focus();
                textBox13.SelectAll();
                return;
            }

            str = textBox13.Text;
            for (i = 0; i < 4; i++)
            {
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            EPC_Word = TagBuffer[comboBox8.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox8.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox8.SelectedIndex, i + 1];
            }

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_ReadProtect(m_hScanner, AccessPassword, Convert.ToByte(EPC_Word), IDTemp, 0);
                        break;
                    case 1:
                        res = Reader.Net_EPC1G2_ReadProtect(m_hSocket, AccessPassword, Convert.ToByte(EPC_Word), IDTemp);
                        break;
                    case 2:
                        res = Reader.EPC1G2_ReadProtect(m_hScanner, AccessPassword, Convert.ToByte(EPC_Word), IDTemp, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Fail to read protect!Please try again!";
                MessageBox.Show(str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                MessageBox.Show("Read protect successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        private void button18_Click(object sender, EventArgs e)
        {
            //解读保护
            if (connect_OK == 0)
                return;
            int i;
            string str;

            if (textBox13.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox13.Focus();
                textBox13.SelectAll();
                return;
            }

            str = textBox13.Text;
            for (i = 0; i < 4; i++)
            {
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            EPC_Word = TagBuffer[comboBox8.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox8.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox8.SelectedIndex, i + 1];
            }

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_RStreadProtect(m_hScanner, AccessPassword, 0);
                        break;
                    case 1:
                        res = Reader.Net_EPC1G2_RStreadProtect(m_hSocket, AccessPassword);
                        break;
                    case 2:
                        res = Reader.EPC1G2_RStreadProtect(m_hScanner, AccessPassword, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res != OK)
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Fail to unlock read protect!Please try again!";
                MessageBox.Show(str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                MessageBox.Show("UnLockRead successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region EAS报警
        private void button22_Click(object sender, EventArgs e)
        {
            //EAS报警
            if (connect_OK == 0)
                return;
            string str;
            int i;
            if (comboBox7.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (textBox21.Text.Length != 8)
            {
                MessageBox.Show("Please input correct accesspassword!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox21.Focus();
                textBox21.SelectAll();
                return;
            }
            str = textBox21.Text;
            for (i = 0; i < 4; i++)
            {
                AccessPassWord[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            EPC_Word = TagBuffer[comboBox7.SelectedIndex, 0];
            for (i = 0; i < TagBuffer[comboBox7.SelectedIndex, 0] * 2; i++)
            {
                IDTemp[i] = TagBuffer[comboBox7.SelectedIndex, i + 1];
            }

            int Alarm = 0;
            if (radioButton58.Checked == true)
                Alarm = 0;
            if (radioButton57.Checked == true)
                Alarm = 1;

            m_antenna_sel = 0;
            if (checkBox1.Checked == true)
                m_antenna_sel += 1;
            if (checkBox2.Checked == true)
                m_antenna_sel += 2;
            if (checkBox3.Checked == true)
                m_antenna_sel += 4;
            if (checkBox4.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != OK)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            for (i = 0; i < 5; i++)
            {
                Thread.Sleep(30);
                switch (ComMode)
                {
                    case 0:
                        res = Reader.EPC1G2_ChangeEas(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(Alarm), AccessPassWord, 0);
                        break;
                    case 1:
                        res = Reader.Net_EPC1G2_ChangeEas(m_hSocket, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(Alarm), AccessPassWord);
                        break;
                    case 2:
                        res = Reader.EPC1G2_ChangeEas(m_hScanner, Convert.ToByte(EPC_Word), IDTemp, Convert.ToByte(Alarm), AccessPassWord, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }

            if (res == OK)
            {
                MessageBox.Show("Set EAS alarm successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.ReportError(ref str);
                if (str == "Unbeknown Error!")
                    str = "Fail to set EAS alarm!";
                if (res == 9)
                    str = "The accesspassword is error!";
                MessageBox.Show(str, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            //检测EAS报警
            if (connect_OK == 0)
                return;
            int i;
            nidEvent = 3;
            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox2.SelectedIndex];

            if (button21.Text == "EasAlarm")
            {
                m_antenna_sel = 0;
                if (checkBox1.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox2.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox3.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox4.Checked == true)
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
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                                case 2:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != OK)
                        {
                            MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            button10.Text = "EasAlarm";
                            return;
                        }
                        break;
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        button10.Text = "EasAlarm";
                        return;
                }
                button21.Text = "Stop";
                Read_times = 0;
                timer1.Interval = Interval;
                timer1.Enabled = true;
            }
            else
            {
                button21.Text = "EasAlarm";
                timer1.Enabled = false;
            }
        }
        #endregion


        #region ISO6B读写
        private void button23_Click(object sender, EventArgs e)
        {
            //6B列出标签
            if (connect_OK == 0)
                return;
            int i, j;
            string str, strtemp;
            nidEvent = 5;

            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox9.SelectedIndex];
            if (button23.Text == "List Tag ID")
            {
                m_antenna_sel = 0;
                if (checkBox9.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox10.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox11.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox12.Checked == true)
                    m_antenna_sel += 8;

                switch (m_antenna_sel)
                {
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                                case 2:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != 0)
                        {
                            MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        break;
                }

                button23.Text = "Stop";
                Read_times = 0;
                k = 0;
                listView2.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;

            }
            else
            {
                button23.Text = "List Tag ID";
                timer1.Enabled = false;
                comboBox12.Items.Clear();

                for (i = 0; i < k; i++)
                {
                    str = listView2.Items[i].SubItems[1].Text;
                    for (j = 0; j < str.Length / 2; j++)
                    {
                        TagNumber[i, j] = (byte)Convert.ToInt16((str[j * 2].ToString() + str[j * 2 + 1].ToString()), 16);
                    }
                    strtemp = (i + 1).ToString("D2") + ".";
                    comboBox12.Items.Add(strtemp + str);
                }
                if (k != 0)
                {
                    comboBox12.SelectedIndex = 0;
                }
            }

        }

        private void button31_Click(object sender, EventArgs e)
        {
            //6B块读
            if (connect_OK == 0)
                return;
            int i;
            nidEvent = 6;

            if (comboBox12.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt16(textBox24.Text) < 0 || Convert.ToInt16(textBox24.Text) > 223)
            {
                MessageBox.Show("Please input start address of Tag between 0 and 223!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox24.Focus();
                textBox24.SelectAll();
                return;
            }

            if (Convert.ToInt16(textBox23.Text) < 1 || Convert.ToInt16(textBox23.Text) > 32)
            {
                MessageBox.Show("Please input length of data between 1 and 32!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox23.Focus();
                textBox23.SelectAll();
                return;
            }

            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox9.SelectedIndex];

            ptr = Convert.ToInt16(textBox24.Text);
            len = Convert.ToInt16(textBox23.Text);
            if (ptr + len > 224)
                len = 224 - ptr;

            for (i = 0; i < 8; i++)
            {
                IDTemp[i] = TagNumber[comboBox12.SelectedIndex, i];
            }

            if (button31.Text == "Read")
            {
                m_antenna_sel = 0;
                if (checkBox9.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox10.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox11.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox12.Checked == true)
                    m_antenna_sel += 8;

                switch (m_antenna_sel)
                {
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                                case 2:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != 0)
                        {
                            MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        break;
                }

                button31.Text = "Stop";
                Read_times = 0;
                //                k = 0;
                //                listView1.Items.Clear();
                listBox2.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;
            }
            else
            {
                button31.Text = "Read";
                timer1.Enabled = false;
            }


        }

        private void button30_Click(object sender, EventArgs e)
        {
            //6B块写
            if (connect_OK == 0)
                return;
            int i;
            string str;
            nidEvent = 7;
            if (comboBox12.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt16(textBox24.Text) < 8 || Convert.ToInt16(textBox24.Text) > 223)
            {
                MessageBox.Show("Please input start address of Tag between 8 and 223!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox24.Focus();
                textBox24.SelectAll();
                return;
            }

            if (Convert.ToInt16(textBox23.Text) < 1 || Convert.ToInt16(textBox23.Text) > 16)
            {
                MessageBox.Show("Please input length of data between 1 and 16!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox23.Focus();
                textBox23.SelectAll();
                return;
            }

            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox9.SelectedIndex];

            ptr = Convert.ToInt16(textBox24.Text);
            len = Convert.ToInt16(textBox23.Text);
            if (ptr + len > 224)
                len = 224 - ptr;

            for (i = 0; i < 8; i++)
            {
                IDTemp[i] = TagNumber[comboBox12.SelectedIndex, i];
            }

            str = textBox22.Text;
            for (i = 0; i < textBox22.Text.Length / 2; i++)
            {
                mask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            if (button30.Text == "Write")
            {
                m_antenna_sel = 0;
                if (checkBox9.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox10.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox11.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox12.Checked == true)
                    m_antenna_sel += 8;

                switch (m_antenna_sel)
                {
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                                case 2:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != 0)
                        {
                            MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        break;
                }

                button30.Text = "Stop";
                Read_times = 0;
                //                k = 0;
                //                listView1.Items.Clear();
                listBox2.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;
            }
            else
            {
                button30.Text = "Write";
                timer1.Enabled = false;
            }

        }

        private void button32_Click(object sender, EventArgs e)
        {
            //6B选择性标签
            if (connect_OK == 0)
                return;
            string str, strtemp;
            int i, j;
            nidEvent = 8;
            cmd = 0;

            if (radioButton62.Checked == true)
                cmd += 0;
            if (radioButton61.Checked == true)
                cmd += 1;
            if (radioButton60.Checked == true)
                cmd += 2;
            if (radioButton59.Checked == true)
                cmd += 3;

            if (Convert.ToInt16(textBox46.Text) < 0 || Convert.ToInt16(textBox46.Text) > 223)
            {
                MessageBox.Show("Please input the start address of data between 0 and 223!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox46.Focus();
                textBox46.SelectAll();
                return;
            }
            ptr = Convert.ToInt16(textBox46.Text);
            if (textBox32.Text.Length > 8)
            {
                MessageBox.Show("Please input suitable length data!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox32.Focus();
                textBox32.SelectAll();
                return;
            }
            str = textBox32.Text;

            for (i = 0; i < textBox32.Text.Length / 2; i++)
            {
                mask[i] = (byte)Convert.ToInt16((str[i * 2].ToString() + str[i * 2 + 1].ToString()), 16);
            }

            switch (textBox32.Text.Length / 2)
            {
                case 1:
                    Mask = 0x80;
                    break;
                case 2:
                    Mask = 0xC0;
                    break;
                case 3:
                    Mask = 0xE0;
                    break;
                case 4:
                    Mask = 0xF0;
                    break;
                case 5:
                    Mask = 0xF8;
                    break;
                case 6:
                    Mask = 0xFC;
                    break;
                case 7:
                    Mask = 0xfe;
                    break;
                case 8:
                    Mask = 0xFF;
                    break;
            }


            int[] timrinterval = new int[] { 10, 20, 30, 50, 100, 200, 500 };
            Interval = timrinterval[comboBox9.SelectedIndex];
            if (button32.Text == "List ID of Selected Tags")
            {
                m_antenna_sel = 0;
                if (checkBox9.Checked == true)
                    m_antenna_sel += 1;
                if (checkBox10.Checked == true)
                    m_antenna_sel += 2;
                if (checkBox11.Checked == true)
                    m_antenna_sel += 4;
                if (checkBox12.Checked == true)
                    m_antenna_sel += 8;

                switch (m_antenna_sel)
                {
                    case 0:
                        MessageBox.Show("Please choose one antenna at least!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    case 1:
                    case 2:
                    case 4:
                    case 8:
                        for (i = 0; i < 3; i++)
                        {
                            switch (ComMode)
                            {
                                case 0:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                    break;
                                case 1:
                                    res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                    break;
                                case 2:
                                    res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                    break;
                            }
                            if (res == OK)
                                break;

                        }
                        if (res != 0)
                        {
                            MessageBox.Show("Fail to set antenna! Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        break;
                }

                button32.Text = "Stop";
                Read_times = 0;
                k = 0;
                listView2.Items.Clear();
                timer1.Interval = Interval;
                timer1.Enabled = true;

            }
            else
            {
                button32.Text = "List ID of Selected Tags";
                timer1.Enabled = false;
                comboBox12.Items.Clear();

                for (i = 0; i < k; i++)
                {
                    str = listView2.Items[i].SubItems[1].Text;
                    for (j = 0; j < str.Length / 2; j++)
                    {
                        TagNumber[i, j] = (byte)Convert.ToInt16((str[j * 2].ToString() + str[j * 2 + 1].ToString()), 16);
                    }
                    strtemp = (i + 1).ToString("D2") + ".";
                    comboBox12.Items.Add(strtemp + str);
                }
            }

        }

        private void button34_Click(object sender, EventArgs e)
        {
            //6B设置保护
            if (connect_OK == 0)
                return;
            int i, j;
            if (comboBox12.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt16(textBox47.Text) < 8 || Convert.ToInt16(textBox47.Text) > 223)
            {
                MessageBox.Show("Please input an address of tag memory between 8 and 223!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox47.Focus();
                textBox47.SelectAll();
                return;
            }
            ptr = Convert.ToInt16(textBox47.Text);

            for (i = 0; i < 8; i++)
            {
                IDTemp[i] = TagNumber[comboBox12.SelectedIndex, i];
            }

            j = (int)MessageBox.Show("Do you determine to protect address" + ptr.ToString() + " of the tag ?", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (j != 1)
                return;

            m_antenna_sel = 0;
            if (checkBox9.Checked == true)
                m_antenna_sel += 1;
            if (checkBox10.Checked == true)
                m_antenna_sel += 2;
            if (checkBox11.Checked == true)
                m_antenna_sel += 4;
            if (checkBox12.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != 0)
                    {
                        MessageBox.Show("Fail to set antenna!Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }
            switch (ComMode)
            {
                case 0:
                    res = Reader.ISO6B_WriteProtect(m_hScanner, IDTemp, Convert.ToByte(ptr), 0);
                    break;
                case 1:
                    res = Reader.Net_ISO6B_WriteProtect(m_hSocket, IDTemp, Convert.ToByte(ptr));
                    break;
                case 2:
                    res = Reader.ISO6B_WriteProtect(m_hScanner, IDTemp, Convert.ToByte(ptr), RS485Address);
                    break;
            }
            if (res == OK)
            {
                MessageBox.Show("Set write protect successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Fail to set write protect !Please try again!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void button33_Click(object sender, EventArgs e)
        {
            //检查保护
            if (connect_OK == 0)
                return;
            int i;
            byte protect = 0;
            if (comboBox12.SelectedIndex < 0)
            {
                MessageBox.Show("Please identify a tag first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Convert.ToInt16(textBox47.Text) < 0 || Convert.ToInt16(textBox47.Text) > 223)
            {
                MessageBox.Show("Please input an address of tag memory between 0 and 223!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox47.Focus();
                textBox47.SelectAll();
                return;
            }
            ptr = Convert.ToInt16(textBox47.Text);

            for (i = 0; i < 8; i++)
            {
                IDTemp[i] = TagNumber[comboBox12.SelectedIndex, i];
            }

            m_antenna_sel = 0;
            if (checkBox9.Checked == true)
                m_antenna_sel += 1;
            if (checkBox10.Checked == true)
                m_antenna_sel += 2;
            if (checkBox11.Checked == true)
                m_antenna_sel += 4;
            if (checkBox12.Checked == true)
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
                            case 0:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, 0);
                                break;
                            case 1:
                                res = Reader.Net_SetAntenna(m_hSocket, m_antenna_sel);
                                break;
                            case 2:
                                res = Reader.SetAntenna(m_hScanner, m_antenna_sel, RS485Address);
                                break;
                        }
                        if (res == OK)
                            break;

                    }
                    if (res != 0)
                    {
                        MessageBox.Show("Fail to set antenna! Please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    MessageBox.Show("Please choose one antenna!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            for (i = 0; i < 5; i++)
            {
                switch (ComMode)
                {
                    case 0:
                        res = Reader.ISO6B_ReadWriteProtect(m_hScanner, IDTemp, Convert.ToByte(ptr), ref protect, RS485Address);
                        break;
                    case 1:
                        res = Reader.Net_ISO6B_ReadWriteProtect(m_hSocket, IDTemp, Convert.ToByte(ptr), ref protect);
                        break;
                    case 2:
                        res = Reader.ISO6B_ReadWriteProtect(m_hScanner, IDTemp, Convert.ToByte(ptr), ref protect, RS485Address);
                        break;
                }
                if (res == OK)
                    break;
            }
            if (res == OK)
            {
                switch (protect)
                {
                    case 0:
                        MessageBox.Show("The address" + ptr.ToString() + " has not been written protection!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case 1:
                        MessageBox.Show("The address" + ptr.ToString() + " has been written protection!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }

            }
            else
            {
                MessageBox.Show("Fail to check protect! Please try again!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        #endregion










    }
}