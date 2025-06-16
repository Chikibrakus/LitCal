using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	/// Логика взаимодействия для CreateOrder.xaml
	/// </summary>
	public partial class CreateOrder : Window
	{

		public CreateOrder()
		{
			InitializeComponent();
			LoadProductCombo();
			productList.SelectionChanged += ProductList_SelectionChanged;
		}

		private void LoadProductCombo()
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					var products = context.Продукции
						.Select(p => new
						{
							p.ID,
							p.Наименование
						})
						.ToList();

					productList.ItemsSource = products;
					productList.SelectedValuePath = "ID";
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при загрузке продуктов: {ex.Message}", "Ошибка",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (productList.SelectedItem != null)
			{
				try
				{
					using (var context = new VectorOrdersDBEntities2())
					{
						int productId = (int)productList.SelectedValue;

						var materials = context.СоставПродукции
							.Where(sp => sp.КодПродукции == productId)
							.Join(context.Материалы,
								sp => sp.КодМатериала,
								m => m.ID,
								(sp, m) => m.Наименование)
							.ToList();

						materialsList.Text = "Материалы:\n" + string.Join("\n", materials);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Ошибка при загрузке материалов: {ex.Message}", "Ошибка",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void CreateOrderBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (var context = new VectorOrdersDBEntities2())
				{
					// Отключаем проверку валидации для store-generated колонок
					context.Configuration.ValidateOnSaveEnabled = false;

					var newOrder = new Заявки
					{
						ДатаСоздания = DateTime.Now,
						КодСтатусаЗаявки = 1,
						Количество = int.Parse(countProduct.Text),
						КодПродукции = (int)productList.SelectedValue,
						КодСотрудника = 2,
						ДатаОкончания = null
					};

					context.Заявки.Add(newOrder);
					context.SaveChanges();

					MessageBox.Show("Заявка создана успешно!");
					this.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка: {ex.InnerException?.Message ?? ex.Message}");
			}
		}
	}
}
