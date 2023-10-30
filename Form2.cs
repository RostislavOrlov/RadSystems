using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RadSystems.Form1;

namespace RadSystems
{
    public partial class Form2 : Form
    {
        private NpgsqlConnection connection;
        private string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=12345;Database=TurFirma;";
        private DataTable dataTable = new DataTable();
        private NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter();
        private NpgsqlDataReader dataReader;
        private Label[] labelsFIO, labelsInfo, labelsTours, labelsSeasons, labelsPayment;
        private TextBox[] textBoxesFIO, textBoxesInfo, textBoxesTours, textBoxesSeasons, textBoxesPayment;
        private DateTimePicker[] dateTimePickersSeasons;
        private DateTimePicker dateTimePickerPayment;
        private CheckBox checkBoxSeasons;
        private ComboBox combobox;
        private Button buttonWithTask;
        private Button buttonCancel;
        private DataGridView dataGridView_Form1 = new DataGridView();
        private string[] schemas = { "tourist", "tourist_info", "tours", "seasons", "payment", "putevki" };
        private Dictionary<string, string> tabPages_Schemas = new Dictionary<string, string>()
        {
            { "Туристы", "tourist" },
            { "Информация о туристах", "tourist_info" },
            { "Туры", "tours" },
            { "Сезоны", "seasons" },
            { "Оплата", "payment" },
            { "Путевки", "putevki" }
        };
        private Dictionary<int, string> buttonsTabIndexesAndSchemas_Form2 = new Dictionary<int, string>()
        {
            { 5, "tourist" },
            { 6, "tourist_info" },
            { 7, "tours" },
            { 8, "seasons" },
            { 9, "payment" },
            { 10, "putevki" }
        };
        private Dictionary<string, int> TabPagesAndButtonsTabIndexes_Form2 = new Dictionary<string, int>()
        {
            { "Туристы", 5 },
            { "Информация о туристах", 6 },
            { "Туры", 7 },
            { "Сезоны", 8 },
            { "Оплата", 9 },
            { "Путевки", 10 }
        };

        public Form2(string buttonText, TabPage tabPage, DataGridView dataGridView)
        {
            dataGridView_Form1 = dataGridView;
            InitializeComponent();
            installUI_byTabPage(buttonText, tabPage);

            connection = new NpgsqlConnection(connectionString);
            connection.Open();

            loadData(tabPage);
        }

        private void installUI_byTabPage(string buttonText, TabPage tabPage)
        {
            switch (tabPage.Text)
            {
                case "Туристы":

                    if (buttonText != "Добавить")
                    {
                        initCombobox(tabPage);
                    }

                    if (buttonText != "Удалить")
                    {
                        initLabelsFIO();
                        initTextBoxesFIO();
                    }
                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;

                case "Информация о туристах":

                    initCombobox(tabPage);
                    if (buttonText != "Удалить")
                    {
                        initLabelsInfo();
                        initTextBoxesInfo();
                    }

                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;

                case "Туры":

                    if (buttonText != "Добавить")
                    {
                        initCombobox(tabPage);
                    }

                    if (buttonText != "Удалить")
                    {
                        initLabelsTours();
                        initTextBoxesTours();
                    }

                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;

                case "Сезоны":

                    if (buttonText != "Добавить")
                    {
                        initCombobox(tabPage);
                    }

                    if (buttonText != "Удалить")
                    {
                        initLabelsSeasons();
                        initDateTimePickersSeasons();
                        initTextBoxesSeasons();
                        initCheckBoxSeasons();
                    }

                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;

                case "Оплата":

                    if (buttonText != "Добавить")
                    {
                        initCombobox(tabPage);
                    }

                    if (buttonText != "Удалить")
                    {
                        initLabelsPayment();
                        initTextBoxesPayment();
                        initDateTimePickerPayment();
                    }

                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;

                case "Путевки":

                    initCombobox(tabPage);
                    initButtonWithTask(buttonText, tabPage);
                    initButtonCancel();
                    break;
            }
        }

        private void loadData(TabPage tabPage)
        {
            string schema;

            switch (tabPage.Text)
            {
                case "Туристы":
                    schema = "tourist";
                    break;

                case "Информация о туристах":
                    schema = "tourist_info";
                    break;

                case "Туры":
                    schema = "tours";
                    break;

                case "Сезоны":
                    schema = "seasons";
                    break;

                case "Оплата":
                    schema = "payment";
                    break;

                case "Путевки":
                    schema = "putevki";
                    break;

                default:
                    schema = "";
                    break;
            }

            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM " + schema, connection);
            adapter.Fill(dataTable);
            dataGridView_ModalWindow.DataSource = dataTable;
        }

