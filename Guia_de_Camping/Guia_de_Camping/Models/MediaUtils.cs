using Aspbrasil.Resources.Locale;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Aspbrasil
{
    class MediaUtils
    {
        /// <summary>
        /// Método utilizado para requisitar ao usuário uma imagem. 
        /// </summary>
        /// <returns>bool = sucesso ao obter a imagem. MediaFile = caso sucesso, o arquivo que foi obtido. NULL caso contrário.</returns>
        public static async Task<Tuple<bool, MediaFile>> RequisitarMidiaSistema(Page paginaParaAlertas, bool incluirVideo = false)
        {
            await CrossMedia.Current.Initialize();

            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            var photosStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage, Permission.Photos });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
                photosStatus = results[Permission.Photos];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted & photosStatus == PermissionStatus.Granted)
            {
                string FileName = Guid.NewGuid().ToString();
                MediaFile file = null;

                //const string gravarVideo = "Gravar Vídeo"; //Não será permitido para não dar problema na exibição (FORMATO)

                string action = null;
                if (incluirVideo)
                {
                    action = await paginaParaAlertas.DisplayActionSheet(AppLanguage.SelecioneUmaFotoOuVideo, AppLanguage.Cancelar, null, AppLanguage.Texto_Tirar_uma_foto, AppLanguage.Escolher_foto_galeria, AppLanguage.Escolher_video_galeria);
                }
                else
                {
                    action = await paginaParaAlertas.DisplayActionSheet(AppLanguage.SelecioneUmaFoto, AppLanguage.Cancelar, null, AppLanguage.Texto_Tirar_uma_foto, AppLanguage.Escolher_foto_galeria);
                }

                if (action == AppLanguage.Texto_Tirar_uma_foto)
                {
                    await Task.Delay(500);
                    if (!CrossMedia.Current.IsCameraAvailable)
                    {
                        await paginaParaAlertas.DisplayAlert(AppLanguage.Titulo_CameraNaoDisponivel, AppLanguage.Mensagem_CameraNaoDisponivel, "OK");
                    }

                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "FotoUsuarioApp",
                        Name = FileName + ".jpg",
                        PhotoSize = PhotoSize.Medium
                    });
                }
                else if (action == AppLanguage.Escolher_foto_galeria)
                {
                    // Aguarda para que o popup anterior  
                    await Task.Delay(500);
                    file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium
                    });
                }
                else if (action == AppLanguage.Escolher_video_galeria)
                {
                    await Task.Delay(500);
                    file = await CrossMedia.Current.PickVideoAsync();
                }

                if (file != null)
                {
                    return new Tuple<bool, MediaFile>(true, file);
                }
            }
            else
            {
                await paginaParaAlertas.DisplayAlert(AppLanguage.Titulo_PermissaoFotoNegada, AppLanguage.Mensagem_PermissaoFotoNegada, "OK");

                //On iOS you may want to send your user to the settings screen.
                CrossPermissions.Current.OpenAppSettings();
            }
            return new Tuple<bool, MediaFile>(false, null);
        }
    }
}