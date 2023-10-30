using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Npgsql;

namespace RadSystems
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection connection;
        private string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12345;Database=TurFirma;";
        private DataTable dataTable = new DataTable();
        private NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter();
        private Form2 modal_window;
        private List<DataGridView> dataGridViews = new List<DataGridView>();
        private string[] schemas = { "tourist", "tourist_info", "tours", "seasons", "payment", "putevki" };
        private Dictionary<int[], int> tabPagesBybuttonTabIndexes = new Dictionary<int[], int>()
        {
            {new int[]{ 3, 4, 5 }, 0},
            {new int[]{ 7, 8, 9 }, 1},
            {new int[]{ 11, 12, 13 }, 2},
            {new int[]{ 15, 16, 17 }, 3},
            {new int[]{ 19, 20, 21 }, 4},
            {new int[]{ 22 }, 5}
        };

        public Form1()
        {
            InitializeComponent();
            appendDGV_toArray();
            connection = new NpgsqlConnection(connectionString);

            connection.Open();

            loadData();
        }

        private void appendDGV_toArray()
        {
            dataGridViews.Add(dataGridView1);
            dataGridViews.Add(dataGridView3);
            dataGridViews.Add(dataGridView4);
            dataGridViews.Add(dataGridView5);
            dataGridViews.Add(dataGridView6);
            dataGridViews.Add(dataGridView7);
        }

        //ОДИН ОБРАБОТЧИК ДЛЯ ВСЕХ ВКЛАДОК (switch case для всех вкладок и в зависимости от названия вкладки загружать соответствующие данные)
        private void loadData()
        {
            for (int i = 0; i < schemas.Length; i++)
            {
                dataTable = new DataTable();
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM " + schemas[i], connection);
                adapter.Fill(dataTable);
                dataGridViews[i].DataSource = dataTable;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            int currentTabPage = getCurrentTabPage(currentButton);
            
            modal_window = new Form2(button1.Text, this.tabControl1.TabPages[currentTabPage], dataGridViews[currentTabPage]);
            modal_window.ShowDialog();

            if (currentTabPage == 0)
            {
                dataTable.Clear();
                string sql = "SELECT * FROM putevki";
                dataAdapter.SelectCommand = new NpgsqlCommand(sql, connection);
                dataAdapter.Fill(dataTable);

                BindingSource bindingSource2 = new BindingSource();
                bindingSource2.DataSource = dataTable;

                dataGridView7.DataSource = bindingSource2;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            int currentTabPage = getCurrentTabPage(currentButton);

            modal_window = new Form2(button2.Text, this.tabControl1.TabPages[currentTabPage], dataGridViews[currentTabPage]);
            modal_window.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Button currentButton = (Button)sender;
            int currentTabPage = getCurrentTabPage(currentButton);

            modal_window = new Form2(button3.Text, this.tabControl1.TabPages[currentTabPage], dataGridViews[currentTabPage]);
            modal_window.ShowDialog();
            
        }

        private int getCurrentTabPage(Button currentButton)
        {
            int currentTabPage = 0;
            foreach (var key in tabPagesBybuttonTabIndexes.Keys)
                if (key.Contains(currentButton.TabIndex))
                    currentTabPage = tabPagesBybuttonTabIndexes[key];

            return currentTabPage;
        }

        //ЗАПРОСЫ (АГРЕГИРОВАННЫЕ И ПАРАМЕТРИЗОВАННЫЕ)
        private void button4_Click(object sender, EventArgs e)
        {
            fetchRowsFromQuery("Агрегированный запрос");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fetchRowsFromQuery("Параметризованный запрос");
        }

        private void fetchRowsFromQuery(string myString)
        {
            string sqlQuery;
            dataTable = new DataTable();
            switch (myString)
            {
                case "Агрегированный запрос":
                    sqlQuery = this.textBox1.Text;
                    break;
                case "Параметризованный запрос":
                    sqlQuery = this.richTextBox1.Text;
                    break;
                default: 
                    sqlQuery = "";
                    break;
            }   
            NpgsqlCommand command = new NpgsqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(sqlQuery, connection);
            adapter.Fill(dataTable);
            dataGridView2.DataSource = dataTable;
        }


        //ИМПОРТ И ЭКСПОРТ В ФОРМАТ XML

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            DataTable table = new DataTable();
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Visible)
                {
                    table.Columns.Add();
                }
            }
            object[] cellValues = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for(int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                table.Rows.Add(cellValues);
            }
            return table;
        }

        //экспорт XmlWriter

        private void button6_Click(object sender, EventArgs e)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            XmlWriter writer = XmlWriter.Create("exportWriter.xml", settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("export");
            for (int j = 0; j < dataGridView2.RowCount - 1; j++)
            {
                writer.WriteStartElement("record");
                writer.WriteString(j.ToString());
                for (int i = 0; i < dataGridView2.ColumnCount; i++)
                {
                    writer.WriteStartElement(dataGridView2.Columns[i].HeaderText);
                    writer.WriteString(dataGridView2.Rows[j].Cells[i].Value.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            MessageBox.Show("Данные были экспортированы в xml-файл", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            foreach (DataColumn column in dataTable.Columns)
            {
                Console.WriteLine(column.ColumnName);
            }
        }

        //экспорт XmlDocument
        private void button8_Click(object sender, EventArgs e)
        {
            XmlDocument document = new XmlDocument();
            XmlNode root = document.CreateElement("export");
            document.AppendChild(root);
            XmlNode record1 = document.CreateElement("record");
            record1.InnerText = 0.ToString();
            root.AppendChild(record1);
            XmlNode column1 = document.CreateElement("дата формирования");
            column1.InnerText = 2.ToString();
            record1.AppendChild(column1);
            for (int j = 0; j < dataGridView2.RowCount - 1; j++)
            {
                XmlNode record = document.CreateElement("record");
                record.InnerText = j.ToString();
                root.AppendChild(record);
                for (int i = 0; i < dataGridView2.ColumnCount; i++)
                {
                    XmlNode column = document.CreateElement(dataGridView2.Columns[i].HeaderText);
                    column.InnerText = dataGridView2.Rows[j].Cells[i].Value.ToString();
                    record.AppendChild(column);
                }
            }
            document.Save("exportDocument.xml");
            MessageBox.Show("Данные были экспортированы в xml-файл", "Выполнено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //импорт XmlReader

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(Path.GetExtension(openFileDialog.FileName));
                if (Path.GetExtension(openFileDialog.FileName) == ".xml")
                {
                    XmlReader reader = XmlReader.Create(openFileDialog.FileName);
                    DataSet ds = new DataSet();
                    ds.ReadXml(reader);
                    dataGridView2.DataSource = ds.Tables[0];
                }
            }
        }

        //импорт XmlDocument
        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(Path.GetExtension(openFileDialog.FileName));
                if (Path.GetExtension(openFileDialog.FileName) == ".xml")
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(openFileDialog.FileName);
                    XmlElement root = xmlDocument.DocumentElement;
                    DataTable dt = new DataTable();
                    foreach (XmlNode head in root.SelectSingleNode("record").ChildNodes)
                    {
                        if (head.Name != "#text")
                        {
                            dt.Columns.Add(head.Name);
                        }
                    }
                    foreach (XmlNode node in root.SelectNodes("record"))
                    {
                        DataRow row = dt.NewRow();
                        foreach (XmlNode node1 in node.ChildNodes)
                        {
                            if (node1.Name != "#text")
                            {
                                row[node1.Name] = node1.InnerText;
                            }
                        }
                        dt.Rows.Add(row);
                    }
                    dataGridView2.DataSource = dt;
                }
            }
        }

        private void button22_Click(object sender, EventArgs e)
        { 
            modal_window = new Form2("Добавить", this.tabControl1.TabPages[0], dataGridViews[0]);
            modal_window.ShowDialog();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        
    }
}
