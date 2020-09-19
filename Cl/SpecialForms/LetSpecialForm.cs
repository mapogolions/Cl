using System;
using System.Collections.Generic;
using System.Linq;
using Cl.Contracts;
using Cl.Extensions;
using Cl.Types;

namespace Cl.SpecialForms
{
    internal class LetSpecialForm : TaggedSpecialForm
    {
        internal LetSpecialForm(ClObj cdr) : base(ClSymbol.Let, cdr) { }

        public override IContext Reduce(IContext ctx)
        {
            var tail = BuiltIn.Tail(Cdr);
            if (tail == ClCell.Nil || BuiltIn.Cdr(tail) != ClCell.Nil)
                throw new InvalidOperationException(Errors.Eval.InvalidLetBodyFormat);
            var beginBody = VariableDefinitionExpressions()
                .Append(BuiltIn.Car(tail)).ListOf();
            var begin = new ClCell(ClSymbol.Begin, beginBody);
            var lambda = BuiltIn.ListOf(ClSymbol.Lambda, ClCell.Nil, begin);
            return BuiltIn.ListOf(lambda).Reduce(ctx);
        }

        private IEnumerable<ClObj> VariableDefinitionExpressions()
        {
            var pairs = BuiltIn.Head(Cdr).Cast<ClCell>();
            var bindings = BuiltIn.Seq(pairs)
                .Select(x => {
                    var cell = x.CastOrThrow<ClCell>(Errors.Eval.InvalidBindingsFormat);
                    if (cell == ClCell.Nil)
                        throw new InvalidOperationException(Errors.Eval.InvalidBindingsFormat);
                    if (BuiltIn.Cdr(cell) == ClCell.Nil || BuiltIn.Cddr(cell) != ClCell.Nil)
                        throw new InvalidOperationException(Errors.Eval.InvalidBindingsFormat);
                    return cell;
                });
            return bindings
                .Select(x => BuiltIn.ListOf(ClSymbol.Define, BuiltIn.First(x), BuiltIn.Second(x)));
        }
    }
}
