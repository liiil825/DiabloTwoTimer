#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Settings;
partial class HotkeySettingsControl
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.grpHotkeys = new System.Windows.Forms.GroupBox();
        this.lblStartNext = new System.Windows.Forms.Label();
        this.txtStartNext = new System.Windows.Forms.TextBox();
        this.lblPause = new System.Windows.Forms.Label();
        this.txtPause = new System.Windows.Forms.TextBox();
        this.lblDeleteHistory = new System.Windows.Forms.Label();
        this.txtDeleteHistory = new System.Windows.Forms.TextBox();
        this.lblRecordLoot = new System.Windows.Forms.Label();
        this.txtRecordLoot = new System.Windows.Forms.TextBox();
        this.grpHotkeys.SuspendLayout();
        this.SuspendLayout();
        //
        // grpHotkeys
        //
        this.grpHotkeys.Controls.Add(this.txtRecordLoot);
        this.grpHotkeys.Controls.Add(this.lblRecordLoot);
        this.grpHotkeys.Controls.Add(this.txtDeleteHistory);
        this.grpHotkeys.Controls.Add(this.lblDeleteHistory);
        this.grpHotkeys.Controls.Add(this.txtPause);
        this.grpHotkeys.Controls.Add(this.lblPause);
        this.grpHotkeys.Controls.Add(this.txtStartNext);
        this.grpHotkeys.Controls.Add(this.lblStartNext);
        this.grpHotkeys.Location = new System.Drawing.Point(10, 10);
        this.grpHotkeys.Name = "grpHotkeys";
        this.grpHotkeys.Size = new System.Drawing.Size(330, 220);
        this.grpHotkeys.TabIndex = 0;
        this.grpHotkeys.TabStop = false;
        this.grpHotkeys.Text = "快捷键设置";
        //
        // lblStartNext
        //
        this.lblStartNext.AutoSize = true;
        this.lblStartNext.Location = new System.Drawing.Point(15, 34);
        this.lblStartNext.Name = "lblStartNext";
        this.lblStartNext.Size = new System.Drawing.Size(110, 20);
        this.lblStartNext.TabIndex = 0;
        this.lblStartNext.Text = "开始/下一局:";
        //
        // txtStartNext
        //
        this.txtStartNext.BackColor = ColorNormal;
        this.txtStartNext.Cursor = Cursors.Hand;
        this.txtStartNext.Location = new System.Drawing.Point(130, 30);
        this.txtStartNext.Name = "txtStartNext";
        this.txtStartNext.ReadOnly = true;
        this.txtStartNext.Size = new System.Drawing.Size(180, 27);
        this.txtStartNext.TabIndex = 1;
        this.txtStartNext.Tag = "StartNext";
        this.txtStartNext.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
        this.txtStartNext.Enter += new EventHandler(this.OnTextBoxEnter);
        this.txtStartNext.Leave += new EventHandler(this.OnTextBoxLeave);
        //
        // lblPause
        //
        this.lblPause.AutoSize = true;
        this.lblPause.Location = new System.Drawing.Point(15, 74);
        this.lblPause.Name = "lblPause";
        this.lblPause.Size = new System.Drawing.Size(110, 20);
        this.lblPause.TabIndex = 2;
        this.lblPause.Text = "暂停/恢复:";
        //
        // txtPause
        //
        this.txtPause.BackColor = ColorNormal;
        this.txtPause.Cursor = Cursors.Hand;
        this.txtPause.Location = new System.Drawing.Point(130, 70);
        this.txtPause.Name = "txtPause";
        this.txtPause.ReadOnly = true;
        this.txtPause.Size = new System.Drawing.Size(180, 27);
        this.txtPause.TabIndex = 3;
        this.txtPause.Tag = "Pause";
        this.txtPause.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
        this.txtPause.Enter += new EventHandler(this.OnTextBoxEnter);
        this.txtPause.Leave += new EventHandler(this.OnTextBoxLeave);
        //
        // lblDeleteHistory
        //
        this.lblDeleteHistory.AutoSize = true;
        this.lblDeleteHistory.Location = new System.Drawing.Point(15, 114);
        this.lblDeleteHistory.Name = "lblDeleteHistory";
        this.lblDeleteHistory.Size = new System.Drawing.Size(110, 20);
        this.lblDeleteHistory.TabIndex = 4;
        this.lblDeleteHistory.Text = "删除最后记录:";
        //
        // txtDeleteHistory
        //
        this.txtDeleteHistory.BackColor = ColorNormal;
        this.txtDeleteHistory.Cursor = Cursors.Hand;
        this.txtDeleteHistory.Location = new System.Drawing.Point(130, 110);
        this.txtDeleteHistory.Name = "txtDeleteHistory";
        this.txtDeleteHistory.ReadOnly = true;
        this.txtDeleteHistory.Size = new System.Drawing.Size(180, 27);
        this.txtDeleteHistory.TabIndex = 5;
        this.txtDeleteHistory.Tag = "Delete";
        this.txtDeleteHistory.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
        this.txtDeleteHistory.Enter += new EventHandler(this.OnTextBoxEnter);
        this.txtDeleteHistory.Leave += new EventHandler(this.OnTextBoxLeave);
        //
        // lblRecordLoot
        //
        this.lblRecordLoot.AutoSize = true;
        this.lblRecordLoot.Location = new System.Drawing.Point(15, 154);
        this.lblRecordLoot.Name = "lblRecordLoot";
        this.lblRecordLoot.Size = new System.Drawing.Size(110, 20);
        this.lblRecordLoot.TabIndex = 6;
        this.lblRecordLoot.Text = "记录掉落:";
        //
        // txtRecordLoot
        //
        this.txtRecordLoot.BackColor = ColorNormal;
        this.txtRecordLoot.Cursor = Cursors.Hand;
        this.txtRecordLoot.Location = new System.Drawing.Point(130, 150);
        this.txtRecordLoot.Name = "txtRecordLoot";
        this.txtRecordLoot.ReadOnly = true;
        this.txtRecordLoot.Size = new System.Drawing.Size(180, 27);
        this.txtRecordLoot.TabIndex = 7;
        this.txtRecordLoot.Tag = "Record";
        this.txtRecordLoot.KeyDown += new KeyEventHandler(this.OnHotkeyInput);
        this.txtRecordLoot.Enter += new EventHandler(this.OnTextBoxEnter);
        this.txtRecordLoot.Leave += new EventHandler(this.OnTextBoxLeave);
        //
        // HotkeySettingsControl
        //
        this.Controls.Add(this.grpHotkeys);
        this.Name = "HotkeySettingsControl";
        this.Size = new System.Drawing.Size(350, 250);
        this.grpHotkeys.ResumeLayout(false);
        this.grpHotkeys.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private GroupBox grpHotkeys;
    private Label lblStartNext;
    private TextBox txtStartNext;
    private Label lblPause;
    private TextBox txtPause;
    private Label lblDeleteHistory;
    private TextBox txtDeleteHistory;
    private Label lblRecordLoot;
    private TextBox txtRecordLoot;
}