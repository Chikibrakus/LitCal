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
using System.Windows.Navigation;
using System.Windows.Shapes;
using КалькуляторДляЛитейщиков.Admin;
using КалькуляторДляЛитейщиков.Data;
using КалькуляторДляЛитейщиков.Employes;

namespace КалькуляторДляЛитейщиков
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ConnectDB.conObj = new VectorOrdersDBEntities2();
			loginBox.Text = "ADMIN";
			passBox.Password = "ADMIN";
		}

		private void enterBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Валидация ввода
				if (string.IsNullOrWhiteSpace(loginBox.Text))
				{
					ShowWarning("Введите логин");
					loginBox.Focus();
					return;
				}

				if (string.IsNullOrWhiteSpace(passBox.Password))
				{
					ShowWarning("Введите пароль");
					passBox.Focus();
					return;
				}

				// Проверка длины логина и пароля
				if (loginBox.Text.Trim().Length < 3 || loginBox.Text.Trim().Length > 50)
				{
					ShowWarning("Логин должен быть от 3 до 50 символов");
					loginBox.Focus();
					return;
				}

				if (passBox.Password.Trim().Length < 4)
				{
					ShowWarning("Пароль должен быть не менее 4 символов");
					passBox.Focus();
					return;
				}

				// Поиск пользователя в базе данных
				var user = ConnectDB.conObj.Сотрудники.FirstOrDefault(emp =>
					emp.Логин == loginBox.Text.Trim());

				if (user == null)
				{
					ShowWarning("Пользователь с таким логином не найден");
					loginBox.Focus();
					return;
				}

				// Проверка пароля
				if (user.Пароль != passBox.Password.Trim())
				{
					ShowWarning("Неверный пароль");
					passBox.Clear();
					passBox.Focus();
					return;
				}

				// Авторизация успешна - перенаправление в зависимости от роли
				HandleSuccessfulAuthorization(user);
			}
			catch (Exception ex)
			{
				ShowError($"Произошла ошибка: {ex.Message}");
			}
		}

		private void HandleSuccessfulAuthorization(Сотрудники user)
		{
			loginBox.Clear();
			passBox.Clear();

			switch (user.IDДолжности)
			{
				case 1: // Админ
					AdminPanel adminPanel = new AdminPanel(user);
					adminPanel.Show();
					this.Close();
					break;
				case 2: // Другая роль
					var monitorOrders = new MonitorOrders(user);
					monitorOrders.Show();
					this.Close();
					break;
				default:
					ShowWarning("Неизвестная роль пользователя");
					break;
			}
		}

		private void ShowWarning(string message)
		{
			MessageBox.Show(message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		private void ShowError(string message)
		{
			MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
