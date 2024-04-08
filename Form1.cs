﻿using Microsoft.Web.WebView2.Core;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Win32;
using System.Security.Policy;
using System.Windows.Forms;

namespace TrayWeather3
{
    public partial class Form1 : Form
    {
        private string AppName = "TrayWeather";

        private string versionPrg = "0.3";

        private string currentServerUrl = string.Empty;

        private bool CityPropChanged = false;

        private string postfixTheme = @"-dark\";

        private int globalHostIndex = 0;
        
        private int X;
        private int Y;

        private bool bComboBoxHostsChanged = false;

        private TwHosts? twHosts;

        private List<TwHosts>? twHostsList;

        private bool firstTimeNavigationCompleted = true;

        private bool firstTimeTimerRaised = true;

        private NotifyIcon trayIcon;

        private string temperature = string.Empty;

        private static int minutes = 0;

        private bool hidden = false;

        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        private TwOptions? options;

        private string iconFilename = Application.StartupPath + @"icons-light\+.ico";

        private MenuStrip leftClickedMenu;

        public Form1()
        {
            leftClickedMenu = new MenuStrip()
            {
                Items = { new ToolStripMenuItem("Браузер", null, openDetailsInBrowser) }
            };

            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                //Icon = new System.Drawing.Icon(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + "+.ico"),
                Icon = new System.Drawing.Icon(iconFilename),

                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items = { new ToolStripMenuItem("Получить данные", null, GetWetherNow),
                                new ToolStripMenuItem("Настройка", null, Setup),
                                new ToolStripMenuItem("Смотреть лог", null, ViewLog),
                                new ToolStripMenuItem("О программе", null, About),
                                new ToolStripMenuItem("Выход", null, Exit) }
                },
                Visible = true,
                Text = "Weather",
            };
            trayIcon.Click += trayIcon_Click;
            trayIcon.MouseUp += new MouseEventHandler(trayIcon_MouseUp);

            InitializeComponent();

