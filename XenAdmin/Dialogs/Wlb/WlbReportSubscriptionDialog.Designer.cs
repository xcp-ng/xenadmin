namespace XenAdmin.Dialogs.Wlb
{
    partial class WlbReportSubscriptionDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WlbReportSubscriptionDialog));
            this.tableLayoutPanelSubscriptionName = new System.Windows.Forms.TableLayoutPanel();
            this.subNameTextBox = new System.Windows.Forms.TextBox();
            this.labelSubscriptionName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSubNameAsterisk = new System.Windows.Forms.Label();
            this.labelDataRange = new System.Windows.Forms.Label();
            this.labelEmailTo = new System.Windows.Forms.Label();
            this.emailToTextBox = new System.Windows.Forms.TextBox();
            this.labelEmailCc = new System.Windows.Forms.Label();
            this.emailCcTextBox = new System.Windows.Forms.TextBox();
            this.labelEmailBcc = new System.Windows.Forms.Label();
            this.emailBccTextBox = new System.Windows.Forms.TextBox();
            this.labelEmailAddressFormat = new System.Windows.Forms.Label();
            this.labelEmailReplyTo = new System.Windows.Forms.Label();
            this.emailReplyTextBox = new System.Windows.Forms.TextBox();
            this.labelEmailSubject = new System.Windows.Forms.Label();
            this.emailSubjectTextBox = new System.Windows.Forms.TextBox();
            this.labelReportRenderFormat = new System.Windows.Forms.Label();
            this.labelEmailComment = new System.Windows.Forms.Label();
            this.emailCommentRichTextBox = new System.Windows.Forms.RichTextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupReportParameters = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanelRpParams = new System.Windows.Forms.TableLayoutPanel();
            this.rpParamComboBox = new System.Windows.Forms.ComboBox();
            this.dateTimePickerSchedStart = new System.Windows.Forms.DateTimePicker();
            this.labelEnd = new System.Windows.Forms.Label();
            this.labelStarting = new System.Windows.Forms.Label();
            this.groupDeliveryOptions = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanelDeliveryOptions = new System.Windows.Forms.TableLayoutPanel();
            this.labelToAsterisk = new System.Windows.Forms.Label();
            this.labelReplyToAsterisk = new System.Windows.Forms.Label();
            this.rpRenderComboBox = new System.Windows.Forms.ComboBox();
            this.groupScheduleOptions = new XenAdmin.Controls.DecentGroupBox();
            this.tableLayoutPanelSchOptions = new System.Windows.Forms.TableLayoutPanel();
            this.dateTimePickerSubscriptionRunTime = new System.Windows.Forms.DateTimePicker();
            this.labelRunAt = new System.Windows.Forms.Label();
            this.schedDeliverComboBox = new System.Windows.Forms.ComboBox();
            this.labelDeliverOn = new System.Windows.Forms.Label();
            this.dateTimePickerSchedEnd = new System.Windows.Forms.DateTimePicker();
            this.tableLayoutPanelSubscriptionName.SuspendLayout();
            this.groupReportParameters.SuspendLayout();
            this.tableLayoutPanelRpParams.SuspendLayout();
            this.groupDeliveryOptions.SuspendLayout();
            this.tableLayoutPanelDeliveryOptions.SuspendLayout();
            this.groupScheduleOptions.SuspendLayout();
            this.tableLayoutPanelSchOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelSubscriptionName
            // 
            resources.ApplyResources(this.tableLayoutPanelSubscriptionName, "tableLayoutPanelSubscriptionName");
            this.tableLayoutPanelSubscriptionName.Controls.Add(this.subNameTextBox, 1, 1);
            this.tableLayoutPanelSubscriptionName.Controls.Add(this.labelSubscriptionName, 0, 1);
            this.tableLayoutPanelSubscriptionName.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelSubscriptionName.Controls.Add(this.labelSubNameAsterisk, 2, 1);
            this.tableLayoutPanelSubscriptionName.Name = "tableLayoutPanelSubscriptionName";
            // 
            // subNameTextBox
            // 
            resources.ApplyResources(this.subNameTextBox, "subNameTextBox");
            this.subNameTextBox.Name = "subNameTextBox";
            // 
            // labelSubscriptionName
            // 
            resources.ApplyResources(this.labelSubscriptionName, "labelSubscriptionName");
            this.labelSubscriptionName.Name = "labelSubscriptionName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanelSubscriptionName.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // labelSubNameAsterisk
            // 
            this.labelSubNameAsterisk.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelSubNameAsterisk, "labelSubNameAsterisk");
            this.labelSubNameAsterisk.ForeColor = System.Drawing.Color.White;
            this.labelSubNameAsterisk.Image = global::XenAdmin.Properties.Resources.asterisk;
            this.labelSubNameAsterisk.Name = "labelSubNameAsterisk";
            // 
            // labelDataRange
            // 
            resources.ApplyResources(this.labelDataRange, "labelDataRange");
            this.labelDataRange.Name = "labelDataRange";
            // 
            // labelEmailTo
            // 
            resources.ApplyResources(this.labelEmailTo, "labelEmailTo");
            this.labelEmailTo.Name = "labelEmailTo";
            // 
            // emailToTextBox
            // 
            resources.ApplyResources(this.emailToTextBox, "emailToTextBox");
            this.emailToTextBox.Name = "emailToTextBox";
            // 
            // labelEmailCc
            // 
            resources.ApplyResources(this.labelEmailCc, "labelEmailCc");
            this.labelEmailCc.Name = "labelEmailCc";
            // 
            // emailCcTextBox
            // 
            resources.ApplyResources(this.emailCcTextBox, "emailCcTextBox");
            this.emailCcTextBox.Name = "emailCcTextBox";
            // 
            // labelEmailBcc
            // 
            resources.ApplyResources(this.labelEmailBcc, "labelEmailBcc");
            this.labelEmailBcc.Name = "labelEmailBcc";
            // 
            // emailBccTextBox
            // 
            resources.ApplyResources(this.emailBccTextBox, "emailBccTextBox");
            this.emailBccTextBox.Name = "emailBccTextBox";
            // 
            // labelEmailAddressFormat
            // 
            resources.ApplyResources(this.labelEmailAddressFormat, "labelEmailAddressFormat");
            this.tableLayoutPanelDeliveryOptions.SetColumnSpan(this.labelEmailAddressFormat, 3);
            this.labelEmailAddressFormat.Name = "labelEmailAddressFormat";
            // 
            // labelEmailReplyTo
            // 
            resources.ApplyResources(this.labelEmailReplyTo, "labelEmailReplyTo");
            this.labelEmailReplyTo.Name = "labelEmailReplyTo";
            // 
            // emailReplyTextBox
            // 
            resources.ApplyResources(this.emailReplyTextBox, "emailReplyTextBox");
            this.emailReplyTextBox.Name = "emailReplyTextBox";
            // 
            // labelEmailSubject
            // 
            resources.ApplyResources(this.labelEmailSubject, "labelEmailSubject");
            this.labelEmailSubject.Name = "labelEmailSubject";
            // 
            // emailSubjectTextBox
            // 
            resources.ApplyResources(this.emailSubjectTextBox, "emailSubjectTextBox");
            this.emailSubjectTextBox.Name = "emailSubjectTextBox";
            // 
            // labelReportRenderFormat
            // 
            resources.ApplyResources(this.labelReportRenderFormat, "labelReportRenderFormat");
            this.labelReportRenderFormat.Name = "labelReportRenderFormat";
            // 
            // labelEmailComment
            // 
            resources.ApplyResources(this.labelEmailComment, "labelEmailComment");
            this.labelEmailComment.Name = "labelEmailComment";
            // 
            // emailCommentRichTextBox
            // 
            resources.ApplyResources(this.emailCommentRichTextBox, "emailCommentRichTextBox");
            this.emailCommentRichTextBox.Name = "emailCommentRichTextBox";
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // groupReportParameters
            // 
            resources.ApplyResources(this.groupReportParameters, "groupReportParameters");
            this.groupReportParameters.Controls.Add(this.tableLayoutPanelRpParams);
            this.groupReportParameters.Name = "groupReportParameters";
            this.groupReportParameters.TabStop = false;
            // 
            // tableLayoutPanelRpParams
            // 
            resources.ApplyResources(this.tableLayoutPanelRpParams, "tableLayoutPanelRpParams");
            this.tableLayoutPanelRpParams.Controls.Add(this.rpParamComboBox, 1, 0);
            this.tableLayoutPanelRpParams.Controls.Add(this.labelDataRange, 0, 0);
            this.tableLayoutPanelRpParams.Name = "tableLayoutPanelRpParams";
            // 
            // rpParamComboBox
            // 
            this.rpParamComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.rpParamComboBox, "rpParamComboBox");
            this.rpParamComboBox.FormattingEnabled = true;
            this.rpParamComboBox.Items.AddRange(new object[] {
            resources.GetString("rpParamComboBox.Items"),
            resources.GetString("rpParamComboBox.Items1"),
            resources.GetString("rpParamComboBox.Items2"),
            resources.GetString("rpParamComboBox.Items3")});
            this.rpParamComboBox.Name = "rpParamComboBox";
            this.rpParamComboBox.SelectedIndexChanged += new System.EventHandler(this.rpParamComboBox_SelectedIndexChanged);
            // 
            // dateTimePickerSchedStart
            // 
            resources.ApplyResources(this.dateTimePickerSchedStart, "dateTimePickerSchedStart");
            this.dateTimePickerSchedStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSchedStart.MinDate = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerSchedStart.Name = "dateTimePickerSchedStart";
            this.dateTimePickerSchedStart.ValueChanged += new System.EventHandler(this.dateTimePickerSchedStart_ValueChanged);
            // 
            // labelEnd
            // 
            resources.ApplyResources(this.labelEnd, "labelEnd");
            this.labelEnd.Name = "labelEnd";
            // 
            // labelStarting
            // 
            resources.ApplyResources(this.labelStarting, "labelStarting");
            this.labelStarting.Name = "labelStarting";
            // 
            // groupDeliveryOptions
            // 
            resources.ApplyResources(this.groupDeliveryOptions, "groupDeliveryOptions");
            this.groupDeliveryOptions.Controls.Add(this.tableLayoutPanelDeliveryOptions);
            this.groupDeliveryOptions.Name = "groupDeliveryOptions";
            this.groupDeliveryOptions.TabStop = false;
            // 
            // tableLayoutPanelDeliveryOptions
            // 
            resources.ApplyResources(this.tableLayoutPanelDeliveryOptions, "tableLayoutPanelDeliveryOptions");
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailAddressFormat, 0, 0);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailTo, 0, 1);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailToTextBox, 1, 1);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelToAsterisk, 2, 1);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailCc, 0, 2);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailCcTextBox, 1, 2);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailBcc, 0, 3);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailBccTextBox, 1, 3);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailReplyTo, 0, 4);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailReplyTextBox, 1, 4);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelReplyToAsterisk, 2, 4);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailSubject, 0, 5);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailSubjectTextBox, 1, 5);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelReportRenderFormat, 0, 6);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.rpRenderComboBox, 1, 6);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.labelEmailComment, 0, 7);
            this.tableLayoutPanelDeliveryOptions.Controls.Add(this.emailCommentRichTextBox, 1, 7);
            this.tableLayoutPanelDeliveryOptions.Name = "tableLayoutPanelDeliveryOptions";
            // 
            // labelToAsterisk
            // 
            this.labelToAsterisk.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelToAsterisk, "labelToAsterisk");
            this.labelToAsterisk.ForeColor = System.Drawing.Color.White;
            this.labelToAsterisk.Image = global::XenAdmin.Properties.Resources.asterisk;
            this.labelToAsterisk.Name = "labelToAsterisk";
            // 
            // labelReplyToAsterisk
            // 
            resources.ApplyResources(this.labelReplyToAsterisk, "labelReplyToAsterisk");
            this.labelReplyToAsterisk.Image = global::XenAdmin.Properties.Resources.asterisk;
            this.labelReplyToAsterisk.Name = "labelReplyToAsterisk";
            // 
            // rpRenderComboBox
            // 
            resources.ApplyResources(this.rpRenderComboBox, "rpRenderComboBox");
            this.rpRenderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rpRenderComboBox.FormattingEnabled = true;
            this.rpRenderComboBox.Items.AddRange(new object[] {
            resources.GetString("rpRenderComboBox.Items"),
            resources.GetString("rpRenderComboBox.Items1")});
            this.rpRenderComboBox.Name = "rpRenderComboBox";
            // 
            // groupScheduleOptions
            // 
            resources.ApplyResources(this.groupScheduleOptions, "groupScheduleOptions");
            this.groupScheduleOptions.Controls.Add(this.tableLayoutPanelSchOptions);
            this.groupScheduleOptions.Name = "groupScheduleOptions";
            this.groupScheduleOptions.TabStop = false;
            // 
            // tableLayoutPanelSchOptions
            // 
            resources.ApplyResources(this.tableLayoutPanelSchOptions, "tableLayoutPanelSchOptions");
            this.tableLayoutPanelSchOptions.Controls.Add(this.dateTimePickerSubscriptionRunTime, 1, 0);
            this.tableLayoutPanelSchOptions.Controls.Add(this.labelRunAt, 0, 0);
            this.tableLayoutPanelSchOptions.Controls.Add(this.labelStarting, 0, 1);
            this.tableLayoutPanelSchOptions.Controls.Add(this.dateTimePickerSchedStart, 1, 1);
            this.tableLayoutPanelSchOptions.Controls.Add(this.schedDeliverComboBox, 4, 0);
            this.tableLayoutPanelSchOptions.Controls.Add(this.labelDeliverOn, 3, 0);
            this.tableLayoutPanelSchOptions.Controls.Add(this.dateTimePickerSchedEnd, 4, 1);
            this.tableLayoutPanelSchOptions.Controls.Add(this.labelEnd, 3, 1);
            this.tableLayoutPanelSchOptions.Name = "tableLayoutPanelSchOptions";
            // 
            // dateTimePickerSubscriptionRunTime
            // 
            resources.ApplyResources(this.dateTimePickerSubscriptionRunTime, "dateTimePickerSubscriptionRunTime");
            this.dateTimePickerSubscriptionRunTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSubscriptionRunTime.Name = "dateTimePickerSubscriptionRunTime";
            this.dateTimePickerSubscriptionRunTime.ShowUpDown = true;
            // 
            // labelRunAt
            // 
            resources.ApplyResources(this.labelRunAt, "labelRunAt");
            this.labelRunAt.Name = "labelRunAt";
            // 
            // schedDeliverComboBox
            // 
            this.schedDeliverComboBox.AllowDrop = true;
            resources.ApplyResources(this.schedDeliverComboBox, "schedDeliverComboBox");
            this.schedDeliverComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.schedDeliverComboBox.FormattingEnabled = true;
            this.schedDeliverComboBox.MaximumSize = new System.Drawing.Size(124, 0);
            this.schedDeliverComboBox.Name = "schedDeliverComboBox";
            // 
            // labelDeliverOn
            // 
            resources.ApplyResources(this.labelDeliverOn, "labelDeliverOn");
            this.labelDeliverOn.Name = "labelDeliverOn";
            // 
            // dateTimePickerSchedEnd
            // 
            resources.ApplyResources(this.dateTimePickerSchedEnd, "dateTimePickerSchedEnd");
            this.dateTimePickerSchedEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerSchedEnd.MaximumSize = new System.Drawing.Size(124, 0);
            this.dateTimePickerSchedEnd.MinDate = new System.DateTime(2008, 1, 1, 0, 0, 0, 0);
            this.dateTimePickerSchedEnd.MinimumSize = new System.Drawing.Size(0, 21);
            this.dateTimePickerSchedEnd.Name = "dateTimePickerSchedEnd";
            this.dateTimePickerSchedEnd.ValueChanged += new System.EventHandler(this.dateTimePickerSchedEnd_ValueChanged);
            // 
            // WlbReportSubscriptionDialog
            // 
            this.AcceptButton = this.saveButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tableLayoutPanelSubscriptionName);
            this.Controls.Add(this.groupScheduleOptions);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupDeliveryOptions);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupReportParameters);
            this.Name = "WlbReportSubscriptionDialog";
            this.tableLayoutPanelSubscriptionName.ResumeLayout(false);
            this.tableLayoutPanelSubscriptionName.PerformLayout();
            this.groupReportParameters.ResumeLayout(false);
            this.tableLayoutPanelRpParams.ResumeLayout(false);
            this.tableLayoutPanelRpParams.PerformLayout();
            this.groupDeliveryOptions.ResumeLayout(false);
            this.tableLayoutPanelDeliveryOptions.ResumeLayout(false);
            this.tableLayoutPanelDeliveryOptions.PerformLayout();
            this.groupScheduleOptions.ResumeLayout(false);
            this.tableLayoutPanelSchOptions.ResumeLayout(false);
            this.tableLayoutPanelSchOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelSubscriptionName;
        private System.Windows.Forms.TextBox subNameTextBox;
        private System.Windows.Forms.Label labelDataRange;
        private System.Windows.Forms.Label labelEmailTo;
        private System.Windows.Forms.TextBox emailToTextBox;
        private System.Windows.Forms.Label labelEmailCc;
        private System.Windows.Forms.TextBox emailCcTextBox;
        private System.Windows.Forms.Label labelEmailBcc;
        private System.Windows.Forms.TextBox emailBccTextBox;
        private System.Windows.Forms.Label labelEmailAddressFormat;
        private System.Windows.Forms.Label labelEmailReplyTo;
        private System.Windows.Forms.TextBox emailReplyTextBox;
        private System.Windows.Forms.Label labelEmailSubject;
        private System.Windows.Forms.TextBox emailSubjectTextBox;
        private System.Windows.Forms.Label labelReportRenderFormat;
        private System.Windows.Forms.Label labelEmailComment;
        private System.Windows.Forms.RichTextBox emailCommentRichTextBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private XenAdmin.Controls.DecentGroupBox groupReportParameters;
        private System.Windows.Forms.ComboBox rpParamComboBox;
        private XenAdmin.Controls.DecentGroupBox groupDeliveryOptions;
        private System.Windows.Forms.ComboBox rpRenderComboBox;
        private XenAdmin.Controls.DecentGroupBox groupScheduleOptions;
        private System.Windows.Forms.Label labelEnd;
        private System.Windows.Forms.Label labelStarting;
        private System.Windows.Forms.Label labelDeliverOn;
        private System.Windows.Forms.DateTimePicker dateTimePickerSchedStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerSubscriptionRunTime;
        private System.Windows.Forms.Label labelRunAt;
        private System.Windows.Forms.Label labelToAsterisk;
        private System.Windows.Forms.Label labelReplyToAsterisk;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDeliveryOptions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRpParams;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSchOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox schedDeliverComboBox;
        private System.Windows.Forms.DateTimePicker dateTimePickerSchedEnd;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSubscriptionName;
        private System.Windows.Forms.Label labelSubNameAsterisk;
    }
}

