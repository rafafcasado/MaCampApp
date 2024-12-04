using Aspbrasil.Models;
using Xamarin.Forms;

namespace Aspbrasil.Views.Menu
{
    class MenuTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate MasterPageItemTemplate;
        private readonly DataTemplate MasterPageSubItemTemplate;
        private readonly DataTemplate DivisoriaTemplate;

        public MenuTemplateSelector()
        {
            // Retain instances!
            this.MasterPageItemTemplate = new DataTemplate(typeof(MasterPageItemViewCell));
            this.MasterPageSubItemTemplate = new DataTemplate(typeof(MasterPageSubItemViewCell));
            this.DivisoriaTemplate = new DataTemplate(typeof(DivisoriaMenuViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var itemMenu = item as ItemMenu;
            if (itemMenu == null)
                return null;

            switch (itemMenu.TipoLayout)
            {
                case TipoLayoutMenu.Item:
                    return this.MasterPageItemTemplate;
                case TipoLayoutMenu.SubItem:
                    return this.MasterPageSubItemTemplate;
                case TipoLayoutMenu.Divisoria:
                    return this.DivisoriaTemplate;
                default:
                    return this.MasterPageItemTemplate;
            }
        }
    }
}