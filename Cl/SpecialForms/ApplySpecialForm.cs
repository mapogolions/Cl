using System.Collections.Generic;
using System.Linq;
using Cl.Contracts;
using Cl.Extensions;
using Cl.Types;

namespace Cl.SpecialForms
{
    internal class ApplySpecialForm : ClCell
    {
        internal ApplySpecialForm(ClCallable car, IClObj cdr) : base(car, cdr) { }

        public override IContext Reduce(IContext ctx)
        {
            var (args, env) = EvalArgs(ctx);
            if (Car is NativeFn nativeFn)
            {
                var value = nativeFn.Fn.Invoke(args.ToArray());
                return new Context(value, env);
            }
            var fn = (ClFn) Car;
            fn.LexicalEnv.Bind(BuiltIn.Seq(fn.Varargs), args);
            var (result, _) = fn.Body.Reduce(new Context(fn.LexicalEnv));
            return ctx.FromResult(result);
        }

        private (IEnumerable<IClObj>, IEnv) EvalArgs(IContext ctx)
        {
            var obj = Cdr.CastOrThrow<ClCell>(Errors.Eval.InvalidFunctionCall);
            var (reversedArgs, env) = BuiltIn.Seq(obj)
                .Aggregate<IClObj, IContext>(
                    ctx.FromResult(Nil.Given),
                    (ctx, arg) => {
                        var values = ctx.Value;
                        var (value, env) = arg.Reduce(ctx);
                        return new Context(new ClCell(value, values), env);
                    });
            return (BuiltIn.Seq(reversedArgs).Reverse(), env);
        }
    }
}
