using System;
using Cl.Contracts;
using Cl.Types;
using NUnit.Framework;
using static Cl.Extensions.FpUniverse;

namespace Cl.Tests.EvaluatorTests
{
    [TestFixture]
    public class EvalDefinitionTests
    {
        private IEnv _env;
        private IContext _context;

        [SetUp]
        public void BeforeEach()
        {
            _env = new Env();
            _context = new Context(_env);
        }

        [Test]
        public void EvalDefinition_CreateSharedReference()
        {
            _env.Bind(Var.Bar, Value.Bar);
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, Var.Bar);

            Ignore(expr.Reduce(_context));

            Assert.That(Object.ReferenceEquals(_env.Lookup(Var.Foo), _env.Lookup(Var.Bar)), Is.True);
        }

        [Test]
        public void EvalDefinition_ThrowException_WhenRighSideVariableDoesNotExist()
        {
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, Var.Bar);
            Assert.That(() => expr.Reduce(_context),
                Throws.InvalidOperationException.With.Message.StartWith("Unbound variable"));
        }

        [Test]
        public void EvalDefinition_ScopeRules()
        {
            var outEnv = new Env((Var.Foo, Value.Foo));
            var inEnv = new Env(outEnv);
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, Value.One);

            Ignore(expr.Reduce(_context.FromEnv(inEnv)));

            Assert.That(inEnv.Lookup(Var.Foo), Is.EqualTo(Value.One));
            Assert.That(outEnv.Lookup(Var.Foo), Is.EqualTo(Value.Foo));
        }

        [Test]
        public void EvalDefinition_OverrideExistingBinding()
        {
            _env.Bind(Var.Foo, ClBool.True);
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, Value.Foo);

            var context = expr.Reduce(_context);

            Assert.That(context.Env.Lookup(Var.Foo), Is.EqualTo(Value.Foo));
        }

        [Test]
        public void EvalDefinition_ReturnNil_WhenOperationIsSuccessful()
        {
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, ClBool.False);
            var context = expr.Reduce(_context);
            Assert.That(context.Result, Is.EqualTo(Nil.Given));
        }

        [Test]
        public void EvalDefinition_CreateNewBinding()
        {
            var expr = BuiltIn.ListOf(ClSymbol.Define, Var.Foo, ClBool.True);
            var context = expr.Reduce(_context);
            Assert.That(context.Env.Lookup(Var.Foo), Is.EqualTo(ClBool.True));
        }
    }
}