        public void buttonWithTask_Click(object sender, EventArgs e)
        {
            Button buttonWithTask = sender as Button;
            string schema = buttonsTabIndexesAndSchemas_Form2[buttonWithTask.TabIndex];
            switch (buttonsTabIndexesAndSchemas_Form2[buttonWithTask.TabIndex])
            {
                case "tourist":
                    string sqlTourist = "";
                    if (buttonWithTask.Text == "Добавить")
                        sqlTourist = "INSERT INTO tourist(lastname, firstname, patronymic) VALUES (@last_name, @first_name, @patronymic)";
                    if (buttonWithTask.Text == "Изменить")
                        sqlTourist = "UPDATE tourist SET lastname = @last_name, firstname = @first_name, patronymic = @patronymic WHERE tourist_id = @tourist_id";
                    if (buttonWithTask.Text == "Удалить")
                        sqlTourist = "DELETE FROM tourist WHERE tourist_id = @tourist_id";
                    
                    NpgsqlCommand commandTourist = new NpgsqlCommand(sqlTourist, connection);
                    if ((buttonWithTask.Text == "Добавить") || (buttonWithTask.Text == "Изменить"))
                    {
                        commandTourist.Parameters.AddWithValue("last_name", this.textBoxesFIO[0].Text);
                        commandTourist.Parameters.AddWithValue("first_name", this.textBoxesFIO[1].Text);
                        commandTourist.Parameters.AddWithValue("patronymic", this.textBoxesFIO[2].Text);
                    }

                    if (buttonWithTask.Text != "Добавить")
                        commandTourist.Parameters.AddWithValue("tourist_id", Int32.Parse(this.combobox.Text.Split(' ')[0]));
                    commandTourist.Prepare();

                    commandTourist.ExecuteNonQuery();
                    if ((buttonWithTask.Text != "Удалить"))
                        for (int i = 0; i < textBoxesFIO.Length; ++i)
                        {
                            this.textBoxesFIO[i].Text = "";
                        }
                    break;

                case "tourist_info":
                    string sqlTouristInfo = "";
                    if (buttonWithTask.Text == "Добавить")
                        sqlTouristInfo = "INSERT INTO tourist_info(email, passport_series, city, country, num, post, tourist_id) VALUES (@email, @passport_series, @city, @country, @num, @post, @tourist_id)";
                    if (buttonWithTask.Text == "Изменить")
                        sqlTouristInfo = "UPDATE tourist_info SET email = @email, passport_series = @passport_series, city = @city, country = @country, num = @num, post = @post WHERE tourist_id = @tourist_id";
                    if (buttonWithTask.Text == "Удалить")
                        sqlTouristInfo = "DELETE FROM tourist_info WHERE tourist_id = @tourist_id";
                    NpgsqlCommand commandTouristInfo = new NpgsqlCommand(sqlTouristInfo, connection);
                    if ((buttonWithTask.Text == "Добавить") || (buttonWithTask.Text == "Изменить"))
                    {
                        commandTouristInfo.Parameters.AddWithValue("email", this.textBoxesInfo[0].Text);
                        commandTouristInfo.Parameters.AddWithValue("city", this.textBoxesInfo[1].Text);
                        commandTouristInfo.Parameters.AddWithValue("num", this.textBoxesInfo[2].Text);
                        commandTouristInfo.Parameters.AddWithValue("passport_series", this.textBoxesInfo[3].Text);
                        commandTouristInfo.Parameters.AddWithValue("country", this.textBoxesInfo[4].Text);
                        commandTouristInfo.Parameters.AddWithValue("post", this.textBoxesInfo[5].Text);
                        commandTouristInfo.Parameters.AddWithValue("tourist_id", Int32.Parse(this.combobox.Text.Split(' ')[0]));
                    }
                    else
                    {
                        commandTouristInfo.Parameters.AddWithValue("tourist_id", Int32.Parse(this.combobox.Text.Split(' ')[0]));
                    }

                    commandTouristInfo.Prepare();

                    commandTouristInfo.ExecuteNonQuery();
                    if (buttonWithTask.Text != "Удалить")
                        for (int i = 0; i < textBoxesInfo.Length; ++i)
                        {
                            this.textBoxesInfo[i].Text = "";
                        }
                    break;

                case "tours":
                    string sqlTours = "";
                    if (buttonWithTask.Text == "Добавить")
                        sqlTours = "INSERT INTO tours(tour_name, price, tour_info) VALUES (@tour_name, @price, @tour_info)";
                    if (buttonWithTask.Text == "Изменить")
                        sqlTours = "UPDATE tours SET tour_name = @tour_name, price = @price, tour_info = @tour_info WHERE tour_id = @tour_id";
                    if (buttonWithTask.Text == "Удалить")
                        sqlTours = "DELETE FROM tours WHERE tour_id = @tour_id";
                    NpgsqlCommand commandTours = new NpgsqlCommand(sqlTours, connection);
                    if (buttonWithTask.Text != "Удалить")
                    {
                        commandTours.Parameters.AddWithValue("tour_name", this.textBoxesTours[0].Text);
                        commandTours.Parameters.AddWithValue("price", Convert.ToInt32(this.textBoxesTours[1].Text));
                        commandTours.Parameters.AddWithValue("tour_info", this.textBoxesTours[2].Text);
                    }

                    if (buttonWithTask.Text != "Добавить")
                        commandTours.Parameters.AddWithValue("tour_id", int.Parse(this.combobox.Text.Split(' ')[0]));
                    commandTours.Prepare();

                    commandTours.ExecuteNonQuery();
                    if (buttonWithTask.Text != "Удалить")
                        for (int i = 0; i < textBoxesTours.Length; ++i)
                        {
                            this.textBoxesTours[i].Text = "";
                        }
                    break;

                case "seasons":
                    string sqlSeasons = "";
                    if (buttonWithTask.Text == "Добавить")
                        sqlSeasons = "INSERT INTO seasons(tour_id, start_date1, end_date, is_closed, seats_amount) VALUES (@tour_id, @start_date1, @end_date, @is_closed, @seats_amount)";
                    if (buttonWithTask.Text == "Изменить")
                        sqlSeasons = "UPDATE seasons SET tour_id = @tour_id, start_date1 = @start_date1, end_date = @end_date, is_closed = @is_closed, seats_amount = @seats_amount WHERE season_id = @season_id";
                    if (buttonWithTask.Text == "Удалить")
                        sqlSeasons = "DELETE FROM seasons WHERE season_id = @season_id";
                    NpgsqlCommand commandSeasons = new NpgsqlCommand(sqlSeasons, connection);
                    if (buttonWithTask.Text != "Удалить")
                    {
                        commandSeasons.Parameters.AddWithValue("tour_id", int.Parse(this.textBoxesSeasons[0].Text));
                        commandSeasons.Parameters.AddWithValue("start_date1", DateTime.Parse(this.dateTimePickersSeasons[0].Text));
                        commandSeasons.Parameters.AddWithValue("end_date", DateTime.Parse(this.dateTimePickersSeasons[1].Text));
                        commandSeasons.Parameters.AddWithValue("is_closed", this.checkBoxSeasons.Checked);
                        commandSeasons.Parameters.AddWithValue("seats_amount", int.Parse(this.textBoxesSeasons[1].Text));
                    }
                    
                    if (buttonWithTask.Text != "Добавить")
                        commandSeasons.Parameters.AddWithValue("tourist_id", this.combobox.Text.Split(' ')[0]);
                    commandSeasons.Prepare();

                    commandSeasons.ExecuteNonQuery();
                    if (buttonWithTask.Text != "Удалить")
                        for (int i = 0; i < textBoxesSeasons.Length; ++i)
                        {
                            this.textBoxesSeasons[i].Text = "";
                        }
                    break;

                case "payment":
                    string sqlPayment = "";
                    if (buttonWithTask.Text == "Добавить")
                        sqlPayment = "INSERT INTO payment(putevki_id, payment_date, amount) VALUES (@putevki_id, @payment_date, @amount)";
                    if (buttonWithTask.Text == "Изменить")
                        sqlPayment = "UPDATE payment SET putevki_id = @putevki_id, payment_date = @payment_date, amount = @amount WHERE payment_id = @payment_id";
                    if (buttonWithTask.Text == "Удалить")
                        sqlPayment = "DELETE FROM payment WHERE payment_id = @payment_id";
                    NpgsqlCommand commandPayment = new NpgsqlCommand(sqlPayment, connection);
                    if (buttonWithTask.Text != "Удалить")
                    {
                        commandPayment.Parameters.AddWithValue("putevki_id", Int32.Parse(this.textBoxesPayment[0].Text));
                        commandPayment.Parameters.AddWithValue("amount", Int32.Parse(this.textBoxesPayment[1].Text));
                        commandPayment.Parameters.AddWithValue("payment_date", DateTime.Parse(this.dateTimePickerPayment.Text));
                    }
                    if (buttonWithTask.Text != "Добавить")
                        commandPayment.Parameters.AddWithValue("payment_id", Int32.Parse(this.combobox.Text.Split(' ')[0]));
                    commandPayment.Prepare();

                    commandPayment.ExecuteNonQuery();
                    if (buttonWithTask.Text != "Удалить")
                        for (int i = 0; i < textBoxesPayment.Length; ++i)
                        {
                            this.textBoxesPayment[i].Text = "";
                        }
                    break;

                case "putevki":
                    string sqlPutevki = "DELETE FROM putevki WHERE putevki_id = @putevki_id";
                    NpgsqlCommand commandPutevki = new NpgsqlCommand(sqlPutevki, connection);
                    commandPutevki.Parameters.AddWithValue("putevki_id", Int32.Parse(this.combobox.Text.Split(' ')[0]));
                    commandPutevki.Prepare();
                    commandPutevki.ExecuteNonQuery();
                    break;
                    /*case "Добавить":
                        {
                            string sqlInsert = "INSERT INTO tourist(lastname, firstname, patronymic) VALUES (@last_name, @first_name, @patronymic)";
                            NpgsqlCommand command = new NpgsqlCommand(sqlInsertTourist, connection);
                            command.Parameters.AddWithValue("last_name", this.textBoxesFIO[0].Text);
                            command.Parameters.AddWithValue("first_name", this.textBoxesFIO[1].Text);
                            command.Parameters.AddWithValue("patronymic", this.textBoxesFIO[2].Text);
                            command.Prepare();

                            command.ExecuteNonQuery();
                            for (int i = 0; i < textBoxesFIO.Length; ++i)
                            {
                                this.textBoxesFIO[i].Text = "";
                            }
                            break;
                        }

                    case "Изменить":
                        {
                            string sqlInsertTourist = "UPDATE tourist SET lastname = @last_name, firstname = @first_name, patronymic = @patronymic WHERE tourist_id = @tourist_id";
                            NpgsqlCommand command = new NpgsqlCommand(sqlInsertTourist, connection);
                            command.Parameters.AddWithValue("last_name", this.textBoxesFIO[0].Text);
                            command.Parameters.AddWithValue("first_name", this.textBoxesFIO[1].Text);
                            command.Parameters.AddWithValue("patronymic", this.textBoxesFIO[2].Text);
                            command.Prepare();

                            command.ExecuteNonQuery();
                            for (int i = 0; i < textBoxesFIO.Length; ++i)
                            {
                                this.textBoxesFIO[i].Text = "";
                            }
                            break;
                        }

                    case "Удалить":
                        {
                            string sqlDeleteTourist = "DELETE FROM tourist WHERE firstname = @first_name AND lastname = @last_name AND patronymic = @patronymic";
                            NpgsqlCommand command = new NpgsqlCommand(sqlDeleteTourist, connection);
                            command.Parameters.AddWithValue("last_name", this.textBoxesFIO[0].Text);
                            command.Parameters.AddWithValue("first_name", this.textBoxesFIO[1].Text);
                            command.Parameters.AddWithValue("patronymic", this.textBoxesFIO[2].Text);
                            command.Prepare();

                            command.ExecuteNonQuery();
                            for (int i = 0; i < textBoxesFIO.Length; ++i)
                            {
                                this.textBoxesFIO[i].Text = "";
                            }
                            break;
                        }*/
            }

            dataTable.Clear();
            string sqlForUpdate = "SELECT * FROM " + schema;
            dataAdapter.SelectCommand = new NpgsqlCommand(sqlForUpdate, connection);
            dataAdapter.Fill(dataTable);

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = dataTable;

            dataGridView_ModalWindow.DataSource = bindingSource;
            dataGridView_Form1.DataSource = bindingSource;

          

        }

