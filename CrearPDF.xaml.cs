using Diagnosis.Modelos;
using QuestPDF.Infrastructure;
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
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using Colors = QuestPDF.Helpers.Colors;
using System.IO;
using System.Reflection;

namespace Diagnosis
{
    /// <summary>
    /// Lógica de interacción para CrearPDF.xaml
    /// </summary>
    public partial class CrearPDF : Window
    {
        private CrearPDFVM vm;
        public CrearPDF()
        {
            InitializeComponent();
            vm = new CrearPDFVM();
            this.DataContext = vm;

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            contador.Text = (cajaTexto.Text.Length).ToString();

            if (cajaTexto.Text.Length >= 140)
            {

                cajaTexto.IsReadOnly = true;
            }
            else
            {

                cajaTexto.IsReadOnly = false;
            }

        }

       
    }
}
