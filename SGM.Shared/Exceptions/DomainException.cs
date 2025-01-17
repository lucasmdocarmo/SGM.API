﻿using SGM.Shared.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGM.Shared.Core.Excepctions
{
    public sealed class DomainException : InternalAppException
    {
        public override string Message { get; }
        public DomainException(string message) : base(message)
        {
            this.Message = message;
        }
    }
}
