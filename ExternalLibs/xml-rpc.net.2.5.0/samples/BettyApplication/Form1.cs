using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Windows.Forms;

using CookComputing.XmlRpc;

namespace WindowsApplication4
{
  /// <summary>
  /// Summary description for Form1.
  /// </summary>
  public class Form1 : System.Windows.Forms.Form
  {
    private System.Windows.Forms.TextBox txtStateNumber;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button butGetStateName;
    private System.Windows.Forms.Label labStateName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtStateNumber1;
    private System.Windows.Forms.TextBox txtStateNumber2;
    private System.Windows.Forms.Button butGetStateNames;
    private System.Windows.Forms.TextBox txtStateNumber3;
    private System.Windows.Forms.Label labStateNames1;
    private System.Windows.Forms.Label labStateNames2;
    private System.Windows.Forms.Label labStateNames3;

    public Form1()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      if( disposing )
      {
      }
      base.Dispose( disposing );
    }

		#region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.labStateName = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.butGetStateName = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.labStateNames3 = new System.Windows.Forms.Label();
      this.labStateNames2 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.txtStateNumber3 = new System.Windows.Forms.TextBox();
      this.txtStateNumber2 = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtStateNumber1 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.labStateNames1 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.butGetStateNames = new System.Windows.Forms.Button();
      this.txtStateNumber = new System.Windows.Forms.TextBox();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // labStateName
      // 
      this.labStateName.Location = new System.Drawing.Point(105, 48);
      this.labStateName.Name = "labStateName";
      this.labStateName.Size = new System.Drawing.Size(113, 16);
      this.labStateName.TabIndex = 1;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.butGetStateName,
                                                                            this.labStateName,
                                                                            this.label2,
                                                                            this.label1});
      this.groupBox1.Location = new System.Drawing.Point(8, 8);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(247, 73);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "examples.getStateName";
      // 
      // butGetStateName
      // 
      this.butGetStateName.Location = new System.Drawing.Point(167, 21);
      this.butGetStateName.Name = "butGetStateName";
      this.butGetStateName.Size = new System.Drawing.Size(68, 21);
      this.butGetStateName.TabIndex = 2;
      this.butGetStateName.Text = "Get Name";
      this.butGetStateName.Click += new System.EventHandler(this.butGetStateName_Click);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(80, 16);
      this.label2.TabIndex = 0;
      this.label2.Text = "State Name:";
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(16, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(80, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "State Number:";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.labStateNames3,
                                                                            this.labStateNames2,
                                                                            this.label5,
                                                                            this.txtStateNumber3,
                                                                            this.txtStateNumber2,
                                                                            this.label4,
                                                                            this.txtStateNumber1,
                                                                            this.label3,
                                                                            this.labStateNames1,
                                                                            this.label6,
                                                                            this.butGetStateNames});
      this.groupBox2.Location = new System.Drawing.Point(13, 95);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(241, 193);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "examples.getStateStruct";
      // 
      // labStateNames3
      // 
      this.labStateNames3.Location = new System.Drawing.Point(98, 164);
      this.labStateNames3.Name = "labStateNames3";
      this.labStateNames3.Size = new System.Drawing.Size(113, 16);
      this.labStateNames3.TabIndex = 1;
      // 
      // labStateNames2
      // 
      this.labStateNames2.Location = new System.Drawing.Point(100, 140);
      this.labStateNames2.Name = "labStateNames2";
      this.labStateNames2.Size = new System.Drawing.Size(111, 16);
      this.labStateNames2.TabIndex = 1;
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(5, 87);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(88, 16);
      this.label5.TabIndex = 0;
      this.label5.Text = "State Number 3:";
      // 
      // txtStateNumber3
      // 
      this.txtStateNumber3.Location = new System.Drawing.Point(99, 85);
      this.txtStateNumber3.Name = "txtStateNumber3";
      this.txtStateNumber3.Size = new System.Drawing.Size(48, 20);
      this.txtStateNumber3.TabIndex = 5;
      this.txtStateNumber3.Text = "";
      // 
      // txtStateNumber2
      // 
      this.txtStateNumber2.Location = new System.Drawing.Point(99, 58);
      this.txtStateNumber2.Name = "txtStateNumber2";
      this.txtStateNumber2.Size = new System.Drawing.Size(48, 20);
      this.txtStateNumber2.TabIndex = 4;
      this.txtStateNumber2.Text = "";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(4, 60);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(89, 16);
      this.label4.TabIndex = 0;
      this.label4.Text = "State Number 2:";
      // 
      // txtStateNumber1
      // 
      this.txtStateNumber1.Location = new System.Drawing.Point(99, 29);
      this.txtStateNumber1.Name = "txtStateNumber1";
      this.txtStateNumber1.Size = new System.Drawing.Size(48, 20);
      this.txtStateNumber1.TabIndex = 3;
      this.txtStateNumber1.Text = "";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(3, 31);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(91, 16);
      this.label3.TabIndex = 0;
      this.label3.Text = "State Number 1:";
      // 
      // labStateNames1
      // 
      this.labStateNames1.Location = new System.Drawing.Point(99, 114);
      this.labStateNames1.Name = "labStateNames1";
      this.labStateNames1.Size = new System.Drawing.Size(113, 16);
      this.labStateNames1.TabIndex = 1;
      // 
      // label6
      // 
      this.label6.Location = new System.Drawing.Point(6, 114);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(80, 16);
      this.label6.TabIndex = 0;
      this.label6.Text = "State Names:";
      // 
      // butGetStateNames
      // 
      this.butGetStateNames.Location = new System.Drawing.Point(163, 33);
      this.butGetStateNames.Name = "butGetStateNames";
      this.butGetStateNames.Size = new System.Drawing.Size(71, 21);
      this.butGetStateNames.TabIndex = 6;
      this.butGetStateNames.Text = "Get Names";
      this.butGetStateNames.Click += new System.EventHandler(this.butGetStateNames_Click);
      // 
      // txtStateNumber
      // 
      this.txtStateNumber.Location = new System.Drawing.Point(111, 30);
      this.txtStateNumber.Name = "txtStateNumber";
      this.txtStateNumber.Size = new System.Drawing.Size(48, 20);
      this.txtStateNumber.TabIndex = 0;
      this.txtStateNumber.Text = "";
      // 
      // Form1
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(262, 295);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.groupBox2,
                                                                  this.txtStateNumber,
                                                                  this.groupBox1});
      this.Name = "Form1";
      this.Text = "BettyApp";
      this.groupBox1.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() 
    {
      Application.Run(new Form1());
    }

    private void butGetStateName_Click(object sender, System.EventArgs e)
    {
      labStateName.Text = "";
      IStateName betty = XmlRpcProxyGen.Create<IStateName>();
      Cursor = Cursors.WaitCursor;
      try
      {
        int num = Convert.ToInt32(txtStateNumber.Text);
        labStateName.Text = betty.GetStateName(num);
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
      Cursor = Cursors.Default;
    }

    private void butGetStateNames_Click(object sender, System.EventArgs e)
    {
      labStateNames1.Text = labStateNames2.Text = labStateNames3.Text = "";
      IStateName betty 
        = (IStateName)XmlRpcProxyGen.Create(typeof(IStateName));
      StateStructRequest request;
      string retstr = "";
      Cursor = Cursors.WaitCursor;
      try
      {
        request.state1 = Convert.ToInt32(txtStateNumber1.Text);
        request.state2 = Convert.ToInt32(txtStateNumber2.Text);
        request.state3 = Convert.ToInt32(txtStateNumber3.Text);
        retstr = betty.GetStateNames(request);
        String[] names = retstr.Split(',');
        if (names.Length > 2)
          labStateNames3.Text = names[2];
        if (names.Length > 1)
          labStateNames2.Text = names[1];
        if (names.Length > 0)
          labStateNames1.Text = names[0];
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
      Cursor = Cursors.Default;
    }

    private void HandleException(Exception ex)
    {
      string msgBoxTitle = "Error";
      try
      {
        throw ex;
      }
      catch(XmlRpcFaultException fex)
      {
        MessageBox.Show("Fault Response: " + fex.FaultCode + " " 
          + fex.FaultString, msgBoxTitle,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch(WebException webEx)
      {
        MessageBox.Show("WebException: " + webEx.Message, msgBoxTitle,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (webEx.Response != null)
          webEx.Response.Close();
      }
      catch(Exception excep)
      {
        MessageBox.Show(excep.Message, msgBoxTitle,
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

  }
}
