using System;
using System.Text.RegularExpressions;

namespace Aspbrasil.CustomExtensions
{
	public static class ColorExtensions
	{
		/// <summary>
        ///     Obtém o valor hexadecimal de um objeto Xamarin.Forms.Color.
        /// </summary>
        /// <returns>A string hexadecimal.</returns>
        /// <param name="color">Xamarin Forms Color</param>
		public static string ObterHexString(this Xamarin.Forms.Color color)
		{
			var red = (int)(color.R * 255);
			var green = (int)(color.G * 255);
			var blue = (int)(color.B * 255);
			var alpha = (int)(color.A * 255);
			var hex = $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";

			return hex;
		}
	}

	public static class StringExtensions
	{
		/// <summary>
		///     Trunca uma String e acrescenta ... no final.
		/// </summary>
        /// <returns></returns>
		/// <param name="s">String para ser truncada</param>
		/// <param name="tamanhoMaximo">número de chars para truncar</param>
		public static string Truncar(this string s, int tamanhoMaximo)
		{
			if (String.IsNullOrEmpty(s) || tamanhoMaximo <= 0)
			{
				return String.Empty;
			}
			if (s.Length > tamanhoMaximo)
			{
				return s.Substring(0, tamanhoMaximo) + "...";
			}
			return s;
		}
        
        /// <summary>
        ///     Método utilizado para remover caracteres especiais e substituir caracteres acentuados por caracteres comuns.
        /// </summary>
        /// <param name="textoOriginal">texto original, com ou sem acentuação e caracteres especiais</param>
        /// <param name="removerEspacos">Padrão: false. Caso false troca apenas os caracteres acentuados. Caso true, remove também os espaços do texto.</param>
        /// <returns></returns>
        public static string RemoverCaracteresEspeciaisESubstituirAcentos(this string textoOriginal, bool removerEspacos = false)
        {
            string charsAcentuados = "αßÁáÀàÂâÃãĀāÅåÄäÆæÇçÉéÈèÊêÍíÌìÎîÑñÓóÒòÔôÖöŌōØøÚúÙùÜüŽž";
            string charsRegulares = "abAaAaAaAaAaAaAaAaCcEeEeEeIiIiIiNnOoOoOoOoOoOoUuUuUuZz";

            string textoAlterado = "";
            string regexPattern = "\\p{P}+";

            if (removerEspacos)
            {
                regexPattern = "[\\W]";
            }

            string textoSemCaracteresEspeciais = Regex.Replace(textoOriginal, regexPattern, "");

            //Procura por caracteres acentuados
            for (int i = 0; i < textoSemCaracteresEspeciais.Length; i++)
            {
                // se encontrar acentuação
                if (charsAcentuados.Contains(textoSemCaracteresEspeciais[i].ToString()))
                {
                    int specialPosition = charsAcentuados.IndexOf(textoSemCaracteresEspeciais[i]);
                    // utiliza o caracter correspondente sem acentuação
                    textoAlterado += charsRegulares[specialPosition].ToString();
                }
                else
                {
                    // Se não encontrar nenhum acento, utiliza o caracter que já está no texto
                    textoAlterado += textoSemCaracteresEspeciais[i].ToString();
                }
            }
            return textoAlterado;
        }
    }
}