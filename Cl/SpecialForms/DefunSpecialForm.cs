using Cl.Contracts;
using Cl.Types;

namespace Cl.SpecialForms
{
    internal class DefunSpecialForm : TaggedSpecialForm
    {
        internal DefunSpecialForm(ClObj cdr) : base(ClSymbol.Defun, cdr) { }

        public override IContext Reduce(IContext ctx)
        {
            var funcName = BuiltIn.Head(Cdr);
            var lambda = new ClCell(ClSymbol.Lambda, BuiltIn.Tail(Cdr));
            return BuiltIn.ListOf(ClSymbol.Define, funcName, lambda).Reduce(ctx);
        }
    }
}
