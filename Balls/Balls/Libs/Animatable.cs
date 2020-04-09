
using System;
using System.Drawing;
using System.Threading;

namespace Balls{
    public abstract class Animatable{
        /** Abstract class implementing animate-able object **/
        
        private Thread _thread;
        public  bool IsAlive => _thread != null && _thread.IsAlive;
        protected bool _stop;

        protected Animatable(Rectangle rectangle) {
            Update(rectangle);
        }
        public abstract void Update(Rectangle rectangle);
        public void Start(){
            if (_thread != null && _thread.IsAlive) return;
            _stop = false;
            ThreadStart th = Move;
            _thread = new Thread(th);
            _thread.Start();

        } 
        public abstract void Draw(Graphics g); // Object drawing method
      
        protected abstract void Move();  // Object changing method
        public void Stop() {
            _stop = true;
        }
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