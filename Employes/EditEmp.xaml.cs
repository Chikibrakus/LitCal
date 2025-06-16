using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using КалькуляторДляЛитейщиков.Data;

namespace КалькуляторДляЛитейщиков.Employes
{
	/// <summary>
	/// Логика взаимодействия для editEmp.xaml
	/// </summary>
	public partial class editEmp : Window
	{
		private string _employeeLogin;
		private int _employeeID;
		private Сотрудники _employee;
		private List<Должности> _positions;
		private List<СтатусСотрудника> _statuses;
		public editEmp(string _login, int _id)
		{
			InitializeComponent();
			_employeeID = _id;
			LoadComboBoxData();
			LoadEmployeeData();
		}

		private void LoadComboBoxData()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Загрузка должностей
					_positions = context.Должности.ToList();
					empPostBox.ItemsSource = _positions;
					empPostBox.DisplayMemberPath = "Наименование";
					empPostBox.SelectedValuePath = "ID";

					// Загрузка статусов
					_statuses = context.СтатусСотрудника.ToList();
					statusCombo.ItemsSource = _statuses;
					statusCombo.DisplayMemberPath = "Наименование";
					statusCombo.SelectedValuePath = "ID";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadEmployeeData()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					_employee = context.Сотрудники
						.FirstOrDefault(e => e.ID == _employeeID);

					if (_employee != null)
					{
						secondNameTxt.Text = _employee.Фамилия;
						nameTxt.Text = _employee.Имя;
						thirdTxt.Text = _employee.Отчество;
						empPostBox.SelectedIndex = _employee.IDДолжности - 1;
						passportTxt.Text = _employee.СерияИНомерПаспорта;
						loginTxt.Text = _employee.Логин;
						passTxt.Text = _employee.Пароль;
						statusCombo.SelectedIndex = _employee.IDСтатуса - 1;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var employee = context.Сотрудники.Find(_employeeID);
					if (employee != null)
					{
						// Проверка обязательных полей
						if (string.IsNullOrWhiteSpace(secondNameTxt.Text) ||
							string.IsNullOrWhiteSpace(nameTxt.Text) ||
							empPostBox.SelectedItem == null ||
							statusCombo.SelectedItem == null)
						{
							MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
								MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}

						employee.Фамилия = secondNameTxt.Text;
						employee.Имя = nameTxt.Text;
						employee.Отчество = thirdTxt.Text;

						// Используем SelectedValue вместо SelectedIndex
						employee.IDДолжности = (int)empPostBox.SelectedValue;
						employee.СерияИНомерПаспорта = passportTxt.Text;

						// Исправлено: сохраняем логин из поля логина, а не паспорта
						employee.Логин = loginTxt.Text;
						employee.Пароль = passTxt.Text;

						// Используем SelectedValue вместо SelectedIndex
						employee.IDСтатуса = (int)statusCombo.SelectedValue;

						// Проверяем изменения перед сохранением
						if (context.ChangeTracker.HasChanges())
						{
							context.SaveChanges();
							MessageBox.Show("Данные сотрудника успешно обновлены!", "Успех",
								MessageBoxButton.OK, MessageBoxImage.Information);
							this.Close();
						}
						else
						{
							MessageBox.Show("Нет изменений для сохранения", "Информация",
								MessageBoxButton.OK, MessageBoxImage.Information);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка сохранения данных: {ex.Message}\n\nПодробности: {ex.InnerException?.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}