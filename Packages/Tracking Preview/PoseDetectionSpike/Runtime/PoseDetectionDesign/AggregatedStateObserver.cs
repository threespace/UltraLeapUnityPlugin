using System;

public class AggregatedStateObserver : IStateObserver
{
    public bool IsObserved => throw new NotImplementedException();

    public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event EventHandler OnStateObserved;
    public event EventHandler OnStateLost;
}