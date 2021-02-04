using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ASC.Utilities.Navigation
{
    /// <summary>
    /// ViewComponet para Menus de navegação dinâmica
    /// </summary>
    [ViewComponent(Name = "ASC.Utilities.Navigation.LeftNavigation")]
    public class LeftNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(NavigationMenu menu)
        {
            menu.MenuItems = menu.MenuItems.OrderBy(p => p.Sequence).ToList();
            return View(menu);
        }
    }
}
