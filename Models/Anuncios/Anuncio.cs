﻿using static MaCamp.Utils.Enumeradores;

namespace MaCamp.Models.Anuncios
{
    public class Anuncio
    {
        public string? Titulo { get; set; }
        public string? SubTitulo { get; set; }
        public string? UrlImagem { get; set; }
        public string? UrlExterna { get; set; }
        public TipoAnuncio Tipo { get; set; }
    }
}