using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TXTextControl;
using TXTextControl.DocumentServer.Fields;

namespace tx_checkbox_mailmerge
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        DataSet ds;

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ds = new DataSet();
            ds.ReadXml("data.xml", XmlReadMode.Auto);

            mailMerge1.Merge(ds.Tables[0]);
        }

        private void mailMerge1_DataRowMerged(object sender, TXTextControl.DocumentServer.MailMerge.DataRowMergedEventArgs e)
        {
            e.MergedRow = processFormCheckBoxFields(e.MergedRow, ds.Tables[0].Rows[e.DataRowNumber]);
        }

        private byte[] processFormCheckBoxFields(byte[] document, DataRow dataRow)
        {
            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
            {
                tx.Create();
                tx.Load(document, TXTextControl.BinaryStreamType.InternalUnicodeFormat);

                foreach (IFormattedText textPart in tx.TextParts)
                {
                    foreach (ApplicationField appField in textPart.ApplicationFields)
                    {
                        if (appField.TypeName == "FORMCHECKBOX")
                        {
                            FormCheckBox checkbox = new FormCheckBox(appField);

                            if(dataRow.Table.Columns[checkbox.Name] != null)
                                checkbox.Checked = Convert.ToBoolean(dataRow[checkbox.Name]);
                        }
                    }
                }

                byte[] data;
                tx.Save(out data, BinaryStreamType.InternalUnicodeFormat);
                return data;
            }
        }
    }
}
