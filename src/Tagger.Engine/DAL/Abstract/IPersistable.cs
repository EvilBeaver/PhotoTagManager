﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Tagger.Engine.DAL
{
    interface IPersistable
    {
        Identifier Key { get; set; }
    }
}
