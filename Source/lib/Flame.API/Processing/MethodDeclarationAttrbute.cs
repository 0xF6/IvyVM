namespace Flame.API.Processing
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public class MethodDeclarationAttrbute : DeclarationAttribute
    {
        private readonly bool _isGlobal;

        public MethodDeclarationAttrbute(bool isGlobal)
        {
            _isGlobal = isGlobal;
        }
    }
}