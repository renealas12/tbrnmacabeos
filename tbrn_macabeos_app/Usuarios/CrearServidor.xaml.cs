using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

//using FluentFTP;
using System.Net;
using System.Threading;
using System.IO;

using tbrn_macabeos_app.Clases;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace tbrn_macabeos_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrearServidor: ContentPage
    {
        public int servidor;
        public int acceso;
        public int usuario;

        public CrearServidor(int parameter1, int parameter2, int parameter3)
        {
            InitializeComponent();
            servidor = parameter1;
            acceso = parameter2;
            usuario = parameter3;
        }

        public string fotoPath;
        public string fotoName;
        public string serverPath;
        public string uploadPath;
        public string accesoAsignado;
        public int accessid;
        public string filename;


        public Xamarin.Forms.ImageSource PhotoStream { get; set; }
        
        private async void btnFoto_click(object sender, EventArgs e)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        await DisplayAlert("Permisos de Camara", "Debe darnos Autorizacion para usar su Camara", "Aceptar");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No hay camara", "No se deteca la camara.", "Ok");
                        return;
                    }

                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        // Variable para guardar la foto en el album público
                        Name = txtDui.Text.ToString(),
                        SaveToAlbum = true
                    });

                    if (file == null)
                        return;

                    this.imgCamara.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            return stream;
                            
                        });

                    await DisplayAlert("Foto realizada", "Localización: " + file.AlbumPath, "Ok");
                    fotoPath = file.AlbumPath;
                    byte[] myBinary = File.ReadAllBytes(@fotoPath);
                    PhotoStream = ImageSource.FromStream(()=>new MemoryStream(myBinary));

                }
                else
                {
                    await DisplayAlert("Permiso denegado", "Da permisos de cámara al Aplicacion.", "Ok");
                }
            }

            catch (Exception ex)
            {
               
            }
        }
        
        private async void uploadFoto()
        {
            waitActivityIndicator.IsRunning = true;

            var username = "admin@hidraulicadelistmo.com";
            var password = "r@ul1t012";

            Stream requestStream = null;
            FileStream fileStream = null;
            FtpWebResponse uploadResponse = null;
            filename = Path.GetFileName(fotoPath);

            try
            {
                FtpWebRequest uploadRequest =
                  (FtpWebRequest)WebRequest.Create("ftp://ftp.hidraulicadelistmo.com/admin/securitb/images" + @"/"+filename);
                uploadRequest.Method = WebRequestMethods.Ftp.UploadFile;
                //Since the FTP you are downloading to is secure, send
                //in user name and password to be able upload the file
                ICredentials credentials = new NetworkCredential(username, password);
                uploadRequest.Credentials = credentials;
                //UploadFile is not supported through an Http proxy
                //so we disable the proxy for this request.
                uploadRequest.Proxy = null;
                //uploadRequest.UsePassive = false; <--found from another forum and did not make a difference
                requestStream = uploadRequest.GetRequestStream();
                fileStream = File.Open(fotoPath, FileMode.Open);
                byte[] buffer = new byte[1024];
                int bytesRead;
                while (true)
                {
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                        break;
                    requestStream.Write(buffer, 0, bytesRead);
                }
                //The request stream must be closed before getting
                //the response.
                requestStream.Close();
                uploadResponse =
                  (FtpWebResponse)uploadRequest.GetResponse();
            }
            catch (UriFormatException ex)
            {
                await DisplayAlert("Error en Imagen", ex.Message, "Aceptar", "Cancelar");                
            }
            catch (IOException ex)
            {
                await DisplayAlert("Error en Imagen", ex.Message, "Aceptar", "Cancelar");
            }
            catch (WebException ex)
            {
                await DisplayAlert("Error en Imagen", ex.Message, "Aceptar", "Cancelar");
            }
            finally
            {
                if (uploadResponse != null)
                    uploadResponse.Close();
                if (fileStream != null)
                    fileStream.Close();
                if (requestStream != null)
                    requestStream.Close();
            }
            waitActivityIndicator.IsRunning = false;
        }


        //public async void UploadFile(string source, string destination)
        //{
        //    string filename = Path.GetFileName(source);

        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServer + destination + filename);

        //    request.Method = WebRequestMethods.Ftp.UploadFile;

        //    request.Credentials = new NetworkCredential(usuarioFTP, contraseñaFTP);

        //    StreamReader sourceStream = new StreamReader(source);

        //    byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());

        //    request.ContentLength = fileContents.Length;

        //    Stream requestStream = request.GetRequestStream();

        //    requestStream.Write(fileContents, 0, fileContents.Length);

        //    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

        //    await DisplayAlert("Carga de Imagen", response.StatusDescription, "Aceptar", "Cancelar");

        //    response.Close();

        //    requestStream.Close();

        //    sourceStream.Close();

        //}

        private async void OnChangeDui(object sender, TextChangedEventArgs e)
        {
            var allowedchar = "0123456789";

                if(!txtDui.Text.All(allowedchar.Contains))
                {
                    await DisplayAlert("DUI Erroneo", "Favor no Introducir el Guion (-)", "Aceptar", "Cancelar");
                    txtDui.Text = "";
                }

        }

        private async void OnChangeTelefono(object sender, TextChangedEventArgs e)
        {
            var allowedchar = "0123456789";

            if (!txtTelefono.Text.All(allowedchar.Contains))
            {
                await DisplayAlert("Caracter Erroneo", "Favor no Introducir caracteres Equivocos", "Aceptar", "Cancelar");
                txtTelefono.Text = "";
            }

        }

        private async void OnChangeTelTrab(object sender, TextChangedEventArgs e)
        {
            var allowedchar = "0123456789";

            if (!txtTelefonoTrabajo.Text.All(allowedchar.Contains))
            {
                await DisplayAlert("Caracter Erroneo", "Favor no Introducir caracteres Equivocos", "Aceptar", "Cancelar");
                txtTelefonoTrabajo.Text = "";
            }

        }

        private async void OnChangeTelEmer(object sender, TextChangedEventArgs e)
        {
            var allowedchar = "0123456789";

            if (!txtTelefonoContacto.Text.All(allowedchar.Contains))
            {
                await DisplayAlert("Caracter Erroneo", "Favor no Introducir caracteres Equivocos", "Aceptar", "Cancelar");
                txtTelefonoContacto.Text = "";
            }

        }

        private async void bntCrearServidor_click(object sender, EventArgs e)
        {
            accesoAsignado = txtAcceso.SelectedItem.ToString();
            var nombreFoto = Path.GetFileName(fotoPath);
            uploadPath = "/images/" + nombreFoto;
            serverPath = "http://hidraulicadelistmo.com/admin/securitb/images/" + nombreFoto;
            var Administrador = "Administrador";
            var Lider = "Lider";
            var Servidor = "Servidor";
                        
            if (acceso>=2)
            {       
                if (String.Equals(Administrador,accesoAsignado))
                    {                        
                        accessid = 2;
                    }
                if (String.Equals(Lider, accesoAsignado))
                    {
                        accessid = 2;                        
                    }
                if(String.Equals(Servidor, accesoAsignado))
                    {
                        accessid = 3;                       
                    }
            }
            else
            {
                if (String.Equals(Administrador, accesoAsignado))
                {
                    accessid = 1;                    
                }
                if (String.Equals(Lider, accesoAsignado))
                {
                    accessid = 2;                    
                }
                if (String.Equals(Servidor, accesoAsignado))
                {
                    accessid = 3;                    
                }
            }
            try
            {
                uploadFoto();

                var fechaNacimiento = txtFechaNacimiento.Date.ToString("yyyy/M/d");
                var fechaIngreso = txtFechaIngreso.Date.ToString("yyyy/M/d");
                var radioPropio = txtRadioPropio.SelectedItem.ToString();
                
                UseManager manager = new UseManager();
                manager.registrarServidor(txtNombre.Text.ToString(), txtApellido.Text.ToString(), txtDui.Text.ToString(), 
                    fechaNacimiento, String.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.ToString(),
                    String.IsNullOrWhiteSpace(txtLugarTrabajo.Text) ? null : txtLugarTrabajo.Text.ToString(),
                    String.IsNullOrWhiteSpace(txtTelefonoTrabajo.Text) ? null : txtTelefonoTrabajo.Text.ToString(),
                    String.IsNullOrWhiteSpace(txtContacto.Text) ? null : txtContacto.Text.ToString(), 
                    String.IsNullOrWhiteSpace(txtTelefonoContacto.Text) ? null : txtTelefonoContacto.Text.ToString(),
                    fechaIngreso,serverPath,radioPropio,accessid);

                await DisplayAlert("Registro", "Registro Exitoso", "Aceptar", "Cancelar");

                txtNombre.Text = "";
                txtApellido.Text = "";
                txtDui.Text = "";
                txtTelefono.Text = "";
                txtLugarTrabajo.Text = "";
                txtTelefonoTrabajo.Text = "";
                txtContacto.Text = "";
                txtTelefonoContacto.Text = "";

                await Navigation.PushAsync(new MenuUsuarios(servidor, acceso, usuario));
            }
         catch (Exception e1) { Console.WriteLine(e1.Message.ToString()); }
        }
    }
}