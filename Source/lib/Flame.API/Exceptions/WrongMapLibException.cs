﻿namespace FlameAPI.Exceptions
{
    using System;

    public class WrongMapLibException : Exception
    {
        public WrongMapLibException(string message) : base(message) { }
    }
}