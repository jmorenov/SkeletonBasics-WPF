//------------------------------------------------------------------------------
// <copyright file="Movimiento7.cs" author="Javier Moreno">
//     Copyright (c) Javier Moreno <jmorenov28@gmail.com>
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Kinect;
    using System.Windows;
    using System.Windows.Media;

     /// <summary>
     /// Clase que realiza los cálculos necesarios para comprobar, 
     /// pasado un objeto Skeleton, si los brazos están en posición recta, 
     /// de frente y las manos sin juntarse.
     /// </summary>
    class Movimiento7
    {
        /// <summary>
        /// Objeto Skeleton donde se almacena la detección del cuerpo actual.
        /// </summary>
        private Skeleton skeleton;

        /// <summary>
        /// Índice de error.
        /// </summary>
        private float ERROR = 0.09F;

        /// <summary>
        /// Lista con los JoinType que hay que comprobar para que el movimiento sea el correcto.
        /// </summary>
        private static List<JointType> jointsTypes = new List<JointType> { JointType.ShoulderLeft, JointType.ElbowLeft, JointType.WristLeft, JointType.HandLeft,
                                                                            JointType.ShoulderRight, JointType.ElbowRight, JointType.WristRight, JointType.HandRight};

        /// <summary>
        /// Lista de tamaño igual al número de JoinType que hay que comprobar, 
        /// en esta lista se almacenan los valores de error de cada posición.
        /// </summary>
        private List<int> diff_positions = new List<int>(jointsTypes.Count);

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush usado para pintar los puntos que son incorrectos y están por debajo.
        /// </summary>
        private readonly Brush underPositionJointBrush = Brushes.Yellow;

        /// <summary>
        /// Brush usado para pintar los puntos que son incorrectos y están por encima.
        /// </summary>
        private readonly Brush upPositionJointBrush = Brushes.Turquoise;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen usado para pintar las partes de los brazos que están mal colocadas.
        /// </summary>
        private readonly Pen failBonePen = new Pen(Brushes.Red, 6);

        public Movimiento7() { skeleton = null; }

        public Movimiento7(Skeleton s) { setSkeleton(s); }

        /// <summary>
        /// Inicializa la detección del cuerpo, se reinician los calculos 
        /// con el nuevo valor de Skeleton también.
        /// </summary>
        public void setSkeleton(Skeleton s)
        {
            skeleton = s;
            checkArms();
        }

        /// <summary>
        /// Calcula la diferencia entre dos valores.
        /// </summary>
        private float diff(float v1, float v2) { return Math.Abs(v1 - v2); }

        /// <summary>
        /// Comprueba el error que hay entre dos puntos.
        /// </summary>
        /// <return> 0 : Posición correcta. </return>
        /// <return> 1: Posición incorrecta y punto por encima. </return>
        /// <return> -1: Posición incorrecta y punto por debajo. </return>
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

        /// <summary>
        /// Realiza los calculos para comprobar si las posiciones de los brazos son correctas.
        /// Almacena los resultados en la lista de enteros diff_positions.
        /// </summary>
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

        /// <summary>
        /// Devuelve el objeto Brush con el que pintar la posición del cuerpo Joint.
        /// </summary>
        public Brush getBrush(Joint joint)
        {
            if (jointsTypes.Contains(joint.JointType))
            {
                int i = jointsTypes.IndexOf(joint.JointType);
                if (diff_positions[i] == -1)
                    return underPositionJointBrush;
                else if (diff_positions[i] == 1)
                    return upPositionJointBrush;
            }
            return trackedJointBrush;

        }

        /// <summary>
        /// Devuelve el objeto Pen con el que pintar el hueso que se une con la parte del cuerpo JointType.
        /// </summary>
        public Pen getPen(JointType j)
        {
            if (jointsTypes.Contains(j))
            {
                int i = jointsTypes.IndexOf(j);
                if (diff_positions[i] != 0)
                    return failBonePen;
            }
            return trackedBonePen;
        }
    }
}