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
using КалькуляторДляЛитейщиков.Employes;

namespace КалькуляторДляЛитейщиков.Admin
{
	/// <summary>
	/// Логика взаимодействия для AdminPanel.xaml
	/// </summary>
	public partial class AdminPanel : Window
	{
		Сотрудники admin;
		public AdminPanel(Сотрудники user)
		{
			InitializeComponent();
			admin = user;
			this.Title = $"Панель администратора: {admin.Фамилия}" + $" {admin.Имя}";
		}

		private void OrdersPanel_Click(object sender, RoutedEventArgs e)
		{
			MonitorOrders monitorOrders = new MonitorOrders(admin);
			monitorOrders.Show();
			this.Close();
		}

		private void EmployersPanel_Click(object sender, RoutedEventArgs e)
		{
			MonitorEmp monitorEmpp = new MonitorEmp(admin);
			monitorEmpp.Show();
			this.Close();
		}
	}
}
