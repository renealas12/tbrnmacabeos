﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using tbrn_macabeos_app.Clases;

namespace tbrn_macabeos_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListarUsuarios : ContentPage
    {
        public int servidor;
        public int acceso;
        public int usuario;
        public ListarUsuarios(int parameter1, int parameter2, int parameter3)
        {
            InitializeComponent();
            fillLista();
            servidor = parameter1;
            acceso = parameter2;
            usuario = parameter3;
        }

       public IEnumerable<fullServidor> servidores;

        private async void fillLista()
        {
            try
            {
                UseManager manager = new UseManager();
                var result = await manager.listarServidores();

                servidores = result;

                if (result != null)
                {
                    lstUsuarios.ItemsSource = result;
                }
            }catch (Exception e1) {
                await DisplayAlert("Error en Lista", "Favor vuelva a cargar Servidor no listo", "Aceptar", "Cancelar");
                Console.WriteLine(e1.Message.ToString());
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            //thats all you need to make a search  

            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                lstUsuarios.ItemsSource = servidores;
            }

            else
            {
                lstUsuarios.ItemsSource = servidores.Where(x => x.nombre.Contains(e.NewTextValue));
            }
        }

        private async void Eliminar_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            UseManager manager = new UseManager();
            manager.deshabilitarUsuario(servidorVar.id);

            await Navigation.PushAsync(new MenuUsuarios(servidor, acceso, usuario));

        }

        private async void Modificar_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            await Navigation.PushAsync(new ModificarUsuario(servidorVar.id, servidor, acceso, usuario));

        }

        private async void ModificarCultos_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            await Navigation.PushAsync(new ListarCompromisoServidor(servidor, acceso, usuario, servidorVar.id));

        }

        private async void ResetPassword_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            await Navigation.PushAsync(new CambiarContrasenaServidor(servidor, acceso, usuario, servidorVar.id));

        }

        private async void AgregarCultos_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            await Navigation.PushAsync(new AgregarCompromisoServidor(servidor, acceso, usuario, servidorVar.id));

        }
        private async void RehabilitarCultos_Click(object sender, EventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            fullServidor servidorVar = (fullServidor)item.CommandParameter;

            await Navigation.PushAsync(new ListarCompromisoDesServidor(servidor, acceso, usuario, servidorVar.id));

        }

        private async void bntBack_click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MenuUsuarios(servidor, acceso, usuario));
        }
    }
}