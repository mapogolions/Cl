using System;
using Cl.Abs;
using Cl.Types;
using NUnit.Framework;
using static Cl.Extensions.FpUniverse;

namespace Cl.Tests.EvaluatorTests
{
    [TestFixture]
    public class EvalDefinitionTests
    {
        [Test]
        public void EvalDefinition_CreateSharedReference()
        {
            var a = new ClSymbol("a");
            var b = new ClSymbol("b");
            var env = new Env();
            env.Bind(b, new ClString("foo"));
            var evaluator = new Evaluator(env);

            var expr = BuiltIn.ListOf(ClSymbol.Define, a, b);
            Ignore(evaluator.EvalDefinition(expr));

            Assert.That(Object.ReferenceEquals(env.Lookup(a), env.Lookup(b)), Is.True);
        }

        [Test]
        public void EvalDefinition_ThrowException_WhenRighSideVariableDoesNotExist()
        {
            var evaluator = new Evaluator(new Env());
            var expr = BuiltIn.ListOf(ClSymbol.Define, new ClSymbol("a"), new ClSymbol("b"));

            Assert.That(() => evaluator.EvalDefinition(expr),
                Throws.InvalidOperationException.With.Message.EqualTo("Unbound variable"));
        }

        [Test]
        public void EvalDefinition_ScopeRules()
        {
            var a = new ClSymbol("a");
            var outerScope = new Env();
            outerScope.Bind(a, ClBool.True);
            var innerScope = new Env(outerScope);
            innerScope.Bind(a, ClBool.False);
            var evaluator = new Evaluator(innerScope);

            var expr = BuiltIn.ListOf(ClSymbol.Define, a, new ClFixnum(0));
            Ignore(evaluator.EvalDefinition(expr));

            Assert.That(innerScope.Lookup(a), Is.EqualTo(new ClFixnum(0)));
            Assert.That(outerScope.Lookup(a), Is.EqualTo(ClBool.True));
        }
        [Test]
        public void EvalDefinition_OverrideExistingBinding()
        {
            var env = new Env();
            var a = new ClSymbol("a");
            env.Bind(a, ClBool.True);
            var evaluator = new Evaluator(env);

            var expr = BuiltIn.ListOf(ClSymbol.Define, a, ClBool.False);
            Ignore(evaluator.EvalDefinition(expr));

            Assert.That(env.Lookup(a), Is.EqualTo(ClBool.False));
        }

        [Test]
        public void EvalDefinition_ReturnNil_WhenOperationIsSuccessful()
        {
            var evaluator = new Evaluator(new Env());
            var expr = BuiltIn.ListOf(ClSymbol.Define, new ClSymbol("a"), ClBool.False);


            Assert.That(evaluator.EvalDefinition(expr), Is.EqualTo(Nil.Given));
        }

        [Test]
        public void EvalDefinition_CreateNewBinding_WhenEnvironmentDoesNotContainBinding()
        {
            var env = new Env();
            var evaluator = new Evaluator(env);
            var a = new ClSymbol("a");

            var expr = BuiltIn.ListOf(ClSymbol.Define, a, ClBool.True);
            Ignore(evaluator.EvalDefinition(expr));

            Assert.That(env.Lookup(a), Is.EqualTo(ClBool.True));
        }
    }
}