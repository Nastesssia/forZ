using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TO_Project_2
{
    public partial class SignForm : Form
    {
        public SignForm()
        {
            InitializeComponent();
        }

        private void buttonSign_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Text;

            using (var context = new Model1())
            {
                try
                {
                    var user = context.Users
                        .FirstOrDefault(u => u.login == login && u.password == password);

                    if (user != null)
                    {
                        Form mainForm = null;

                        switch (user.type.ToLower())
                        {
                            case "менеджер":
                                mainForm = new MainFormManager();
                                break;
                            case "техник":
                                var master = context.Masters.FirstOrDefault(m => m.userID == user.userID);
                                if (master != null)
                                {
                                    mainForm = new MainFormMaster(master.masterID);
                                }
                                break;
                            case "оператор":
                                mainForm = new MainFormOperator();
                                break;
                            case "заказчик":
                                var client = context.Clients.FirstOrDefault(c => c.userID == user.userID);
                                if (client != null)
                                {
                                    mainForm = new MainFormClient(client.clientID);
                                }
                                break;
                            default:
                                MessageBox.Show("Неизвестный тип пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                        }

                        if (mainForm != null)
                        {
                            mainForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при попытке входа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loginTextBox_TextChanged(object sender, EventArgs e)
        {
            // Можно добавить валидацию логина
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            // Можно добавить валидацию пароля
        }
    }
}
