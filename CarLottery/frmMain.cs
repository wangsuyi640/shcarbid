#define TEST

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CarBidWebClient
{
    
    public class frmMain : Form
    {
        /// <summary>
        /// START : add by mz
        /// </summary>
        private UdpBIDProxyClient _cc_udp;

        private bool checkPass = false;

        private string _urlLogin = "about:blank";

        private string _urlBid = "about:blank";

        private int _priceInputRight = 240;

        private int _priceInputBottom = 304;

        private int _preBidRight = 100;

        private int _preBidBottom = 304;

        private int _bidRight = 350;

        private int _bidBottom = 230;
        /// <summary>
        /// END
        /// </summary>

        private BIDContext _ctx = new BIDContext();

        private long _lastTick;

        private IContainer components;

        private WebBrowser wbMain;

        private Panel pnControl;

        // 自动狙击
        private RadioButton AutoFireBox;
        private GroupBox AutoFireGroupBox;
        private RadioButton AutoFire_1;
        public DateTime FireTime_1 = Convert.ToDateTime("11:29:40");
        public int FireBid_1 = 900;
        public int AutoAheadBid_1 = 0;    //提前出价价格
        private RadioButton AutoFire_2;
        public DateTime FireTime_2 = Convert.ToDateTime("11:29:40");
        public int FireBid_2 = 900;
        public int AutoAheadBid_2 = 0;
        public int AutoFireTimeSecond;   //生成54-57秒的随机数
        public Random random = new Random();
        public string AutoFireTime = "11:29:";   

        // 手动狙击
        private RadioButton ManuFireBox;
        private GroupBox ManuFireGroupBox;
        private Button ManuFire_900;
        public int FireBid_900 = 900;
        private Button ManuFire_1000;
        public int FireBid_1000 = 1000;
        private Button ManuFire_1200;
        public int FireBid_1200 = 1200;
        public bool ManuFlag = false;

        // 界面上的测试环境按钮
        private Button btnCheck;

        //界面上的开始拍卖按钮
        private Button btnLogin;

        //界面上的刷新按钮
        private Button btnRefresh;

        //界面上的重置证书按钮
        private Button btnResetFiddlerCert;

        private Label lblInfo;

        private System.Windows.Forms.Timer tmUI;

        public frmMain()
        {
            this.InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bid.log");
            BIDLogger.GetLogger(null).AddHandler(new FileLogger(path));
            base.ClientSize = new Size(this.pnControl.Width + 900, 700);
            this.wbMain.Navigate("about:blank");
            this.wbMain.ScrollBarsEnabled = false;
            this.tmUI.Enabled = true;
            this.AutoFireTimeSecond = this.random.Next(54, 57); //生成54 - 57秒的随机数
            this.AutoFireTime += this.AutoFireTimeSecond.ToString();

            // Load License
            //try
            //{
            //    string licenseText = System.IO.File.ReadAllText(@"license.dat");
            //    Tuple<string, int> resTuple = EncryptionHelper.ParserLicense(licenseText);
            //    this._ctx.BIDNumberMore = resTuple.Item2;
            //    if (resTuple.Item1 != "0")
            //    {
            //        System.Windows.Forms.MessageBox.Show(resTuple.Item1);
            //        Application.Exit();
            //    }
            //}
            //catch (System.IO.FileNotFoundException ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("license文件不存在！");
            //    BIDLogger.GetLogger(null).Append(ex.ToString());
            //    Application.Exit();
            //}
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show("未知错误 请换一台干净的Windows 7电脑");
            //    BIDLogger.GetLogger(null).Append(ex.ToString());
            //    Application.Exit();
            //}
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Log(string str)
        {
            BIDLogger.GetLogger(null).Append(str, 1);
        }

        private void tmUI_Tick(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" 本地时间：");
            stringBuilder.Append(DateTime.Now.ToString("HH:mm:ss"));
            stringBuilder.AppendLine();

            // 这个很关键 业务逻辑应该按照这个时间来组织            
            if (this._ctx.UpdateTime.Ticks > 0L)
            {
                stringBuilder.Append(" 国拍时间：");
                stringBuilder.Append(this._ctx.UpdateTime.ToString("HH:mm:ss"));
                stringBuilder.AppendLine();
                stringBuilder.Append(" 最新价格：");
                stringBuilder.Append(this._ctx.BaseAmount);
                stringBuilder.AppendLine();
            }

            // 判定license中的BIDNumberMore和真正登录的BIDNumber是否是同一个
            if (this.checkPass && this._ctx.BIDNumer > 0)
            {
                if (this._ctx.BIDNumberMore != this._ctx.BIDNumer)
                {
#if TEST
                    // Nothing to do
#else
                    //System.Windows.Forms.MessageBox.Show("登录帐号和license中不一致");
                    //Application.Exit();
#endif
                }
                stringBuilder.Append(" 当前用户：");
                stringBuilder.Append(this._ctx.BIDNumer);
                stringBuilder.AppendLine();
            }

            if (this.checkPass && this._ctx.imageReqFlag && this._ctx.BIDAimedFlag)
            {
                stringBuilder.Append(" 当前出价：");
                stringBuilder.Append(this._ctx.BIDAmount);
                stringBuilder.AppendLine();
            }

            // 核心步骤一：  11:29:40 去申请验证码 加1000， 如果是手动加300/600/900,直接用ManuFlag
            if (this.checkPass && ((!this._ctx.bidCompleteFlag && !this._ctx.imageReqFlag && (DateTime.Compare(this._ctx.UpdateTime, this._ctx.bidAimedTime) >= 0)) && !this.ManuFlag))
            {
                this._ctx.BIDAmount = this._ctx.BaseAmount + this._ctx.AddBIDAmount;
                this.DoBidPre(this._ctx.BIDAmount);
            }
            // 核心步骤二： this._ctx.BIDAimedFlag = true 输入完验证码按了回车

            // 核心步骤三：价格到达区间 价格自动提交，AheadBIDAmount是设定的提前出价价格
#if TEST
            if (this._ctx.BIDAimedFlag && DateTime.Compare(this._ctx.UpdateTime, this._ctx.bidEndTime) >= 0)           
#else
            // 到了11点29分54秒 - 57秒 随机出价
            if (this._ctx.BIDAimedFlag && DateTime.Compare(this._ctx.UpdateTime, Convert.ToDateTime(this.AutoFireTime)) >= 0)
#endif
            {
                this.DoBid();
            }

            if (this._ctx.bidAimedTime.Ticks > 0)
            {
                stringBuilder.Append("输验证码时间：");
                stringBuilder.Append(this._ctx.bidAimedTime.ToString("HH:mm:ss"));
                stringBuilder.AppendLine();
                stringBuilder.Append("警告：输完验证码请按F9！");
                stringBuilder.AppendLine();
                stringBuilder.Append("自动出价时间：");
                //stringBuilder.Append(this._ctx.bidEndTime.ToString("HH:mm:ss"));
                stringBuilder.Append(this.AutoFireTime.ToString());
                stringBuilder.AppendLine();
            }
            this.lblInfo.Text = stringBuilder.ToString();
        }

        private void InitCommon()
        {
            // 初始化各个时间节点
#if TEST
            this._ctx.imageReqFlag = false;
            this._ctx.bidCompleteFlag = false;
            this._ctx.bidAllowFlag = false;

            TestInfo testInfo = HttpHelper.GetTestInfo();
            try
            {
                this._ctx.bidStartTime = Convert.ToDateTime(testInfo.test_start_time);
                this._ctx.bidMidTime = Convert.ToDateTime(testInfo.test_mid_time);
                this._ctx.bidEndTime = Convert.ToDateTime(testInfo.test_end_time);
                this._ctx.bidAimedTime = Convert.ToDateTime(testInfo.test_bid_time);          
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
#else
            this._ctx.bidStartTime = Convert.ToDateTime("10:30:00");
            this._ctx.bidMidTime = Convert.ToDateTime("11:00:00");
            this._ctx.bidAimedTime = Convert.ToDateTime("11:29:40");   //硬code写死，改这里狙击时间 47秒 和 40秒
            this._ctx.bidEndTime = Convert.ToDateTime("11:30:00");
#endif

            this._ctx.BIDNumer = 88886666;
            //this._ctx.AddBIDAmount = 800;      //硬code写死，2016年1月40s加价800
            this._ctx.AddBIDAmount = (this.random.Next(600,800)/100)*100;
            this._ctx.AheadBIDAmount = 0;
            this.ManuFlag = false;
            this.AutoFire_1.Text = this._ctx.bidAimedTime.ToString("HH:mm:ss");
     
            if (DateTime.Now.Month != 2)
            {
                System.Windows.Forms.MessageBox.Show("软件版本不对 请联系我！");
                Application.Exit();
            }
        }

        //测试环境按钮
        private void btnCheck_Click(object sender, EventArgs e)
        {
            //1. Common Initialization
            this.InitCommon();

            //2. using the test page of guopai
            this.wbMain.Navigate(BIDWebConfig.UrlTest);

            //3. 初始化连接地址和坐标信息
            LoginInfo loginInfo = HttpHelper.GetLogInfo();
            if (loginInfo == null)
            {
                System.Windows.Forms.MessageBox.Show("无法连接服务器，请等待");
                return;
            }
            else if (loginInfo.login_url == "NA" || loginInfo.bid_url == "NA")
            {
                System.Windows.Forms.MessageBox.Show("云端登录信息尚未初始化，请等待...");
                return;
            }
            else if (loginInfo.priceInputRight == 0 || loginInfo.priceInputBottom == 0 || loginInfo.preBidRight == 0 || loginInfo.preBidBottom == 0 || loginInfo.bidRight == 0 || loginInfo.bidBottom == 0)
            {
                System.Windows.Forms.MessageBox.Show("云端坐标信息尚未初始化，采用默认设置，5分钟后请刷新...");
            }
            else
            {
                this._urlLogin = loginInfo.login_url;
                this._urlBid = loginInfo.bid_url;
                this._priceInputRight = loginInfo.priceInputRight;
                this._priceInputBottom = loginInfo.priceInputBottom;
                this._preBidRight = loginInfo.preBidRight;
                this._preBidBottom = loginInfo.preBidBottom;
                this._bidRight = loginInfo.bidRight;
                this._bidBottom = loginInfo.bidBottom;
            }

            //4. start the udp thread
            if (this._cc_udp != null)
            {
                this._cc_udp.Stop();
            }
            this._cc_udp = new UdpBIDProxyClient(this._ctx);

            // passed
            StringBuilder alertMessageStringBuilder = new StringBuilder();
            alertMessageStringBuilder.Append("请确认右边页面测试1，2，3，4全部绿色通过！");
            alertMessageStringBuilder.AppendLine();
            alertMessageStringBuilder.Append("确认后才可点击开始拍卖按钮！");
            System.Windows.Forms.MessageBox.Show(alertMessageStringBuilder.ToString());
            checkPass = true;
        }
        
        //开始拍卖按钮
        private void btnLoginNormal_Click(object sender, EventArgs e)
        { 
#if TEST
            this.wbMain.Navigate("http://moni.51hupai.org/?new=2");  
#else
            if (!checkPass)
            {
                System.Windows.Forms.MessageBox.Show("尚未通过完成，请点击测试环境按钮");
                return;
            }

            // 这个用C#浏览器句柄本地打开URL  然后fiddler默认就是自动截获该APP的WEB请求！
            this.wbMain.Navigate(this._urlLogin);
#endif
        }

        // Refresh Button
        private void btnLoginDirect_Click(object sender, EventArgs e)
        {
            if (this._ctx.Cookie != null)
            {
                string[] array = this._ctx.Cookie.Split(new char[]
                {
                    ';'
                }, StringSplitOptions.RemoveEmptyEntries);
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    this.wbMain.Document.Cookie = text.Trim();
                }
            }
            this.wbMain.Navigate(this._urlBid);
        }

        private void btnResetFiddlerCert_Click(object sender, EventArgs e)
        {
            UdpClient udpClient = null;
            udpClient = new UdpClient(0);
            IPEndPoint epUdpServer = new IPEndPoint(IPAddress.Parse(BIDWebConfig.yayaServer), 990);
            udpClient.Connect(epUdpServer);
            StringBuilder msgSendStringBuilder = new StringBuilder();
            msgSendStringBuilder.Append("<TYPE>UPDATE</TYPE><INFO>");

            // 显然此处的发送时间不应该再是本地的时间了：
            this._ctx.BaseAmount += 100;
            msgSendStringBuilder.Append(this._ctx.UpdateTime.ToString("HH:mm:ss"));
            msgSendStringBuilder.Append("^");
            msgSendStringBuilder.Append(this._ctx.BaseAmount);
            msgSendStringBuilder.Append("</INFO>");
            byte[] msgSend = System.Text.Encoding.Default.GetBytes(msgSendStringBuilder.ToString());
            udpClient.Send(msgSend, msgSend.Length);
            msgSendStringBuilder.Clear();
            //CertMaker.removeFiddlerGeneratedCerts(true);
            //CertMaker.createRootCert();
            //CertMaker.trustRootCert();
        }

        // 按 Alt+[0-9] 分别加价 0 300 600 900...... 并且去自动申请验证码
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                this._ctx.bidAllowFlag = true;
                return;
            }

            //if (e.KeyCode >= System.Windows.Forms.Keys.D0 && e.KeyCode <= System.Windows.Forms.Keys.D9 && e.Alt)
            //{
            //    if (DateTime.Now.Ticks < this._lastTick + 2500000L)
            //    {
            //        return;
            //    }

            //    int num = e.KeyCode - System.Windows.Forms.Keys.D0;
            //    num = this._ctx.BaseAmount + num * 300;
            //    this._ctx.BIDAmount = num;
            //    this.DoBidPre(num);
            //    this._lastTick = DateTime.Now.Ticks;
            //}
        }

        // 实际测试：一开始，都是响应frmMain_KeyDown的 但是一旦鼠标在右边浏览器上按了一下，之后便都是wbMain_PreviewKeyDown了
        private void wbMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F9)
            {
                this._ctx.bidAllowFlag = true;
                return;
            }

            //if (e.KeyCode == System.Windows.Forms.Keys.Return)
            //{
            //    System.Windows.Forms.MessageBox.Show("验证码已经输入！");
            //    return;
            //}
            
            //if (e.KeyCode >= System.Windows.Forms.Keys.D0 && e.KeyCode <= System.Windows.Forms.Keys.D9 && e.Alt)
            //{
            //    if (DateTime.Now.Ticks < this._lastTick + 2500000L)
            //    {
            //        return;
            //    }

            //    int num = e.KeyCode - System.Windows.Forms.Keys.D0;
            //    num = this._ctx.BaseAmount + num * 300;
            //    this._ctx.BIDAmount = num;
            //    this.DoBidPre(num);
            //    this._lastTick = DateTime.Now.Ticks;
            //}             
        }

        private void DoBidPre(int bidPrice)
        {
            Win32WindowHelper.Rect rect = Win32WindowHelper.Win32GetWindowPosition();
            
            // 第一步： 点击相应的输入框
            Win32InputHelper.Win32MouseClick(rect.Right - this._priceInputRight, rect.Bottom - this._priceInputBottom);

            // 第二步： TODO 清空现有输入框中的内容

            // 第三步： 输入价格：有两种方法 推荐第二种
            // 方法一：直接输入的方式
            //bool inputRes = Win32InputHelper.Win32BidPriceInput("81000");

            // 方法二：使用剪贴板来传递价格
            Clipboard.SetText(bidPrice.ToString());
            Win32InputHelper.Win32ClipboardPaste();

            //Clipboard.GetText();

            // 加了这个Show 上面的代码竟然没有执行成功
            //System.Windows.Forms.MessageBox.Show("XXX 111");

            // 第四步： 点击出价按钮
            Win32InputHelper.Win32MouseClick(rect.Right - this._preBidRight, rect.Bottom - this._preBidBottom);

            this._ctx.imageReqFlag = true;

            //怎么确定输完验证码
            this._ctx.BIDAimedFlag = true;
        }

        private void DoBid()
        {
            // 这里需要用户手动确认其验证码已经输入完毕
            if (this._ctx.bidAllowFlag == true)
            {
                Win32WindowHelper.Rect rect = Win32WindowHelper.Win32GetWindowPosition();
                // 点最后的出价进入排队, 只点一次会不会出现意外，一次点没反应？？
                Win32InputHelper.Win32MouseClick(rect.Right - this._bidRight, rect.Bottom - this._bidBottom);
                this._ctx.imageReqFlag = false;
                this._ctx.BIDAimedFlag = false;
                this._ctx.bidCompleteFlag = true;
                this._ctx.bidAllowFlag = false;
            }

        }

        private void UpdateCookie()
        {
            this._ctx.Cookie = this.wbMain.Document.Cookie;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void MouseClickEvent(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(e.Button.ToString());
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.wbMain = new System.Windows.Forms.WebBrowser();
            this.pnControl = new System.Windows.Forms.Panel();
            this.AutoFireGroupBox = new System.Windows.Forms.GroupBox();
            this.AutoFireBox = new System.Windows.Forms.RadioButton();
            this.AutoFire_1 = new System.Windows.Forms.RadioButton();
            this.AutoFire_2 = new System.Windows.Forms.RadioButton();
            this.ManuFireGroupBox = new System.Windows.Forms.GroupBox();
            this.ManuFireBox = new System.Windows.Forms.RadioButton();
            this.ManuFire_900 = new System.Windows.Forms.Button();
            this.ManuFire_1000 = new System.Windows.Forms.Button();
            this.ManuFire_1200 = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnResetFiddlerCert = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.tmUI = new System.Windows.Forms.Timer(this.components);
            this.pnControl.SuspendLayout();
            this.AutoFireGroupBox.SuspendLayout();
            this.ManuFireGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // wbMain
            // 
            this.wbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbMain.Location = new System.Drawing.Point(231, 0);
            this.wbMain.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbMain.Name = "wbMain";
            this.wbMain.Size = new System.Drawing.Size(401, 436);
            this.wbMain.TabIndex = 7;
            this.wbMain.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.wbMain_PreviewKeyDown);
            // 
            // pnControl
            // 
            this.pnControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnControl.Controls.Add(this.AutoFireBox);
            this.pnControl.Controls.Add(this.AutoFireGroupBox);
            this.pnControl.Controls.Add(this.ManuFireBox);
            this.pnControl.Controls.Add(this.ManuFireGroupBox);
            this.pnControl.Controls.Add(this.btnCheck);
            this.pnControl.Controls.Add(this.btnLogin);
            this.pnControl.Controls.Add(this.btnRefresh);
            this.pnControl.Controls.Add(this.btnResetFiddlerCert);
            this.pnControl.Controls.Add(this.lblInfo);
            this.pnControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnControl.Location = new System.Drawing.Point(0, 0);
            this.pnControl.Name = "pnControl";
            this.pnControl.Size = new System.Drawing.Size(231, 436);
            this.pnControl.TabIndex = 6;
            // 
            // AutoFireGroupBox
            // 
            //this.AutoFireGroupBox.Controls.Add(this.AutoFireBox);
           
            this.AutoFireGroupBox.Controls.Add(this.AutoFire_1);      
            //this.AutoFireGroupBox.Controls.Add(this.AutoFire_2);
            this.AutoFireGroupBox.Location = new System.Drawing.Point(6, 277);
            this.AutoFireGroupBox.Name = "AutoFireGroupBox";
            this.AutoFireGroupBox.Size = new System.Drawing.Size(214, 88);
            this.AutoFireGroupBox.TabIndex = 21;
            this.AutoFireGroupBox.TabStop = false;
            // 
            // AutoFireBox
            // 
            this.AutoFireBox.AutoSize = true;
            this.AutoFireBox.Checked = true; 
            this.AutoFireBox.Location = new System.Drawing.Point(6, 270);
            this.AutoFireBox.Name = "AutoFireBox";
            this.AutoFireBox.Size = new System.Drawing.Size(72, 16);
            this.AutoFireBox.TabIndex = 20;
            this.AutoFireBox.Text = "自动狙击";
            this.AutoFireBox.UseVisualStyleBackColor = true;
            this.AutoFireBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // AutoFire_1
            // 
            this.AutoFire_1.AutoSize = true;
            this.AutoFire_1.Checked = true;
            this.AutoFire_1.Location = new System.Drawing.Point(31, 23);
            this.AutoFire_1.Name = "AutoFire_1";
            this.AutoFire_1.Size = new System.Drawing.Size(137, 16);
            this.AutoFire_1.TabIndex = 21;
            this.AutoFire_1.TabStop = true;
            this.AutoFire_1.Text = this._ctx.bidAimedTime.ToString("HH:mm:ss");
            this.AutoFire_1.UseVisualStyleBackColor = true;
            this.AutoFire_1.CheckedChanged += new System.EventHandler(this.AutoFire_CheckedChanged);
           
            // 
            // AutoFire_2
            // 
            this.AutoFire_2.AutoSize = true;
            this.AutoFire_2.Location = new System.Drawing.Point(31, 55);
            this.AutoFire_2.Name = "AutoFire_2";
            this.AutoFire_2.Size = new System.Drawing.Size(137, 16);
            this.AutoFire_2.TabIndex = 22;
            this.AutoFire_2.TabStop = true;
            this.AutoFire_2.Text = "11:29:40 输入验证码";
            this.AutoFire_2.UseVisualStyleBackColor = true;
            this.AutoFire_2.CheckedChanged += new System.EventHandler(this.AutoFire_CheckedChanged);
            // 
            // ManuFireGroupBox
            // 
            //this.ManuFireGroupBox.Controls.Add(this.ManuFireBox);
            this.ManuFireGroupBox.Controls.Add(this.ManuFire_900);
            this.ManuFireGroupBox.Controls.Add(this.ManuFire_1000);
            this.ManuFireGroupBox.Controls.Add(this.ManuFire_1200);
            this.ManuFireGroupBox.Location = new System.Drawing.Point(6, 410);
            this.ManuFireGroupBox.Name = "ManuFireGroupBox";
            this.ManuFireGroupBox.Size = new System.Drawing.Size(214, 110);
            this.ManuFireGroupBox.TabIndex = 21;
            this.ManuFireGroupBox.TabStop = false;
            // 
            // ManuFireBox
            // 
            this.ManuFireBox.AutoSize = true;
            this.ManuFireBox.Location = new System.Drawing.Point(6, 403);
            this.ManuFireBox.Name = "ManuFireBox";
            this.ManuFireBox.Size = new System.Drawing.Size(72, 16);
            this.ManuFireBox.TabIndex = 20;
            this.ManuFireBox.Text = "手动狙击";
            this.ManuFireBox.UseVisualStyleBackColor = true;
            this.ManuFireBox.CheckedChanged += new System.EventHandler(this.CheckBox_CheckedChanged);
            // 
            // ManuFire_900
            // 
            this.ManuFire_900.AutoSize = true;
            this.ManuFire_900.Location = new System.Drawing.Point(10, 23);
            this.ManuFire_900.Name = "ManuFire_900";
            this.ManuFire_900.Size = new System.Drawing.Size(50, 16);
            this.ManuFire_900.TabIndex = 22;
            this.ManuFire_900.TabStop = true;
            this.ManuFire_900.Text = "加价900";
            this.ManuFire_900.Enabled = false;
            this.ManuFire_900.UseVisualStyleBackColor = true;
            this.ManuFire_900.Click += new System.EventHandler(this.MunaFire_900_Changed);
            // 
            // ManuFire_1000
            // 
            this.ManuFire_1000.AutoSize = true;
            this.ManuFire_1000.Location = new System.Drawing.Point(80, 23);
            this.ManuFire_1000.Name = "ManuFire_1000";
            this.ManuFire_1000.Size = new System.Drawing.Size(50, 16);
            this.ManuFire_1000.TabIndex = 22;
            this.ManuFire_1000.TabStop = true;
            this.ManuFire_1000.Text = "加价1000";
            this.ManuFire_1000.Enabled = false;
            this.ManuFire_1000.UseVisualStyleBackColor = true;
            this.ManuFire_1000.Click += new System.EventHandler(this.MunaFire_1000_Changed);
            // 
            // ManuFire_1200
            // 
            this.ManuFire_1200.AutoSize = true;
            this.ManuFire_1200.Location = new System.Drawing.Point(150, 23);
            this.ManuFire_1200.Name = "ManuFire_1200";
            this.ManuFire_1200.Size = new System.Drawing.Size(50, 16);
            this.ManuFire_1200.TabIndex = 22;
            this.ManuFire_1200.TabStop = true;
            this.ManuFire_1200.Text = "加价1200";
            this.ManuFire_1200.Enabled = false;
            this.ManuFire_1200.UseVisualStyleBackColor = true;
            this.ManuFire_1200.Click += new System.EventHandler(this.MunaFire_1200_Changed);
        
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCheck.Location = new System.Drawing.Point(6, 349);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(100, 34);
            this.btnCheck.TabIndex = 18;
            this.btnCheck.Text = "测试环境";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLogin.Location = new System.Drawing.Point(116, 349);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(100, 34);
            this.btnLogin.TabIndex = 16;
            this.btnLogin.Text = "开始拍卖";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLoginNormal_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Location = new System.Drawing.Point(6, 389);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 34);
            this.btnRefresh.TabIndex = 17;
            this.btnRefresh.Text = "刷新页面";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnLoginDirect_Click);
            // 
            // btnResetFiddlerCert
            // 
            this.btnResetFiddlerCert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnResetFiddlerCert.Location = new System.Drawing.Point(116, 389);
            this.btnResetFiddlerCert.Name = "btnResetFiddlerCert";
            this.btnResetFiddlerCert.Size = new System.Drawing.Size(100, 34);
            this.btnResetFiddlerCert.TabIndex = 9;
            this.btnResetFiddlerCert.Text = "重置证书";
            this.btnResetFiddlerCert.UseVisualStyleBackColor = true;
            this.btnResetFiddlerCert.Click += new System.EventHandler(this.btnResetFiddlerCert_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblInfo.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblInfo.ForeColor = System.Drawing.Color.DarkRed;
            this.lblInfo.Location = new System.Drawing.Point(0, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(229, 213);
            this.lblInfo.TabIndex = 12;
            // 
            // tmUI
            //
            this.tmUI.Interval = 250;
            this.tmUI.Tick += new System.EventHandler(this.tmUI_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 436);
            this.Controls.Add(this.wbMain);
            this.Controls.Add(this.pnControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拍牌小助手";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClickEvent);
            this.pnControl.ResumeLayout(false);
            this.pnControl.PerformLayout();
            this.AutoFireGroupBox.PerformLayout();
            this.ManuFireGroupBox.ResumeLayout(false);
            this.ManuFireGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        private void AutoFire_CheckedChanged(object sender, EventArgs e)
        {
            this._ctx.bidCompleteFlag = false;
            if(this.AutoFire_1.Enabled == true && this.AutoFire_1.Checked == true)
            {
                this._ctx.bidAimedTime = this.FireTime_1;
                this._ctx.AddBIDAmount = this.FireBid_1;
                this._ctx.AheadBIDAmount = this.AutoAheadBid_1;
            }
            else if(this.AutoFire_2.Enabled == true && this.AutoFire_2.Checked == true)
            {
                this._ctx.bidAimedTime = this.FireTime_2;
                this._ctx.AddBIDAmount = this.FireBid_2;
                this._ctx.AheadBIDAmount = this.AutoAheadBid_2;
            }
        }
        private void MunaFire_900_Changed(object sender, EventArgs e)
        {           
            this._ctx.AddBIDAmount = this.FireBid_900;
            this._ctx.AheadBIDAmount = 0;
            this._ctx.BIDAmount = this._ctx.BaseAmount + this._ctx.AddBIDAmount;  
            this.DoBidPre(this._ctx.BIDAmount);
        }
        private void MunaFire_1000_Changed(object sender, EventArgs e)
        {
            this._ctx.AddBIDAmount = this.FireBid_1000;
            this._ctx.AheadBIDAmount = 0;
            this._ctx.BIDAmount = this._ctx.BaseAmount + this._ctx.AddBIDAmount;
            this.DoBidPre(this._ctx.BIDAmount);
        }
        private void MunaFire_1200_Changed(object sender, EventArgs e)
        {
            this._ctx.AddBIDAmount = this.FireBid_1200;
            this._ctx.AheadBIDAmount = 0;
            this._ctx.BIDAmount = this._ctx.BaseAmount + this._ctx.AddBIDAmount;
            this.DoBidPre(this._ctx.BIDAmount);
        }
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AutoFireBox.Checked == true)
            {
                this.AutoFire_1.Enabled = true;
                this.AutoFire_2.Enabled = true;
                this.ManuFlag = false;
                this.ManuFire_900.Enabled = false;
                this.ManuFire_1000.Enabled = false;
                this.ManuFire_1200.Enabled = false;
            }
            else {
                this.AutoFire_1.Enabled = false;
                this.AutoFire_2.Enabled = false;
                this.ManuFlag = true;
                this.ManuFire_900.Enabled = true;
                this.ManuFire_1000.Enabled = true;
                this.ManuFire_1200.Enabled = true;
            }

            if (this.ManuFireBox.Checked == true)
            {
                this.ManuFlag = true;
                this.ManuFire_900.Enabled = true;
                this.ManuFire_1000.Enabled = true;
                this.ManuFire_1200.Enabled = true;
            }
            else
            {
                this.ManuFlag = false;
                this.ManuFire_900.Enabled = false;
                this.ManuFire_1000.Enabled = false;
                this.ManuFire_1200.Enabled = false;
            }

        }
    }
}
