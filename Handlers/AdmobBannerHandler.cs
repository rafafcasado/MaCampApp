using MaCamp.CustomControls;

namespace MaCamp.Handlers
{
    public partial class AdmobBannerHandler
    {
        private static IPropertyMapper<AdMobBannerView, AdmobBannerHandler> Mapper => new PropertyMapper<AdMobBannerView, AdmobBannerHandler>(ViewMapper);

        public AdmobBannerHandler() : base(Mapper)
        {
        }
    }
}