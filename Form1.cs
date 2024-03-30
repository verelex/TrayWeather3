using Microsoft.Web.WebView2.Core;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Win32;

namespace TrayWeather3
{
    public partial class Form1 : Form
    {
        private string postfixTheme = @"-dark\";

        private int globalHostIndex = 0;

        private bool bComboBoxHostsChanged = false;

        private TwHosts? twHosts;

        private List<TwHosts> twHostsList;

        private bool firstTimeNavigationCompleted = true;

        private bool firstTimeTimerRaised = true;

        private NotifyIcon trayIcon;

        private string temperature = string.Empty;

        private static int minutes = 0;

        private bool hidden = false;

        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        private TwOptions? options;

        public Form1()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + "+.ico"),
                ContextMenuStrip = new ContextMenuStrip()
                {
                    /*Items = { new ToolStripMenuItem("Get now", null, GetWetherNow),
                                new ToolStripMenuItem("Setup", null, Setup),
                                new ToolStripMenuItem("View log", null, ViewLog),
                                new ToolStripMenuItem("Exit", null, Exit) }*/
                    Items = { new ToolStripMenuItem("Получить", null, GetWetherNow),
                                new ToolStripMenuItem("Настройка", null, Setup),
                                new ToolStripMenuItem("Смотреть лог", null, ViewLog),
                                new ToolStripMenuItem("Выход", null, Exit) }
                },
                Visible = true,
                Text = "Weather",
            };
            trayIcon.Click += trayIcon_Click;

            InitializeComponent();

            webView21.NavigationCompleted += webView21_NavigationCompleted;
        }

        private int GetCurrentTheme()
        {
            int ret = -1;
            try
            {
                ret = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return ret;
        }

        private Uri FormatHosts(int hostIndex)
        {
            twHosts = twHostsList[hostIndex];

            string url = twHosts.hst.Replace("%HST%", options.key);

            if (twHosts.end != null)
            {
                url += twHosts.end.Replace("%END%", options.q);
            }

            Uri uri = new Uri(url, UriKind.Absolute);

            return uri;
        }

        private void SetPostfixThemeString()
        {
            // Detect dark/light mode
            int theme = GetCurrentTheme();

            if (theme == 1)
            {
                postfixTheme = @"-dark\";
            }
            if (theme == 0)
            {
                postfixTheme = @"-light\";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetPostfixThemeString();

            //
            webView21.EnsureCoreWebView2Async();

            XMLWorker xmlWorker = new XMLWorker();
            options = new TwOptions();
            options = xmlWorker.LoadConfig("city.conf");
            textBox1.Text = options.key;
            textBox2.Text = options.q;
            textBox3.Text = options.rph;
            if(options.dhi != null)
            {
                globalHostIndex = Int32.Parse(options.dhi);
            }

            trayIcon.Text = options.key;
            this.ShowInTaskbar = false;
            this.Hide();
            hidden = true;

            minutes = SetInformation(options.rph);

            xmlWorker.LoadConfig2("hosts.conf", out twHostsList);

            //
            /*twHosts = twHostsList[globalHostIndex]; // for each? if temperature is still =="" then run next

            string url = twHosts.hst.Replace("%HST%", options.key);
            if (twHosts.end != null)
            {
                url += twHosts.end.Replace("%END%", options.q);
            }

            // modify EH
            //webView21.NavigationCompleted += new EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs>((sender, e) => webView21_NavigationCompleted(sender, e, twHosts));
            */
            //webView21.Source = new Uri(url, UriKind.Absolute); // setting this property (to a different value) is equivalent to calling the Navigate(String) method
            
            webView21.Source = FormatHosts(globalHostIndex);

            SetComboBoxItems();
            comboBoxHosts.SelectedText = twHostsList[globalHostIndex].trm;

            // modify EH
            //myTimer.Tick += new EventHandler((sender, e) => TimerEventCtrl(sender, e, ref statusChecker));
            myTimer.Tick += TimerEventCtrl;

            // Sets the timer interval to 2 sec and then from config
            myTimer.Interval = 2000;
            myTimer.Start();

            trayIcon.Text = twHosts.trm + "\n" + options.key;
        }

        //private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e, TwHosts twHosts)

        private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string myScript = string.Empty;
            string webData = string.Empty;
            switch (twHosts.trm)
            {
                case "MAILRU":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°').Replace('−', '-');
                    break;

                case "YANDEX":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[1].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+').Replace('−', '-');
                    break;

                case "GISMETEO":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[1].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+').Replace('−', '-');
                    break;

                case "METEOPROG":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°').Replace('−', '-');
                    break;

                default:
                    //temperature = webData;
                    break;
            }

            if (firstTimeNavigationCompleted)
            {
                SetTrayDegree();
                firstTimeNavigationCompleted = false;
            }
            /*int x = 0;
            Int32.TryParse(webData, out x);*/
        }

        private void TimerEventCtrl(Object myObject, EventArgs myEventArgs/*, ref StatusChecker sch*/)
        {
            myTimer.Stop();

            if (firstTimeTimerRaised)
            {
                SetTimerInterval(minutes);
                firstTimeTimerRaised = false;
            }

            if (!firstTimeNavigationCompleted)
            {
                webView21.CoreWebView2.Reload();
                SetTrayDegree();
            }

            myTimer.Enabled = true;
        }

        private void SetTimerInterval(int Minutes)
        {
            //                 sec    min
            myTimer.Interval = 1000 * 60 * Minutes;
        }

        private void SetTrayDegree()
        {
            if (temperature != null || temperature != "")
            {
                LogWriter lw = new LogWriter();
                lw.LogWrite($" {twHosts.trm} {options.key} Температура = {temperature} °C ");

                string s = string.Empty;
                if (!(temperature.StartsWith("-"))) // if T >= 0
                {
                    s = "+";
                }
                string IcoFullName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + s + temperature + ".ico";
                //MessageBox.Show(IcoFullName);
                trayIcon.Icon = new System.Drawing.Icon(IcoFullName);
            }
            else
            {
                string IcoFullName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + "error1.ico";
                //MessageBox.Show(IcoFullName);
                trayIcon.Icon = new System.Drawing.Icon(IcoFullName);
            }

        }

        void GetWetherNow(object sender, EventArgs e)
        {
            SetTrayDegree();
        }

        void Setup(object sender, EventArgs e)
        {
            if (hidden)
            {
                this.ControlBox = false;
                this.Show();
                this.BringToFront();
                hidden = false;
            }
            else
            {
                this.Hide();
                hidden = true;
            }
        }

        void ViewLog(object sender, EventArgs e)
        {
            string pathToFile = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt";
            Process.Start("notepad.exe", pathToFile);
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void trayIcon_Click(object sender, EventArgs e)
        {
            var eventArgs = e as MouseEventArgs;
            switch (eventArgs.Button)
            {
                // Left click to reactivate
                case MouseButtons.Left:
                    //
                    break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkTextBoxesTextChanged() || bComboBoxHostsChanged)
            {
                DialogResult = MessageBox.Show("Сохранить конфиг?", "Опции были изменены", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (DialogResult == DialogResult.Yes)
                {
                    options.SetAll(textBox1.Text, textBox2.Text, textBox3.Text, globalHostIndex.ToString());
                    XMLWorker xmlWorker = new XMLWorker();
                    xmlWorker.SaveConfig("city.conf", options);
                }
                bComboBoxHostsChanged = false;
            }
            Setup(sender, e); //hide form
        }

        private bool checkTextBoxesTextChanged()
        {
            if (String.Equals(options.key, textBox1.Text) &&
            String.Equals(options.q, textBox2.Text) &&
               String.Equals(options.rph, textBox3.Text))
            {
                return false;
            }
            return true;
        }

        private int SetInformation(string s)
        {
            int countPerHour = 1;
            Int32.TryParse(s, out countPerHour);
            int runs1 = 24 * countPerHour;

            if (countPerHour != 0)
            {
                int runs2 = 60 / countPerHour;
                label4.Text = $"Всего в сутки: {runs1} раз (каждые {runs2} минут)";
                LogWriter logWriter = new LogWriter();
                logWriter.LogWrite($" SetInformation(): Срабатывание таймера каждые {runs2} минут");
                return runs2;
            }
            return 15;
        }

        private void SetComboBoxItems()
        {
            comboBoxHosts.Items.Clear();
            for (int i = 0; i < twHostsList.Count; i++)
            {
                comboBoxHosts.Items.Add(twHostsList[i].trm);
            }
        }

        private void comboBoxHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            globalHostIndex = comboBoxHosts.SelectedIndex;
            //
            bComboBoxHostsChanged = true;
            webView21.CoreWebView2.Navigate( FormatHosts(globalHostIndex).ToString() );
            //
            trayIcon.Text = twHosts.trm + "\n" + options.key;
        }
    }
}
