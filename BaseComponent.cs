using BitcoinDayTrader.Services;
using Microsoft.AspNetCore.Components;

namespace BitcoinDayTrader.Components
{
    public class BaseComponent : ComponentBase
    {
        [Inject] RenderManager _renderManager { get; set; }
        protected override void OnAfterRender(bool firstRender)
        {
            _renderManager.CountRender(this);
            base.OnAfterRender(firstRender);
        }

        public bool DefaultShouldRender = false;
        //protected bool RenderNow = false;
        //protected override bool ShouldRender()
        //{
        //    if (DefaultShouldRender)
        //        return true;

        //    if (RenderNow)
        //    {
        //        RenderNow = false;
        //        return true;
        //    }
        //    return false;
        //}
    }
}
