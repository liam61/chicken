using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// 作者：jefferychen
/// git网址：https://gitee.com/jefferychen
/// 功能描述：吃鸡游戏跑圈机制
/// 运行环境：Visual Studio 2015
/// 创建时间：2018-1-27
/// 版本：1.0
/// 联系邮箱：871073013@qq.com
/// </summary>
namespace Chicken
{
    public partial class Form1 : Form
    {
        private Bitmap map_bg = null;

        private Graphics mGraphics = null;

        private Brush mBrush = null;

        //outer 指外圈;  inner 指内圈
        private Pen mPen_outer = null;

        private Pen mPen_inner = null;

        //理想点，但是用于画图的话还要再减去圆的r
        private PointF mPoint_outer;

        private PointF mPoint_inner;

        private float mRadius_outer = 0;

        private float mRadius_inner = 0;

        private DrawHelper mDrawHelper = null;

        private bool isStart = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //以下两句是为了设置控件样式为双缓冲，这可以有效减少闪烁的问题
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            UpdateStyles();

            this.DoubleBuffered = true; //双缓冲

            init_bg();
        }

        private void init_bg()
        {
            int width = pBox_home.Width;
            int height = pBox_home.Height;

            map_bg = new Bitmap(width, height); //设置要涂改的背景
            mGraphics = Graphics.FromImage(map_bg);

            pBox_home.Image = map_bg;   //添加背景  

            mBrush = new SolidBrush(Color.Blue);
            mPen_outer = new Pen(mBrush, 2);

            mBrush = new SolidBrush(Color.White);
            mPen_inner = new Pen(mBrush, 2);

            mPoint_outer = new Point(width / 2, height / 2);

            mDrawHelper = new DrawHelper(mGraphics);

            mRadius_outer = mDrawHelper.LevelList[0];
        }

        private void Menu_start_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            Menu_start.Enabled = false;
            Menu_pause.Enabled = true;

            if (!isStart)
            {
                timer2.Enabled = true;
                //mPoint_inner = PointOfRandom(mPoint_outer, mRadius_outer, mRadius_inner);

                //drawCircle(mPen_inner, mPoint_inner, mRadius_inner);

                label1.Text = "毒圈级别:" + (mDrawHelper.Level + 1) + ", " + mDrawHelper.Interval + "秒后开始缩圈!";
                label1.Location = new Point((pBox_home.Width - label1.Width) / 2, (pBox_home.Height - label1.Height) / 2);
                //pBox_home.Refresh();
                this.Refresh();
                //System.Threading.Thread.Sleep(5000);
            }
            isStart = true;
        }

        private void Menu_pause_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            Menu_start.Enabled = true;
            Menu_pause.Enabled = false;

            label1.Text = "游戏已暂停!";
            this.Refresh();
        }

        private void Menu_exit_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            mGraphics.Clear(Color.LightSteelBlue);

            mRadius_outer--;
            label1.Text = "";

            if (!mDrawHelper.isIntersect(mPoint_outer, mRadius_outer, mPoint_inner, mRadius_inner))
            {
                mDrawHelper.drawCircle(mPen_outer, mPoint_outer, mRadius_outer);
                mDrawHelper.drawCircle(mPen_inner, mPoint_inner, mRadius_inner);
            }
            else
            {
                if (mRadius_outer != mRadius_inner)  //外圈和内圈圆心重合,半径相同
                {
                    // 图三过程
                    // k = y/x
                    // y = kx
                    // x^2+y^y = 1
                    // x^2 = 1/(k^2+1)
                    float k = (mPoint_outer.Y - mPoint_inner.Y) / (mPoint_outer.X - mPoint_inner.X);

                    float x_off = 1 * (float)Math.Sqrt(1 / (k * k + 1));

                    // k<0  x+x_off
                    mPoint_outer.X += 1 * (mPoint_outer.X < mPoint_inner.X ? 1 : -1) * x_off;
                    mPoint_outer.Y = k * (mPoint_outer.X - mPoint_inner.X) + mPoint_inner.Y;

                    //reCalcPoint_outer(mPoint_outer, mPoint_inner);

                    mDrawHelper.drawCircle(mPen_outer, mPoint_outer, mRadius_outer);
                    mDrawHelper.drawCircle(mPen_inner, mPoint_inner, mRadius_inner);
                }
                else
                {
                    timer2.Enabled = true;

                    mDrawHelper.drawCircle(mPen_outer, mPoint_outer, mRadius_outer);

                    label1.Text = "毒圈级别:" + (mDrawHelper.Level + 1) + ", " + mDrawHelper.Interval + "秒后开始缩圈!";

                    this.Refresh();
                }
            }
            pBox_home.Refresh();
        }

        /// <summary>
        /// 后台时间表，用于毒圈（外圈）和内圈重合后，暂停时间以及生成下一个内圈圆心和内圈半径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (mDrawHelper.Level < mDrawHelper.LevelList.Count - 1 && mRadius_outer == mDrawHelper.LevelList[mDrawHelper.Level])
            {
                mDrawHelper.Level++;
                mRadius_inner = mDrawHelper.LevelList[mDrawHelper.Level];
                mPoint_inner = mDrawHelper.PointOfRandom(mPoint_outer, mRadius_outer, mRadius_inner);

                mDrawHelper.drawCircle(mPen_inner, mPoint_inner, mRadius_inner);
                pBox_home.Refresh();
                timer2.Enabled = false;

                System.Threading.Thread.Sleep(mDrawHelper.Interval*1000);
            }
            else
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                mGraphics.Clear(Color.LightSteelBlue);
                pBox_home.Refresh();
                label1.Text = "游戏结束啦！~~";
            }
        }
    }
}
