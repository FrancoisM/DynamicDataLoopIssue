using System;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using ReactiveUI.Testing;
using Xunit;

namespace DynamicDataLoopIssue
{
    public class WithLoop
    {
        [Fact]
        public void Test_DD_loop()
        {
            new TestScheduler().With(scheduler =>
            {
                var counter = 0;
                var facturationList = new SourceList<FakeViewModel>();
                var disposable1 = facturationList
                    .Connect()
                    .Bind(out var facturations)
                    .DisposeMany()
                    .Subscribe();
                var htChanged = facturationList
                    .Connect()
                    .WhenValueChanged(p => p.InnerViewModel.Quantity);
                var disposable2 = htChanged.Subscribe(_ => counter++);

                facturationList.Add(new FakeViewModel(1));

                counter.Should().Be(1);
                disposable1.Dispose();
                disposable2.Dispose();
            });
        }

        private class FakeViewModel : ReactiveObject
        {
            public FakeViewModel(int quantity) => InnerViewModel = new FakeInnerViewModel(quantity);

            public IFakeInnerViewModel InnerViewModel { get; }

        }

        private class FakeInnerViewModel : ReactiveObject, IFakeInnerViewModel
        {
            public FakeInnerViewModel(int quantity)
            {
                Quantity = quantity;
            }

            public int Quantity { get; }
        }

        public interface IFakeInnerViewModel
        {
            int Quantity { get; }
        }
    }
}
