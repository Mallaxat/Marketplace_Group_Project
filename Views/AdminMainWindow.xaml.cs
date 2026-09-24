using Marketplace_Group_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Marketplace_Group_Project.Views
{
    
    /// Логика взаимодействия для AdminMainWindow.xaml
    
    public partial class AdminMainWindow : Window
    {
        public AdminMainWindow()
        {
            InitializeComponent();
		}

		private void dtg_products_CurrentCellChanged(object sender, EventArgs e)
		{
			if (this.DataContext != null)
			{
				AdminMainViewModel vm = (DataContext as AdminMainViewModel);
				vm.SaveProductCommand.Execute(null);
			}
		}
	}
}
