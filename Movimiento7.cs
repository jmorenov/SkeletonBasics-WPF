using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class Movimiento7
    {
        private Skeleton skeleton;
        private float ERROR = 0.09F;

        private static List<JointType> jointsTypes = new List<JointType> { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft,
                                                                            JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight};
        private List<int> diff_positions = new List<int>(8);
        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        private readonly Brush underPositionJointBrush = Brushes.Yellow;
        private readonly Brush upPositionJointBrush = Brushes.Turquoise;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        private readonly Pen failBonePen = new Pen(Brushes.Red, 6);

        public Movimiento7()
        {
            skeleton = null;
        }

        public Movimiento7(Skeleton s)
        {
            setSkeleton(s);
        }

        public void setSkeleton(Skeleton s)
        {
            skeleton = s;
            checkArms();
        }

        private float diff(float v1, float v2)
        {
            return Math.Abs(v1 - v2);
        }

        /**
         * @return 0 : Posición correcta.
         * @return 1: Posición incorrecta y punto por encima.
         * @return -1: Posición incorrecta y punto por debajo.
         */
        private int checkPoints(SkeletonPoint P1, SkeletonPoint P2)
        {
            if (diff(P1.X, P2.X) <= ERROR && diff(P1.Y, P2.Y) <= ERROR)
                return 0;
            else
            {
                if (P1.Y > P2.Y)
                    return -1;
                else
                    return 1;
            }
        }

        private void checkArms()
        {
            if (skeleton != null)
            {
                SkeletonPoint p1, p2;
                for(int i=0; i<7; i++)
                {
                    p1 = skeleton.Joints[jointsTypes[i]].Position;
                    p2 = skeleton.Joints[jointsTypes[i+1]].Position;

                    diff_positions[i + 1] = checkPoints(p1, p2);

                    if (i == 2)
                        i++;
                }
            }
        }

        public Brush getBrush(Joint joint)
        {
            int i = jointsTypes.IndexOf(joint.JointType);
            if (diff_positions[i] == -1)
                return underPositionJointBrush;
            else if (diff_positions[i] == 1)
                return upPositionJointBrush;
            return trackedJointBrush;
        }

        public Pen getPen(JointType j)
        {
            int i = jointsTypes.IndexOf(j);
            if (diff_positions[i] == 0)
                return trackedBonePen;
            else
                return failBonePen;
        }
    }
}