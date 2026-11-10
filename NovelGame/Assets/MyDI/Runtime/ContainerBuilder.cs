using System;
using System.Collections.Generic;

namespace MyDIContainer
{
    public class ContainerBuilder
    {
        private readonly Dictionary<Type, Registration> _registers = new Dictionary<Type, Registration>();
        private bool _isBuilt = false;

        /// <summary>
        /// インターフェースと実態のクラスを設定する関数
        /// </summary>
        /// <typeparam name="TInterface">インターフェース</typeparam>
        /// <typeparam name="TClass">実態のクラス</typeparam>
        /// <param name="lifetime">ライフタイム</param>
        /// <exception cref="Exception"></exception>
        public void Register<TInterface, TClass>(Lifetime lifetime) where TClass : TInterface
        {
            if (_isBuilt is true)
            {
                throw new Exception($"{typeof(TInterface).Name} はビルド後に追加しようとしています。");
            }

            if (_registers.ContainsKey(typeof(TInterface)) is false)
            {
                var registration = new Registration(typeof(TClass), lifetime);
                _registers.Add(typeof(TInterface), registration);
            }
            else
            {
                throw new Exception($"{typeof(TInterface).Name}は既に登録されていました。");
            }
        }

        public Container Build()
        {
            var registers = new Dictionary<Type, Registration>(_registers);
            _isBuilt = true;
            return new Container(registers);
        }
    }
}
