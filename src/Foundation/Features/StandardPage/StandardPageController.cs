﻿using EPiServer.DataAbstraction;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace Foundation.Features.StandardPage
{
    public class StandardPageController : PageController<StandardPage>
    {
        private readonly CategoryRepository _categoryRepository;

        public StandardPageController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public ActionResult Index(StandardPage currentPage)
        {
            var model = StandardPageViewModel.Create(currentPage, _categoryRepository);
            return View(model);
        }
    }
}