using Cl.Contracts;
using Cl.Types;
using NUnit.Framework;

namespace Cl.Tests.EvaluatorTests
{
    [TestFixture]
    public class EvalLetTests
    {
        private IEnv _env;
        private IContext _ctx;

        [SetUp]
        public void BeforeEach()
        {
            _env = new Env();
            _ctx = new Context(_env);
        }

        /**
         * (let ((foo 1)
         *       (bar foo))
         *   bar)
         */
        [Test]
        public void EvalLet_EarlyCanAssingLater()
        {
            var bindings = BuiltIn.ListOf(BuiltIn.ListOf(Var.Foo, Value.One),
                BuiltIn.ListOf(Var.Bar, Var.Foo));
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Var.Bar);

            var ctx = expr.Reduce(_ctx);

            Assert.That(ctx.Value, Is.EqualTo(Value.One));
        }

        /**
         * (let () 1)
         */
        [Test]
        public void EvalLet_LikeConstantFunction()
        {
            var expr = BuiltIn.ListOf(ClSymbol.Let, ClCell.Nil, Value.One);
            var ctx = expr.Reduce(_ctx);
            Assert.That(ctx.Value, Is.EqualTo(Value.One));
        }

        /**
         * (let ((foo #t)
         *       (bar #f)
         *       ()) 1)
         */
         [Test]
         public void EvalLet_ThrowException_WhenAtLeastOneBindingIsNil()
         {
            var foo = BuiltIn.ListOf(Var.Foo, ClBool.True);
            var bar = BuiltIn.ListOf(Var.Bar, ClBool.False);
            var bindings = BuiltIn.ListOf(foo, bar, ClCell.Nil);
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Value.One);
            var errorMessage = Errors.Eval.InvalidBindingsFormat;

            Assert.That(() => expr.Reduce(_ctx),
                Throws.InvalidOperationException.With.Message.EqualTo(errorMessage));
         }


        /**
         * (let (()) 1)
         */
        [Test]
        public void EvalLet_ThrowException_WhenBindingIsNil()
        {
            var bindings = BuiltIn.ListOf(ClCell.Nil);
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Value.One);
            var errorMessage = Errors.Eval.InvalidBindingsFormat;

            Assert.That(() => expr.Reduce(_ctx),
                Throws.InvalidOperationException.With.Message.EqualTo(errorMessage));
        }

        /**
         * (let ((foo #t #f)) foo)
         */
        [Test]
        public void EvalLet_ThrowExceptionWhenTooManyValuesArePassedForBinding()
        {
            var bindings = BuiltIn.ListOf(BuiltIn.ListOf(Var.Foo, ClBool.True, ClBool.False));
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Var.Foo);
            var errorMessage = Errors.Eval.InvalidBindingsFormat;

            Assert.That(() => expr.Reduce(_ctx),
                Throws.InvalidOperationException.With.Message.EqualTo(errorMessage));
        }


        /**
         * (let ((foo 1)) foo)
         */
        [Test]
        public void EvalLet_LikeIdentityFunction()
        {
            var bindings = BuiltIn.ListOf(BuiltIn.ListOf(Var.Foo, Value.One));
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Var.Foo);

            var ctx = expr.Reduce(_ctx);

            Assert.That(ctx.Value, Is.EqualTo(Value.One));
        }

        /**
         * (let ((foo 1))
         * ;; empty body
         *               )
         */
        [Test]
        public void EvalLet_ThrowException_WhenBodyIsEmpty()
        {
            var bindings = BuiltIn.ListOf(BuiltIn.ListOf(Var.Foo, Value.One));
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings);
            var errorMessage = Errors.Eval.InvalidLetBodyFormat;

            Assert.That(() => expr.Reduce(_ctx),
                Throws.InvalidOperationException.With.Message.EqualTo(errorMessage));
        }

        /**
         * (let ((foo "foo"))
         *   foo #f)
         */
        [Test]
        public void EvalLet_ThrowException_WhenCompoundBody()
        {
            var bindings = BuiltIn.ListOf(BuiltIn.ListOf(Var.Foo, Value.One));
            var expr = BuiltIn.ListOf(ClSymbol.Let, bindings, Var.Foo, ClBool.False);
            var errorMessage = Errors.Eval.InvalidLetBodyFormat;

            Assert.That(() => expr.Reduce(_ctx),
                Throws.InvalidOperationException.With.Message.EqualTo(errorMessage));
        }
    }
}