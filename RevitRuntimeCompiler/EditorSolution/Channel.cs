﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorSolution
{
    public class Channel
    {
        public Task WriteAsync(string message) => Task.CompletedTask;
    }
}
