using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.FrontEnd.Components
{
    public class ConnectedViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await StateManager.CheckConnectionAsync(500);
            return View();
        }
    }
}
