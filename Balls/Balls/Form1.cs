using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Balls{
    public partial class Form1 : Form{
        Animator animator;
        Graphics g;
        Rectangle rect;
        int radius;
        BallProducer[] ballProducers;
        BallCommonData commonData;
        BallConsumer ballConsumer;
        bool flag = true;
        public Form1(){
            InitializeComponent();
            g = panel1.CreateGraphics();
            rect = panel1.ClientRectangle;
            animator = new Animator(g, rect);
            radius = 20;
        }
        private void panel1_Click(object sender, EventArgs e) {

            if (flag) {
                Point consumerPos = new Point(panel1.Width / 2 - radius, panel1.Height / 2 - radius);
                commonData = new BallCommonData(3, animator, consumerPos);
                ballConsumer = new BallConsumer(commonData, rect, consumerPos.X, consumerPos.Y, radius, animator);
                ballProducers = new BallProducer[3];
                ballProducers[0] = new BallProducer(commonData, 0, panel1.Width / 2 - radius, 0, radius, rect, animator, consumerPos, Color.Red);
                ballProducers[1] = new BallProducer(commonData, 1, 0, panel1.Height / 2 - radius, radius, rect, animator, consumerPos, Color.Green);
                ballProducers[2] = new BallProducer(commonData, 2, panel1.Width - 2 * radius, panel1.Height / 2 - radius, radius, rect, animator, consumerPos, Color.Blue);
                animator.Start(ballConsumer);
                animator.Start(ballProducers[0]);
                animator.Start(ballProducers[1]);
                animator.Start(ballProducers[2]);
                flag = false;
            }
            
            
            
        }
        private void Stop() {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            foreach (var ballProducer in ballProducers) ballProducer.Abort();
            ballConsumer.Abort();
            animator.Stop();
            
        }
    }
}
