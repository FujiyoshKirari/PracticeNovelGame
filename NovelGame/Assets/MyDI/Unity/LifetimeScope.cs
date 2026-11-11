using MyDIContainer;
using UnityEngine;

namespace MyDIContainer
{
    [DefaultExecutionOrder(-5000)]
    public abstract class LifetimeScope : MonoBehaviour
    {
        public Container Container { get; private set; }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            ContainerBuilder builder = new ContainerBuilder();
            Configure(builder);

            Container = builder.Build();
        }

        protected abstract void Configure(IBuilder builder);
    }
}