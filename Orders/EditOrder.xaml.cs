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
	/// Логика взаимодействия для editOrder.xaml
	/// </summary>
	public partial class editOrder : Window
	{
		private int _orderId;

		public editOrder(int ID)
		{
			InitializeComponent();
			_orderId = ID;
			LoadData();
			LoadComboBoxes();
		}

		private void LoadData()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var order = context.Заявки
						.Where(o => o.ID == _orderId)
						.Select(o => new
						{
							o.ID,
							o.ДатаСоздания,
							o.ДатаОкончания,
							o.Количество,
							StatusId = o.КодСтатусаЗаявки,
							//ProductId = o.КодПродукции
						})
						.FirstOrDefault();

					if (order != null)
					{
						orderIDTxt.Content = $"№Заявки: {order.ID}";
						dateOfCreationTxt.Content = $"Дата создания: {order.ДатаСоздания:dd.MM.yyyy HH:mm}";
						endDatePick.Text = order.ДатаОкончания?.ToString("dd.MM.yyyy HH:mm") ?? "";
						countTxt.Text = order.Количество.ToString();

						// Устанавливаем выбранные значения в ComboBox
						statusOrderCombo.SelectedValue = order.StatusId;
						//productCombo.SelectedValue = order.ProductId;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке данных заявки: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void LoadComboBoxes()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Загрузка статусов заявок
					var statuses = context.СтатусЗаявки
						.Select(s => new { s.ID, s.Наименование })
						.ToList();

					statusOrderCombo.ItemsSource = statuses;
					statusOrderCombo.DisplayMemberPath = "Наименование";
					statusOrderCombo.SelectedValuePath = "ID";

					// Загрузка продукции
					var products = context.Продукции
						.Select(p => new { p.ID, p.Наименование })
						.ToList();

					productCombo.ItemsSource = products;
					productCombo.DisplayMemberPath = "Наименование";
					productCombo.SelectedValuePath = "ID";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке справочников: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var order = context.Заявки.FirstOrDefault(o => o.ID == _orderId);

					if (order != null)
					{
						// Обновляем данные
						if (DateTime.TryParse(endDatePick.Text, out var endDate))
							order.ДатаОкончания = endDate;

						if (int.TryParse(countTxt.Text, out var quantity))
							order.Количество = quantity;

						order.КодСтатусаЗаявки = (int)statusOrderCombo.SelectedValue;
						//order.КодПродукции = (int)productCombo.SelectedValue;

						context.SaveChanges();
						MessageBox.Show("Данные успешно сохранены", "Успех",
							MessageBoxButton.OK, MessageBoxImage.Information);
						this.Close();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
