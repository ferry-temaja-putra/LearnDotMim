﻿using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthSyncServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private WebProxyServerProvider webProxyServer;

        public SyncController(WebProxyServerProvider proxy)
        {
            webProxyServer = proxy;
        }

        [HttpPost]
        public async Task Post()
        {
            await webProxyServer.HandleRequestAsync(HttpContext);
        }
    }
}
