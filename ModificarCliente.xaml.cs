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
    /// Lógica de interacción para ModificarCliente.xaml
    /// </summary>
    public partial class ModificarCliente : Window
    {
        private ModificarClienteVM vm;
        public ModificarCliente()
        {
            InitializeComponent();
            vm = new ModificarClienteVM();
            this.DataContext = vm;
        }

        private void botonModificar_Click(object sender, RoutedEventArgs e)
        {            
            var nombreBindingExpression = nombre.GetBindingExpression(TextBox.TextProperty);            
            nombreBindingExpression?.UpdateSource();            
            
            var emailBindingExpression = email.GetBindingExpression(TextBox.TextProperty);           
            emailBindingExpression?.UpdateSource();
        }
    }
}
