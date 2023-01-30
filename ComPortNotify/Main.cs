using Microsoft.Win32;
using System.Diagnostics;

namespace ComPortNotify;
public partial class Main : Form {
    private NotifyIcon systemTrayIcon;
    private ComPortObserver? observer;





    public Main() {
        InitializeComponent();


        Load += (x, y) => {
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
        };


        systemTrayIcon = new NotifyIcon();
        systemTrayIcon.Visible = true;
        systemTrayIcon.Icon = this.Icon;

        var autorunMenuItem = new ToolStripMenuItem();
        autorunMenuItem.Text = "Autorun";
        autorunMenuItem.Click += AutorunClick;
        autorunMenuItem.Checked = IsAutorun();

        systemTrayIcon.ContextMenuStrip = new ContextMenuStrip();
        systemTrayIcon.ContextMenuStrip.Items.Add(autorunMenuItem);
        systemTrayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) => {
            systemTrayIcon.Visible = false;
            Application.Exit();
        });


        if (Process.GetProcesses().Count(x => x.ProcessName == Process.GetCurrentProcess().ProcessName) > 1) {
            Notify("Info", "The program is already running");
            systemTrayIcon.Visible = false;
            Process.GetCurrentProcess().Kill();
            return;
        }


        observer = new ComPortObserver();
        observer.onPortsChanged += PortsChanged;
    }





    private void PortsChanged(List<string> addPorts, List<string> removePorts) {
        var info = new List<string>();

        foreach (string port in removePorts) {
            info.Add($"- {port}");
        }

        foreach (string port in addPorts) {
            info.Add($"+ {port}");
        }

        Notify(
            "Ports changed", 
            string.Join(Environment.NewLine, info)
        );
    }





    private void Notify(string title, string text, int timeSec = 3, ToolTipIcon icon = ToolTipIcon.Info) {
        systemTrayIcon.BalloonTipIcon = icon;
        systemTrayIcon.BalloonTipTitle = title;
        systemTrayIcon.BalloonTipText = text;
        systemTrayIcon.ShowBalloonTip(1000 * timeSec);
    }





    private void AutorunClick(object? sender, EventArgs e) {
        if(sender is not ToolStripMenuItem autorunMenuItem) {
            return;
        }

        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        if (registryKey is null || appName is null) {
            return;
        }

        if (!autorunMenuItem.Checked) {
            registryKey.SetValue(appName, Application.ExecutablePath);
        } else {
            registryKey.DeleteValue(appName, false);
        }

        autorunMenuItem.Checked = IsAutorun();
    }





    private bool IsAutorun() {
        var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        if (registryKey is null) {
            return false;
        }

        return registryKey.GetValue(appName) is not null;
    }



}
