using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class DrawHelper
    {
        private List<int> levelList = new List<int>();

        private int level = 0;

        private int interval = 2;    //每次时间间隔 以秒为单位

        private Random random = new Random(DateTime.Now.Millisecond);

        private Graphics mGraphics = null;

        public List<int> LevelList
        {
            get
            {
                return levelList;
            }
        }

        public int Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public int Interval
        {
            get
            {
                return interval;
            }
        }

        public DrawHelper(Graphics mGraphics)
        {
            this.mGraphics = mGraphics;

            init();
        }

        private void init()
        {
            levelList.Add(800);
            levelList.Add(400);
            levelList.Add(240);
            levelList.Add(130);
            levelList.Add(70);
            levelList.Add(30);
            levelList.Add(15);
        }

        public void drawCircle(Pen pen, PointF point, float radius)
        {
            //x, y 这个点为 把整个圆（椭圆）包含在其中的最小正方形（矩形）的左上角那个点
            //DrawEllipse 函数中width和height指的是直径

            Rectangle mRect = new Rectangle((int)(point.X - radius), (int)(point.Y - radius), (int)(radius * 2), (int)(radius * 2));

            mGraphics.DrawEllipse(pen, mRect);
        }

        /// <summary>
        /// 在圆心为point，半径为radius的圆内，产生一个半径为radius_inner的圆的圆心
        /// </summary>
        /// <param name="point">外圆圆心</param>
        /// <param name="radius_outer">外圆半径</param>
        /// <param name="radius_inner">内圆半径</param>
        /// <returns>内圆圆心</returns>
        public PointF PointOfRandom(PointF point, float radius_outer, float radius_inner)
        {
            int x = random.Next((int)(point.X - (radius_outer - radius_inner)), (int)(point.X + (radius_outer - radius_inner)));
            int y = random.Next((int)(point.Y - (radius_outer - radius_inner)), (int)(point.Y + (radius_outer - radius_inner)));

            while (!isInRegion(x - point.X, y - point.Y, radius_outer - radius_inner))
            {
                x = random.Next((int)(point.X - (radius_outer - radius_inner)), (int)(point.X + (radius_outer - radius_inner)));
                y = random.Next((int)(point.Y - (radius_outer - radius_inner)), (int)(point.Y + (radius_outer - radius_inner)));
            }

            PointF p = new PointF(x, y);
            return p;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_off">与大圆圆心的x方向偏移量</param>
        /// <param name="y_off">与大圆圆心的y方向偏移量</param>
        /// <param name="distance">大圆与小圆半径的差</param>
        /// <returns>判断点是否在范围内</returns>
        public bool isInRegion(float x_off, float y_off, float distance)
        {
            if (x_off * x_off + y_off * y_off <= distance * distance)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断两个圆是否重合，或者是相内切
        /// </summary>
        /// <param name="p_outer">外圆圆心</param>
        /// <param name="r_outer">外圆半径</param>
        /// <param name="p_inner">内圆圆心</param>
        /// <param name="r_inner">内圆半径</param>
        /// <returns>是否相内切</returns>
        public bool isIntersect(PointF p_outer, float r_outer, PointF p_inner, float r_inner)
        {
            //判定条件：两圆心的距离 + r_inner = r_outer

            float distance = (float)Math.Sqrt((p_outer.X - p_inner.X) * (p_outer.X - p_inner.X) + (p_outer.Y - p_inner.Y) * (p_outer.Y - p_inner.Y));

            if (distance + r_inner >= r_outer)
            {
                return true;
            }
            return false;
        }
    }
}