            webView21.NavigationCompleted += webView21_NavigationCompleted;
        }

        void openDetailsInBrowser(object sender, EventArgs e)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = currentServerUrl,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private int GetCurrentTheme() // dark or light theme
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

        private void LoadIconPack()
        {
            //
        }

        private Uri FormatHosts(int hostIndex)
        {
            string url = string.Empty;

            if (twHostsList != null)
            {
                twHosts = twHostsList[hostIndex];
                url = twHosts.hst.Replace("%HST%", options.cnm); // options.cnm= city name
            }

            if (twHosts.end != null)
            {
                //url += twHosts.end.Replace($"%END{hostIndex}%", options.id1);  // options.ids = city numbers
                url += getCityIdByHostIndex(hostIndex);
            }
            currentServerUrl = url;

            Uri uri = new Uri(url, UriKind.Absolute);

            return uri;
        }

        private string getCityIdByHostIndex(int idx)
        {
            string ret = string.Empty;

            switch (idx)
            {
                case 2:
                    ret = twHosts?.end?.Replace($"%END{idx}%", options?.id1);
                    break;

                case 4:
                    ret = twHosts?.end?.Replace($"%END{idx}%", options?.id2);
                    break;

                case 5:
                    ret = twHosts?.end?.Replace($"%END{idx}%", options?.id3);
                    break;

                case 6:
                    ret = twHosts?.end?.Replace($"%END{idx}%", options?.idn);
                    break;

                default:
                    ret = string.Empty;
                    break;
            }
            return ret;
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
            options = xmlWorker.LoadConfig(Application.StartupPath + "city.conf");
            textBox1.Text = options.cnm;
            textBox2.Text = options.id1;
            textBox3.Text = options.rph;
            textBox4.Text = options.id2;
            textBox5.Text = options.id3;
            textBox6.Text = options.idn;
            //TODO: choose color = options.icl

            if (options.dhi != null)
            {
                globalHostIndex = Int32.Parse(options.dhi);
            }

            trayIcon.Text = options.cnm;
            this.ShowInTaskbar = false;
            this.Hide();
            hidden = true;

            minutes = SetInformation(options.rph);

            xmlWorker.LoadConfig2(Application.StartupPath + "hosts.conf", out twHostsList);

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

            SetComboBoxHostsItems();
            comboBoxHosts.SelectedText = twHostsList[globalHostIndex].trm;

            label9.Text = globalHostIndex.ToString() + " из " + twHostsList.Count().ToString();

            // modify EH
            //myTimer.Tick += new EventHandler((sender, e) => TimerEventCtrl(sender, e, ref statusChecker));
            myTimer.Tick += TimerEventCtrl;

            // Sets the timer interval to 2 sec and then from config
            myTimer.Interval = 2000;
            myTimer.Start();

            trayIcon.Text = twHosts.trm + "\n" + options.cnm;

            // Данные по автозагрузке
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk.GetValue(AppName) != null) checkBoxRegAutorun.Checked = true;
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
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+').Replace('−', '-');
                    break;

                case "METEOPROG":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°').Replace('−', '-');
                    break;

                case "ACCUWEATHER":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°', 'C').Replace('−', '-');
                    break;

                case "METEOVESTI":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°').Replace('−', '-');
                    break;

                case "RP5":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[1].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', ' ', '°', 'C').Replace('−', '-');
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
            if (temperature != "")
            {
                LogWriter lw = new LogWriter();
                string temp = temperature;

                if (String.Equals(temperature, "null"))
                {
                    temp = "Нет сети/данных";
                }
                lw.LogWrite($" {twHosts.trm} {options.cnm} Температура = {temp} °C ");

                string s = string.Empty;
                if (!(temperature.StartsWith("-"))) // if T >= 0
                {
                    s = "+";
                }
                string IcoFullPath = Application.StartupPath + "icons" + postfixTheme + s + temperature + ".ico";
                //string IcoFullName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + s + temperature + ".ico";
                //MessageBox.Show(IcoFullName);
                trayIcon.Icon = new System.Drawing.Icon(IcoFullPath);
            }
            else
            {
                string IcoFullPath = Application.StartupPath + "icons" + postfixTheme + "error1.ico";
                //string IcoFullName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + "error1.ico";
                //MessageBox.Show(IcoFullName);
                trayIcon.Icon = new System.Drawing.Icon(IcoFullPath);
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
            string pathToFile = Application.StartupPath + "log.txt";
            Process.Start("notepad.exe", pathToFile);
        }

        void About(object sender, EventArgs e)
        {
            var isDebuggerAttached = System.Diagnostics.Debugger.IsAttached;
            string dbgq = string.Empty;

            if (isDebuggerAttached)
            {
                dbgq = "\n ___DEBUG_MODE___";
            }
            MessageBox.Show("Автор: Николаев Александр\n https://github.com/verelex\n trayweather@mynv.ru" + dbgq, $"{AppName} {versionPrg}");
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void trayIcon_Click(object sender, EventArgs e) // TODO: show msgbox - сводка информации
        {
            var eventArgs = e as MouseEventArgs;
            switch (eventArgs?.Button)
            {
                // Left click to reactivate
                case MouseButtons.Left:
                    //displayLocation = control.PointToScreen(position);
                    leftClickedMenu.Show();
                    break;
            }
        }

        private void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            X = e.X;
            Y = e.Y;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (checkTextBoxesTextChanged() || bComboBoxHostsChanged)
            {
                /*if(CityPropChanged == true)
                {
                    options.key = textBox1.Text;
                    options.q = textBox2.Text;
                    comboBoxHosts_SelectedIndexChanged(sender, e);
                }*/
                DialogResult = MessageBox.Show("Сохранить конфиг?", "Опции были изменены", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (DialogResult == DialogResult.Yes)
                {
                    options.SetAll(textBox1.Text,
                                   textBox2.Text,
                                   textBox4.Text,
                                   textBox5.Text,
                                   textBox6.Text,
                                   textBox3.Text,
                                   globalHostIndex.ToString(),
                                   "def");
                    XMLWorker xmlWorker = new XMLWorker();
                    xmlWorker.SaveConfig(Application.StartupPath + "city.conf", options);
                    //
                    webView21.CoreWebView2.Navigate(FormatHosts(globalHostIndex).ToString());
                    trayIcon.Text = twHosts.trm + "\n" + options.cnm;
                }
                bComboBoxHostsChanged = false;
                CityPropChanged = false;
            }
            Setup(sender, e); //hide form
        }

        private bool checkTextBoxesTextChanged()
        {
            if (String.Equals(options.cnm, textBox1.Text) &&
                String.Equals(options.id1, textBox2.Text) &&
                String.Equals(options.id2, textBox4.Text) &&
                String.Equals(options.id3, textBox5.Text) &&
                String.Equals(options.idn, textBox6.Text) &&
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

        private void SetComboBoxHostsItems()
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

            bComboBoxHostsChanged = true;

            label9.Text = (globalHostIndex+1).ToString() + " из " + twHostsList.Count().ToString();
        }

        private void checkBoxRegAutorun_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk != null)
            {
                if (checkBoxRegAutorun.Checked)
                    rk.SetValue(AppName, Application.ExecutablePath);
                else
                    rk.DeleteValue(AppName, false);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CityPropChanged = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxIcoColors.Enabled = true;
            comboBoxIcoColors.Items.Clear();
            comboBoxIcoColors.Items.Add("def");
            comboBoxIcoColors.Items.Add("red");
            comboBoxIcoColors.Items.Add("yellow");
            comboBoxIcoColors.SelectedIndex = 0;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxIcoColors.Enabled = false;
        }
    }
}
