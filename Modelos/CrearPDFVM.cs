using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using QuestPDF.Infrastructure;
using System;
using System.Windows.Shapes;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using Colors = QuestPDF.Helpers.Colors;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using System.Printing;
using System.Windows;
using System.Linq;

namespace Diagnosis.Modelos
{
    internal class CrearPDFVM : ObservableObject
    {
        private ServicioPDF servicioPDF;
        private Reparacion reparacionSeleccionada;

        public Reparacion ReparacionSeleccionada
        {
            get { return reparacionSeleccionada; }
            set
            {
                SetProperty(ref reparacionSeleccionada, value);
            }
        }

        private float? duracion;
        public float? Duracion
        {
            get { return duracion; }
            set
            {
                if (duracion != value)
                {
                    SetProperty(ref duracion, value);
                }
            }
        }

        private string? observaciones;
        public string? Observaciones
        {
            get { return observaciones; }
            set
            {
                SetProperty(ref observaciones, value);
            }
        }

        private string nombrePDF;
        public string NombrePDF
        {
            get { return nombrePDF; }
            set
            {
                SetProperty(ref nombrePDF, value);
            }
        }

        private string rutaPDF;
        public string RutaPDF
        {
            get { return rutaPDF; }
            set
            {
                SetProperty(ref rutaPDF, value);
            }
        }

        private DatosPDF datosPDF;
        public DatosPDF DatosPDF
        {
            get { return datosPDF; }
            set
            {
                SetProperty(ref datosPDF, value);
            }
        }

        public RelayCommand CrearResumenCommand { get; }
        public RelayCommand BuscadorRutaCommand { get; }

        public CrearPDFVM()
        {
            servicioPDF = new ServicioPDF(); 
            datosPDF = new DatosPDF(); 
            ReparacionSeleccionada = WeakReferenceMessenger.Default.Send<ReparacionSeleccionadaMessage>();
            CrearResumenCommand = new RelayCommand(GenerarPDF);
            BuscadorRutaCommand = new RelayCommand(GeneradorRuta);

            NombrePDF = ("R-" + ReparacionSeleccionada.Id + "-" + ReparacionSeleccionada.Matricula?.Matricula);

            WeakReferenceMessenger.Default.Register<CrearPDFVM, DatosPDFMessage>(this, (r, m) =>
            {
                if (!m.HasReceivedResponse)
                {
                    m.Reply(DatosPDF);
                }
            });
        }

        private void GeneradorRuta()
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                Title = "Guardar PDF",
                FileName = (NombrePDF + ".pdf")
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                DatosPDF.Ruta = saveFileDialog.FileName;
            }
        }

        private void GenerarPDF()
        {
            DatosPDF.Observaciones = Observaciones ?? string.Empty; // Asigna cadena vacía si Observaciones es null
            DatosPDF.Reparacion = ReparacionSeleccionada;

            servicioPDF.GenerarPDF();
        }
    }
}

