using System;

namespace CommonTool.Alogrithm
{
    /// <summary>
    /// 旋转矩阵计算
    /// </summary>
    public class RotatedAffine
    {
        /// <summary>
        /// 一个点绕圆心转一定角度后求转动后的坐标
        /// </summary>
        /// <param name="rotationX">需要转动的点的坐标x</param>
        /// <param name="rotationY">需要转动的点坐标y</param>
        /// <param name="rotationAngle">转动的角度</param>
        /// <param name="cirX">圆心坐标x</param>
        /// <param name="cirY">圆心坐标y</param>
        /// <param name="rotatedX">转动后的坐标x</param>
        /// <param name="rotatedY">转动后的坐标y</param>
        /// <returns></returns>
        public static bool Math_Transfer(
           double rotationX, double rotationY, double rotationAngle,
           double cirX, double cirY,
           out double rotatedX, out double rotatedY)
        {
            /*
                (rx0, ry0)为旋转中心， ( x, y)为被旋转的点， (x0,y0)旋转后的点
                x0= cos (a) * (x-rx0) – sin (a) * (y-ry0) +rx0
                y0= cos (a) * (y-ry0) + sin (a) * (x-rx0) +ry0
            */

            rotatedX = Math.Cos(rotationAngle) * (rotationX - cirX) - Math.Sin(rotationAngle) * (rotationY - cirY) + cirX;
            rotatedY = Math.Cos(rotationAngle) * (rotationY - cirY) + Math.Sin(rotationAngle) * (rotationX - cirX) + cirY;
            rotatedX = Math.Round(rotatedX, 3);
            rotatedY = Math.Round(rotatedY, 3);
            return true;

        }
    }
}
