using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Import_and_Export_XML_File
{
    public partial class Form1 : Form
    {

        DataSet ds = new DataSet();
        DataTable sumTable = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Select File",
                Filter = "XML File (*.xml)|*.xml|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ds.ReadXml(openFileDialog1.FileName);
                    DataTable calTable = ds.Tables[0];

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Model", typeof(string));
                    dt.Columns.Add("Cena_bez_DPH", typeof(decimal));
                    dt.Columns.Add("Cena_s_DPH", typeof(decimal));

                    sumTable.Columns.Clear();
                    sumTable.Rows.Clear();
                    sumTable.Columns.Add("Model", typeof(string));
                    sumTable.Columns.Add("Cena_bez_DPH", typeof(decimal));
                    sumTable.Columns.Add("Cena_s_DPH", typeof(decimal));

                    AggregateData(calTable);

                    dataGridView1.DataSource = sumTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load or process the file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AggregateData(DataTable calTable)
        {
            Dictionary<string, (double sumaBezDPH, double sumaSDPH)> sumsByModel = new Dictionary<string, (double, double)>();

            foreach (DataRow row in calTable.Rows)
            {
                string modelu = row["Model"].ToString();
                if (int.TryParse(row["DPH"].ToString(), out int dph) && double.TryParse(row["Cena"].ToString(), out double cenaBdph))
                {
                    double cenaSdph = cenaBdph * (1 + dph / 100.0);

                    if (!sumsByModel.ContainsKey(modelu))
                    {
                        sumsByModel[modelu] = (0, 0);
                    }

                    sumsByModel[modelu] = (sumsByModel[modelu].sumaBezDPH + cenaBdph, sumsByModel[modelu].sumaSDPH + cenaSdph);
                }
            }

            foreach (var modelSum in sumsByModel)
            {
                sumTable.Rows.Add(modelSum.Key, modelSum.Value.sumaBezDPH, modelSum.Value.sumaSDPH);
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save File";
            sfd.Filter = "XML File (*.xml)|*.xml";
            sfd.ShowDialog();

            sumTable.TableName = "Sale";
            sumTable.WriteXml(sfd.FileName);
            MessageBox.Show("The File is saved!");

        }
    }
}

#region original approach
/*
private void import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select File";
            openFileDialog1.Filter = "XML File (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.ShowDialog();

            ds.ReadXml(openFileDialog1.FileName);

            DataTable calTable = ds.Tables[0];
            DataTable dt = new DataTable(); 
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Cena_bez_DPH", typeof(decimal));
            dt.Columns.Add("Cena_s_DPH", typeof(decimal));

            Dictionary<string, (double sumaBezDPH, double sumaSDPH)> sumsByModel = new Dictionary<string, (double, double)>();

            foreach (DataRow row in calTable.Rows)
            {
                string modelu = row["Model"].ToString();
                int dph = Convert.ToInt32(row["DPH"]);
                double cenaBdph = Convert.ToDouble(row["Cena"]);
                double cenaSdph = cenaBdph + (cenaBdph * ((double)dph / 100));
                dt.Rows.Add(modelu, cenaSdph, cenaBdph);

                if (!sumsByModel.ContainsKey(modelu))
                {
                    sumsByModel[modelu] = (0, 0);
                }

                sumsByModel[modelu] = (sumsByModel[modelu].sumaBezDPH + cenaBdph, sumsByModel[modelu].sumaSDPH + cenaSdph);

                dt.Rows.Add(modelu, cenaBdph, cenaSdph);
            }

            sumTable.Columns.Add("Model", typeof(string));
            sumTable.Columns.Add("Cena_bez_DPH", typeof(decimal));
            sumTable.Columns.Add("Cena_s_DPH", typeof(decimal));

            foreach (var modelSum in sumsByModel)
            {
                sumTable.Rows.Add(modelSum.Key, modelSum.Value.sumaBezDPH, modelSum.Value.sumaSDPH);
            }

            dataGridView1.DataSource = sumTable; 
           
        }
 */

#endregion