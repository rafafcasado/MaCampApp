using Aspbrasil.DataAccess;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aspbrasil.Models.DataAccess
{
    public class ConfiguracoesAnunciosDA
    {
        private static ConfiguracoesAnuncios _configs = null;
        public static async Task<ConfiguracoesAnuncios> ObterConfigs(bool forcarAtualizacao = false)
        {
            if (_configs == null || forcarAtualizacao)
            {
                string jsonconfigs = await new HttpClient().GetStringAsync("https://guiadecampingsanuncios.homologacao.net/?altTemplate=Configs_Anuncios");
                _configs = JsonConvert.DeserializeObject<ConfiguracoesAnuncios>(jsonconfigs);
            }
            return _configs;
        }
    }
}


//List<Anuncio> anunciossimulados = new List<Anuncio>
//{
//    new Anuncio
//    {
//        URLExterna = "https://image.shutterstock.com/image-vector/bottle-concept-horizontal-webpage-banner-260nw-1121908541.jpg",
//        URLImagem = "https://image.shutterstock.com/image-vector/bottle-concept-horizontal-webpage-banner-260nw-1121908541.jpg",
//        Titulo = "Título do anúncio 0",
//        Subtitulo = "Sub 0",
//        Tipo = TipoAnuncio.Banner
//    },
//    new Anuncio
//    {
//        URLExterna = "https://image.shutterstock.com/image-vector/vector-web-banner-template-business-260nw-583239223.jpg",
//        URLImagem = "https://image.shutterstock.com/image-vector/vector-web-banner-template-business-260nw-583239223.jpg",
//        Titulo = "Título do anúncio 4",
//        Subtitulo = "Sub 4",
//        Tipo = TipoAnuncio.Banner
//    },
//    new Anuncio
//    {
//        URLExterna = "https://timeline.canaltech.com.br/311275.700/razer-atualiza-linha-blade-stealth-com-mais-potencia-e-modelo-4k-touch.jpg",
//        URLImagem = "https://timeline.canaltech.com.br/311275.700/razer-atualiza-linha-blade-stealth-com-mais-potencia-e-modelo-4k-touch.jpg",
//        Titulo = "Título do anúncio 1",
//        Subtitulo = "Sub 1",
//        Tipo = TipoAnuncio.Nativo
//    },
//    new Anuncio
//    {
//        URLExterna = "https://timeline.canaltech.com.br/311235.700/de-novo-samsung-e-pega-usando-imagens-profissionais-para-divulgar-smartphone.jpg",
//        URLImagem = "https://timeline.canaltech.com.br/311235.700/de-novo-samsung-e-pega-usando-imagens-profissionais-para-divulgar-smartphone.jpg",
//        Titulo = "Título do anúncio 5",
//        Subtitulo = "Sub 5",
//        Tipo = TipoAnuncio.Nativo
//    },
//    new Anuncio
//    {
//        URLExterna = "https://st2.depositphotos.com/1687997/10109/v/950/depositphotos_101095492-stock-illustration-sale-banner-with-moderns-style.jpg",
//        URLImagem = "https://st2.depositphotos.com/1687997/10109/v/950/depositphotos_101095492-stock-illustration-sale-banner-with-moderns-style.jpg",
//        Titulo = "Título do anúncio 2",
//        Subtitulo = "Sub 2",
//        Tipo = TipoAnuncio.Popup
//    },
//    new Anuncio
//    {
//        URLExterna = "https://st2.depositphotos.com/2697407/8808/v/950/depositphotos_88086616-stock-illustration-christmas-sale-banner-template-with.jpg",
//        URLImagem = "https://st2.depositphotos.com/2697407/8808/v/950/depositphotos_88086616-stock-illustration-christmas-sale-banner-template-with.jpg",
//        Titulo = "Título do anúncio 3",
//        Subtitulo = "Sub 3",
//        Tipo = TipoAnuncio.Popup
//    },
//};

//string jsonItens = JsonConvert.SerializeObject(anunciossimulados);

//_anuncios = JsonConvert.DeserializeObject<List<Anuncio>>(jsonItens);

