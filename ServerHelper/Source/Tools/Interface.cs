using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace ServerHelper
{
    internal class Interface
    {
        internal static readonly NotifyIcon icon = new NotifyIcon();

        private const string path = @"C:\Users\mathi\OneDrive\Documents\Coding\ServerHelper\ServerHelper\AppIcon.ico";

        public static void InitIconMenu()
        {
            // icone
            Interface.icon.Icon = new Icon(path);
            Interface.icon.Visible = true;
            Interface.icon.Text = "Clic droit pour ouvrir";

            // menu
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripLabel titleMenuItem = new ToolStripLabel();
            ToolStripMenuItem logsMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem closeMenuItem = new ToolStripMenuItem();
            ToolStripSeparator separator = new ToolStripSeparator();

            menu.Items.AddRange(new ToolStripItem[] {titleMenuItem, separator, logsMenuItem, closeMenuItem});

            // title
            titleMenuItem.Text = "ServerHelper";
            titleMenuItem.Visible = true;
            titleMenuItem.Enabled = false;
            titleMenuItem.AutoSize = true;

            // separ    
            separator.AutoSize = true;

            // logs 
            logsMenuItem.Text = "Logs";
            logsMenuItem.Visible = true;
            logsMenuItem.Enabled = true;
            logsMenuItem.AutoSize = true;
            logsMenuItem.Click += new EventHandler(Interface.LogsButtonClick);

            // exit 
            closeMenuItem.Text = "Exit";
            closeMenuItem.Visible = true;
            closeMenuItem.Enabled = true;
            closeMenuItem.AutoSize = true;
            closeMenuItem.Click += new EventHandler(Interface.CloseButtonClick);
                
            // place le menu sur l'icône
            Interface.icon.ContextMenuStrip = menu;
            menu.ResumeLayout(false);

            // Enable to receive windows event
            Application.Run();
        }

        private static void CloseButtonClick (object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        private static void LogsButtonClick(object sender, EventArgs e)
        {
            Process.Start(ServerMain.LogsDir);
        }
    }
}
