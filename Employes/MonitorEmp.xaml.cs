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
using КалькуляторДляЛитейщиков.Admin;
using КалькуляторДляЛитейщиков.Data;

namespace КалькуляторДляЛитейщиков.Employes
{
	/// <summary>
	/// Логика взаимодействия для MonitorEmp.xaml
	/// </summary>
	public partial class MonitorEmp : Window
	{
		private Сотрудники admin;
		public MonitorEmp(Сотрудники user)
		{
			InitializeComponent();
			LoadEmployees();
			SetupDataGridColumns();
			empTable.SelectionChanged += EmpTable_SelectionChanged; // Добавьте эту строку
			admin = user;
		}

		private void EmpTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (empTable.SelectedItem != null)
			{
				// Получаем ID выбранного сотрудника
				var selected = empTable.SelectedItem;
				var _login = (string)selected.GetType().GetProperty("Логин").GetValue(selected);
				int _id = (int)selected.GetType().GetProperty("ID").GetValue(selected);

				// Открываем окно редактирования
				editEmp editWindow = new editEmp(_login, _id);
				editWindow.Owner = this;
				editWindow.ShowDialog();

				// Обновляем данные после закрытия окна редактирования
				LoadEmployees();

				// Сбрасываем выделение
				empTable.SelectedItem = null;
			}
		}

		private void LoadEmployees()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var employees = context.Сотрудники
						.Include("Должности")
						.Include("СтатусСотрудника")
						.Select(e => new
						{
							e.ID,
							e.Фамилия,
							e.Имя,
							e.Отчество,
							Должность = e.Должности.Наименование,
							Статус = e.СтатусСотрудника.Наименование,
							e.СерияИНомерПаспорта,
							e.Логин,
							e.Пароль
						})
						.ToList();

					empTable.ItemsSource = employees;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SetupDataGridColumns()
		{
			empTable.AutoGenerateColumns = false;
			empTable.Columns.Clear();

			// Добавляем колонки вручную для лучшего контроля
			//empTable.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("ID") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Фамилия", Binding = new Binding("Фамилия") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new Binding("Имя") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Отчество", Binding = new Binding("Отчество") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Должность", Binding = new Binding("Должность") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new Binding("Статус") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Паспорт", Binding = new Binding("СерияИНомерПаспорта") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Логин", Binding = new Binding("Логин") });
			empTable.Columns.Add(new DataGridTextColumn { Header = "Пароль", Binding = new Binding("Пароль") });
		}

		private void refreshBtn_Click(object sender, RoutedEventArgs e)
		{
			LoadEmployees();
		}

		private void addEmpBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					CreateEmp createEmp = new CreateEmp();
					createEmp.Show();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string GenerateLogin(string lastName, string firstName)
		{
			// Простая генерация логина из фамилии и имени
			return $"{lastName.ToLower()}.{firstName.ToLower().Substring(0, 1)}";
		}

		private void resetBtn_Click(object sender, RoutedEventArgs e)
		{
			ResetFields();
		}

		private void ResetFields()
		{
			secondName.Text = "";
			nameTxt.Text = "";
			thirdName.Text = "";
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			AdminPanel adminPanel = new AdminPanel(admin);
			this.Close();
			adminPanel.Show();
		}
	}
}