        private void initButtonWithTask(string buttonText, TabPage tabPage)
        {
            buttonWithTask = new Button();
            buttonWithTask.Text = buttonText;
            buttonWithTask.TabIndex = TabPagesAndButtonsTabIndexes_Form2[tabPage.Text];
            buttonWithTask.Width = 250;
            buttonWithTask.Height = 23;
            buttonWithTask.Location = new Point { X = 35, Y = 486 };
            buttonWithTask.Click += buttonWithTask_Click;
            this.Controls.Add(buttonWithTask);
        }

        private void initButtonCancel()
        {
            buttonCancel = new Button();
            buttonCancel.Text = "Закрыть";
            buttonCancel.Width = 250;
            buttonCancel.Height = 23;
            buttonCancel.Location = new Point { X = 325, Y = 486 };
            buttonCancel.Click += buttonCancel_Click;
            this.Controls.Add(buttonCancel);
        }

        private void initCombobox(TabPage tabPage)
        {
            combobox = new ComboBox();
            combobox.Location = new Point { X = 100, Y = 290 };
            combobox.Items.Add("Выбрать...");
            combobox.SelectedIndex = 0;
            combobox.Width = 350;
            combobox.DropDownStyle = ComboBoxStyle.DropDownList;
            fillCombobox(tabPage);
            this.Controls.Add(combobox);
        }

