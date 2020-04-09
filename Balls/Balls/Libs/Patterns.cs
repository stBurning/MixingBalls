using System;
using System.Collections.Generic;
using System.Threading;

namespace Balls{
    public abstract class CommonData<T>{
        protected static int MaxSize;
        protected readonly Queue<T>[] Vals;

        public CommonData(int maxSize) {
            MaxSize = maxSize;
            Vals = new Queue<T>[3];

            for (int i = 0; i < 3; i++) {
                Vals[i] = new Queue<T>();
            }
        }
        public void Add(int index, T value) {
            index = Math.Abs(index % 3);
            var q = Vals[index];
            Monitor.Enter(q);
            try {
                while (q.Count >= MaxSize) { Monitor.Wait(q); }
                q.Enqueue(value);
                Monitor.PulseAll(q);
            }catch (Exception e) { Console.WriteLine(e.StackTrace); }
            finally { Monitor.Exit(q); }
        }
        public abstract T[] GetNextData();
    }
    
    public abstract class Producer<T>{
        
        public static readonly int ValNum = 3;
        protected readonly int ValIndex;
        private Thread _thread;
        protected readonly CommonData<T> Data;

        protected Producer(CommonData<T> d, int valIndex) {
            this.ValIndex = Math.Abs(valIndex % ValNum);
            this.Data = d;
            //Start();
        }
        protected void Start() {
            ThreadStart th = Produce;
            _thread = new Thread(th);
            _thread.Start();
        }

        protected abstract void Produce();
        public void Abort() {
            try {
                _thread.Abort();
                _thread.Join();
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
    public abstract class Consumer<T>{
        
        private Thread _thread;
        protected readonly CommonData<T> Data;
        protected Consumer(CommonData<T> data) {
            Data = data;
            Start();
        }
        private void Start() {
            ThreadStart th = Consume;
            _thread = new Thread(th);
            _thread.Start();
        }
        protected abstract void Consume();

        public void Abort() {
            try {
                _thread.Abort();
                _thread.Join();
            } catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
    
    

    
}