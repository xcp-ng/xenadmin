using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;
using System.Web;

using CookComputing.XmlRpc;

namespace MathApp
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtArg1;
		private System.Windows.Forms.TextBox txtArg2;
		private System.Windows.Forms.Button butAdd;
		private System.Windows.Forms.Button butSubtract;
		private System.Windows.Forms.Button butMultiply;
		private System.Windows.Forms.Button butDivide;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labResult;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtUrl;
		private System.ComponentModel.IContainer components;
		private IMath mathProxy;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			mathProxy = XmlRpcProxyGen.Create<IMath>();
			txtUrl.Text = "http://www.cookcomputing.com/xmlrpcsamples/math.rem";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
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
			this.components = new System.ComponentModel.Container();
			this.txtArg2 = new System.Windows.Forms.TextBox();
			this.labResult = new System.Windows.Forms.Label();
			this.txtArg1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butMultiply = new System.Windows.Forms.Button();
			this.butSubtract = new System.Windows.Forms.Button();
			this.butDivide = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butAdd = new System.Windows.Forms.Button();
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtArg2
			// 
			this.txtArg2.Location = new System.Drawing.Point(72, 96);
			this.txtArg2.Name = "txtArg2";
			this.txtArg2.Size = new System.Drawing.Size(80, 20);
			this.txtArg2.TabIndex = 1;
			this.txtArg2.Text = "";
			// 
			// labResult
			// 
			this.labResult.Location = new System.Drawing.Point(16, 24);
			this.labResult.Name = "labResult";
			this.labResult.Size = new System.Drawing.Size(184, 32);
			this.labResult.TabIndex = 0;
			// 
			// txtArg1
			// 
			this.txtArg1.Location = new System.Drawing.Point(72, 56);
			this.txtArg1.Name = "txtArg1";
			this.txtArg1.Size = new System.Drawing.Size(80, 20);
			this.txtArg1.TabIndex = 1;
			this.txtArg1.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Arg 1:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 24);
			this.label2.TabIndex = 0;
			this.label2.Text = "Arg 2:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 24);
			this.label3.TabIndex = 4;
			this.label3.Text = "Url:";
			// 
			// butMultiply
			// 
			this.butMultiply.Location = new System.Drawing.Point(168, 96);
			this.butMultiply.Name = "butMultiply";
			this.butMultiply.Size = new System.Drawing.Size(56, 24);
			this.butMultiply.TabIndex = 2;
			this.butMultiply.Text = "Multiply";
			this.butMultiply.Click += new System.EventHandler(this.butMultiply_Click);
			// 
			// butSubtract
			// 
			this.butSubtract.Location = new System.Drawing.Point(240, 56);
			this.butSubtract.Name = "butSubtract";
			this.butSubtract.Size = new System.Drawing.Size(56, 24);
			this.butSubtract.TabIndex = 2;
			this.butSubtract.Text = "Subtract";
			this.butSubtract.Click += new System.EventHandler(this.butSubtract_Click);
			// 
			// butDivide
			// 
			this.butDivide.Location = new System.Drawing.Point(240, 96);
			this.butDivide.Name = "butDivide";
			this.butDivide.Size = new System.Drawing.Size(56, 24);
			this.butDivide.TabIndex = 2;
			this.butDivide.Text = "Divide";
			this.butDivide.Click += new System.EventHandler(this.butDivide_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labResult);
			this.groupBox1.Location = new System.Drawing.Point(16, 144);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(208, 64);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Result";
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(168, 56);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(56, 24);
			this.butAdd.TabIndex = 2;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// txtUrl
			// 
			this.txtUrl.Location = new System.Drawing.Point(72, 16);
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.Size = new System.Drawing.Size(224, 20);
			this.txtUrl.TabIndex = 1;
			this.txtUrl.Text = "";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 221);
			this.Controls.Add(this.txtUrl);
			this.Controls.Add(this.txtArg2);
			this.Controls.Add(this.txtArg1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDivide);
			this.Controls.Add(this.butMultiply);
			this.Controls.Add(this.butSubtract);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "MathApp";
			this.groupBox1.ResumeLayout(false);
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
			catch(XmlRpcServerException xmlRpcEx)
			{
				MessageBox.Show("XmlRpcServerException: " + xmlRpcEx.Message, 
					msgBoxTitle,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch(Exception defEx)
			{
				MessageBox.Show("Exception: " + defEx.Message, msgBoxTitle,
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e)
		{
      mathProxy.Url = txtUrl.Text;
			Cursor = Cursors.WaitCursor;
			try
			{
				labResult.Text = "";
				int a = Convert.ToInt32(txtArg1.Text);
				int b = Convert.ToInt32(txtArg2.Text);
				int result = mathProxy.Add(a, b);
				labResult.Text = txtArg1.Text + " + " + txtArg2.Text + " = "
					+ result.ToString();
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			Cursor = Cursors.Default;
		}

		private void butSubtract_Click(object sender, System.EventArgs e)
		{
      mathProxy.Url = txtUrl.Text;
			Cursor = Cursors.WaitCursor;
			try
			{
				labResult.Text = "";
				int a = Convert.ToInt32(txtArg1.Text);
				int b = Convert.ToInt32(txtArg2.Text);
				int result = mathProxy.Subtract(a, b);
				labResult.Text = txtArg1.Text + " - " + txtArg2.Text + " = "
					+ result.ToString();
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			Cursor = Cursors.Default;
		}

		private void butMultiply_Click(object sender, System.EventArgs e)
		{
      mathProxy.Url = txtUrl.Text;
			Cursor = Cursors.WaitCursor;
			try
			{
				labResult.Text = "";
				int a = Convert.ToInt32(txtArg1.Text);
				int b = Convert.ToInt32(txtArg2.Text);
				int result = mathProxy.Multiply(a, b);
				labResult.Text = txtArg1.Text + " * " + txtArg2.Text + " = "
					+ result.ToString();
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			Cursor = Cursors.Default;
		}

		private void butDivide_Click(object sender, System.EventArgs e)
		{
      mathProxy.Url = txtUrl.Text;
			Cursor = Cursors.WaitCursor;
			try
			{
				labResult.Text = "";
				int a = Convert.ToInt32(txtArg1.Text);
				int b = Convert.ToInt32(txtArg2.Text);
				int result = mathProxy.Divide(a, b);
				labResult.Text = txtArg1.Text + " / " + txtArg2.Text + " = "
					+ result.ToString();
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			Cursor = Cursors.Default;
		}
	}

	public interface IMath : IXmlRpcProxy
	{
		[XmlRpcMethod("math.Add")]
		int Add(int a, int b);

		[XmlRpcMethod("math.Subtract")]
		int Subtract(int a, int b);

		[XmlRpcMethod("math.Multiply")]
		int Multiply(int a, int b);

		[XmlRpcMethod("math.Divide")]
		int Divide(int a, int b);
	}
}
