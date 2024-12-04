using System;
using Xamarin.Forms;

namespace Aspbrasil.CustomControls
{
    /// <summary>
    /// ListView que não responde ao "ScrollsToTop" do iOS (ao clicar na barra de Status).
    /// Não é possível ter dois Scroll na mesma tela que respondam ao toque na barra de status,
    /// por isso, deve ser utilizado, por exemplo, no Menu Lateral, para que a listagem da home responda ao toque na barra de status.
    /// </summary>
    public class ListViewNonScrollsToTop : ListView
    {
    }
}
