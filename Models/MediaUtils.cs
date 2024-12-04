using MaCamp.Resources.Locale;

namespace MaCamp.Models
{
    internal class MediaUtils
    {
        /// <summary>
        /// Método utilizado para requisitar ao usuário uma imagem. 
        /// </summary>
        /// <returns>bool = sucesso ao obter a imagem. FileResult = caso sucesso, o arquivo que foi obtido. NULL caso contrário.</returns>
        public static async Task<Tuple<bool, FileResult?>> RequisitarMidiaSistema(Page paginaParaAlertas, bool incluirVideo = false)
        {
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            var storageReadStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            var storageWriteStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            var photosStatus = await Permissions.CheckStatusAsync<Permissions.Photos>();

            if (cameraStatus != PermissionStatus.Granted || storageReadStatus != PermissionStatus.Granted || storageWriteStatus != PermissionStatus.Granted)
            {
                var requestedCamera = await Permissions.RequestAsync<Permissions.Camera>();
                var requestedStorageRead = await Permissions.RequestAsync<Permissions.StorageRead>();
                var requestedStorageWrite = await Permissions.RequestAsync<Permissions.StorageWrite>();
                var requestedPhotos = await Permissions.RequestAsync<Permissions.Photos>();

                if (requestedCamera == PermissionStatus.Granted && requestedStorageRead == PermissionStatus.Granted && requestedStorageWrite == PermissionStatus.Granted && requestedPhotos == PermissionStatus.Granted)
                {
                    var FileName = Guid.NewGuid().ToString();

                    //var string gravarVideo = "Gravar Vídeo"; //Não será permitido para não dar problema na exibição (FORMATO)

                    var videoParams = new[]
                    {
                        AppLanguage.Texto_Tirar_uma_foto,
                        AppLanguage.Escolher_foto_galeria,
                        AppLanguage.Escolher_video_galeria
                    };

                    var photoParams = new[]
                    {
                        AppLanguage.Texto_Tirar_uma_foto,
                        AppLanguage.Escolher_foto_galeria
                    };

                    var title = incluirVideo ? AppLanguage.SelecioneUmaFotoOuVideo : AppLanguage.SelecioneUmaFoto;
                    var listParams = incluirVideo ? videoParams : photoParams;
                    var action = await paginaParaAlertas.DisplayActionSheet(title, AppLanguage.Cancelar, null, listParams);
                    await Task.Delay(500);

                    if (action == AppLanguage.Texto_Tirar_uma_foto)
                    {
                        if (!MediaPicker.IsCaptureSupported)
                        {
                            await paginaParaAlertas.DisplayAlert(AppLanguage.Titulo_CameraNaoDisponivel, AppLanguage.Mensagem_CameraNaoDisponivel, "OK");
                        }

                        var takePhotoFile = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                        {
                            //Directory = "FotoUsuarioApp",
                            //Name = FileName + ".jpg"
                        });

                        return new Tuple<bool, FileResult?>(true, takePhotoFile);
                    }

                    if (action == AppLanguage.Escolher_foto_galeria)
                    {
                        var pickPhotoFile = await MediaPicker.PickPhotoAsync();

                        return new Tuple<bool, FileResult?>(true, pickPhotoFile);
                    }

                    if (action == AppLanguage.Escolher_video_galeria)
                    {
                        var pickVideoFile = await MediaPicker.PickVideoAsync();

                        return new Tuple<bool, FileResult?>(true, pickVideoFile);
                    }
                }
            }
            else
            {
                await paginaParaAlertas.DisplayAlert(AppLanguage.Titulo_PermissaoFotoNegada, AppLanguage.Mensagem_PermissaoFotoNegada, "OK");

                //On iOS you may want to send your user to the settings screen.
                AppInfo.Current.ShowSettingsUI();
            }

            return new Tuple<bool, FileResult?>(false, null);
        }
    }
}