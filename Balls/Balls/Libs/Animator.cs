﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Balls{
    public class Animator{
        private Graphics _mainG;
        private int _width, _height;
        private readonly List<Animatable> _items = new List<Animatable>();
        private Thread _t;
        private bool _stop;
        private BufferedGraphics _bg;
        private bool _bgChanged;
        private readonly object _obj = new object();
        public Animator(Graphics g, Rectangle r) {
            Update(g, r);
        }

        public void Update(Graphics g, Rectangle r) {
            _mainG = g;
            _width = r.Width;
            _height = r.Height;
            Monitor.Enter(_obj);
            _bgChanged = true;
            _bg = BufferedGraphicsManager.Current.Allocate(
                _mainG,
                new Rectangle(0, 0, _width, _height)
            );
            Monitor.Exit(_obj);
            Monitor.Enter(_items);
            foreach (var item in _items) {
                item.Update(new Rectangle(0, 0, _width, _height));
            }
            Monitor.Exit(_items);
        }

        private void Animate() {
            while (!_stop) {
                Monitor.Enter(_obj);
                _bgChanged = false;
                Graphics g = _bg.Graphics;
                
                Monitor.Exit(_obj);
                g.Clear(Color.White);

                Monitor.Enter(_items);
                int cnt = _items.Count;
                for (int i = 0; i < cnt; i++) {
                    if (!_items[i].IsAlive) {
                        _items.Remove(_items[i]);
                        i--;
                        cnt--;
                    }
                }
                foreach (var item in _items) {item.Draw(g); }
                Monitor.Exit(_items);
                Monitor.Enter(_obj);
                if (!_bgChanged) {
                    try { _bg.Render(); }
                    catch (Exception) {// ignored
                    }
                }
                Monitor.Exit(_obj);
                Thread.Sleep(30);
            }
        }

        public void Start(Animatable item) { 
            if (_t == null || !_t.IsAlive) {
                _stop = false;
                ThreadStart th = Animate;
                _t = new Thread(th);
                _t.Start();
            }
            Monitor.Enter(_items);
            Thread.Sleep(15);
            item.Start();
            _items.Add(item);
            Monitor.Exit(_items);
        }

        public void Stop() {
            _stop = true;
            Monitor.Enter(_items);
            foreach (var item in _items) { item.Stop();}
            _items.Clear();
            Monitor.Exit(_items);
        }
    }
}
