using IPChecker.Properties;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Timers;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace IPChecker
{
    public partial class Checker : ServiceBase
    {
        public static string CurrentIP;

        public Checker()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LoadIP();
            StartInterval();
        }

        public void LoadIP()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(Settings.Default.RegistryPath);
                if (key == null)
                {
                    ApplySettings(GetExternalIP());
                    key = Registry.CurrentUser.OpenSubKey(Settings.Default.RegistryPath);
                }
                CurrentIP = key.GetValue(Settings.Default.RegistryIPProperty).ToString();
            }
            catch (Exception e)
            {
                WriteLog(e.Message);
            }
        }

        public void StartInterval()
        {
            var ServiceTimer = new Timer();
            ServiceTimer.Enabled = true;
            ServiceTimer.Interval = 60000 * Settings.Default.VerifyInterval;
            ServiceTimer.Elapsed += new ElapsedEventHandler(VerifyIP);
        }

        protected override void OnStop()
        {
        }

        public void VerifyIP(object sender, EventArgs e)
        {
            string publicIP = GetExternalIP();
            WriteLog(string.Format("Local IP: {0}. External IP: {1}", CurrentIP, publicIP));
            if (publicIP != CurrentIP)
            {
                SendMail("Your external IP address has changed", string.Format("Old IP: {0}. New IP: {1}", CurrentIP, publicIP));
                CurrentIP = publicIP;
                ApplySettings(CurrentIP);
            }
        }

        public string GetExternalIP()
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadString("https://api.ipify.org");
        }

        public void SendMail(string Subject, string Message)
        {
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("admin@skyweb.nu", "[S'p@HS,(iMVl|-]-vy@");
            client.EnableSsl = true;
            client.Port = 587;
            client.Host = "skyweb.nu";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("ip@skyweb.nu");
            mail.To.Add(Settings.Default.EmailTo);
            mail.Subject = Subject;
            mail.Body = Message;
            client.Send(mail);
        }

        public void WriteLog(string Text)
        {
            if (!EventLog.SourceExists("IPChecker"))
            {
                EventLog.CreateEventSource("IPChecker", "Application");
            }

            EventLog.WriteEntry("IPChecker", Text);
        }
        
        public void ApplySettings(string value)
        {
            try
            {
                var key = Registry.CurrentUser.CreateSubKey(Settings.Default.RegistryPath);
                key.SetValue(Settings.Default.RegistryIPProperty, value);
                key.Close();
            }
            catch (Exception e)
            {
                WriteLog(e.Message);
            }
        }

        public void ChangeDNS()
        {
            var x = new TransIPDomainService.DomainServicePortTypeClient();
            cookiecon
            x.ClientCredentials.
            var dns1 = new TransIPDomainService.DnsEntry();
            dns1.type = "";
            var y = new TransIPDomainService.DnsEntry[] { dns1 };
            x.setDnsEntries("skyweb.nu", y);
        }
    }
}
