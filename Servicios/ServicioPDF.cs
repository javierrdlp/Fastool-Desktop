using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Windows;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using Colors = QuestPDF.Helpers.Colors;
using CommunityToolkit.Mvvm.ComponentModel;
using Diagnosis.Clases;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Properties;
using System.IO;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;

namespace Diagnosis.Servicios
{
    public class ServicioPDF: ObservableObject
    {

        private DatosPDF datosPDF;
        public DatosPDF DatosPDF
        {
            get { return datosPDF; }
            set
            {
                SetProperty(ref datosPDF, value);
            }
        }
          
        public void GenerarPDF()
        {
            DatosPDF = WeakReferenceMessenger.Default.Send<DatosPDFMessage>();
            //Esto sale en un cuadro de diálogo si no se ha asignado una ruta donde guardar el archivo.
            if (string.IsNullOrEmpty(DatosPDF.Ruta))
            {
                MessageBox.Show("Por favor, seleccione una ruta para guardar el PDF.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Esto hace falta para usar el plugin, es la licencia.
            QuestPDF.Settings.License = LicenseType.Community;

            const float horizontalMargin = 1f;
            const float verticalMargin = 1f;
            bool pdfCreado = false;

            //A partir de aquí ya es la creación del PDF, su nombre, diseño.
            try
            {
                Document.Create(container =>
                {


                    container.Page(page =>
                    {
                        //Proepiedades generales del formato de la página del PDF
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        //Configuración la "decoración" del PDF
                        page.Background()
                            .PaddingVertical(verticalMargin, Unit.Inch)
                            .RotateRight()
                            .Decoration(decoration =>
                            {
                                decoration.Before().RotateRight().RotateRight().Element(DrawSide);
                                decoration.Content().Extend();
                                decoration.After().Element(DrawSide);

                                //Configurtación marcos laterales personalizables con el nombre de la compañía.
                                void DrawSide(IContainer container)
                                {
                                    container
                                        .Height(horizontalMargin, Unit.Inch)
                                        .AlignMiddle()
                                        .Row(row =>
                                        {
                                            row.AutoItem().PaddingRight(16).Text("NOMBRE COMPAÑIA").FontSize(16).FontColor("#FF060E88");
                                            row.RelativeItem().PaddingTop(12).ExtendHorizontal().LineHorizontal(2).LineColor("#FF060E88");
                                        });
                                }
                            });
                        //Configurtación del título.
                        page.Header()
                            .Text("FASTOOL")
                            .Bold()
                            .Italic()
                            .AlignCenter()
                            .FontFamily("Wide Latin")
                            .FontSize(20).FontColor("#FF060E88");

                        page.Content()
                        .Layers(layers =>
                        {
                            layers
                                .Layer()
                                .AlignMiddle()
                                .AlignCenter();


                            //En la capa de arriba ya se traen todos los datos necesarios de la clase ReparaciónSeleccionada y entre esto y el ToString de cada clase he configurado
                            // la vista del pdf.
                            layers
                            .PrimaryLayer()
                                .PaddingVertical(1, Unit.Centimetre)
                                .PaddingHorizontal(1, Unit.Centimetre)
                                .Column(x =>
                                {
                                    x.Spacing(20);
                                    x.Item().Text(DatosPDF.Reparacion.Matricula?.IdCliente?.ToString()).FontSize(14).Bold();
                                    x.Item().Text(DatosPDF.Reparacion.ToString()).FontSize(14).SemiBold();
                                    x.Spacing(20);
                                    x.Item().Text("OBSERVACIONES:").FontSize(14).SemiBold();
                                    x.Item().Text(DatosPDF.Observaciones).FontSize(14).SemiBold();

                                });

                        });


                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Resumen reparación ID " + DatosPDF.Reparacion.Id + ".");

                            });
                    });
                })
                .GeneratePdf(DatosPDF.Ruta);
                pdfCreado = true;
            }
            catch (IOException ex)
            {
                pdfCreado = false;
                MessageBox.Show("PDF abierto, ciérrelo para poder sobrescribirlo y vuelva a intentarlo.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window?.Close();                

            }            

            if (pdfCreado)
            {
                MessageBox.Show("PDF creado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                var window2 = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                window2?.Close();
            }
           
        }
    }
}
