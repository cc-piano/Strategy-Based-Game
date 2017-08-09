using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIObserver
{

    public interface IObservable
    {
        void AddObserver(IObserver o);
        void RemoveObserver(IObserver o);
        void NotifyObservers();
    }

    public class UIObeserver : IObservable
    {
        public List<IObserver> observers;
        public UIObeserver()
        {
            observers = new List<IObserver>();
        }
        public void AddObserver(IObserver o)
        {
            observers.Add(o);
        }

        public void RemoveObserver(IObserver o)
        {
            observers.Remove(o);
        }

        public void NotifyObservers()
        {
            foreach (IObserver observer in observers)
                observer.ParsingFinished();
        }
    }

    public interface IObserver
    {
        void AddToListOfObservers();
        void ParsingFinished();
    }
}
