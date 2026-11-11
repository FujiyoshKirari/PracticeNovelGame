using System;
using MyDIContainer;
using NUnit.Framework;

namespace TestContainer
{
    public interface IFoo { }
    public class Foo : IFoo { }

    public interface IBar { }
    public class Bar : IBar { }

    public class FooWithBar : IFoo
    {
        public IBar Bar { get; }
        public FooWithBar(IBar bar) { Bar  = bar; }
    }

    public class NeedsBar
    {
        public IBar Bar;

        [InjectAttribute]
        public void Construct(IBar bar) { Bar = bar; }
    }

    public interface IA { }
    public class A : IA { public A(IB b) { } }

    public interface IB { }
    public class B : IB { public B(IA a) { } }

    public class ContainerTests
    {
        [Test]
        public void 登録した型をResolveすると実装クラスが返る()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, Foo>(Lifetime.Singleton);
            var container = builder.Build();

            var foo = container.Resolve<IFoo>();

            Assert.IsInstanceOf<Foo>(foo);
        }

        [Test]
        public void 未登録の型をResolveすると例外になり型名がわかる()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            var ex = Assert.Throws<Exception>(() => container.Resolve<IFoo>());

            Assert.That(ex.Message, Does.Contain("IFoo"));
        }

        [Test]
        public void 同じ型を2回登録すると例外になり型名がわかる()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, Foo>(Lifetime.Singleton);

            var ex = Assert.Throws<Exception>(() => builder.Register<IFoo, Foo>(Lifetime.Singleton));
            var container = builder.Build();

            Assert.That(ex.Message, Does.Contain("IFoo"));
        }

        [Test]
        public void コンストラクタの引数が自動で注入される()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, FooWithBar>(Lifetime.Singleton);
            builder.Register<IBar, Bar>(Lifetime.Singleton);
            var container = builder.Build();

            var foo = (FooWithBar)container.Resolve<IFoo>();

            Assert.IsInstanceOf<Bar>(foo.Bar);
        }

        [Test]
        public void Singletonで登録すると毎回同じインスタンスが返る()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, Foo>(Lifetime.Singleton);
            var container = builder.Build();

            var a = container.Resolve<IFoo>();
            var b = container.Resolve<IFoo>();

            Assert.AreSame(a, b);
        }

        [Test]
        public void Transientで登録すると毎回別のインスタンスが返る()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, Foo>(Lifetime.Transient);
            var container = builder.Build();

            var a = container.Resolve<IFoo>();
            var b = container.Resolve<IFoo>();

            Assert.AreNotSame(a, b);
        }

        [Test]
        public void 循環参照があると例外になる()
        {
            var builder = new ContainerBuilder();
            builder.Register<IA, A>(Lifetime.Transient);
            builder.Register<IB, B>(Lifetime.Transient);
            var container = builder.Build();

            var ex = Assert.Throws<Exception>(() => container.Resolve<IA>());

            Assert.That(ex.Message, Does.Contain("IA"));
        }

        [Test]
        public void 依存先が未登録で失敗したあと再度Resolveしても同じ理由で失敗する()
        {
            var builder = new ContainerBuilder();
            builder.Register<IFoo, FooWithBar>(Lifetime.Transient);   // IBarは登録しない
            var container = builder.Build();

            Assert.Throws<Exception>(() => container.Resolve<IFoo>());
            var ex = Assert.Throws<Exception>(() => container.Resolve<IFoo>());

            Assert.That(ex.Message, Does.Contain("IBar"));   // 「IFooが循環」ではなく「IBar未登録」のはず
        }

        [Test]
        public void Build後にBuilderへ登録してもContainerには影響しない()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            Assert.Throws<Exception>(() => builder.Register<IFoo, Foo>(Lifetime.Transient));   // Buildのあとで登録

            Assert.Throws<Exception>(() => container.Resolve<IFoo>());
        }

        [Test]
        public void クラスで登録したらクラスで取り出せる()
        {
            var builder = new ContainerBuilder();
            builder.Register<Foo>(Lifetime.Transient);
            var container = builder.Build();

            var foo = container.Resolve<Foo>();
            Assert.IsInstanceOf<Foo>(foo);
        }

        [Test]
        public void クラスで登録しただけではインターフェースでは取り出せない()
        {
            var builder = new ContainerBuilder();
            builder.Register<Foo>(Lifetime.Transient);
            var container = builder.Build();

            var ex = Assert.Throws<Exception>(() => container.Resolve<IFoo>());
            Assert.That(ex.Message, Does.Contain("IFoo"));
        }

        [Test]
        public void Injectを付けたメソッドに依存が注入される()
        {
            var builder = new ContainerBuilder();
            builder.Register<IBar, Bar>(Lifetime.Transient);
            var container = builder.Build();

            var target = new NeedsBar();
            container.Inject(target);

            Assert.IsInstanceOf<Bar>(target.Bar);
        }

        [Test]
        public void RegisterInstanceしたオブジェクトにBuild時に注入される()
        {
            var builder = new ContainerBuilder();
            builder.Register<IBar, Bar>(Lifetime.Transient);

            var target = new NeedsBar();          // シーン上にすでにあるMonoBehaviourの代わり
            builder.RegisterInstance(target);

            builder.Build();

            Assert.IsInstanceOf<Bar>(target.Bar);
        }

        [Test]
        public void RegisterInstanceしたオブジェクトはResolveでそのものが返る()
        {
            var builder = new ContainerBuilder();
            builder.Register<IBar, Bar>(Lifetime.Transient);
            var target = new NeedsBar();
            builder.RegisterInstance(target);
            var container = builder.Build();

            Assert.AreSame(target, container.Resolve<NeedsBar>());
        }
    }
}

