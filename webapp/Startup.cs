﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(SmartAdminMvc.Startup))]

namespace SmartAdminMvc
{



        public partial class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                ConfigureAuth(app);
            }
        }
    }

