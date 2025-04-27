using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Diagnosis.Clases;
using Diagnosis.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnosis.Modelos
{
    class MainWindowVM : ObservableObject
    {
        public RelayCommand AbrirVentanaAyudaCommand { get; }

      
        public MainWindowVM()
        {
            
          
            AbrirVentanaAyudaCommand = new RelayCommand(AbrirAyuda);
          
        }

        private void AbrirAyuda()
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = @"Resources\Ayuda_FasTool.chm",
                UseShellExecute = true
            });
        }


    }
}
