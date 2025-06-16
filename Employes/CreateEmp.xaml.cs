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

namespace КалькуляторДляЛитейщиков
{
	/// <summary>
	/// Логика взаимодействия для CreateEmp.xaml
	/// </summary>
	public partial class CreateEmp : Window
	{
		public CreateEmp()
		{
			InitializeComponent();
			LoadPostCombo();
		}

		private void LoadPostCombo()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Получаем список всех должностей
					var positions = context.Должности
						.Select(p => new
						{
							p.ID,
							p.Наименование
						})
						.ToList();

					// Заполняем ComboBox
					empPostBox.ItemsSource = positions;
					empPostBox.DisplayMemberPath = "Наименование";
					empPostBox.SelectedValuePath = "ID";

					//// Устанавливаем первую должность по умолчанию
					//if (positions.Count > 0)
					//{
					//	empPostBox.SelectedIndex = 0;
					//}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке должностей: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void addEmpBtn_Click(object sender, RoutedEventArgs e)
		{
			// Проверка заполнения обязательных полей
			if (string.IsNullOrWhiteSpace(secondNameTxt.Text) ||
				string.IsNullOrWhiteSpace(nameTxt.Text) ||
				string.IsNullOrWhiteSpace(thirdNameTxt.Text) ||
				string.IsNullOrWhiteSpace(passportTxt.Text) ||
				string.IsNullOrWhiteSpace(loginTxt.Text) ||
				string.IsNullOrWhiteSpace(passTxt.Text) ||
				empPostBox.SelectedItem == null)
			{
				MessageBox.Show("Заполните все обязательные поля!",
					"Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			// Проверка формата паспорта (10 цифр)
			if (passportTxt.Text.Length != 10 || !passportTxt.Text.All(char.IsDigit))
			{
				MessageBox.Show("Серия и номер паспорта должны состоять из 10 цифр!",
					"Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Проверка уникальности логина
					if (context.Сотрудники.Any(emp => emp.Логин == loginTxt.Text))
					{
						MessageBox.Show("Сотрудник с таким логином уже существует!",
							"Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					// Проверка уникальности паспорта
					if (context.Сотрудники.Any(emp => emp.СерияИНомерПаспорта == passportTxt.Text))
					{
						MessageBox.Show("Сотрудник с таким паспортом уже существует!",
							"Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					// Создаем нового сотрудника
					var newEmployee = new Сотрудники
					{
						Фамилия = secondNameTxt.Text,
						Имя = nameTxt.Text,
						Отчество = thirdNameTxt.Text,
						IDДолжности = (int)empPostBox.SelectedValue,
						IDСтатуса = 1, // "Действующий"
						СерияИНомерПаспорта = passportTxt.Text,
						Логин = loginTxt.Text,
						Пароль = passTxt.Text // В реальном приложении пароль нужно хэшировать
					};

					context.Сотрудники.Add(newEmployee);
					context.SaveChanges();

					MessageBox.Show("Сотрудник успешно добавлен!", "Успех",
						MessageBoxButton.OK, MessageBoxImage.Information);

					// Закрываем форму или очищаем поля
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при добавлении сотрудника: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
