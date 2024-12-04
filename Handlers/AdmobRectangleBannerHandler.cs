using MaCamp.CustomControls;

namespace MaCamp.Handlers
{
    public partial class AdmobRectangleBannerHandler
    {
        private static IPropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler> Mapper => new PropertyMapper<AdmobRectangleBannerView, AdmobRectangleBannerHandler>(ViewMapper);

        public AdmobRectangleBannerHandler() : base(Mapper)
        {
        }
    }
}