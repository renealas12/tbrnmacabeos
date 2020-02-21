using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using tbrn_macabeos_app.Clases;

namespace tbrn_macabeos_app.Equipos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AgregarEquipo : ContentPage
    {
        public int servidor;
        public int acceso;
        public int usuario;
        public AgregarEquipo(int parameter1, int parameter2, int parameter3)
        {
            InitializeComponent();
            servidor = parameter1;
            acceso = parameter2;
            usuario = parameter3;
        }

        private async void bntAgregarRadio_click(object sender, EventArgs e)
        {
            var serial = txtSerial.Text.ToString();
            var numero = Int32.Parse(txtNumero.Text.ToString());
            var estatus = txtObservacion.Text.ToString();
            try
            {
                UseManager manager = new UseManager();
                manager.registrarEquipo(serial, numero, estatus);

                await DisplayAlert("Registro", "Registro Exitoso", "Aceptar", "Cancelar");

                await Navigation.PushAsync(new MenuEquipos(servidor, acceso, usuario));
            }
            catch (Exception e1) { Console.WriteLine(e1.Message.ToString()); } 
        }    
    }
}