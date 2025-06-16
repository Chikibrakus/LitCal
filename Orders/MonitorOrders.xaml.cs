using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace КалькуляторДляЛитейщиков
{
	/// <summary>
	/// Логика взаимодействия для MonitorOrders.xaml
	/// </summary>
	public partial class MonitorOrders : Window
	{
		private Сотрудники user;
		public class OrderViewModel
		{
			public int ID { get; set; }
			public DateTime ДатаСоздания { get; set; }
			public DateTime? ДатаОкончания { get; set; }
			public string Статус { get; set; }
			public int Количество { get; set; }
			public string Продукция { get; set; }
			public string Материал { get; set; }
			public string ТипПродукции { get; set; }
		}

		public MonitorOrders(Сотрудники user)
		{
			InitializeComponent();
			this.user = user;
			LoadOrders();
			dataGrid.SelectionChanged += DataGrid_SelectionChanged;
		}

		private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dataGrid.SelectedItem is OrderViewModel selectedOrder)
			{
				editOrder editOrder = new editOrder(selectedOrder.ID);
				editOrder.ShowDialog();
			}
		}

		private void GetOrderDetails(int orderId)
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Здесь можно выполнить дополнительный запрос к БД по orderId
					var orderDetails = context.Заявки
						.Where(o => o.ID == orderId)
						.Select(o => new
						{
							o.ID,
							o.ДатаСоздания,
							o.ДатаОкончания,
							Status = context.СтатусЗаявки.FirstOrDefault(s => s.ID == o.КодСтатусаЗаявки).Наименование,
							// Другие нужные поля
						})
						.FirstOrDefault();

					if (orderDetails != null)
					{
						MessageBox.Show($"Детали заявки #{orderDetails.ID}\n" +
									  $"Статус: {orderDetails.Status}\n" +
									  $"Дата создания: {orderDetails.ДатаСоздания}\n" +
									  $"Дата окончания: {orderDetails.ДатаОкончания}",
									  "Информация о заявке");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при получении деталей заявки: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


		private void LoadOrders()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var orders = context.Заявки
						.Join(context.СтатусЗаявки,
							заявка => заявка.КодСтатусаЗаявки,
							статус => статус.ID,
							(заявка, статус) => new { Заявка = заявка, Статус = статус })
						.Join(context.Продукции,
							комб => комб.Заявка.КодПродукции,
							продукция => продукция.ID,
							(комб, продукция) => new { комб.Заявка, комб.Статус, Продукция = продукция })
						.Join(context.ТипПродукции,
							комб => комб.Продукция.КодТипаПродукции,
							тип => тип.ID,
							(комб, тип) => new { комб.Заявка, комб.Статус, комб.Продукция, ТипПродукции = тип })
						.Join(context.СоставПродукции,
							комб => комб.Продукция.ID,
							состав => состав.КодПродукции,
							(комб, состав) => new { комб.Заявка, комб.Статус, комб.Продукция, комб.ТипПродукции, Состав = состав })
						.Join(context.Материалы,
							комб => комб.Состав.КодМатериала,
							материал => материал.ID,
							(комб, материал) => new OrderViewModel
							{
								ID = комб.Заявка.ID,
								ДатаСоздания = комб.Заявка.ДатаСоздания,
								ДатаОкончания = комб.Заявка.ДатаОкончания,
								Статус = комб.Статус.Наименование,
								Количество = комб.Заявка.Количество,
								Продукция = комб.Продукция.Наименование,
								Материал = материал.Наименование,
								ТипПродукции = комб.ТипПродукции.Наименование
							})
						.ToList();

					dataGrid.ItemsSource = orders;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void refreshBtn_Click(object sender, RoutedEventArgs e)
		{
			LoadOrders();
		}

		private void createOrderBtn_Click(object sender, RoutedEventArgs e)
		{
			CreateOrder createOrder = new CreateOrder();
			createOrder.Show();
		}

		private void backBtn_Click(object sender, RoutedEventArgs e)
		{
			if (user.IDДолжности == 1) //Админ
			{
				AdminPanel adminPanel = new AdminPanel(user);
				this.Close();
				adminPanel.Show();
			}
			else
			{
				MainWindow mainWindow = new MainWindow();
			}
		}
	}
}
