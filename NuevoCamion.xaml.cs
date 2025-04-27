using Diagnosis.Modelos;
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

namespace Diagnosis
{
    /// <summary>
    /// Lógica de interacción para NuevoCamion.xaml
    /// </summary>
    public partial class NuevoCamion : Window
    {
        private NuevoCamionVM vm;
        public NuevoCamion()
        {
            InitializeComponent();
            vm = new NuevoCamionVM();
            this.DataContext = vm;
        }

        private void CancelarClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
