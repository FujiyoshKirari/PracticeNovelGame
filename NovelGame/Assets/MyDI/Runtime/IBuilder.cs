namespace MyDIContainer
{
    public interface IBuilder
    {
        /// <summary>
        /// インターフェースと実態のクラスを設定する関数
        /// </summary>
        /// <typeparam name="TInterface">インターフェース</typeparam>
        /// <typeparam name="TClass">実態のクラス</typeparam>
        /// <param name="lifetime">ライフタイム</param>
        /// <exception cref="Exception"></exception>
        void Register<TInterface, TClass>(Lifetime lifetime) where TClass : TInterface;

        /// <summary>
        /// クラスを登録する関数
        /// </summary>
        /// <typeparam name="T">登録するクラス</typeparam>
        /// <param name="lifetime"></param>
        void Register<TClass>(Lifetime lifetime);

        /// <summary>
        /// シーン上のオブジェクトを登録する関数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterInstance<T>(T instance);
    }
}