        private void fillCombobox(TabPage tabPage)
        {
            connection = new NpgsqlConnection(connectionString);
            connection.Open();

            try
            {
                string schema = "";
                if (tabPage.Text == "Информация о туристах")
                    schema = "tourist";
                else
                    schema = tabPages_Schemas[tabPage.Text];
                string sqlQuerySelect = "SELECT * FROM " + schema;
                NpgsqlCommand command = new NpgsqlCommand(sqlQuerySelect, connection);
                NpgsqlDataReader dataReader = command.ExecuteReader();
                string[] strings = new string[dataReader.FieldCount];

                while (dataReader.Read())
                {

                    for (int i = 0; i < dataReader.FieldCount; ++i)
                    {
                        strings[i] += dataReader.GetValue(i);
                    }

                    string string_for_tourist = "";

                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        string_for_tourist += dataReader.GetValue(i).ToString();
                        string_for_tourist += " ";
                    }

                    combobox.Items.Add((string_for_tourist));
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                if (dataReader != null && !dataReader.IsClosed)
                {
                    dataReader.Close();
                }
            }
            connection.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void initLabelsFIO()
        {
            string[] labelFIOText = { "Фамилия", "Имя", "Отчество" };
            labelsFIO = new Label[labelFIOText.Length];
            for (int i = 0; i < labelFIOText.Length; i++)
            {
                labelsFIO[i] = new Label();
                labelsFIO[i].Location = new Point(i * 200 + 20, 352);
                labelsFIO[i].Text = labelFIOText[i];
                labelsFIO[i].Show();
                labelsFIO[i].AutoSize = true;
                labelsFIO[i].Font = new Font("Calibri", 12);
                this.Controls.Add(labelsFIO[i]);
            }
        }

        private void initTextBoxesFIO()
        {
            textBoxesFIO = new TextBox[labelsFIO.Length];
            for (int i = 0; i < textBoxesFIO.Length; i++)
            {
                textBoxesFIO[i] = new TextBox();
                textBoxesFIO[i].Location = new Point { X = i * 200 + 20, Y = 372 };
                textBoxesFIO[i].Width = 150;
                this.Controls.Add(textBoxesFIO[i]);
            }
        }

        private void initLabelsInfo()
        {
            string[] labelInfoText = { "Электронная почта", "Город", "Телефон", "Паспорт", "Страна", "Почтовый индекс" };
            labelsInfo = new Label[labelInfoText.Length];
            for (int i = 0; i < labelInfoText.Length; i++)
            {
                labelsInfo[i] = new Label();
                if (i == 0 || i == 1 || i == 2)
                    labelsInfo[i].Location = new Point(i * 200 + 20, 332);
                else
                    labelsInfo[i].Location = new Point((i - 3) * 200 + 20, 402);
                labelsInfo[i].Text = labelInfoText[i];
                labelsInfo[i].AutoSize = true;
                labelsInfo[i].Font = new Font("Calibri", 12);
                this.Controls.Add(labelsInfo[i]);
            }
        }

        private void initTextBoxesInfo()
        {
            textBoxesInfo = new TextBox[labelsInfo.Length];
            try
            {
                for (int i = 0; i < textBoxesInfo.Length; i++)
                {
                    textBoxesInfo[i] = new TextBox();
                    if ((i == 0) || (i == 1) || (i == 2))
                        textBoxesInfo[i].Location = new Point { X = i * 200 + 20, Y = 352 };
                    else
                        textBoxesInfo[i].Location = new Point { X = (i - 3) * 200 + 20, Y = 422 };

                    textBoxesInfo[i].Width = 150;
                    this.Controls.Add(textBoxesInfo[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void initLabelsTours()
        {
            string[] labelTourText = { "Название тура", "Цена", "Информация о туре" };
            labelsTours = new Label[labelTourText.Length];
            for (int i = 0; i < labelTourText.Length; i++)
            {
                labelsTours[i] = new Label();
                labelsTours[i].Location = new Point(i * 200 + 20, 352);
                labelsTours[i].Text = labelTourText[i];
                labelsTours[i].Show();
                labelsTours[i].AutoSize = true;
                labelsTours[i].Font = new Font("Calibri", 12);
                this.Controls.Add(labelsTours[i]);
            }
        }

        private void initTextBoxesTours()
        {
            textBoxesTours = new TextBox[labelsTours.Length];
            for (int i = 0; i < textBoxesTours.Length; i++)
            {
                textBoxesTours[i] = new TextBox();
                textBoxesTours[i].Location = new Point { X = i * 200 + 20, Y = 372 };
                textBoxesTours[i].Width = 150;
                this.Controls.Add(textBoxesTours[i]);
            }
        }

        private void initLabelsSeasons()
        {
            string[] strings = { "Дата начала", "Дата окончания", "id тура", "Количество мест", "Закрыт" };
            labelsSeasons = new Label[strings.Length];
            for (int i = 0; i < strings.Length; i++)
            {
                labelsSeasons[i] = new Label();
                if (i == 0 || i == 1)
                    labelsSeasons[i].Location = new Point(i * 200 + 20, 382);
                else
                    labelsSeasons[i].Location = new Point((i - 2) * 200 + 20, 332);
                labelsSeasons[i].Text = strings[i];
                labelsSeasons[i].Show();
                labelsSeasons[i].AutoSize = true;
                labelsSeasons[i].Font = new Font("Calibri", 12);
                this.Controls.Add(labelsSeasons[i]);
            }
        }

        private void initDateTimePickersSeasons()
        {
            string[] strings = { "Дата начала", "Дата окончания" };
            dateTimePickersSeasons = new DateTimePicker[strings.Length];
            for (int i = 0; i < strings.Length; ++i)
            {
                dateTimePickersSeasons[i] = new DateTimePicker();
                dateTimePickersSeasons[i].Location = new Point { X = i * 200 + 20, Y = 402 };
                this.Controls.Add(dateTimePickersSeasons[i]);
            }

        }

        private void initTextBoxesSeasons()
        {
            textBoxesSeasons = new TextBox[2];
            for (int i = 0; i < textBoxesSeasons.Length; i++)
            {
                textBoxesSeasons[i] = new TextBox();
                textBoxesSeasons[i].Location = new Point { X = i * 200 + 20, Y = 352 };
                textBoxesSeasons[i].Width = 150;
                this.Controls.Add(textBoxesSeasons[i]);
            }
        }

        private void initCheckBoxSeasons()
        {
            checkBoxSeasons = new CheckBox();
            checkBoxSeasons.Location = new Point(420, 352);
            this.Controls.Add(checkBoxSeasons);
        }

        private void initLabelsPayment()
        {
            string[] labelsPaymentText = { "id путёвки", "Стоимость", "Дата оплаты" };
            labelsPayment = new Label[labelsPaymentText.Length];
            for (int i = 0; i < labelsPaymentText.Length; i++)
            {
                labelsPayment[i] = new Label();
                labelsPayment[i].Location = new Point(i * 200 + 20, 352);
                labelsPayment[i].Text = labelsPaymentText[i];
                labelsPayment[i].Show();
                labelsPayment[i].AutoSize = true;
                labelsPayment[i].Font = new Font("Calibri", 12);
                this.Controls.Add(labelsPayment[i]);
            }
        }

        private void initTextBoxesPayment()
        {
            textBoxesPayment = new TextBox[2];
            for (int i = 0; i < textBoxesPayment.Length; i++)
            {
                textBoxesPayment[i] = new TextBox();
                textBoxesPayment[i].Location = new Point { X = i * 200 + 20, Y = 372 };
                textBoxesPayment[i].Width = 150;
                this.Controls.Add(textBoxesPayment[i]);
            }
        }

        private void initDateTimePickerPayment()
        {
            dateTimePickerPayment = new DateTimePicker();
            dateTimePickerPayment.Location = new Point(420, 372);
            dateTimePickerPayment.Width = 150;
            this.Controls.Add(dateTimePickerPayment);
        }

    }
}