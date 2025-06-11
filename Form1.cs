using Microsoft.Web.WebView2.Core;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Win32;
using System.Security.Policy;
using System.Windows.Forms;
using System.Globalization;

namespace TrayWeather3
{
    public partial class Form1 : Form
    {
        private string AppName = "TrayWeather";

        private string versionPrg = "3.4";

        private string iconPackName = "default";

        private string currentServerUrl = string.Empty;

        private bool CityPropChanged = false;

        private string postfixTheme = @"-dark\";

        private int globalHostIndex = 0;

        private bool bComboBoxHostsChanged = false, bRadioButtonsChanged = false;

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

        private string iconFilename = Application.StartupPath + @"q.ico"; // startup unknown temperature

        public Form1()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(iconFilename),

                ContextMenuStrip = new ContextMenuStrip() // shows when Right clicked button
                {
                    Items = { new ToolStripMenuItem("Получить данные", null, GetWetherNow),
                                new ToolStripMenuItem("Настройка", null, Setup),
                                new ToolStripMenuItem("Браузер", null, openDetailsInBrowser),
                                new ToolStripMenuItem("Смотреть лог", null, ViewLog),
                                new ToolStripMenuItem("О программе", null, About),
                                new ToolStripMenuItem("Выход", null, Exit) }
                },
                Visible = true,
                Text = "Weather",
            };
            trayIcon.Click += trayIcon_Click;

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
                url = twHosts.hst.Replace("%HST%", options.GetElementByIndex(hostIndex));
            }

            if ( ! twHosts.end.Equals(string.Empty) ) // если добавочная строка не пустая
            {
                url += getCityIdByHostIndex(hostIndex);
            }
            currentServerUrl = url; // для последующего открытия в браузере через шелл екзекут

            Uri uri = new Uri(url, UriKind.Absolute);

            return uri;
        }

        private string getCityIdByHostIndex(int idx) // формируем вторую строку (окончание адреса)
        {
            string ret = string.Empty;
            string id = string.Empty;

            switch (idx)
            {
                case 2:
                    ret = twHosts.end; // gismeteo second string complete
                    break;

                case 4:
                    id = options?.id5; // accuweather only hack 
                    int value;
                    int.TryParse(string.Join("", id?.Where(c => char.IsDigit(c))), out value);
                    ret = twHosts?.end?.Replace($"%END{idx+1}%", value.ToString());
                    break;

                default:
                    ret = string.Empty;
                    break;
            }
            return ret;
        }

        private void SetPostfixThemeString()
        {
            if (iconPackName.Equals("default")) // using standard icons
            {
                // Detect dark/light mode
                int theme = GetCurrentTheme();

                if (theme == 1)
                {
                    postfixTheme = @"-dark\"; // describe path
                }
                if (theme == 0)
                {
                    postfixTheme = @"-light\";
                }
            }
            else // using custom icons
            {
                postfixTheme = $"-{iconPackName}\\";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webView21.EnsureCoreWebView2Async();

            loadConfigs();

            SetPostfixThemeString();

            trayIcon.Text = options.id1;
            this.ShowInTaskbar = false;
            this.Hide();
            hidden = true;

            minutes = SetInformation_WriteLog(options.rph);

            //xmlWorker.LoadConfig2(Application.StartupPath + "hosts.conf", out twHostsList);

            webView21.Source = FormatHosts(globalHostIndex);

            SetComboBoxHostsItems();

            //comboBoxHosts.SelectedText = twHostsList[globalHostIndex].rsn;

            setLabelInfoText();

            myTimer.Tick += TimerEventCtrl;

            // Sets the timer interval to 2 sec and then from config
            myTimer.Interval = 2000;
            myTimer.Start();

            setTrayIconText();

            ReadRegistry_SetCheckBoxAutorun();
        }

        private void ReadRegistry_SetCheckBoxAutorun() 
        {
            // Данные по автозагрузке
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk.GetValue(AppName) != null) checkBoxRegAutorun.Checked = true;
        }

        private void loadConfigs() 
        {
            XMLWorker xmlWorker = new XMLWorker();
            options = new TwOptions();
            options = xmlWorker.LoadConfig1(Application.StartupPath + "city.conf");
            textBoxCNM.Text = options.id1;
            textBoxCNY.Text = options.id2;
            textBox1.Text = options.id3;
            textBox6.Text = options.id4;
            textBox2.Text = options.id5;
            textBox3.Text = options.id6;
            textBox4.Text = options.id7;
            textBox5.Text = options.id8;
            textBoxRPH.Text = options.rph;
            if(!options.icl.Equals("default")) // if config have custom icon pack
            {
                iconPackName = options.icl;
            }

            if (options.dhi != null)
            {
                globalHostIndex = Int32.Parse(options.dhi);
            }

            xmlWorker.LoadConfig2(Application.StartupPath + "hosts.conf", out twHostsList);
        }

        private void setTrayIconText()
        {
            trayIcon.Text = twHosts.rsn + "\n" + options.id1;
        }

        private void setLabelInfoText()
        {
            labelInfo.Text = (globalHostIndex + 1).ToString() + " из " + twHostsList.Count().ToString();
        }

        private async void webView21_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            string myScript = string.Empty;
            string webData = string.Empty;

            /*switch (twHosts.rsn)
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

                case "OPENWEATHERMAP":
                    myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[0].innerText";
                    webData = await webView21.ExecuteScriptAsync(myScript);
                    temperature = webData.Trim('\"', '+', '°').Replace('−', '-');
                    break;

                default:
                    //temperature = webData;
                    break;
            }*/
            myScript = $"document.documentElement.getElementsByClassName('{twHosts.cls}')[{twHosts.eit}].innerText";
            webData = await webView21.ExecuteScriptAsync(myScript);
            temperature = webData.Trim(twHosts.tva.ToCharArray()).Replace('−', '-');

            if (temperature.IndexOf('.', StringComparison.CurrentCulture) != -1)
            {
                float f = float.Parse(temperature, CultureInfo.InvariantCulture.NumberFormat);
                int result = (int)Math.Round(f);
                temperature = result.ToString();
            }

            if (firstTimeNavigationCompleted)
            {
                SetTrayDegree();
                firstTimeNavigationCompleted = false;
            }
            
            if (CityPropChanged)
            {
                SetTrayDegree();
                CityPropChanged = false;
            }
        }

        private void TimerEventCtrl(Object myObject, EventArgs myEventArgs/*, ref StatusChecker sch*/)
        {
            myTimer.Stop();

            if (firstTimeTimerRaised)
            {
                SetTimerInterval(minutes);
                firstTimeTimerRaised = false;
            }

            if (!firstTimeNavigationCompleted) // если не первый раз
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
                lw.LogWrite($" {twHosts.rsn} {options.id1} Температура = {temp} °C ");

                string s = string.Empty;
                if (!(temperature.StartsWith("-"))) // if T >= 0
                {
                    s = "+";
                }

                // изменить: вынести общую часть за пределы if/else and +=

                string IcoFullPath = Application.StartupPath + "icons" + postfixTheme + s + temperature + ".ico";
                //string IcoFullName = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + @"\icons" + postfixTheme + s + temperature + ".ico";
                //MessageBox.Show(IcoFullName);
                trayIcon.Icon = new System.Drawing.Icon(IcoFullPath);
            }
            else
            {
                string IcoFullPath = Application.StartupPath + "err.ico";
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
            /*var eventArgs = e as MouseEventArgs;
            switch (eventArgs?.Button)
            {
                // Left click to reactivate
                case MouseButtons.Left:
                    //
                    break;
            }*/
        }

        private void button1_Click(object sender, EventArgs e) // close Options window
        {
            if (checkTextBoxesTextChanged() || bComboBoxHostsChanged || bRadioButtonsChanged)
            {
                DialogResult = MessageBox.Show("Сохранить конфиг?", "Опции были изменены", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (DialogResult == DialogResult.Yes)
                {
                    options.SetAll(textBoxCNM.Text,
                                   textBoxCNY.Text,
                                   textBox1.Text,
                                   textBox6.Text,
                                   textBox2.Text,
                                   textBox3.Text,
                                   textBox4.Text,
                                   textBox5.Text,
                                   textBoxRPH.Text,
                                   globalHostIndex.ToString(),
                                   iconPackName);
                    XMLWorker xmlWorker = new XMLWorker();
                    xmlWorker.SaveConfig1(Application.StartupPath + "city.conf", options);
                    //
                    webView21.CoreWebView2.Navigate(FormatHosts(globalHostIndex).ToString());
                    setTrayIconText();
                    minutes = SetInformation_WriteLog(options.rph);
                }
                bComboBoxHostsChanged = false;
                CityPropChanged = true;
            }
            Setup(sender, e); //hide form
        }

        private bool checkTextBoxesTextChanged()
        {
            if (String.Equals(options.id1, textBoxCNM.Text) &&
                String.Equals(options.id2, textBoxCNY.Text) &&
                String.Equals(options.id3, textBox1.Text) &&
                String.Equals(options.id4, textBox6.Text) &&
                String.Equals(options.id5, textBox2.Text) &&
                String.Equals(options.id6, textBox3.Text) &&
                String.Equals(options.id7, textBox4.Text) &&
                String.Equals(options.id8, textBox5.Text) &&
                String.Equals(options.rph, textBoxRPH.Text))
            {
                return false;
            }
            return true;
        }

        private int SetInformation_WriteLog(string s)
        {
            int countPerHour = 1;
            Int32.TryParse(s, out countPerHour);
            int runs1 = 24 * countPerHour;

            if (countPerHour != 0)
            {
                //int runs2 = 60 / countPerHour;
                double runs2 = Math.Round(60.0 / countPerHour, MidpointRounding.ToPositiveInfinity);

                label4.Text = $"Всего в сутки: {runs1} раз (каждые {runs2} минут)";
                LogWriter logWriter = new LogWriter();
                logWriter.LogWrite($" Срабатывание таймера каждые {runs2} минут");
                return (int)runs2;
            }
            return 15;
        }

        private void SetComboBoxHostsItems()
        {
            comboBoxHosts.Items.Clear();
            for (int i = 0; i < twHostsList.Count; i++)
            {
                comboBoxHosts.Items.Add(twHostsList[i].rsn);
            }
            comboBoxHosts.SelectedText = twHostsList[globalHostIndex].rsn;
        }

        private void comboBoxHosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            globalHostIndex = comboBoxHosts.SelectedIndex;

            bComboBoxHostsChanged = true;

            setLabelInfoText();
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

        private void textBoxCNM_TextChanged(object sender, EventArgs e) // убрать это
        {
            //CityPropChanged = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e) // custom icons
        {
            comboBoxIcoColors.Enabled = true;
            comboBoxIcoColors.Items.Clear();
            comboBoxIcoColors.Items.Add("default");
            comboBoxIcoColors.Items.Add("yellow");
            if (iconPackName.Equals("default"))
            {
                comboBoxIcoColors.SelectedIndex = 0; // standart icons
            }
            else
            {
                comboBoxIcoColors.SelectedIndex = 1; // custom icons
            }
            bRadioButtonsChanged = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e) // default icons
        {
            comboBoxIcoColors.Enabled = false; // иконки по умолчанию
            bRadioButtonsChanged = true;
        }
    }
}
