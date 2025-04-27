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
    /// Lógica de interacción para ModificarCamion.xaml
    /// </summary>
    public partial class ModificarCamion : Window
    {
        private ModificarCamionVM vm;
        public ModificarCamion()
        {
            InitializeComponent();
            vm = new ModificarCamionVM();
            this.DataContext = vm;
        }

        private void botonModificar_Click(object sender, RoutedEventArgs e)
        {
            var nombreBindingExpression = modelo.GetBindingExpression(TextBox.TextProperty);
            nombreBindingExpression?.UpdateSource();

            var cifBindingExpression = marca.GetBindingExpression(TextBox.TextProperty);
            cifBindingExpression?.UpdateSource();

            var emailBindingExpression = idCliente.GetBindingExpression(TextBox.TextProperty);
            emailBindingExpression?.UpdateSource();
        }
    }
}
