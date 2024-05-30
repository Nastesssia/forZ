﻿using System;
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
    public partial class RequestForm : Form
    {
        private int? requestId;
        private int clientId;

        public RequestForm(int? requestId, int clientId)
        {
            InitializeComponent();
            this.requestId = requestId;
            this.clientId = clientId;
            if (requestId.HasValue)
            {
                LoadRequestData();
            }
        }

        private void LoadRequestData()
        {
            using (var context = new Model1())
            {
                var request = context.Requests.FirstOrDefault(r => r.requestID == requestId);

                if (request != null)
                {
                    textBoxNumberRequest.Text = request.requestID.ToString();
                    textBoxComputerTechType.Text = request.computerTechType;
                    textBoxComputerTechModel.Text = request.computerTechModel;
                    richTextBoxProblemDescription.Text = request.problemDescription;
                    textBoxNubmerClient.Text = request.clientID.ToString();
                }
                else
                {
                    MessageBox.Show("Заявка не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
        }

        private void buttonRequestSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxComputerTechType.Text) || string.IsNullOrWhiteSpace(textBoxComputerTechModel.Text) || string.IsNullOrWhiteSpace(richTextBoxProblemDescription.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new Model1())
            {
                if (requestId.HasValue)
                {
                    var request = context.Requests.FirstOrDefault(r => r.requestID == requestId);

                    if (request != null)
                    {
                        request.computerTechType = textBoxComputerTechType.Text;
                        request.computerTechModel = textBoxComputerTechModel.Text;
                        request.problemDescription = richTextBoxProblemDescription.Text;

                        try
                        {
                            context.SaveChanges();
                            MessageBox.Show("Информация успешно сохранена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            string errorMessage = $"Ошибка при сохранении данных: {ex.Message}";
                            Exception innerEx = ex.InnerException;
                            while (innerEx != null)
                            {
                                errorMessage += $"\nВнутреннее исключение: {innerEx.Message}";
                                innerEx = innerEx.InnerException;
                            }
                            MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Заявка не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var newRequest = new Requests
                    {
                        clientID = clientId,
                        computerTechType = textBoxComputerTechType.Text,
                        computerTechModel = textBoxComputerTechModel.Text,
                        problemDescription = richTextBoxProblemDescription.Text,
                        startDate = DateTime.Now,
                        requestStatus = "Новая"
                    };

                    // Не устанавливаем requestID, если он автоинкрементируемый

                    context.Requests.Add(newRequest);

                    try
                    {
                        context.SaveChanges();
                        MessageBox.Show("Новая заявка успешно добавлена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"Ошибка при добавлении новой заявки: {ex.Message}";
                        Exception innerEx = ex.InnerException;
                        while (innerEx != null)
                        {
                            errorMessage += $"\nВнутреннее исключение: {innerEx.Message}";
                            innerEx = innerEx.InnerException;
                        }
                        MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            // Обновление DataGridView в MainFormClient
            var mainForm = Application.OpenForms.OfType<MainFormClient>().FirstOrDefault();
            mainForm?.LoadClientRequests();
        }
        private void buttonDeleteRequest_Click(object sender, EventArgs e)
        {
            using (var context = new Model1())
            {
                var request = context.Requests
                                     .Include("ClientsReviews")
                                     .Include("MasterComments")
                                     .Include("MasterQualityManager")
                                     .Include("PartsOrders")
                                     .FirstOrDefault(r => r.requestID == requestId);

                if (request != null)
                {
                    // Удаление связанных данных
                    context.ClientsReviews.RemoveRange(request.ClientsReviews);
                    context.MasterComments.RemoveRange(request.MasterComments);
                    context.MasterQualityManager.RemoveRange(request.MasterQualityManager);
                    context.PartsOrders.RemoveRange(request.PartsOrders);

                    // Удаление самой заявки
                    context.Requests.Remove(request);

                    try
                    {
                        context.SaveChanges();
                        MessageBox.Show("Заявка успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = $"Ошибка при удалении заявки: {ex.Message}";
                        Exception innerEx = ex.InnerException;
                        while (innerEx != null)
                        {
                            errorMessage += $"\nВнутреннее исключение: {innerEx.Message}";
                            innerEx = innerEx.InnerException;
                        }
                        MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Заявка не найдена.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Обновление DataGridView в MainFormClient
            var mainForm = Application.OpenForms.OfType<MainFormClient>().FirstOrDefault();
            mainForm?.LoadClientRequests();
            var mainFormOp = Application.OpenForms.OfType<MainFormOperator>().FirstOrDefault();
                mainFormOp?.LoadAllRequests(); 
        }
    }
}
