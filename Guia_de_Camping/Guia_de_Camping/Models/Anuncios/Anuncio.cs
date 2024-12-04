namespace Aspbrasil.Models
{
    public class Anuncio
    {
        public string Titulo { get; set; }
        public string Subtitulo { get; set; }
        public string URLImagem { get; set; }
        public string URLExterna { get; set; }
        public TipoAnuncio Tipo { get; set; }
    }
}