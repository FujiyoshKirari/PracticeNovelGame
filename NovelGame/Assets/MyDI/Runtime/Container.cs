using System;
using System.Collections.Generic;

namespace MyDIContainer
{
    public enum Lifetime
    {
        Singleton,
        Transient,
    }

    /// <summary>
    /// 生成したいクラスのタイプと、ライフタイム設定を保存しておくクラス
    /// </summary>
    internal class Registration
    {
        public Type Type { get; }
        public Lifetime Lifetime { get; }
        /// <summary>
        /// シングルトンの場合のインスタンスを保存
        /// </summary>
        public object SingletonObj { get; set; }

        /// <summary>
        /// 無限にインスタンスを生成し続けてしまうバグを防止するためのフラグ
        /// </summary>
        public bool IsResolving { get; set; }

        public Registration(Type type, Lifetime lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }
    }

    public class Container
    {
        private readonly Dictionary<Type, Registration> _registers;

        internal Container(Dictionary<Type, Registration> registers)
        {
            _registers = registers;
        }

        /// <summary>
        /// インターフェースからインスタンスを返す関数
        /// </summary>
        /// <typeparam name="T">インターフェース</typeparam>
        /// <returns>インスタンス</returns>
        public TInterface Resolve<TInterface>()
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        /// <summary>
        /// インターフェースのタイプからインスタンスを返す関数
        /// </summary>
        /// <param name="type">インターフェースのタイプ</param>
        /// <returns>インスタンス</returns>
        /// <exception cref="Exception"></exception>
        public object Resolve(Type type)
        {
            if (_registers.TryGetValue(type, out var registration) is false)
            {
                throw new Exception($"{type.Name}が登録されていません");
            }
            if (registration.IsResolving is true)
            {
                throw new Exception($"{type.Name}が再帰的に生成されようとしています。");
            }

            try
            {
                registration.IsResolving = true;

                switch (registration.Lifetime)
                {
                    case Lifetime.Singleton:
                        registration.SingletonObj ??= CreateInstance(registration.Type);
                        return registration.SingletonObj;
                    case Lifetime.Transient:
                        var instance = CreateInstance(registration.Type);
                        return instance;
                }
                throw new Exception($"{registration.Lifetime}は設定されてません");
            }
            finally
            {
                registration.IsResolving = false;
            }
        }

        /// <summary>
        /// クラスのタイプからインスタンスを生成する
        /// </summary>
        /// <param name="type">クラスのタイプ</param>
        /// <returns></returns>
        private object CreateInstance(Type type)
        {
            var constructor = type.GetConstructors()[0];

            var parameters = constructor.GetParameters();

            object[] args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                var p = Resolve(parameterType);
                args[i] = p;
            }

            var instance = constructor.Invoke(args);
            return instance;
        }
    }
}

