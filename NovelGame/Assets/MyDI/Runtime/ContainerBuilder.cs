using System;
using System.Collections.Generic;

namespace MyDIContainer
{
    public class ContainerBuilder : IBuilder
    {
        private readonly Dictionary<Type, Registration> _registers = new Dictionary<Type, Registration>();
        private bool _isBuilt = false;
        private readonly List<object> _needInjectObjects = new List<object>();

        /// <summary>
        /// インターフェースと実態のクラスを設定する関数
        /// </summary>
        /// <typeparam name="TInterface">インターフェース</typeparam>
        /// <typeparam name="TClass">実態のクラス</typeparam>
        /// <param name="lifetime">ライフタイム</param>
        /// <exception cref="Exception"></exception>
        public void Register<TInterface, TClass>(Lifetime lifetime) where TClass : TInterface
        {
            var registration = new Registration(typeof(TClass), lifetime);

            AddRegistration(typeof(TInterface), registration);
        }

        /// <summary>
        /// クラスを登録する関数
        /// </summary>
        /// <typeparam name="T">登録するクラス</typeparam>
        /// <param name="lifetime"></param>
        public void Register<T>(Lifetime lifetime)
        {
            Register<T, T>(lifetime);
        }


        /// <summary>
        /// シーン上のオブジェクトを登録する関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void RegisterInstance<T>(T instance)
        {
            if (instance == null)
            {
                throw new Exception($"{typeof(T).Name} がnullで登録しようとされました。");
            }
            var type = typeof(T);
            var registrations = new Registration(type, Lifetime.Singleton);
            registrations.SingletonObj = instance;

            AddRegistration(type, registrations);
            _needInjectObjects.Add(instance);
        }

        /// <summary>
        /// 既に登録済みかを確認して辞書に追加する関数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="registration"></param>
        /// <exception cref="Exception"></exception>
        private void AddRegistration(Type key, Registration registration)
        {
            if (_isBuilt is true)
            {
                throw new Exception($"{key.Name} はビルド後に追加しようとしています。");
            }

            if (_registers.ContainsKey(key) is false)
            {
                _registers.Add(key, registration);
            }
            else
            {
                throw new Exception($"{key.Name}は既に登録されていました。");
            }
        }

        public Container Build()
        {
            var registers = new Dictionary<Type, Registration>(_registers);
            _isBuilt = true;
            return new Container(registers, _needInjectObjects);
        }
    }
}
