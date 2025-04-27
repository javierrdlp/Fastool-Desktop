using Diagnosis.Modelos;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Windows;
using QuestPDF.Fluent;
using Colors = QuestPDF.Helpers.Colors;
using System.IO;
using System;
using System.Reflection;

namespace Diagnosis
{
    /// <summary>
    /// Lógica de interacción para Inicio.xaml
    /// </summary>
    public partial class Inicio : Window
    {
        private InicioVM vm;
        public Inicio()
        {
            InitializeComponent();

            vm = new InicioVM();
            this.DataContext = vm;
            this.Closed += VentanaPrincipal_Closed;

        }
        private void VentanaPrincipal_Closed(object sender, EventArgs e)
        {
            // Cierra todas las ventanas abiertas excepto la ventana principal
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                {
                    window.Close();
                }
            }
        }
    }
}
