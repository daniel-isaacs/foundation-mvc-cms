using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace Foundation.Features.Preview
{
    [TemplateDescriptor(
        Inherited = true,
        TemplateTypeCategory = TemplateTypeCategories.MvcController, //Required as controllers for blocks are registered as MvcPartialController by default
        Tags = new[] { RenderingTags.Preview, RenderingTags.Edit },
        AvailableWithoutTag = false)]
    public class PreviewController : ActionControllerBase, IRenderTemplate<BlockData>
    {
        private readonly PreviewControllerHelper _previewControllerHelper;

        public PreviewController(PreviewControllerHelper previewControllerHelper)
        {
            _previewControllerHelper = previewControllerHelper;
        }

        public ActionResult Index(IContent currentContent)
        {
            return _previewControllerHelper.RenderResult(currentContent);
        }
    }
}